﻿Imports System.Windows.Forms
Imports System.IO
Imports System.Collections.Generic

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
        ModelMgr.Clear()
        TextureMgr.Clear()

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

        Dim mdID As Integer = ModelMgr.AddModelData(myHF.GetBaseName(_FileName))
        Dim moID As Integer = ModelMgr.AddModel(myHF.GetBaseName(_FileName))

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

            Dim TexFi As String = _WMO.Textures(i).TexID
            TextureMgr.AddTexture(TexFi, TexFi)
        Next

        'load subsets
        StatusLabel1.Text = "Loading Subsets..."
        ProgressBar1.Value = 0
        ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
        Application.DoEvents()

        For i As Integer = 0 To _WMO.SubSets.Count - 1

            StatusLabel1.Text = "Loading Subsets... " & i & "/" & _WMO.SubSets.Length
            ProgressBar1.Value = 100 * (i / _WMO.SubSets.Length)
            ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * ProgressBar1.Value / 100, 255 * ProgressBar1.Value / 100, 0)
            Application.DoEvents()

            Dim CurrMatID As Integer = -1
            Dim submesh As New sSubMesh

            Dim vi As Integer = ModelMgr.AddVerticesToModelData(mdID, _WMO.SubSets(i).Vertices)

            For j As Integer = 0 To _WMO.SubSets(i).Triangles.Length - 1
                Dim MatID As Byte = _WMO.SubSets(i).Materials(j)
                If MatID < _WMO.Textures.Length Then

                    If MatID <> CurrMatID Then
                        If CurrMatID <> -1 Then ModelMgr.ModelData(mdID).Meshes.Add(submesh)
                        CurrMatID = MatID
                        submesh = New sSubMesh
                        submesh.TextureList.Add(New sTextureEntry(_WMO.Textures(MatID).TexID, "", 0, _WMO.Textures(MatID).Flags, 0, _WMO.Textures(MatID).Blending))

                    End If

                    With _WMO.SubSets(i).Triangles(j)
                        submesh.TriangleList.Add(New sTriangle(vi + .V1, vi + .V2, vi + .V3))
                    End With
                End If
            Next

            ModelMgr.ModelData(mdID).Meshes.Add(submesh)
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
                        myHF.AddM2Model(.ModelFile, .Position, .Orientation, .Scale)
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
