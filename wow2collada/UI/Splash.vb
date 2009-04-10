Public NotInheritable Class Splash

    Private Sub Splash_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("About {0}", ApplicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.LabelProductName.Text = My.Application.Info.ProductName
        Me.LabelVersion.Text = String.Format("Version {0}", My.Application.Info.Version.ToString)
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelCompanyName.Text = My.Application.Info.CompanyName
        Me.TextBoxDescription.Text = My.Application.Info.Description
        Me.ToolStripProgressBar1.Minimum = 0
        Me.ToolStripProgressBar1.Maximum = 100
        Me.ToolStripProgressBar1.Value = 0
        Me.OKButton.Enabled = False
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

    Public Sub UpdateProgress(ByVal Percent As Integer)
        Me.ToolStripProgressBar1.Value = Math.Max(Math.Min(Percent, 100), 0)
        Me.ToolStripProgressBar1.ForeColor = Color.FromArgb(255, 255 * (100 - Percent) / 100, 255 * Percent / 100, 0)
        If Percent >= 100 Then
            Me.OKButton.Enabled = True
            Me.Dispose()
        End If

    End Sub

End Class
