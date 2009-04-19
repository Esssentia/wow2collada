Imports System.Windows.Forms

Public Class HexViewerGoto

    Public Value As Integer

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        DialogResult = System.Windows.Forms.DialogResult.OK
        Value = Val("&H" & TextBox1.Text)
        Close()
    End Sub

    Private Sub TextBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        Select Case e.KeyCode
            Case Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Delete, Keys.Back, Keys.Right, Keys.Left
            Case Else
                Debug.Print(e.KeyCode)
                e.Handled = True
        End Select
    End Sub

    Private Sub TextBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        Select Case e.KeyChar
            Case "a" To "f", "A" To "F", "0" To "9"
            Case Else
                e.KeyChar = Nothing
        End Select
    End Sub

    Private Sub HexViewerGoto_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        TextBox1.Focus()
    End Sub

End Class
