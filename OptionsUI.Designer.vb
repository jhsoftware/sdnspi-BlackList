<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OptionsUI
    Inherits JHSoftware.SimpleDNS.Plugin.OptionsUI

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
    Me.components = New System.ComponentModel.Container
    Me.Label1 = New System.Windows.Forms.Label
    Me.txtDataFile = New System.Windows.Forms.TextBox
    Me.btnBrowse = New System.Windows.Forms.Button
    Me.Label2 = New System.Windows.Forms.Label
    Me.txtDomain = New System.Windows.Forms.TextBox
    Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
    Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
    Me.Label3 = New System.Windows.Forms.Label
    Me.chkMonitor = New System.Windows.Forms.CheckBox
    Me.txtTTL = New ctlTTL
    Me.SuspendLayout()
    '
    'Label1
    '
    Me.Label1.AutoSize = True
    Me.Label1.Location = New System.Drawing.Point(-3, 57)
    Me.Label1.Margin = New System.Windows.Forms.Padding(3, 15, 3, 0)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(93, 13)
    Me.Label1.TabIndex = 2
    Me.Label1.Text = "Compiled data file:"
    '
    'txtDataFile
    '
    Me.txtDataFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.txtDataFile.Location = New System.Drawing.Point(0, 73)
    Me.txtDataFile.Name = "txtDataFile"
    Me.txtDataFile.Size = New System.Drawing.Size(293, 20)
    Me.txtDataFile.TabIndex = 3
    '
    'btnBrowse
    '
    Me.btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnBrowse.Location = New System.Drawing.Point(299, 71)
    Me.btnBrowse.Name = "btnBrowse"
    Me.btnBrowse.Size = New System.Drawing.Size(27, 23)
    Me.btnBrowse.TabIndex = 4
    Me.btnBrowse.Text = "..."
    Me.ToolTip1.SetToolTip(Me.btnBrowse, "Browse")
    Me.btnBrowse.UseVisualStyleBackColor = True
    '
    'Label2
    '
    Me.Label2.AutoSize = True
    Me.Label2.Location = New System.Drawing.Point(-3, 3)
    Me.Label2.Name = "Label2"
    Me.Label2.Size = New System.Drawing.Size(115, 13)
    Me.Label2.TabIndex = 0
    Me.Label2.Text = "Blacklist domain name:"
    '
    'txtDomain
    '
    Me.txtDomain.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.txtDomain.Location = New System.Drawing.Point(0, 19)
    Me.txtDomain.Name = "txtDomain"
    Me.txtDomain.Size = New System.Drawing.Size(326, 20)
    Me.txtDomain.TabIndex = 1
    '
    'OpenFileDialog1
    '
    Me.OpenFileDialog1.DefaultExt = "sdnsbl"
    Me.OpenFileDialog1.Filter = "Compiled black list data file|*.sdnsbl"
    Me.OpenFileDialog1.Title = "Select compiled black list data file"
    '
    'Label3
    '
    Me.Label3.AutoSize = True
    Me.Label3.Location = New System.Drawing.Point(-3, 134)
    Me.Label3.Margin = New System.Windows.Forms.Padding(3, 15, 3, 0)
    Me.Label3.Name = "Label3"
    Me.Label3.Size = New System.Drawing.Size(198, 13)
    Me.Label3.TabIndex = 6
    Me.Label3.Text = "Result DNS records TTL (Time To Live):"
    '
    'chkMonitor
    '
    Me.chkMonitor.AutoSize = True
    Me.chkMonitor.Location = New System.Drawing.Point(0, 99)
    Me.chkMonitor.Name = "chkMonitor"
    Me.chkMonitor.Size = New System.Drawing.Size(252, 17)
    Me.chkMonitor.TabIndex = 5
    Me.chkMonitor.Text = "Automatically re-load data file when it is updated"
    Me.chkMonitor.UseVisualStyleBackColor = True
    '
    'txtTTL
    '
    Me.txtTTL.AutoSize = True
    Me.txtTTL.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
    Me.txtTTL.BackColor = System.Drawing.Color.Transparent
    Me.txtTTL.Location = New System.Drawing.Point(0, 150)
    Me.txtTTL.Name = "txtTTL"
    Me.txtTTL.ReadOnly = False
    Me.txtTTL.Size = New System.Drawing.Size(156, 21)
    Me.txtTTL.TabIndex = 7
    Me.txtTTL.Value = 1800
    '
    'OptionsUI
    '
    Me.Controls.Add(Me.chkMonitor)
    Me.Controls.Add(Me.txtTTL)
    Me.Controls.Add(Me.Label3)
    Me.Controls.Add(Me.txtDomain)
    Me.Controls.Add(Me.Label2)
    Me.Controls.Add(Me.btnBrowse)
    Me.Controls.Add(Me.txtDataFile)
    Me.Controls.Add(Me.Label1)
    Me.Name = "OptionsUI"
    Me.Size = New System.Drawing.Size(326, 181)
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents txtDataFile As System.Windows.Forms.TextBox
  Friend WithEvents btnBrowse As System.Windows.Forms.Button
  Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
  Friend WithEvents Label2 As System.Windows.Forms.Label
  Friend WithEvents txtDomain As System.Windows.Forms.TextBox
  Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
  Friend WithEvents Label3 As System.Windows.Forms.Label
  Friend WithEvents txtTTL As ctlTTL
  Friend WithEvents chkMonitor As System.Windows.Forms.CheckBox

End Class
