Public Class OptionsUI

  Public Overrides Function ValidateData() As Boolean
    If txtDomain.Text.Trim.Length = 0 Then
      MessageBox.Show("Domain name cannot be blank", "DNS Black List Plug-In", MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return False
    End If
    Dim d As JHSoftware.SimpleDNS.DomName
    If Not JHSoftware.SimpleDNS.DomName.TryParse(txtDomain.Text.Trim, d) Then
      MessageBox.Show("Invalid domain name", "DNS Black List Plug-In", MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return False
    End If
    If txtDataFile.Text.Trim.Length = 0 Then
      MessageBox.Show("Compiled data file cannot be blank", "DNS Black List Plug-In", MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return False
    End If

    If Not RemoteGUI Then
      Dim buf(7) As Byte
      Try
        Dim file = System.IO.File.Open(txtDataFile.Text.Trim, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
        file.Read(buf, 0, 7)
        file.Close()
      Catch ex As System.IO.IOException
        MessageBox.Show("Could not access compiled data file." & vbCrLf & vbCrLf & _
                        "Error: " & ex.Message & " (" & ex.GetType.ToString & ")", _
                        "DNS Black List Plug-In", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
      End Try
      REM magic cookie
      If System.Text.Encoding.ASCII.GetString(buf, 0, 6) <> "SDNSBL" Then
        MessageBox.Show("Invalid compiled data file." & vbCrLf & vbCrLf & _
                        "This must be a file compiled with the" & vbCrLf & _
                        "DNS Blacklist Editor tool (dnsbledit.exe)", _
                        "DNS Black List Plug-In", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
      End If
      REM data format version
      If buf(6) <> 1 Then
        MessageBox.Show("Invalid compiled data file." & vbCrLf & vbCrLf & _
                        "This file was compiled with a wrong version" & vbCrLf & _
                        "of DNS Blacklist Editor tool (dnsbledit.exe)", _
                        "DNS Black List Plug-In", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
      End If
    End If

    Return True
  End Function

  Public Overrides Function SaveData() As String
    Dim cfg As New blConfig
    cfg.Domain = JHSoftware.SimpleDNS.DomName.Parse(txtDomain.Text.Trim)
    cfg.DataFile = txtDataFile.Text.Trim
    cfg.Monitor = chkMonitor.Checked
    cfg.TTL = txtTTL.Value
    Return cfg.ConfigXML
  End Function

  Public Overrides Sub LoadData(ByVal config As String)
    If config Is Nothing Then Exit Sub 'new instance
    Dim cfg As New blConfig
    cfg.ConfigXML = config
    txtDomain.Text = cfg.Domain.ToString
    txtDataFile.Text = cfg.DataFile
    chkMonitor.Checked = cfg.Monitor
    txtTTL.Value = cfg.TTL
  End Sub

  Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
    If RemoteGUI Then MessageBox.Show("This function is not available during remote management", _
                                      "Browse for file/folder", MessageBoxButtons.OK, _
                                      MessageBoxIcon.Warning) : Exit Sub
    With OpenFileDialog1
      Dim x As String = txtDataFile.Text.Trim
      If x.Length > 0 Then
        Try
          .InitialDirectory = System.IO.Path.GetDirectoryName(x)
          .FileName = System.IO.Path.GetFileName(x)
        Catch
        End Try
      End If
      If .ShowDialog <> DialogResult.OK Then Exit Sub
      txtDataFile.Text = .FileName
    End With
  End Sub
End Class
