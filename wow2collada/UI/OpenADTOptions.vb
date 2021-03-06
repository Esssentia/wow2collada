﻿Imports System.Windows.Forms
Imports System.IO
Imports System.Collections.Generic
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
        ModelMgr.Clear()
        TextureMgr.Clear()
        SuspendRender = True

        Dim moID As Integer = ModelMgr.AddModel(myHF.GetBaseName(_FileName))
        Dim mdID As Integer = ModelMgr.AddModelData(myHF.GetBaseName(_FileName))
        ModelMgr.Models(moID).ModelDataID = mdID

        'load textures
        StatusLabel1.Text = "Loading Textures... 0/" & _ADT.TextureFiles.Length
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        Application.DoEvents()

        Dim BLP As New FileReaders.BLP
        Dim Key As String

        'add all texture
        For i As Integer = 0 To 15
            For j As Integer = 0 To 15
                StatusLabel1.Text = "Loading Textures... " & i * 8 + j & "/" & 256
                ProgressBar1.Value = 100 * (i * 8 + j) / 256
                ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
                Application.DoEvents()

                For k = 0 To 3
                    If Not _ADT.MCNKs(i, j).AlphaMaps(k) Is Nothing Then
                        Key = "Alpha_" & i.ToString("00") & "_" & j.ToString("00") & "_" & k.ToString("00")
                        TextureMgr.AddTexture(Key, _ADT.MCNKs(i, j).AlphaMaps(k))
                    End If

                    Key = _ADT.TextureFiles(_ADT.MCNKs(i, j).Layer(k).TextureID)
                    TextureMgr.AddTexture(Key, Key)
                Next
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
                    Dim SubMesh As New sSubMesh

                    SubMesh.isADT = True
                    SubMesh.isWotLK = Not .preWotLK

                    For k = 0 To 3
                        If Not _ADT.MCNKs(i, j).AlphaMaps(k) Is Nothing Then
                            Key = "Alpha_" & i.ToString("00") & "_" & j.ToString("00") & "_" & k.ToString("00")
                            SubMesh.TextureList.Add(New sTextureEntry(Key, "", 0, 0, 0, 0, "Alpha" & k.ToString("0")))
                        End If

                        Key = _ADT.TextureFiles(_ADT.MCNKs(i, j).Layer(k).TextureID)
                        If myMPQ.Locate(Key) Then
                            SubMesh.TextureList.Add(New sTextureEntry(Key, "", 0, 0, 0, 0, "Layer" & k.ToString("0")))
                        End If
                    Next

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

                            Dim vi As Integer = ModelMgr.AddVerticesToModelData(mdID, v)
                            SubMesh.TriangleList.Add(New sTriangle(vi + 0, vi + 1, vi + 4))
                            SubMesh.TriangleList.Add(New sTriangle(vi + 1, vi + 2, vi + 4))
                            SubMesh.TriangleList.Add(New sTriangle(vi + 2, vi + 3, vi + 4))
                            SubMesh.TriangleList.Add(New sTriangle(vi + 3, vi + 0, vi + 4))

                        Next
                    Next
                    ModelMgr.ModelData(mdID).Meshes.Add(SubMesh)
                End With
            Next
        Next

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
        frmOG.SetupScene()
        SuspendRender = False
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
        Dim Fullname As String = SaveADTDialog.FileName
        Dim DumpADTDialog As New DumpADT
        DumpADTDialog.Filename = SaveADTDialog.FileName
        DumpADTDialog.ADT = _ADT
        DumpADTDialog.ShowDialog()
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

End Class
