Public Class MainForm

    Public CurrentTexture As String
    Public CurrentFile As String

    Private Sub RenderForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CurrentFile = "World\AZEROTH\WESTFALL\PASSIVEDOODADS\Crate\WestFallCrate.m2"

        For Each i As String In myMPQ.FileTree.Nodes.Keys
            Dim Out As TreeNode = FileList.Nodes.Add(i)
            Out.Nodes.Add("(dummy)")
        Next
        FileList.Sort()

        LoadModelFromMPQ()
    End Sub

    Private Sub QuitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub LoadModelFromMPQ()
        Dim Retval As System.Collections.Generic.List(Of String)
        ListBox1.Items.Clear()
        Retval = myHF.LoadModelFromMPQ(CurrentFile)
        For i As Integer = 0 To Retval.Count - 1
            ListBox1.Items.Add(Retval(i))
        Next
        TrackBar1.Minimum = 0
        TrackBar1.Maximum = Math.Max(0, TextureMgr.Textures.Count - 1)
        TrackBar1.Value = 0
        TrackBar1_ValueChanged(Me, New System.EventArgs)
        StatusLabel1.Text = CurrentFile
        frmOG.SetFileName(CurrentFile)
    End Sub

    Private Sub TreeView1_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles FileList.AfterSelect
        If e.Node.Nodes Is Nothing Or e.Node.Nodes.Count = 0 Then ' only look at leafs
            CurrentFile = e.Node.FullPath
            LoadModelFromMPQ()
        End If
    End Sub

    Private Sub TreeView1_BeforeExpand(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles FileList.BeforeExpand
        If e.Node.Nodes(0).Text = "(dummy)" Then ' not yet filled
            e.Node.Nodes.Clear()
            Dim Parts As String() = e.Node.FullPath().Split("\")
            Dim myNode As wow2collada.FileReaders.Node = wow2collada.myMPQ.FileTree

            For i = 0 To Parts.Length - 1
                myNode = myNode.Nodes(Parts(i))
            Next
            For Each i As wow2collada.FileReaders.Node In myNode.Nodes.Values
                Dim newNode As TreeNode = e.Node.Nodes.Add(i.Data)
                If Not i.Nodes Is Nothing Then 'branch
                    newNode.Nodes.Add("(dummy)")
                Else 'leaf
                    newNode.ContextMenuStrip = FileListPopup
                End If

            Next

            FileList.Sort()
            e.Node.Expand()

        End If
    End Sub

    Private Sub TrackBar1_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TrackBar1.ValueChanged
        Dim i As Integer = TrackBar1.Value

        If i > -1 And i < TextureMgr.Textures.Count Then
            TextureBox.Image = TextureMgr.Textures.ElementAt(i).Value.TextureMap
            CurrentTexture = TextureMgr.Textures.ElementAt(i).Key
            TextureBox.Refresh()
        End If
    End Sub

    Private Sub TexturePopupSaveAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TexturePopupSaveAs.Click
        TextureSaveFile.OverwritePrompt = True
        TextureSaveFile.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
        TextureSaveFile.Filter = "Windows Bitmap File (*.bmp)|*.bmp|JPEG (.jpg)|*.jpg|GIF (*.gif)|*.gif|PNG (*.png)|*.png"
        TextureSaveFile.ShowDialog()
    End Sub

    Private Sub TextureSaveFile_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TextureSaveFile.FileOk
        Dim s As String = TextureSaveFile.FileName
        TextureBox.Image.Save(s)
    End Sub

    Private Sub TexturePopupOpenInViewer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TexturePopupOpenInViewer.Click
        Dim a As New ImageViewer
        a.PictureBox1.Image = TextureBox.Image
        a.ToolStripStatusLabel1.Text = CurrentTexture
        a.Show()
    End Sub

    Private Sub FileListPopupSaveAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileListPopupSaveAs.Click
        Dim ctrl As TreeNode = FileList.SelectedNode
        Dim FullName As String = ctrl.Text
        Dim Extension As String = ""
        Dim i As Integer = FullName.LastIndexOf(".")
        If i > 0 Then Extension = FullName.Substring(i)

        FileListSaveFile.OverwritePrompt = True
        FileListSaveFile.FileName = FullName
        FileListSaveFile.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        FileListSaveFile.Filter = "All Files (*.*)|*.*"
        FileListSaveFile.ShowDialog()
    End Sub

    Private Sub FileList_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles FileList.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Dim myNode As TreeNode = FileList.GetNodeAt(e.X, e.Y)
            If Not myNode Is Nothing Then FileList.SelectedNode = myNode
        End If
    End Sub

    Private Sub FileListSaveFile_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles FileListSaveFile.FileOk
        Dim Dest As String = FileListSaveFile.FileName
        Dim Source As String = FileList.SelectedNode.FullPath
        If Dest > "" Then myMPQ.SaveFileToDisk(Source, Dest)
    End Sub

    Private Sub FileListPopupOpenInHexViewer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FileListPopupOpenInHexViewer.Click
        Dim a As New HexViewer(FileList.SelectedNode.FullPath)
        a.Show()
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveAsToolStripMenuItem.Click
        Dim FullName As String = CurrentFile
        Dim i As Integer = FullName.LastIndexOf("\")
        If i > 0 Then FullName = FullName.Substring(i + 1)
        i = FullName.LastIndexOf(".")
        If i > 0 Then FullName = FullName.Substring(0, i)

        SaveModelDialog.OverwritePrompt = True
        SaveModelDialog.FileName = FullName
        SaveModelDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        SaveModelDialog.Filter = "OBJ File (*.obj)|*.obj|Collada File (*.dae)|*.dae|wow2collada File (*.w2c)|*.w2c"
        SaveModelDialog.ShowDialog()
    End Sub

    Private Sub SaveModelDialog_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles SaveModelDialog.FileOk
        Dim Fullname As String = SaveModelDialog.FileName
        Dim Extension As String = ""
        Dim i As Integer = Fullname.LastIndexOf(".")
        If i > 0 Then Extension = Fullname.Substring(i)

        Select Case Extension
            Case ".obj"
                Dim OBJ As New FileWriters.OBJ
                OBJ.Save(Fullname, ModelMgr)
            Case ".w2c"
                Dim W2C As New FileWriters.W2C
                W2C.Save(Fullname, ModelMgr)
            Case ".dae"
                MsgBox("Collada Export not yet implemented.")
            Case Else
                MsgBox("Woot?")
        End Select
    End Sub

    Private Sub ADTAzeroth2933ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ADTAzeroth2933ToolStripMenuItem.Click
        CurrentFile = "world\maps\azeroth\azeroth_36_49.adt"
        LoadModelFromMPQ()
    End Sub

    Private Sub WMOHumanFarmToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WMOHumanFarmToolStripMenuItem.Click
        CurrentFile = "world\wmo\azeroth\buildings\human_farm\farm.wmo"
        LoadModelFromMPQ()
    End Sub

    Private Sub M2BloodelfGuardToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles M2BloodelfGuardToolStripMenuItem.Click
        CurrentFile = "creature\bloodelfguard\bloodelfmale_guard.m2"
        LoadModelFromMPQ()
    End Sub

    Private Sub M2HumanMaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles M2HumanMaleToolStripMenuItem.Click
        CurrentFile = "character\human\male\humanmale.m2"
        LoadModelFromMPQ()
    End Sub

    Private Sub ADTExplorerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ADTExplorerToolStripMenuItem.Click
        Dim adt As New ADTExplorer
        adt.ShowDialog()
    End Sub

    Private Sub ADTNorthend2525ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ADTNorthend2525ToolStripMenuItem.Click
        CurrentFile = "world\maps\northrend\northrend_25_25.adt"
        LoadModelFromMPQ()
    End Sub

    Private Sub ADTNorthend3233ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ADTNorthend3233ToolStripMenuItem.Click
        CurrentFile = "world\maps\northrend\northrend_32_33.adt"
        LoadModelFromMPQ()
    End Sub

    Private Sub RegionLoaderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RegionLoaderToolStripMenuItem.Click
        Dim reg As New OpenRegionOptions
        reg.ShowDialog()
    End Sub

    Private Sub ContinentHeightmapDumperToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ContinentHeightmapDumperToolStripMenuItem.Click
        Dim adm As New ADTDepthMaps
        adm.ShowDialog()
    End Sub

End Class