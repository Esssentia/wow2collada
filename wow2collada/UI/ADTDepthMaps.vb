Imports System.Windows.Forms
Imports System.IO
Imports System.Collections.Generic
Imports wow2collada.HelperFunctions

Public Class ADTDepthMaps

    Private _ADT As FileReaders.ADT = New FileReaders.ADT
    Private _FileName As String
    Private BMP As Bitmap
    Private DepthMap As Single(,)

    ''' <summary>
    ''' Form Initialization
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub BuildDepthMap(ByVal File As String)

        Dim x1 As Integer = 64
        Dim x2 As Integer = -1
        Dim y1 As Integer = 64
        Dim y2 As Integer = -1
        Dim zh As Single = -99999
        Dim zl As Single = 99999

        Dim z As Single

        For x As Integer = 0 To 63
            For y As Integer = 0 To 63
                _FileName = String.Format(File, x, y)
                If myMPQ.Locate(_FileName) Then
                    x1 = Math.Min(x1, x)
                    x2 = Math.Max(x2, x)
                    y1 = Math.Min(y1, y)
                    y2 = Math.Max(y2, y)
                End If
            Next
        Next

        'x1 = 26
        'x2 = 38
        'y1 = 14
        'y2 = 26

        If x1 <= x2 And y1 <= y2 Then

            ReDim DepthMap((x2 - x1 + 1) * 144, (y2 - y1 + 1) * 144)  'yeah, I know... big chunk of data...

            'get all height data...
            ListBox1.Items.Add("Getting height data of all tiles:")

            For ax = x1 To x2
                For ay = y1 To y2
                    _FileName = String.Format(File, ax, ay)
                    If myMPQ.Locate(_FileName) Then
                        _ADT.Load(myMPQ.LoadFile(_FileName), True)
                        For x As Integer = 0 To 15
                            For y As Integer = 0 To 15
                                Dim zoff As Single = _ADT.MCNKs(x, y).Position.Z
                                For i As Integer = 0 To 8
                                    For j As Integer = 0 To 8
                                        z = _ADT.MCNKs(x, y).HeightMap9x9(i, j) + zoff
                                        DepthMap((ax - x1) * 144 + x * 9 + i, (ay - y1) * 144 + y * 9 + j) = z
                                    Next
                                Next
                            Next
                        Next
                    End If
                    ListBox1.Items.Add(String.Format("  {0} {1}: done", ax, ay))
                    Application.DoEvents()
                Next
            Next

            ListBox1.Items.Add("Normalizing height data of all tiles:")

            For x As Integer = 0 To (x2 - x1 + 1) * 144
                For y As Integer = 0 To (y2 - y1 + 1) * 144
                    z = DepthMap(x, y)
                    zl = IIf(z < zl, z, zl)
                    zh = IIf(z > zh, z, zh)
                Next
                Application.DoEvents()
            Next

            ListBox1.Items.Add(String.Format("  Lowest z: {0}", zl))
            ListBox1.Items.Add(String.Format("  Highest z: {0}", zh))

            Dim f As Single = 255 / (zh - zl)

            For x As Integer = 0 To (x2 - x1 + 1) * 144
                For y As Integer = 0 To (y2 - y1 + 1) * 144
                    DepthMap(x, y) = (DepthMap(x, y) - zl) * f
                Next
                Application.DoEvents()
            Next

            ListBox1.Items.Add("  Normalizing completed.")

            ListBox1.Items.Add("Building map...")

            Dim w As Integer = 144 * (x2 - x1 + 1)
            Dim h As Integer = 144 * (y2 - y1 + 1)

            PictureBox1.Width = w
            PictureBox1.Height = h

            BMP = New Bitmap(w, h, Imaging.PixelFormat.Format32bppArgb)
            Dim GRA As Graphics = Graphics.FromImage(BMP)
            GRA.FillRectangle(Brushes.Black, New Rectangle(0, 0, PictureBox1.Width, PictureBox1.Height))

            For x As Integer = 0 To (x2 - x1 + 1) * 144 - 1
                For y As Integer = 0 To (y2 - y1 + 1) * 144 - 1
                    f = DepthMap(x, y)
                    BMP.SetPixel(x, y, Color.FromArgb(255, f, f, f))
                Next
                Application.DoEvents()
                PictureBox1.Image = BMP

            Next
            GRA.Dispose()

            ListBox1.Items.Add("done.")
        End If

    End Sub

    Private Sub SaveADTDialog_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles SaveImgDialog.FileOk
        Dim Fullname As String = SaveImgDialog.FileName
        PictureBox1.Image.Save(Fullname)
    End Sub

    Private Sub SaveImage(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DumpADTToolStripMenuItem.Click
        Dim FullName As String = _FileName
        Dim i As Integer = FullName.LastIndexOf("\")
        If i > 0 Then FullName = FullName.Substring(i + 1)
        i = FullName.LastIndexOf(".")
        If i > 0 Then FullName = FullName.Substring(0, i)

        SaveImgDialog.OverwritePrompt = True
        SaveImgDialog.FileName = FullName
        SaveImgDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        SaveImgDialog.Filter = "Bitmap File (*.bmp)|*.bmp|PNG File (*.png)|*.png|JPG File (*.jpg)|*.jpg"
        SaveImgDialog.ShowDialog()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ListBox1.Items.Clear()
        Select Case ComboBox1.SelectedItem
            Case "Kalimdor"
                BuildDepthMap("world\maps\kalimdor\kalimdor_{0:d2}_{1:d2}.adt")
            Case "Azeroth"
                BuildDepthMap("world\maps\azeroth\azeroth_{0:d2}_{1:d2}.adt")
            Case "Outland"
                BuildDepthMap("world\maps\expansion01\expansion01_{0:d2}_{1:d2}.adt")
            Case "Northend"
                BuildDepthMap("world\maps\northrend\northrend_{0:d2}_{1:d2}.adt")
        End Select

    End Sub

    Public Function DepthmapFromADT(ByVal ADT As FileReaders.ADT) As Bitmap
        Dim dm(144, 144) As Single
        Dim zh As Single = -99999
        Dim zl As Single = 99999
        Dim z As Single


        '  0/0   0/1   0/2   0/3   0/4   0/5   0/6   0/7   0/8    <- 9x9 matrix
        '     0/0   0/1   0/2   0/3   0/4   0/5   0/6   0/7       <- 8x8 matrix
        '  1/0   1/1   1/2   1/3   1/4   1/5   1/6   1/7   1/8
        '     1/0   1/1   1/2   1/3   1/4   1/5   1/6   1/7
        '  2/0   2/1   2/2   2/3   2/4   2/5   2/6   2/7   2/8
        '     2/0   2/1   2/2   2/3   2/4   2/5   2/6   2/7
        '  3/0   3/1   3/2   3/3   3/4   3/5   3/6   3/7   3/8
        '     3/0   3/1   3/2   3/3   3/4   3/5   3/6   3/7
        '  4/0   4/1   4/2   4/3   4/4   4/5   4/6   4/7   4/8
        '     4/0   4/1   4/2   4/3   4/4   4/5   4/6   4/7
        '  5/0   5/1   5/2   5/3   5/4   5/5   5/6   5/7   5/8
        '     5/0   5/1   5/2   5/3   5/4   5/5   5/6   5/7
        '  6/0   6/1   6/2   6/3   6/4   6/5   6/6   6/7   6/8
        '     6/0   6/1   6/2   6/3   6/4   6/5   6/6   6/7
        '  7/0   7/1   7/2   7/3   7/4   7/5   7/6   7/7   7/8
        '     7/0   7/1   7/2   7/3   7/4   7/5   7/6   7/7
        '  8/0   8/1   8/2   8/3   8/4   8/5   8/6   8/7   8/8



        For x As Integer = 0 To 15
            For y As Integer = 0 To 15
                For i As Integer = 0 To 8
                    For j As Integer = 0 To 8
                        z = ADT.MCNKs(x, y).HeightMap9x9(i, j)
                        dm(x * 9 + i, y * 9 + j) = z
                        zl = IIf(z < zl, z, zl)
                        zh = IIf(z > zh, z, zh)
                    Next
                Next
            Next
        Next

        Dim Depthm As New Bitmap(144, 144, Imaging.PixelFormat.Format32bppArgb)
        Dim f1 As Single
        Dim f2 As Single = IIf(zh = zl, 0, 255 / (zh - zl))

        If f2 <> 0 Then
            For x As Integer = 0 To 143
                For y As Integer = 0 To 143
                    f1 = (dm(x, y) - zl) * f2
                    Depthm.SetPixel(x, y, Color.FromArgb(255, f1, f1, f1))
                Next
            Next
        End If

        Return Depthm
    End Function



End Class
