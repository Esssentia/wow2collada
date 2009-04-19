Imports System.Windows.Forms
Imports System.IO
Imports System.Collections.Generic
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
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
        myHF.Textures.Clear()
        myHF.SubMeshes.Clear()


        'load textures
        StatusLabel1.Text = "Loading Textures... 0/" & _ADT.TextureFiles.Length
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        Application.DoEvents()

        Dim BLP As New FileReaders.BLP

        For i As Integer = 0 To _ADT.TextureFiles.Length - 1

            StatusLabel1.Text = "Loading Textures... " & i & "/" & _ADT.TextureFiles.Length
            ProgressBar1.Value = 100 * (i / _ADT.TextureFiles.Length)
            ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
            Application.DoEvents()

            Dim TexFi As String = _ADT.TextureFiles(i)
            If myMPQ.Locate(TexFi) Then
                If Not myHF.Textures.ContainsKey(TexFi) Then
                    Dim Tex As New HelperFunctions.sTexture
                    Dim TexImg As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(TexFi), TexFi)
                    If Not TexImg Is Nothing Then
                        Tex.ID = TexFi
                        Tex.TexGra = TexImg
                        myHF.Textures(TexFi) = Tex
                    End If
                End If
            End If
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
        ' 00/00  01/00  02/00  03/00  04/00 ... 62/00  63/00
        ' 00/01  01/01  02/01  03/01  04/01 ... 62/01  63/01
        '  ...    ...    ...    ...    ...       ...    ...
        ' 00/62  01/62  02/62  03/62  04/62 ... 62/62  63/62
        ' 00/63  01/63  02/63  03/63  04/63 ... 62/63  63/63
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

        Dim BaseY As Single = Val(s(s.Count - 2))
        Dim BaseX As Single = Val(s(s.Count - 1))

        BaseX = (32 - BaseX) * (1600.0F / 3.0F)
        BaseY = (32 - BaseY) * (1600.0F / 3.0F)

        Dim dx As Single = (1600.0F / 384.0F)
        Dim dy As Single = (1600.0F / 384.0F)

        For i As Integer = 7 To 7 '0 To 15
            For j As Integer = 7 To 7 '0 To 15

                StatusLabel1.Text = "Loading Tiles... " & (i * 16 + j) & "/256"
                ProgressBar1.Value = 100 * (i * 16 + j) / 256
                ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
                Application.DoEvents()

                With _ADT.MCNKs(j, i)

                    Dim SubMesh As sSubMesh
                    SubMesh.TextureList = New System.Collections.Generic.List(Of sTextureEntry)
                    SubMesh.TriangleList = New System.Collections.Generic.List(Of sTriangle)

                    Dim xo As Single = (i - 8) * (1600.0F / 48.0F)
                    Dim yo As Single = (j - 8) * (1600.0F / 48.0F)
                    Dim zo As Single = .Position.Z

                    Dim i0 As Integer = .Layer(0).TextureID - 1
                    Dim i1 As Integer = .Layer(1).TextureID - 1
                    Dim i2 As Integer = .Layer(2).TextureID - 1
                    Dim i3 As Integer = .Layer(3).TextureID - 1

                    Dim s0 As String = ""
                    Dim s1 As String = ""
                    Dim s2 As String = ""
                    Dim s3 As String = ""

                    If i0 > -1 Then s0 = _ADT.TextureFiles(i0)
                    If i1 > -1 Then s1 = _ADT.TextureFiles(i1)
                    If i2 > -1 Then s2 = _ADT.TextureFiles(i2)
                    If i3 > -1 Then s3 = _ADT.TextureFiles(i3)

                    'Dim TexID As String() = New String() {s0, s1, s2, s3}

                    Dim Key As String = "tex" & Rnd() * 1000
                    Dim Tex As New sTexture
                    Dim TexEnt As New sTextureEntry

                    Tex.ID = Key
                    Tex.TexGra = CombineTextures(i, j)
                    TexEnt.TextureID = Key

                    myHF.Textures.Add(Key, Tex)
                    SubMesh.TextureList.Add(TexEnt)

                    For x As Integer = 0 To 7
                        Dim xr As Single = xo + x * dx
                        For y As Integer = 0 To 7
                            Dim yr As Single = yo + y * dy
                            Dim Tri As sTriangle() = New sTriangle() {New sTriangle, New sTriangle, New sTriangle, New sTriangle}

                            Tri(0).P = New sVertex() {New sVertex, New sVertex, New sVertex}
                            Tri(0).P(0).Position = New Vector3(xr + 0.0 * dx, yr + 0.0 * dy, zo + .HeightMap9x9(x + 0, y + 0))
                            Tri(0).P(1).Position = New Vector3(xr + 0.0 * dx, yr + 1.0 * dy, zo + .HeightMap9x9(x + 0, y + 1))
                            Tri(0).P(2).Position = New Vector3(xr + 0.5 * dx, yr + 0.5 * dy, zo + .HeightMap8x8(x + 0, y + 0))
                            Tri(0).P(0).Normal = New Vector3(0, 0, 1)
                            Tri(0).P(1).Normal = New Vector3(0, 0, 1)
                            Tri(0).P(2).Normal = New Vector3(0, 0, 1)
                            Tri(0).P(0).UV = New Vector2(0, 0) 'dummy for now
                            Tri(0).P(1).UV = New Vector2(0, 1) 'dummy for now
                            Tri(0).P(2).UV = New Vector2(0.5, 0.5) 'dummy for now

                            Tri(1).P = New sVertex() {New sVertex, New sVertex, New sVertex}
                            Tri(1).P(0).Position = New Vector3(xr + 0.0 * dx, yr + 1.0 * dy, zo + .HeightMap9x9(x + 0, y + 1))
                            Tri(1).P(1).Position = New Vector3(xr + 1.0 * dx, yr + 1.0 * dy, zo + .HeightMap9x9(x + 1, y + 1))
                            Tri(1).P(2).Position = New Vector3(xr + 0.5 * dx, yr + 0.5 * dy, zo + .HeightMap8x8(x + 0, y + 0))
                            Tri(1).P(0).Normal = New Vector3(0, 0, 1)
                            Tri(1).P(1).Normal = New Vector3(0, 0, 1)
                            Tri(1).P(2).Normal = New Vector3(0, 0, 1)
                            Tri(1).P(0).UV = New Vector2(0, 1) 'dummy for now
                            Tri(1).P(1).UV = New Vector2(1, 1) 'dummy for now
                            Tri(1).P(2).UV = New Vector2(0.5, 0.5) 'dummy for now

                            Tri(2).P = New sVertex() {New sVertex, New sVertex, New sVertex}
                            Tri(2).P(0).Position = New Vector3(xr + 1.0 * dx, yr + 1.0 * dy, zo + .HeightMap9x9(x + 1, y + 1))
                            Tri(2).P(1).Position = New Vector3(xr + 1.0 * dx, yr + 0.0 * dy, zo + .HeightMap9x9(x + 1, y + 0))
                            Tri(2).P(2).Position = New Vector3(xr + 0.5 * dx, yr + 0.5 * dy, zo + .HeightMap8x8(x + 0, y + 0))
                            Tri(2).P(0).Normal = New Vector3(0, 0, 1)
                            Tri(2).P(1).Normal = New Vector3(0, 0, 1)
                            Tri(2).P(2).Normal = New Vector3(0, 0, 1)
                            Tri(2).P(0).UV = New Vector2(1, 1) 'dummy for now
                            Tri(2).P(1).UV = New Vector2(1, 0) 'dummy for now
                            Tri(2).P(2).UV = New Vector2(0.5, 0.5) 'dummy for now

                            Tri(3).P = New sVertex() {New sVertex, New sVertex, New sVertex}
                            Tri(3).P(0).Position = New Vector3(xr + 1.0 * dx, yr + 0.0 * dy, zo + .HeightMap9x9(x + 1, y + 0))
                            Tri(3).P(1).Position = New Vector3(xr + 0.0 * dx, yr + 0.0 * dy, zo + .HeightMap9x9(x + 0, y + 0))
                            Tri(3).P(2).Position = New Vector3(xr + 0.5 * dx, yr + 0.5 * dy, zo + .HeightMap8x8(x + 0, y + 0))
                            Tri(3).P(0).Normal = New Vector3(0, 0, 1)
                            Tri(3).P(1).Normal = New Vector3(0, 0, 1)
                            Tri(3).P(2).Normal = New Vector3(0, 0, 1)
                            Tri(3).P(0).UV = New Vector2(1, 0) 'dummy for now
                            Tri(3).P(1).UV = New Vector2(0, 0) 'dummy for now
                            Tri(3).P(2).UV = New Vector2(0.5, 0.5) 'dummy for now

                            SubMesh.TriangleList.Add(Tri(0))
                            SubMesh.TriangleList.Add(Tri(1))
                            SubMesh.TriangleList.Add(Tri(2))
                            SubMesh.TriangleList.Add(Tri(3))
                        Next
                    Next

                    myHF.SubMeshes.Add(SubMesh)
                End With
            Next
        Next

        'load and parse WMOs
        StatusLabel1.Text = "Loading WMO... 0/" & _WMOs.Count
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        Application.DoEvents()

        For i As Integer = 0 To _WMOs.Count - 1

            StatusLabel1.Text = "Loading Sub-WMO ... " & i & "/" & _WMOs.Count
            ProgressBar1.Value = 100 * (i / _WMOs.Count)
            ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
            Application.DoEvents()

            '    BR = New BinaryReader(myMPQ.LoadFile(_SubWMO(i)))
            '    _WMO.LoadSub(BR.ReadBytes(BR.BaseStream.Length))
        Next

        ''load textures
        'StatusLabel1.Text = "Loading Textures... 0/" & _WMO.Textures.Length
        'ProgressBar1.Value = 0
        'ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        'Application.DoEvents()

        'Dim BLP As New FileReaders.BLP

        'For i As Integer = 0 To _WMO.Textures.Length - 1

        '    StatusLabel1.Text = "Loading Textures... " & i & "/" & _WMO.Textures.Length
        '    ProgressBar1.Value = 100 * (i / _WMO.Textures.Length)
        '    ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
        '    Application.DoEvents()

        '    Dim TexFi As String = _WMO.Textures(i)
        '    If myMPQ.Locate(TexFi) Then
        '        If Not myHF.Textures.ContainsKey(TexFi) Then
        '            Dim Tex As New wow2collada.HelperFunctions.sTexture
        '            Dim TexImg As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(TexFi), TexFi)
        '            If Not TexImg Is Nothing Then
        '                Tex.ID = TexFi
        '                Tex.TexGra = TexImg
        '                myHF.Textures(TexFi) = Tex
        '            End If
        '        End If
        '    End If
        'Next

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

                    Dim MD20 As New FileReaders.M2
                    Dim SKIN As New FileReaders.SKIN
                    Dim FileNameMD20 As String = Modelfile.Substring(0, Modelfile.LastIndexOf(".")) + ".m2"
                    Dim FileNameSKIN As String = Modelfile.Substring(0, Modelfile.LastIndexOf(".")) + "00.skin"
                    MD20.Load(myMPQ.LoadFile(FileNameMD20), FileNameMD20)
                    SKIN.Load(myMPQ.LoadFile(FileNameSKIN), FileNameSKIN)

                    Dim Pos As New Vector3(.Position.Z - 32.0F * (1600.0F / 3.0F) + BaseX - 8 * (1600.0F / 48.0F), .Position.X - 32.0F * (1600.0F / 3.0F) + BaseY - 8 * (1600.0F / 48.0F), .Position.Y)

                    'turn orientation into quaternion
                    Dim xMat As Matrix = Matrix.RotationX(.Orientation.Z * Math.PI / 180) 'this can be X or Y (uncertain)
                    Dim yMat As Matrix = Matrix.RotationY(-.Orientation.X * Math.PI / 180) ' this can be X or Y (uncertain)
                    Dim zMat As Matrix = Matrix.RotationZ(.Orientation.Y * Math.PI / 180) 'this IS z-rot (don't know offset yet)

                    Dim rMat As Matrix = xMat * yMat * zMat
                    Dim Rot As Quaternion = Quaternion.RotationMatrix(rMat)

                    Debug.Print("{0} {1} {2} {3}", Rot.X, Rot.Y, Rot.Z, Rot.W)

                    myHF.CreateVertexBufferFromM2(MD20, SKIN, Pos, Rot, .Scale)

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

    Private Function CombineTexturesOLD(ByVal x As Integer, ByVal y As Integer) As Bitmap
        Dim Out As Bitmap

        With _ADT.MCNKs(x, y)
            Dim bTex As New Dictionary(Of Integer, Bitmap)
            Dim bAlp As New Dictionary(Of Integer, Bitmap)

            For i As Integer = 0 To 3
                Dim iTexID As Integer = .Layer(i).TextureID - 1
                If iTexID > -1 Then
                    Dim sTexID As String = _ADT.TextureFiles(iTexID)
                    If sTexID > "" Then
                        bTex.Add(i, myHF.Textures(sTexID).TexGra.Clone())
                        If Not .AlphaMaps(i) Is Nothing Then bAlp.Add(i, .AlphaMaps(i).Clone())
                    End If
                End If

            Next

            If bTex.Count < 1 Then Return Nothing

            Out = bTex.ElementAt(0).Value

            For i = 1 To 3
                If bTex.ContainsKey(i) Then 'layer active
                    Dim AlphaTex As Bitmap = bTex(i)

                    If bAlp.ContainsKey(i) Then ' with alpha
                        AlphaTex = myHF.CombineBitmaps(AlphaTex, bAlp(i)) 'combine alphatex with alphamap
                    End If
                    Out = myHF.CombineBitmaps(Out, AlphaTex)
                End If
            Next
        End With

        Return Out
    End Function

    Private Function CombineTextures(ByVal x As Integer, ByVal y As Integer) As Bitmap
        Dim Out As Bitmap

        With _ADT.MCNKs(x, y)
            Dim bTex As New Dictionary(Of Integer, Bitmap)
            Dim bAlp As New Dictionary(Of Integer, Bitmap)

            For i As Integer = 0 To 3
                Dim iTexID As Integer = .Layer(i).TextureID - 1
                If iTexID > -1 Then
                    Dim sTexID As String = _ADT.TextureFiles(iTexID)
                    If sTexID > "" Then
                        Dim Bori As Bitmap = myHF.Textures(sTexID).TexGra.Clone()
                        Dim Btmp As New Bitmap(16 * Bori.Width, 16 * Bori.Height, Bori.PixelFormat)

                        For xx As Integer = 0 To Bori.Width - 1
                            For yy As Integer = 0 To Bori.Height - 1
                                Dim Ctmp As Color = Bori.GetPixel(xx, yy)
                                For xi As Integer = 0 To 15
                                    For yi As Integer = 0 To 15
                                        Btmp.SetPixel(xx + Bori.Width * xi, yy + Bori.Height * yi, Ctmp)
                                    Next
                                Next

                            Next
                        Next

                        bTex.Add(i, Btmp)
                        If Not .AlphaMaps(i) Is Nothing Then bAlp.Add(i, .AlphaMaps(i).Clone())
                    End If
                End If

            Next

            If bTex.Count < 1 Then Return Nothing

            Out = bTex.ElementAt(0).Value

            For i = 1 To 3
                If bTex.ContainsKey(i) Then 'layer active
                    Dim AlphaTex As Bitmap = bTex(i)

                    If bAlp.ContainsKey(i) Then ' with alpha
                        AlphaTex = myHF.CombineBitmaps(AlphaTex, bAlp(i)) 'combine alphatex with alphamap
                    End If
                    Out = myHF.CombineBitmaps(Out, AlphaTex)
                End If
            Next
        End With

        Return Out
    End Function




End Class
