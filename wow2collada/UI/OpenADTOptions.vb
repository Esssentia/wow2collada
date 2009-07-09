Imports System.Windows.Forms
Imports System.IO
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports wow2collada.HelperFunctions

Public Class OpenADTOptions

    Private _ADT As FileReaders.ADT
    Private _FileName As String
    Private _ADTFile As Byte()
    Private _WMOs As New List(Of String)
    Private _M2s As New List(Of String)
    Private _M2Selected As New List(Of Integer)
    Private _WMOSelected As New List(Of String)

    ''' <summary>
    ''' Form Initialization, sets the ADT filename and initiates an appropriate filereader
    ''' </summary>
    ''' <param name="FileName">Name of the ADT file to open</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal FileName As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _FileName = FileName
        _ADT = New FileReaders.ADT

    End Sub

    ''' <summary>
    ''' Deal with the Load button (initiate ADT loading on press)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadButton.Click
        LoadButton.Hide()
        OKButton.Show()
        OKButton.Enabled = False
        LoadADT()
        OKButton.Enabled = True
        If AutoClose.Checked Then Me.Close()
    End Sub

    ''' <summary>
    ''' Upon loading of the form, fetch the WMOs and get some preliminary data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OpenADTOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim BR As BinaryReader

        'read the wmo file into memory (faster)
        BR = New BinaryReader(myMPQ.LoadFile(_FileName))
        ReDim _ADTFile(BR.BaseStream.Length - 1)
        _ADTFile = BR.ReadBytes(BR.BaseStream.Length)

        'parse Root WMO
        _ADT.Load(_ADTFile)

        'determine all wmo's
        For i As Integer = 0 To _ADT.WMOPlacements.Count - 1
            _WMOs.Add(_ADT.WMOFiles(_ADT.WMOPlacements(i).MWMO_ID))
            ListViewWMO.Items.Add(myHF.GetBaseName(_ADT.WMOFiles(_ADT.WMOPlacements(i).MWMO_ID)))
            ListViewWMO.Items(ListViewWMO.Items.Count - 1).Checked = True
        Next

        'determine all m2's
        For i As Integer = 0 To _ADT.M2Placements.Count - 1
            _M2s.Add(_ADT.ModelFiles(_ADT.M2Placements(i).MMDX_ID))
            ListViewM2.Items.Add(myHF.GetBaseName(_ADT.ModelFiles(_ADT.M2Placements(i).MMDX_ID)))
            ListViewM2.Items(ListViewM2.Items.Count - 1).Checked = True
        Next

        'display stats
        Label5.Text = _FileName
        Label6.Text = _WMOs.Count
        Label7.Text = _M2s.Count
        Label8.Text = ""
        Label9.Text = ""

    End Sub

    ''' <summary>
    ''' Load an ADT with all the necessary bells and whistles (i.e. WMOs, ADTs, ...)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadADT()

        'reset data structures
        Models.Clear()

        Dim model As New sModel(myHF.GetBaseName(_FileName))

        'load textures
        StatusLabel1.Text = "Loading Textures... 0/" & _ADT.TextureFiles.Length
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        Application.DoEvents()

        Dim BLP As New FileReaders.BLP

        'synthesize textures
        For i As Integer = 0 To 7
            For j As Integer = 0 To 7
                StatusLabel1.Text = "Synthesizing Textures... " & i * 8 + j & "/" & 256
                ProgressBar1.Value = 100 * (i * 8 + j) / 256
                ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
                Application.DoEvents()

                Dim TexKey As String = "Combined_" & i.ToString("00") & j.ToString("00")
                'model.Textures.Add(TexKey, New sTexture(Blend(i, j)))
            Next
        Next

        'load and parse ADT
        StatusLabel1.Text = "Loading Tiles... 0/256"
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        Application.DoEvents()

        'First some fun with coordinates
        '
        ' The WOW world looks like this (by continent/region, i.e. Azeroth, Kalimdor, Outland, Northrend
        '
        ' The coordinate systems is like this:
        ' +17068/+17068                                          -17068/+17068
        '                                  0/0
        ' +17068/-17068                                          -17068/-17068
        '
        ' so, basically, X is reversed to what we are expecting and everything is offset by -17068/-17068
        '
        ' Tiles are organized like this:
        '
        ' 00/00  01/00  02/00  03/00  04/00 ... 62/00  63/00
        ' 00/01  01/01  02/01  03/01  04/01 ... 62/01  63/01
        '  ...    ...    ...    ...    ...       ...    ...
        ' 00/62  01/62  02/62  03/62  04/62 ... 62/62  63/62
        ' 00/63  01/63  02/63  03/63  04/63 ... 62/63  63/63
        '
        '
        ' Each of these squares is 533.3333 x 533.3333 units big (1600/3) and is in one ADT called region_xx_yy.adt
        '
        ' Each of these squares is subdivided within the ADT into 16x16 (256) squares, so each of these squares is 
        ' 33.3333 x 33.3333 units big (1600/48)
        '
        ' Each of these squares is overlayed with a grid of 145 heigth values in two different matrices:
        '
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
        '
        ' One subsquare of the 9x9 matrix is 4.1666 x 4.1666 units big (1600/384), if you take both matrices into account,
        ' the resolution is around 2.0833 x 2.0833 units.
        '
        ' Ok, so why do we care? Because the location of items on the map are in GLOBAL coordinates, not relative ones...
        ' The coordinates given in the MCNK chunks are calculated from the MIDDLE of the grid, which is to say that they have to be corrected as follows:
        '
        ' X = 32 * (1600 / 3) - X°
        ' Y = 32 * (1600 / 3) - Y° 

        Dim s As String() = myHF.GetBaseName(_FileName).Split("_")

        Dim TileX As Single = Val(s(s.Count - 2))
        Dim TileY As Single = Val(s(s.Count - 1))

        Dim dx As Single = (1600.0F / 384.0F)
        Dim dy As Single = (1600.0F / 384.0F)

        For i As Integer = 0 To 15
            For j As Integer = 0 To 15

                StatusLabel1.Text = "Loading Tiles... " & (i * 16 + j) & "/256"
                ProgressBar1.Value = 100 * (i * 16 + j) / 256
                ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
                Application.DoEvents()

                With _ADT.MCNKs(i, j)

                    Dim xo As Single = (i - 8) * (1600.0F / 48.0F)
                    Dim yo As Single = (j - 8) * (1600.0F / 48.0F)
                    Dim zo As Single = .Position.Z
                    Dim xt As Single
                    Dim yt As Single
                    Dim TexKey As String = "Combined_" & i.ToString("00") & j.ToString("00")
                    Dim SubMesh As New sSubMesh
                    SubMesh.TextureList.Add(New sTextureEntry(TexKey, "", 0, 0, 0, 0))

                    For x As Integer = 0 To 7
                        Dim xr As Single = xo + x * dx

                        xt = 0.125 * x
                        For y As Integer = 0 To 7

                            Dim yr As Single = yo + y * dy

                            yt = 0.125 * y
                            Dim v As sVertex() = New sVertex() { _
                                    New sVertex(-(xr + 0.0 * dx), yr + 0.0 * dy, zo + .HeightMap9x9(x + 0, y + 0), 0, 0, 1, xt, yt), _
                                    New sVertex(-(xr + 0.0 * dx), yr + 1.0 * dy, zo + .HeightMap9x9(x + 0, y + 1), 0, 0, 1, xt, yt + 0.125), _
                                    New sVertex(-(xr + 1.0 * dx), yr + 1.0 * dy, zo + .HeightMap9x9(x + 1, y + 1), 0, 0, 1, xt + 0.125, yt + 0.125), _
                                    New sVertex(-(xr + 1.0 * dx), yr + 0.0 * dy, zo + .HeightMap9x9(x + 1, y + 0), 0, 0, 1, xt + 0.125, yt), _
                                    New sVertex(-(xr + 0.5 * dx), yr + 0.5 * dy, zo + .HeightMap8x8(x + 0, y + 0), 0, 0, 1, xt + 0.0625, yt + 0.0625) _
                            }

                            Dim vi As Integer = model.AddVertices(v)
                            SubMesh.TriangleList.Add(New sTriangle(vi + 0, vi + 1, vi + 4))
                            SubMesh.TriangleList.Add(New sTriangle(vi + 1, vi + 2, vi + 4))
                            SubMesh.TriangleList.Add(New sTriangle(vi + 2, vi + 3, vi + 4))
                            SubMesh.TriangleList.Add(New sTriangle(vi + 3, vi + 0, vi + 4))

                        Next
                    Next
                    model.Meshes.Add(SubMesh)
                End With
            Next
        Next

        Models.Add(model)

        If _ADT.WMOPlacements.Count > 0 Then
            For i As Integer = 0 To ListViewWMO.Items.Count - 1
                If ListViewWMO.Items(i).Checked Then
                    _WMOSelected.Add(i)
                End If
            Next

            StatusLabel1.Text = "Loading WMOs... 0/" & _WMOSelected.Count
            ProgressBar1.Value = 0
            ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
            Application.DoEvents()

            For i As Integer = 0 To _WMOSelected.Count - 1
                With _ADT.WMOPlacements(_WMOSelected.ElementAt(i))

                    Dim Modelfile As String = _ADT.WMOFiles(.MWMO_ID)

                    StatusLabel1.Text = "Loading WMOs... " & i & "/" & _WMOSelected.Count
                    ProgressBar1.Value = 100 * (i / _WMOSelected.Count)
                    ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
                    Application.DoEvents()

                    ListBox1.Items.Add("WMO: " & Modelfile.ToLower)
                    ListBox1.SelectedIndex = ListBox1.Items.Count - 1

                    myHF.AddWMOModel(Modelfile, RelPosToAbsPos(TileX, TileY, .Position), RelRotToAbsRot(.Orientation))

                End With
            Next
        End If

        If _ADT.M2Placements.Count > 0 Then
            For i As Integer = 0 To ListViewM2.Items.Count - 1
                If ListViewM2.Items(i).Checked Then
                    _M2Selected.Add(i)
                End If
            Next

            StatusLabel1.Text = "Loading M2s... 0/" & _M2Selected.Count
            ProgressBar1.Value = 0
            ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
            Application.DoEvents()

            For i As Integer = 0 To _M2Selected.Count - 1
                With _ADT.M2Placements(_M2Selected.ElementAt(i))

                    Dim Modelfile As String = _ADT.ModelFiles(.MMDX_ID)

                    StatusLabel1.Text = "Loading M2s... " & i & "/" & _M2Selected.Count
                    ProgressBar1.Value = 100 * (i / _M2Selected.Count)
                    ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
                    Application.DoEvents()

                    ListBox1.Items.Add("M2: " & Modelfile.ToLower)
                    ListBox1.SelectedIndex = ListBox1.Items.Count - 1

                    myHF.AddM2Model(Modelfile, RelPosToAbsPos(TileX, TileY, .Position), RelRotToAbsRot(.Orientation), .Scale)

                End With
            Next
        End If

        StatusLabel1.Text = _FileName
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
    End Sub

    ''' <summary>
    ''' If OK is pressed, close the form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

    Private Sub LoadWMOs_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadWMOs.CheckedChanged
        For i As Integer = 0 To ListViewWMO.Items.Count - 1
            ListViewWMO.Items(i).Checked = LoadWMOs.Checked
        Next
    End Sub

    Private Sub LoadM2s_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadM2s.CheckedChanged
        For i As Integer = 0 To ListViewM2.Items.Count - 1
            ListViewM2.Items(i).Checked = LoadM2s.Checked
        Next
    End Sub

    Private Function RelPosToAbsPos(ByVal TileX As Single, ByVal TileY As Single, ByVal Pos As sVector3) As sVector3
        Dim x As Single = Pos.X
        Dim y As Single = Pos.Y
        Dim z As Single = Pos.Z
        Dim nx As Single
        Dim ny As Single
        Dim nz As Single

        Dim BaseX As Single = (32 - TileX) * (1600.0F / 3.0F)
        Dim BaseY As Single = (32 - TileY) * (1600.0F / 3.0F)


        nx = -(x - 32.0F * (1600.0F / 3.0F) + BaseX - 1600.0F / 6.0F)
        ny = z - 32.0F * (1600.0F / 3.0F) + BaseY - 1600.0F / 6.0F
        nz = y

        Return New sVector3(nx, ny, nz)

    End Function

    Private Function RelRotToAbsRot(ByVal Orientation As sVector3) As sQuaternion
        'turn orientation into quaternion
        Dim nxRot As Single = Orientation.Z '.Orientation.Z 'this can be X or Y (uncertain)
        Dim nyRot As Single = -Orientation.X '-.Orientation.X  ' this can be X or Y (uncertain)
        Dim nzRot As Single = 90 + Orientation.Y 'this IS z-rot

        Return sQuaternion.FromRotationAnglesDEG(nxRot, nyRot, nzRot)
    End Function

    Private Sub SaveADTDialog_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles SaveADTDialog.FileOk
        Dim BLP As New FileReaders.BLP

        Dim Fullname As String = SaveADTDialog.FileName
        Dim Directory As String = myHF.GetBasePath(Fullname)
        Dim Lines As New List(Of String)
        Dim n As Integer

        Lines.Clear()
        For Each Tex As String In _ADT.TextureFiles
            Lines.Add(n.ToString("000") & " " & Tex)
            n += 1
        Next

        File.WriteAllLines(Directory & "/textures.txt", Lines.ToArray)

        Depthmap(Directory & "/")

        For x As Integer = 0 To 15
            For y As Integer = 0 To 15
                Dim ChunkID As String = "(" & x.ToString("00") & "," & y.ToString("00") & ")"
                With _ADT.MCNKs(x, y)

                    n = 0
                    For Each Map As Bitmap In .AlphaMaps
                        If Not Map Is Nothing Then
                            Map.Save(Directory & "/" & ChunkID & "_alpha" & n.ToString("00") & ".png", ImageFormat.Png)
                        End If
                        n += 1
                    Next

                    n = 0
                    Lines.Clear()
                    For Each Layer As FileReaders.ADT.sLayer In .Layer
                        If n = 0 Or Not .AlphaMaps(n) Is Nothing Then
                            Dim ID As Integer = Layer.TextureID
                            Dim Tex As String = _ADT.TextureFiles(ID)
                            Lines.Add(n.ToString("000") & " " & Tex)
                        End If
                        n += 1
                    Next
                    File.WriteAllLines(Directory & "/" & ChunkID & "_layers.txt", Lines.ToArray)

                    Application.DoEvents()
                    If x = y And False Then
                        Dim timer As Long = Now.Ticks
                        Blend(x, y, Directory & "/" & ChunkID).Save(Directory & "/" & ChunkID & "_combined.png", ImageFormat.Png)
                        Debug.Print("(" & x.ToString("00") & "/" & y.ToString("00") & "): " & CSng(Now.Ticks - timer) / 10000000)
                    End If
                End With
            Next
        Next

        For i As Integer = 0 To _ADT.TextureFiles.Length - 1
            Dim TexFi As String = _ADT.TextureFiles(i)
            If myMPQ.Locate(TexFi) Then
                Dim TexImg As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(TexFi), TexFi)
                If Not TexImg Is Nothing Then TexImg.Save(Directory & "/" & myHF.GetBaseName(TexFi) & ".png", System.Drawing.Imaging.ImageFormat.Png)
            End If
        Next

    End Sub

    Private Sub DumpADTToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DumpADTToolStripMenuItem.Click
        Dim FullName As String = _FileName
        Dim i As Integer = FullName.LastIndexOf("\")
        If i > 0 Then FullName = FullName.Substring(i + 1)
        i = FullName.LastIndexOf(".")
        If i > 0 Then FullName = FullName.Substring(0, i)

        SaveADTDialog.OverwritePrompt = True
        SaveADTDialog.FileName = FullName
        SaveADTDialog.InitialDirectory = "d:\temp\test" 'System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        SaveADTDialog.Filter = "Text File (*.txt)|*.txt"
        SaveADTDialog.ShowDialog()
    End Sub

    Public Function Depthmap(Optional ByVal Path As String = "") As Bitmap
        Dim dm(128, 128) As Single
        Dim zh As Single = -99999
        Dim zl As Single = 99999
        Dim z As Single

        For x As Integer = 0 To 15
            For y As Integer = 0 To 15
                For i As Integer = 0 To 7
                    For j As Integer = 0 To 7
                        z = _ADT.MCNKs(x, y).HeightMap8x8(i, j) + _ADT.MCNKs(x, y).Position.Z
                        dm(x * 8 + i, y * 8 + j) = z
                        zl = IIf(z < zl, z, zl)
                        zh = IIf(z > zh, z, zh)
                    Next
                Next
            Next
        Next

        Dim Depthm As New Bitmap(128, 128, PixelFormat.Format32bppArgb)
        Dim f1 As Single
        Dim f2 As Single = 255 / (zh - zl)

        For x As Integer = 0 To 127
            For y As Integer = 0 To 127
                f1 = (dm(x, y) - zl) * f2
                Depthm.SetPixel(x, y, Color.FromArgb(255, f1, f1, f1))
            Next
        Next

        If Path > "" Then
            'Dim ChunkID As String = "(" & x.ToString("00") & "," & y.ToString("00") & ")"
            Depthm.Save(Path + "Depthmap.png", ImageFormat.Png)
        End If

        Return Depthm
    End Function

    Public Function Blend(ByVal x As Integer, ByVal y As Integer, Optional ByVal Path As String = "") As Bitmap

        With _ADT.MCNKs(x, y)

            Dim BLP As New FileReaders.BLP

            Dim Layer0 As New Bitmap(2048, 2048, PixelFormat.Format32bppArgb)
            Dim Layer1 As New Bitmap(2048, 2048, PixelFormat.Format32bppArgb)
            Dim Layer2 As New Bitmap(2048, 2048, PixelFormat.Format32bppArgb)
            Dim Layer3 As New Bitmap(2048, 2048, PixelFormat.Format32bppArgb)

            Dim Alpha1 As New Bitmap(2048, 2048, PixelFormat.Format32bppArgb)
            Dim Alpha2 As New Bitmap(2048, 2048, PixelFormat.Format32bppArgb)
            Dim Alpha3 As New Bitmap(2048, 2048, PixelFormat.Format32bppArgb)

            'First iteration (create pure layers)
            Dim Tex0 As String = _ADT.TextureFiles(.Layer(0).TextureID)
            Dim Tex1 As String = _ADT.TextureFiles(.Layer(1).TextureID)
            Dim Tex2 As String = _ADT.TextureFiles(.Layer(2).TextureID)
            Dim Tex3 As String = _ADT.TextureFiles(.Layer(3).TextureID)
            Dim Do0 As Boolean = myMPQ.Locate(Tex0)
            Dim Do1 As Boolean = myMPQ.Locate(Tex1) And Not .AlphaMaps(1) Is Nothing
            Dim Do2 As Boolean = myMPQ.Locate(Tex2) And Not .AlphaMaps(2) Is Nothing
            Dim Do3 As Boolean = myMPQ.Locate(Tex3) And Not .AlphaMaps(3) Is Nothing

            Dim TexImg0 As Bitmap
            Dim TexImg1 As Bitmap
            Dim TexImg2 As Bitmap
            Dim TexImg3 As Bitmap

            If Do0 Then TexImg0 = BLP.LoadFromStream(myMPQ.LoadFile(Tex0), Tex0)
            If Do1 Then TexImg1 = BLP.LoadFromStream(myMPQ.LoadFile(Tex1), Tex1)
            If Do2 Then TexImg2 = BLP.LoadFromStream(myMPQ.LoadFile(Tex2), Tex2)
            If Do3 Then TexImg3 = BLP.LoadFromStream(myMPQ.LoadFile(Tex3), Tex3)

            Dim grl0 As Graphics = Graphics.FromImage(Layer0)
            Dim grl1 As Graphics = Graphics.FromImage(Layer1)
            Dim grl2 As Graphics = Graphics.FromImage(Layer2)
            Dim grl3 As Graphics = Graphics.FromImage(Layer3)

            Dim fr_rect As New Rectangle(0, 0, 256, 256)

            For lx As Integer = 0 To 7
                For ly As Integer = 0 To 7

                    Dim to_rect As New Rectangle(lx * 256, ly * 256, 256, 256)
                    If Do0 Then grl0.DrawImage(TexImg0, to_rect, fr_rect, GraphicsUnit.Pixel)
                    If Do1 Then grl1.DrawImage(TexImg1, to_rect, fr_rect, GraphicsUnit.Pixel)
                    If Do2 Then grl2.DrawImage(TexImg2, to_rect, fr_rect, GraphicsUnit.Pixel)
                    If Do3 Then grl3.DrawImage(TexImg3, to_rect, fr_rect, GraphicsUnit.Pixel)
                Next
            Next

            Dim gra1 As Graphics = Graphics.FromImage(Alpha1)
            Dim gra2 As Graphics = Graphics.FromImage(Alpha2)
            Dim gra3 As Graphics = Graphics.FromImage(Alpha3)

            If Do1 Then
                gra1.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                gra1.DrawImage(.AlphaMaps(1), New Rectangle(0, 0, 2048, 2048), New Rectangle(0, 0, 64, 64), GraphicsUnit.Pixel)
            End If

            If Do2 Then
                gra2.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                gra2.DrawImage(.AlphaMaps(2), New Rectangle(0, 0, 2048, 2048), New Rectangle(0, 0, 64, 64), GraphicsUnit.Pixel)
            End If

            If Do3 Then
                gra3.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                gra3.DrawImage(.AlphaMaps(3), New Rectangle(0, 0, 2048, 2048), New Rectangle(0, 0, 64, 64), GraphicsUnit.Pixel)
            End If

            If Do0 And Path > "" Then Layer0.Save(Path & "_layer0.png", ImageFormat.Png)
            If Do1 And Path > "" Then Layer1.Save(Path & "_layer1.png", ImageFormat.Png)
            If Do1 And Path > "" Then Alpha1.Save(Path & "_alpha1.png", ImageFormat.Png)
            If Do2 And Path > "" Then Layer2.Save(Path & "_layer2.png", ImageFormat.Png)
            If Do2 And Path > "" Then Alpha2.Save(Path & "_alpha2.png", ImageFormat.Png)
            If Do3 And Path > "" Then Layer3.Save(Path & "_layer3.png", ImageFormat.Png)
            If Do3 And Path > "" Then Alpha3.Save(Path & "_alpha3.png", ImageFormat.Png)

            'now combine the layers into ONE texture

            Dim Combined As New Bitmap(2048, 2048, PixelFormat.Format32bppArgb)

            Dim c0 As Integer
            Dim c0r As Integer
            Dim c0g As Integer
            Dim c0b As Integer
            Dim c1 As Integer
            Dim c1r As Integer
            Dim c1g As Integer
            Dim c1b As Integer

            Dim al As Single

            Dim stco As Integer
            Dim scco As IntPtr

            Dim stl0 As Integer
            Dim scl0 As IntPtr
            Dim stl1 As Integer
            Dim scl1 As IntPtr
            Dim stl2 As Integer
            Dim scl2 As IntPtr
            Dim stl3 As Integer
            Dim scl3 As IntPtr

            Dim sta1 As Integer
            Dim sca1 As IntPtr
            Dim sta2 As Integer
            Dim sca2 As IntPtr
            Dim sta3 As Integer
            Dim sca3 As IntPtr

            Dim CombinedData As BitmapData

            Dim Layer0Data As BitmapData
            Dim Layer1Data As BitmapData
            Dim Layer2Data As BitmapData
            Dim Layer3Data As BitmapData

            Dim Alpha1Data As BitmapData
            Dim Alpha2Data As BitmapData
            Dim Alpha3Data As BitmapData

            Dim Rect As New Rectangle(0, 0, 2048, 2048)

            CombinedData = Combined.LockBits(Rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)
            stco = CombinedData.Stride
            scco = CombinedData.Scan0

            If Do0 Then
                Layer0Data = Layer0.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                stl0 = Layer0Data.Stride
                scl0 = Layer0Data.Scan0
            End If
            If Do1 Then
                Layer1Data = Layer1.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                stl1 = Layer1Data.Stride
                scl1 = Layer1Data.Scan0
                Alpha1Data = Alpha1.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                sta1 = Alpha1Data.Stride
                sca1 = Alpha1Data.Scan0
            End If
            If Do2 Then
                Layer2Data = Layer2.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                stl2 = Layer2Data.Stride
                scl2 = Layer2Data.Scan0
                Alpha2Data = Alpha2.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                sta2 = Alpha2Data.Stride
                sca2 = Alpha2Data.Scan0
            End If
            If Do3 Then
                Layer3Data = Layer3.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                stl3 = Layer3Data.Stride
                scl3 = Layer3Data.Scan0
                Alpha3Data = Alpha3.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                sta3 = Alpha3Data.Stride
                sca3 = Alpha3Data.Scan0
            End If

            For lx As Integer = 0 To 2047
                For ly As Integer = 0 To 2047
                    c0 = Marshal.ReadInt32(scl0, stl0 * ly + 4 * lx)
                    c0r = c0 >> 16 And 255
                    c0g = c0 >> 8 And 255
                    c0b = c0 And 255

                    'color = ((layer0 * (1- alpha1) + layer1 * alpha1) * (1 - alpha2) + layer2 * alpha2) * (1 - alpha3) + layer3 * alpha3

                    If Do1 Then
                        c1 = Marshal.ReadInt32(scl1, stl1 * ly + 4 * lx)
                        al = Marshal.ReadByte(sca1, sta1 * ly + 4 * lx + 3) / 255

                        c1r = c1 >> 16 And 255
                        c1g = c1 >> 8 And 255
                        c1b = c1 And 255

                        c0r = c0r * (1 - al) + c1r * al
                        c0g = c0g * (1 - al) + c1g * al
                        c0b = c0b * (1 - al) + c1b * al

                    End If

                    If Do2 Then
                        c1 = Marshal.ReadInt32(scl2, stl2 * ly + 4 * lx)
                        al = Marshal.ReadByte(sca2, sta2 * ly + 4 * lx + 3) / 255

                        c1r = c1 >> 16 And 255
                        c1g = c1 >> 8 And 255
                        c1b = c1 And 255

                        c0r = c0r * (1 - al) + c1r * al
                        c0g = c0g * (1 - al) + c1g * al
                        c0b = c0b * (1 - al) + c1b * al

                    End If

                    If Do3 Then
                        c1 = Marshal.ReadInt32(scl3, stl3 * ly + 4 * lx)
                        al = Marshal.ReadByte(sca3, sta3 * ly + 4 * lx + 3) / 255

                        c1r = c1 >> 16 And 255
                        c1g = c1 >> 8 And 255
                        c1b = c1 And 255

                        c0r = c0r * (1 - al) + c1r * al
                        c0g = c0g * (1 - al) + c1g * al
                        c0b = c0b * (1 - al) + c1b * al

                    End If

                    c0 = &HFF000000 + (c0r << 16) + (c0g << 8) + c0b

                    Marshal.WriteInt32(scco, stco * ly + 4 * lx, c0)
                Next
            Next

            Combined.UnlockBits(CombinedData)
            If Do0 Then
                Layer0.UnlockBits(Layer0Data)
            End If
            If Do1 Then
                Layer1.UnlockBits(Layer1Data)
                Alpha1.UnlockBits(Alpha1Data)
            End If
            If Do2 Then
                Layer2.UnlockBits(Layer2Data)
                Alpha2.UnlockBits(Alpha2Data)
            End If
            If Do3 Then
                Layer3.UnlockBits(Layer3Data)
                Alpha3.UnlockBits(Alpha3Data)
            End If

            Return Combined
        End With
    End Function

End Class
