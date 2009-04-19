Imports System.Windows.Forms

Public Class HexViewerFind

    Public Value As Byte()

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        DialogResult = System.Windows.Forms.DialogResult.OK
        Value = System.Text.Encoding.ASCII.GetBytes(TextBox1.Text)
        Close()
    End Sub

    Private Sub HexViewerFind_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        TextBox1.Focus()
    End Sub

End Class
