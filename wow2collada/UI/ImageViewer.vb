Imports System.Windows.Forms

Public Class ImageViewer
    Private Sub PictureBox1_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.SizeChanged
        Me.Width = PictureBox1.Width + 6
        Me.Height = PictureBox1.Height + MenuStrip1.Height + StatusStrip1.Height + 30
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveAsToolStripMenuItem.Click
        SaveFileDialog1.OverwritePrompt = True
        SaveFileDialog1.Filter = "Windows Bitmap File (*.bmp)|*.bmp|JPEG (.jpg)|*.jpg|GIF (*.gif)|*.gif|PNG (*.png)|*.png"
        SaveFileDialog1.ShowDialog()
    End Sub

    Private Sub SaveFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles SaveFileDialog1.FileOk
        PictureBox1.Image.Save(SaveFileDialog1.FileName)
    End Sub
End Class
