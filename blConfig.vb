Friend Class blConfig
  Friend Domain As JHSoftware.SimpleDNS.Plugin.DomainName
  Friend DataFile As String
  Friend Monitor As Boolean
  Friend TTL As Integer

  Friend Property ConfigXML() As String
    Get
      Dim doc As New Xml.XmlDocument
      Dim root As Xml.XmlElement = doc.CreateElement("root")
      doc.AppendChild(root)
      Dim elem As Xml.XmlElement = doc.CreateElement("DNSBL")
      elem.SetAttribute("Domain", Domain.ToString)
      elem.SetAttribute("DataFile", DataFile)
      elem.SetAttribute("Monitor", If(Monitor, "true", "false"))
      elem.SetAttribute("TTL", TTL.ToString)
      root.AppendChild(elem)
      Return root.InnerXml
    End Get
    Set(ByVal cfgXML As String)
      Dim doc As New Xml.XmlDocument
      Dim root As Xml.XmlElement = doc.CreateElement("root")
      doc.AppendChild(root)
      root.InnerXml = cfgXML
      For Each node As Xml.XmlNode In root.ChildNodes
        If Not TypeOf node Is Xml.XmlElement Then Continue For
        If DirectCast(node, Xml.XmlElement).Name <> "DNSBL" Then Continue For
        For Each attr As Xml.XmlAttribute In DirectCast(node, Xml.XmlElement).Attributes
          Select Case attr.Name.ToLowerInvariant
            Case "domain"
              Domain = JHSoftware.SimpleDNS.Plugin.DomainName.Parse(attr.Value)
            Case "datafile"
              DataFile = attr.Value
            Case "monitor"
              Monitor = (attr.Value.ToLowerInvariant = "true")
            Case "ttl"
              TTL = Integer.Parse(attr.Value)
          End Select
        Next
        Exit Property
      Next
    End Set
  End Property

End Class
