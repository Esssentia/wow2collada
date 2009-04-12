Imports System.Windows.Forms
Imports System.IO
Imports System.Collections.Generic
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Public Class OpenWMOOptions

    Private _WMO As FileReaders.WMO
    Private _FileName As String
    Private _RootWMO As Byte()
    Private _SubWMO As New List(Of String)
    Private _DoodadSets As New Dictionary(Of String, FileReaders.WMO.sDoodadSet)
    Private _DoodadSelected As New List(Of Integer)

    ''' <summary>
    ''' Form Initialization, sets the WMO filename and initiates an appropriate filereader
    ''' </summary>
    ''' <param name="FileName">Name of the WMO file to open</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal FileName As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _FileName = FileName
        _WMO = New FileReaders.WMO

    End Sub

    ''' <summary>
    ''' Deal with the Load button (initiate WMO loading on press)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadButton.Click
        LoadButton.Hide()
        OKButton.Show()
        OKButton.Enabled = False
        LoadWMO()
        OKButton.Enabled = True
        If AutoClose.Checked Then Me.Close()
    End Sub

    ''' <summary>
    ''' Upon loading of the form, fetch the WMOs and get some preliminary data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OpenWMOOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim BR As BinaryReader

        'read the wmo file into memory (faster)
        BR = New BinaryReader(myMPQ.LoadFile(_FileName))
        ReDim _RootWMO(BR.BaseStream.Length - 1)
        _RootWMO = BR.ReadBytes(BR.BaseStream.Length)

        'parse Root WMO
        _WMO.LoadRoot(_RootWMO)

        'determine all sub wmo's 
        Dim BaseName As String = myHF.GetBasePath(_FileName) & "\" & myHF.GetBaseName(_FileName) & "_"

        Dim n As Integer = 0
        Dim SubFile As String = BaseName & n.ToString.PadLeft(3, "0") & ".wmo"
        While myMPQ.Locate(SubFile)
            _SubWMO.Add(SubFile)
            n += 1
            SubFile = BaseName & n.ToString.PadLeft(3, "0") & ".wmo"
        End While

        'list all doodad sets
        For i As Integer = 0 To _WMO.nSets - 1
            With _WMO.DoodadSets(i)
                ListView1.Items.Add(.name)
                ListView1.Items(ListView1.Items.Count - 1).SubItems.Add(.count)
                _DoodadSets.Add(.name, _WMO.DoodadSets(i))
            End With
        Next
        If ListView1.Items.Count > 0 Then ListView1.Items(0).Checked = True

        'display stats
        Label5.Text = _FileName
        Label6.Text = _SubWMO.Count
        Label7.Text = _WMO.nMaterials
        Label8.Text = _WMO.nDoodads
        Label9.Text = _WMO.nSets

    End Sub

    ''' <summary>
    ''' Load a WMO with all the necessary bells and whistles (i.e. Subsets, Doodads, ...)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadWMO()
        Dim BR As BinaryReader

        'reset data structures
        myHF.m_Textures.Clear()
        myHF.m_TriangleList.Clear()

        'load and parse Sub WMOs
        StatusLabel1.Text = "Loading Sub-WMO... 0/" & _SubWMO.Count
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        Application.DoEvents()

        For i As Integer = 0 To _SubWMO.Count - 1

            StatusLabel1.Text = "Loading Sub-WMO ... " & i & "/" & _SubWMO.Count
            ProgressBar1.Value = 100 * (i / _SubWMO.Count)
            ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
            Application.DoEvents()

            BR = New BinaryReader(myMPQ.LoadFile(_SubWMO(i)))
            _WMO.LoadSub(BR.ReadBytes(BR.BaseStream.Length))
        Next

        'load textures
        StatusLabel1.Text = "Loading Textures... 0/" & _WMO.Textures.Length
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        Application.DoEvents()

        Dim BLP As New FileReaders.BLP

        For i As Integer = 0 To _WMO.Textures.Length - 1

            StatusLabel1.Text = "Loading Textures... " & i & "/" & _WMO.Textures.Length
            ProgressBar1.Value = 100 * (i / _WMO.Textures.Length)
            ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
            Application.DoEvents()

            Dim TexFi As String = _WMO.Textures(i)
            If myMPQ.Locate(TexFi) Then
                If Not wow2collada.myHF.m_Textures.ContainsKey(TexFi) Then
                    Dim Tex As New wow2collada.HelperFunctions.sTexture
                    Dim TexImg As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(TexFi), TexFi)
                    If Not TexImg Is Nothing Then
                        Tex.FileName = TexFi
                        Tex.TexGra = TexImg
                        Tex.TexObj = Texture.FromBitmap(render.m_Device, TexImg, Usage.None, Pool.Managed)
                        myHF.m_Textures(TexFi) = Tex
                    End If
                End If
            End If
        Next

        'load subsets
        StatusLabel1.Text = "Loading Subsets..."
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        Application.DoEvents()

        Dim vi As Integer = 0
        Dim ti As Integer = 0
        For i As Integer = 0 To _WMO.SubSets.Count - 1

            StatusLabel1.Text = "Loading Subsets... " & i & "/" & _WMO.SubSets.Length
            ProgressBar1.Value = 100 * (i / _WMO.SubSets.Length)
            ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
            Application.DoEvents()

            For j As Integer = 0 To _WMO.SubSets(i).Triangles.Length - 1
                Dim MatID As Byte = _WMO.SubSets(i).Materials(j)
                If MatID < _WMO.Textures.Length Then
                    Dim Tri As New HelperFunctions.sTriangle
                    Dim V1 As HelperFunctions.sVertex
                    Dim V2 As HelperFunctions.sVertex
                    Dim V3 As HelperFunctions.sVertex

                    With _WMO.SubSets(i).Triangles(j)
                        V1.Position = _WMO.SubSets(i).Vertices(.VertexIndex1).Position
                        V1.Normal = _WMO.SubSets(i).Vertices(.VertexIndex1).Normal
                        V1.UV = _WMO.SubSets(i).Vertices(.VertexIndex1).TextureCoords

                        V2.Position = _WMO.SubSets(i).Vertices(.VertexIndex2).Position
                        V2.Normal = _WMO.SubSets(i).Vertices(.VertexIndex2).Normal
                        V2.UV = _WMO.SubSets(i).Vertices(.VertexIndex2).TextureCoords

                        V3.Position = _WMO.SubSets(i).Vertices(.VertexIndex3).Position
                        V3.Normal = _WMO.SubSets(i).Vertices(.VertexIndex3).Normal
                        V3.UV = _WMO.SubSets(i).Vertices(.VertexIndex3).TextureCoords

                        Tri.P = New HelperFunctions.sVertex() {V1, V2, V3}
                        Tri.TextureID = _WMO.Textures(MatID)

                        myHF.m_TriangleList.Add(Tri)
                    End With
                End If
            Next
        Next

        If _WMO.nDoodads > 0 And LoadDoodads.Checked Then
            For i As Integer = 0 To ListView1.Items.Count - 1
                If ListView1.Items(i).Checked Then
                    Dim index As Integer = _DoodadSets(ListView1.Items(i).Text).index
                    Dim count As Integer = _DoodadSets(ListView1.Items(i).Text).count
                    For j As Integer = index To index + count - 1
                        _DoodadSelected.Add(j)
                    Next
                End If
            Next

            StatusLabel1.Text = "Loading Doodads... 0/" & _DoodadSelected.Count '_WMO.Doodads.Length
            ProgressBar1.Value = 0
            ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
            Application.DoEvents()

            For i As Integer = 0 To _DoodadSelected.Count - 1 '_WMO.Doodads.Length - 1
                With _WMO.Doodads(_DoodadSelected(i))
                    If .ModelFile > "" Then

                        StatusLabel1.Text = "Loading Doodads... " & i & "/" & _DoodadSelected.Count '_WMO.Doodads.Length
                        ProgressBar1.Value = 100 * (i / _DoodadSelected.Count)
                        ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
                        Application.DoEvents()

                        ListBox1.Items.Add("Doodad: " & .ModelFile.ToLower)
                        ListBox1.SelectedIndex = ListBox1.Items.Count - 1

                        Dim MD20 As New FileReaders.M2
                        Dim SKIN As New FileReaders.SKIN
                        Dim FileNameMD20 As String = .ModelFile.Substring(0, .ModelFile.LastIndexOf(".")) + ".m2"
                        Dim FileNameSKIN As String = .ModelFile.Substring(0, .ModelFile.LastIndexOf(".")) + "00.skin"
                        MD20.LoadFromStream(myMPQ.LoadFile(FileNameMD20), FileNameMD20)
                        SKIN.LoadFromStream(myMPQ.LoadFile(FileNameSKIN), FileNameSKIN)
                        render.CreateVertexBufferFromM2(MD20, SKIN, .Position, .Orientation, .Scale)
                    End If
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

End Class
