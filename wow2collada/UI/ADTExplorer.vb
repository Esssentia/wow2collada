Imports System.Windows.Forms

Public Class ADTExplorer
    Private MD5TRS As New Dictionary(Of String, String)
    Private BMP As Bitmap

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        BuildMap(ComboBox1.SelectedItem)
    End Sub

    Public Sub BuildMap(ByVal Base As String)
        Dim BLP As New FileReaders.BLP
        Dim x1 As Integer = 64
        Dim x2 As Integer = -1
        Dim y1 As Integer = 64
        Dim y2 As Integer = -1

        For x As Integer = 0 To 63
            For y As Integer = 0 To 63
                Dim File As String = String.Format("{0}\map{1:d2}_{2:d2}.blp", Base, x, y).ToLower
                If MD5TRS.ContainsKey(File) Then
                    x1 = Math.Min(x1, x)
                    x2 = Math.Max(x2, x)
                    y1 = Math.Min(y1, y)
                    y2 = Math.Max(y2, y)
                End If
            Next
        Next

        If x1 < x2 And y1 < y2 Then
            Dim f As Single = 128
            Dim xo As Single = 10
            Dim yo As Single = 10
            Dim w As Integer = 20 + f * (x2 - x1 + 1)
            Dim h As Integer = 20 + f * (y2 - y1 + 1)

            PictureBox1.Width = w
            PictureBox1.Height = h

            BMP = New Bitmap(w, h, Imaging.PixelFormat.Format32bppArgb)
            Dim GRA As Graphics = Graphics.FromImage(BMP)

            'Dim f As Single = Math.Min(PictureBox1.Width / (x2 - x1 + 1), PictureBox1.Height / (y2 - y1 + 1))
            'Dim xo As Single = (PictureBox1.Width - f * (x2 - x1 + 1)) / 2
            'Dim yo As Single = (PictureBox1.Height - f * (y2 - y1 + 1)) / 2

            GRA.FillRectangle(Brushes.Black, New Rectangle(0, 0, PictureBox1.Width, PictureBox1.Height))

            For x As Integer = x1 To x1 + x2
                For y As Integer = y1 To y1 + y2
                    Dim File As String = String.Format("{0}\map{1:d2}_{2:d2}.blp", Base, x, y).ToLower
                    If MD5TRS.ContainsKey(File) Then
                        Dim FullFile = "textures\minimap\" & MD5TRS(File)
                        If myMPQ.Locate(FullFile) Then
                            Dim Img As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(FullFile), FullFile)
                            PictureBox2.Image = Img
                            GRA.DrawImage(Img, New Rectangle(xo + (x - x1) * f, yo + (y - y1) * f, f, f))
                            GRA.DrawRectangle(Pens.Gray, New Rectangle(xo + (x - x1) * f, yo + (y - y1) * f, f, f))
                            If f > 20 Then DrawCoords(xo + (x - x1) * f + f - 20, yo + (y - y1) * f + f - 6, x, y, GRA)
                            PictureBox1.Image = BMP
                            Application.DoEvents()
                        End If
                    End If

                Next
            Next
            GRA.Dispose()
        End If
    End Sub

    Private Sub ADTExplorer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim BR As New IO.StreamReader(myMPQ.LoadFile("textures\minimap\md5translate.trs"))
        Dim Line As String
        Dim Regions As New List(Of String)

        While Not BR.EndOfStream
            Line = BR.ReadLine
            If Line.IndexOf("dir:") <> 0 Then
                Dim s As String() = Line.Split(New String() {Chr(9)}, System.StringSplitOptions.RemoveEmptyEntries)
                If s.Count = 2 Then
                    MD5TRS(s(0).ToLower) = s(1).ToLower
                    Dim f As String() = s(0).Split("\")
                    If f.Count > 1 Then
                        Dim a As String = f(f.Count - 2)
                        If Not Regions.Contains(a) Then Regions.Add(a)
                    End If
                End If
            End If
        End While

        ComboBox1.Items.Clear()
        For Each s As String In Regions
            ComboBox1.Items.Add(s)
        Next

    End Sub

    Private Sub DrawCoords(ByVal xo As Integer, ByVal yo As Integer, ByVal n1 As Integer, ByVal n2 As Integer, ByVal gra As Graphics)
        DrawNumber(xo, yo, n1, gra)
        DrawNumber(xo + 12, yo, n2, gra)
        gra.DrawLine(Pens.Gray, xo + 8, yo + 4, xo + 10, yo)
    End Sub

    Private Sub DrawNumber(ByVal xo As Integer, ByVal yo As Integer, ByVal n As Integer, ByVal gra As Graphics)
        Dim d1 = Int(n \ 10)
        Dim d2 = n Mod 10

        DrawDigit(xo, yo, d1, gra)
        DrawDigit(xo + 4, yo, d2, gra)

    End Sub

    Private Sub DrawDigit(ByVal xo As Integer, ByVal yo As Integer, ByVal n As Integer, ByVal gra As Graphics)
        Dim segs As New List(Of Integer)
        Select Case n
            Case 0
                segs.Add(0)
                segs.Add(1)
                segs.Add(2)
                segs.Add(3)
                segs.Add(4)
                segs.Add(6)
            Case 1
                segs.Add(2)
                segs.Add(4)
            Case 2
                segs.Add(0)
                segs.Add(2)
                segs.Add(5)
                segs.Add(3)
                segs.Add(6)
            Case 3
                segs.Add(0)
                segs.Add(2)
                segs.Add(4)
                segs.Add(5)
                segs.Add(6)

            Case 4
                segs.Add(1)
                segs.Add(2)
                segs.Add(5)
                segs.Add(4)
            Case 5
                segs.Add(0)
                segs.Add(1)
                segs.Add(5)
                segs.Add(4)
                segs.Add(6)
            Case 6
                segs.Add(0)
                segs.Add(1)
                segs.Add(3)
                segs.Add(4)
                segs.Add(5)
                segs.Add(6)
            Case 7
                segs.Add(0)
                segs.Add(2)
                segs.Add(4)
            Case 8
                segs.Add(0)
                segs.Add(1)
                segs.Add(2)
                segs.Add(3)
                segs.Add(4)
                segs.Add(5)
                segs.Add(6)
            Case 9
                segs.Add(0)
                segs.Add(1)
                segs.Add(2)
                segs.Add(4)
                segs.Add(5)
                segs.Add(6)
        End Select

        For Each s As Integer In segs
            DrawSegment(xo, yo, gra, s)
        Next

    End Sub

    Private Sub DrawSegment(ByVal xo As Integer, ByVal yo As Integer, ByVal gra As Graphics, ByVal seg As Integer)
        Dim p As Drawing.Pen = Pens.Gray
        Select Case seg
            Case 0
                gra.DrawLine(p, xo, yo, xo + 2, yo)
            Case 1
                gra.DrawLine(p, xo, yo, xo, yo + 2)
            Case 2
                gra.DrawLine(p, xo + 2, yo, xo + 2, yo + 2)
            Case 3
                gra.DrawLine(p, xo, yo + 2, xo, yo + 4)
            Case 4
                gra.DrawLine(p, xo + 2, yo + 2, xo + 2, yo + 4)
            Case 5
                gra.DrawLine(p, xo, yo + 2, xo + 2, yo + 2)
            Case 6
                gra.DrawLine(p, xo, yo + 4, xo + 2, yo + 4)
        End Select
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
