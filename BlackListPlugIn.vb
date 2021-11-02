Imports JHSoftware.SimpleDNS.Plugin
Imports JHSoftware.SimpleDNS
Imports System.Threading.Tasks

Public Class BlackListPlugIn
  Implements ILookupHost
  Implements ILookupTXT
  Implements IListsIPAddress
  Implements IOptionsUI

  Private DataSets(-1) As blDataSet
  Private ListItems(-1) As blItem

  Private config As New blConfig
  Private ListDomSegCt As Integer

  Private WithEvents fMon As System.IO.FileSystemWatcher
  Private LastReload As DateTime

  Public Property Host As IHost Implements IPlugInBase.Host

  Public Function GetPlugInTypeInfo() As TypeInfo Implements IPlugInBase.GetTypeInfo
    With GetPlugInTypeInfo
      .Name = "DNS Blacklist"
      .Description = "Provides data from an IP based DNS blacklist"
      .InfoURL = "https://simpledns.plus/plugin-blacklist"
    End With
  End Function

  Public Function GetOptionsUI(ByVal instanceID As Guid, ByVal dataPath As String) As JHSoftware.SimpleDNS.Plugin.OptionsUI Implements JHSoftware.SimpleDNS.Plugin.IOptionsUI.GetOptionsUI
    Return New OptionsUI
  End Function

  Public Function InstanceConflict(ByVal configXML1 As String, ByVal configXML2 As String, ByRef errorMsg As String) As Boolean Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.InstanceConflict
    Dim cfg1 As New blConfig
    cfg1.ConfigXML = configXML1
    Dim cfg2 As New blConfig
    cfg2.ConfigXML = configXML2
    If cfg1.Domain = cfg2.Domain AndAlso cfg1.DataFile.ToLowerInvariant = cfg2.DataFile.ToLowerInvariant Then
      errorMsg = "Another DNS Black List Plug-In instance with the same domain name and data file already exists."
      Return True
    Else
      Return False
    End If
  End Function

  Sub LoadConfig(ByVal config As String, ByVal instanceID As Guid, ByVal dataPath As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LoadConfig
    Me.config.ConfigXML = config
    ListDomSegCt = Me.config.Domain.SegmentCount + 4
  End Sub

  Public Function LookupHost(name As DomName, ipv6 As Boolean, req As IRequestContext) As Task(Of LookupResult(Of SdnsIP)) Implements ILookupHost.LookupHost
    Return Task.FromResult(Lookup2(name, ipv6, req))
  End Function
  Public Function Lookup2(name As DomName, ipv6 As Boolean, req As IRequestContext) As LookupResult(Of SdnsIP)
    If Not ipv6 Then Return Nothing
    If req.QNameIP Is Nothing OrElse Not req.QNameIP.IsIPv4 Then Return Nothing
    If Not name.EndsWith(config.Domain) Then Return Nothing
    Dim ds As blDataSet = Nothing
    If Not TryFindIPDataSet(DirectCast(req.QNameIP, SdnsIPv4).Data, ds) Then Return Nothing
    Return New LookupResult(Of SdnsIP) With {.Value = New SdnsIPv4(ds.ValueA), .TTL = config.TTL}
  End Function

  Public Function LookupTXT(name As DomName, req As IRequestContext) As Task(Of LookupResult(Of String)) Implements ILookupTXT.LookupTXT
    Return Task.FromResult(LookupTXT2(name, req))
  End Function
  Public Function LookupTXT2(name As DomName, req As IRequestContext) As LookupResult(Of String)
    If req.QNameIP Is Nothing OrElse Not req.QNameIP.IsIPv4 Then Return Nothing
    If Not name.EndsWith(config.Domain) Then Return Nothing
    Dim ds As blDataSet = Nothing
    If Not TryFindIPDataSet(DirectCast(req.QNameIP, SdnsIPv4).Data, ds) Then Return Nothing
    If ds.ValueTXT.Length = 0 Then Return Nothing
    Dim x = System.Text.Encoding.ASCII.GetString(ds.ValueTXT)
    Dim p = 0
    Do
      p = x.IndexOf("$", p)
      If p < 0 Then
        Return New LookupResult(Of String) With {.Value = If(x.Length > 255, x.Substring(0, 255), x), .TTL = config.TTL}
      End If
      If p < x.Length - 1 AndAlso x(p + 1) = "$" Then
        x = x.Substring(0, p) & x.Substring(p + 1)
        p += 1
      Else
        x = x.Substring(0, p) & req.QNameIP.ToString & x.Substring(p + 1)
      End If
    Loop
  End Function

  Private Function TryFindIPDataSet(ByVal IP As UInt32, ByRef ds As blDataSet) As Boolean
    SyncLock config
      If ListItems.Length = 0 Then Return False
      Dim lv, mv, hv As Integer
      hv = ListItems.Length - 1
      While lv < hv
        mv = lv + (hv - lv) \ 2
        If IP <= ListItems(mv).LastIP Then hv = mv Else lv = mv + 1
      End While
      If IP < ListItems(lv).FirstIP OrElse IP > ListItems(lv).LastIP Then Return False
      ds = DataSets(ListItems(lv).DataSetNum)
      Return True
    End SyncLock
  End Function

  Private Function IPlugInBase_StartService() As Task Implements IPlugInBase.StartService
    ReDim DataSets(-1)
    ReDim ListItems(-1)
    LoadDataFile()
    If config.Monitor Then
      fMon = New System.IO.FileSystemWatcher
      fMon.Path = System.IO.Path.GetDirectoryName(config.DataFile)
      fMon.Filter = System.IO.Path.GetFileName(config.DataFile)
      fMon.IncludeSubdirectories = False
      fMon.NotifyFilter = IO.NotifyFilters.LastWrite
      fMon.EnableRaisingEvents = True
    End If
    Return Task.CompletedTask
  End Function

  Private Sub LoadDataFile()
    LastReload = DateTime.UtcNow

    Dim buf(7) As Byte
    Dim ct, b As Integer

    Dim file As System.IO.FileStream
    Dim failCt As Integer
    Do
      Try
        file = System.IO.File.Open(config.DataFile, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
        Exit Do
      Catch ex As System.IO.IOException
        failCt += 1
        REM continue trying for 5 seconds (20 x 1/4 second)
        If failCt >= 20 Then Throw ex
        Threading.Thread.Sleep(250)
      End Try
    Loop

    file.Read(buf, 0, 7)
    REM magic cookie
    If System.Text.Encoding.ASCII.GetString(buf, 0, 6) <> "SDNSBL" Then Throw New Exception("Invalid data file (MC)")

    REM data format version
    If buf(6) <> 1 Then Throw New Exception("Invalid data file (version)")

    REM number of data sets 
    ct = ReadJHInt(file)
    ReDim DataSets(ct - 1)

    REM data sets
    For i = 0 To ct - 1
      b = file.ReadByte
      If b = 0 Then
        file.Read(buf, 0, 4)
        DataSets(i).ValueA = (CUInt(buf(0)) << 24) Or (CUInt(buf(1)) << 16) Or (CUInt(buf(2)) << 8) Or CUInt(buf(3))
      Else
        DataSets(i).ValueA = &H7F000000UI Or CUInt(b)
      End If
      b = file.ReadByte
      ReDim DataSets(i).ValueTXT(b - 1)
      file.Read(DataSets(i).ValueTXT, 0, b)
    Next

    REM number of list entries
    ct = ReadJHInt(file)
    ReDim ListItems(ct - 1)

    REM list entries
    Dim lastIP = 0UI
    For i = 0 To ct - 1
      With ListItems(i)
        .FirstIP = lastIP + ReadJHUInt(file)
        .LastIP = .FirstIP + ReadJHUInt(file)
        lastIP = .LastIP
        .DataSetNum = ReadJHInt(file)
      End With
    Next

    file.Close()
  End Sub

  Private Function ReadJHInt(ByVal file As System.IO.Stream) As Integer
    Dim rv, b As Integer
    Do
      b = file.ReadByte()
      If b < 128 Then Return (rv Or b)
      rv = (rv Or (b And &H7F)) << 7
    Loop
  End Function

  Private Function ReadJHUInt(ByVal file As System.IO.Stream) As UInteger
    Dim rv, b As UInteger
    Do
      b = CType(file.ReadByte(), UInt32)
      If b < 128 Then Return (rv Or b)
      rv = (rv Or (b And &H7FUI)) << 7
    Loop
  End Function

  Public Sub StopService() Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.StopService
    DataSets = Nothing
    ListItems = Nothing
    If fMon IsNot Nothing Then
      fMon.EnableRaisingEvents = False
      fMon.Dispose()
      fMon = Nothing
    End If
  End Sub

  Private Sub fMon_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles fMon.Changed
    If DateTime.UtcNow.Subtract(LastReload).TotalSeconds < 5 Then Exit Sub
    SyncLock config
      Try
        LoadDataFile()
        Host.LogLine("Data file reloaded")
      Catch ex As Exception
        ReDim DataSets(-1)
        ReDim ListItems(-1)
        Host.LogLine("Error reloading data file: " & ex.Message)
      End Try
    End SyncLock
  End Sub

  Public Function ListsIPAddress(ip As SdnsIP) As Task(Of Boolean) Implements IListsIPAddress.ListsIPAddress
    If ip.IPVersion <> 4 Then Return Task.FromResult(False)
    Dim ds As blDataSet = Nothing
    Return Task.FromResult(TryFindIPDataSet(DirectCast(ip, SdnsIPv4).Data, ds))
  End Function

End Class

Friend Structure blDataSet
  Friend ValueA As UInt32
  Friend ValueTXT As Byte()
End Structure

Friend Structure blItem
  Friend FirstIP As UInt32
  Friend LastIP As UInt32
  Friend DataSetNum As Integer
End Structure