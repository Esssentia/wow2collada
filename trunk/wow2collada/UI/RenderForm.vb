Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Public Class RenderForm

    Private MouseIsDown As Boolean
    Private MousePosX As Integer
    Private MousePosY As Integer
    Private ModelOldPX As Single
    Private ModelOldPY As Single
    Private ModelOldPZ As Single
    Private ModelOldRX As Single
    Private ModelOldRZ As Single
    Public CurrentTexture As String
    Public CurrentFile As String

    Private Sub RenderForm_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Dim Mult As Integer = 1
        Dim AngleStep As Single = 5
        Dim PositionStep As Single = 0.1

        If e.Modifiers = Keys.Shift Then Mult = 10
        If e.Modifiers = Keys.Control Then Mult = 50

        Select Case e.KeyCode
            Case Keys.Up, Keys.W
                wow2collada.render.Camera_MoveForward(PositionStep * Mult)
            Case Keys.Down, Keys.S
                wow2collada.render.Camera_MoveForward(-PositionStep * Mult)
            Case Keys.Left, Keys.A
                wow2collada.render.Camera_RotateY(-AngleStep * Mult)
            Case Keys.Right, Keys.D
                wow2collada.render.Camera_RotateY(AngleStep * Mult)
            Case Keys.X
                wow2collada.render.Camera_MoveUpDown(-PositionStep * Mult)
            Case Keys.Space
                wow2collada.render.Camera_MoveUpDown(PositionStep * Mult)
            Case Keys.Q
                wow2collada.render.Camera_Strafe(PositionStep * Mult)
            Case Keys.E
                wow2collada.render.Camera_Strafe(-PositionStep * Mult)
            Case Keys.Y
                wow2collada.render.Camera_RotateZ(AngleStep * Mult)
            Case Keys.C
                wow2collada.render.Camera_RotateZ(-AngleStep * Mult)
            Case Else
                'Debug.Print(e.KeyCode)
        End Select

        e.Handled = True
    End Sub

    Private Sub RenderForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CurrentFile = "World\AZEROTH\WESTFALL\PASSIVEDOODADS\Crate\WestFallCrate.m2"
        StatusLabel1.Text = CurrentFile
        For Each i As String In wow2collada.myMPQ.FileTree.Nodes.Keys
            Dim Out As TreeNode = FileList.Nodes.Add(i)
            Out.Nodes.Add("(dummy)")
        Next
        FileList.Sort()

        LoadModelFromMPQ()
    End Sub

    Private Sub SolidToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SolidToolStripMenuItem.Click
        wow2collada.render.AlterFillMode(FillMode.Solid)
        VerticesToolStripMenuItem.Checked = False
        WireframeToolStripMenuItem.Checked = False
        SolidToolStripMenuItem.Checked = True
    End Sub

    Private Sub WireframeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WireframeToolStripMenuItem.Click
        wow2collada.render.AlterFillMode(FillMode.WireFrame)
        VerticesToolStripMenuItem.Checked = False
        WireframeToolStripMenuItem.Checked = True
        SolidToolStripMenuItem.Checked = False
    End Sub

    Private Sub VerticesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VerticesToolStripMenuItem.Click
        wow2collada.render.AlterFillMode(FillMode.Point)
        VerticesToolStripMenuItem.Checked = True
        WireframeToolStripMenuItem.Checked = False
        SolidToolStripMenuItem.Checked = False
    End Sub

    Private Sub QuitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub pic3d_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic3d.MouseDown
        MouseIsDown = True
        MousePosX = e.X
        MousePosY = e.Y
        'ModelOldPX = wow2collada.render.LOOKAT_POSITION.X
        'ModelOldPY = wow2collada.render.LOOKAT_POSITION.Y
        'ModelOldPZ = wow2collada.render.CAM_POSITION.Z
        'ModelOldRX = wow2collada.render.ROT_VECTOR.X
        'ModelOldRZ = wow2collada.render.ROT_VECTOR.Z
        'OLD_LOOKAT_POSITION = wow2collada.render.LOOKAT_POSITION
    End Sub

    Private Sub pic3d_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles pic3d.MouseLeave
        MouseIsDown = False
    End Sub

    Private Sub pic3d_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic3d.MouseMove
        If MouseIsDown Then
            Select Case e.Button
                Case Windows.Forms.MouseButtons.Left 'move view only (not implemented yet, might never be...)

                Case Windows.Forms.MouseButtons.Right 'move orientation
                    'Debug.Print wow2collada.render.VIEW_VECTOR
                    wow2collada.render.Camera_RotateX((e.Y - MousePosY) / 20)
                    wow2collada.render.Camera_RotateY((e.X - MousePosX) / 20)
                    MousePosX = e.X
                    MousePosY = e.Y

            End Select
        End If
        'wow2collada.render.CAM_POSITION.Z = Math.Max(Math.Min(ModelOldPZ - 20 * (e.X - MousePosX) / 400, -1), -200)
        'wow2collada.render.ROT_VECTOR.X = (ModelOldRX + (e.Y - MousePosY) / 100) Mod (Math.PI * 2)
        'wow2collada.render.ROT_VECTOR.Z = (ModelOldRZ - (e.X - MousePosX) / 100) Mod (Math.PI * 2)
    End Sub

    Private Sub pic3d_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic3d.MouseUp
        MouseIsDown = False
    End Sub

    Private Sub LoadModelFromMPQ()
        Dim Retval As System.Collections.Generic.List(Of String)
        ListBox1.Items.Clear()
        Retval = myHF.LoadModelFromMPQ(CurrentFile)
        For i As Integer = 0 To Retval.Count - 1
            ListBox1.Items.Add(Retval(i))
        Next
        TrackBar1.Minimum = 0
        TrackBar1.Maximum = Math.Max(0, myHF.Textures.Count - 1)
        TrackBar1.Value = 0
        TrackBar1_ValueChanged(Me, New System.EventArgs)
    End Sub

    Private Sub ResetViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetViewToolStripMenuItem.Click
        wow2collada.render.ResetView()
    End Sub

    Private Sub TreeView1_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles FileList.AfterSelect
        If e.Node.Nodes Is Nothing Or e.Node.Nodes.Count = 0 Then ' only look at leafs
            CurrentFile = e.Node.FullPath
            StatusLabel1.Text = CurrentFile
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

        If i > -1 And i < myHF.Textures.Count Then
            TextureBox.Image = myHF.Textures.ElementAt(i).Value.TexGra
            CurrentTexture = myHF.Textures.ElementAt(i).Value.ID
        End If
    End Sub

    Private Sub pic3d_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles pic3d.Resize
        If Not wow2collada.render Is Nothing Then
            wow2collada.CanvasTainted = True
            wow2collada.render = New wow2collada.render3d(wow2collada.frm.pic3d)
            wow2collada.render.InitializeGraphics()
            wow2collada.render.SetupScene()
            wow2collada.CanvasTainted = False
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

    Private Sub FileList_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles FileList.KeyDown
        e.Handled = True
    End Sub

    Private Sub FileList_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles FileList.KeyPress
        e.Handled = True
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
        If Dest > "" Then wow2collada.myMPQ.SaveFileToDisk(Source, Dest)
    End Sub

    Private Sub FileListPopupOpenInHexViewer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FileListPopupOpenInHexViewer.Click
        Dim a As New HexViewer
        a.FileName = FileList.SelectedNode.FullPath
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
        SaveModelDialog.Filter = "OBJ File (*.obj)|*.obj|Collada File (*.dae)|*.dae"
        SaveModelDialog.ShowDialog()
    End Sub

    Private Sub SaveModelDialog_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles SaveModelDialog.FileOk
        Dim Fullname As String = SaveModelDialog.FileName
        Dim Extension As String = ""
        Dim i As Integer = Fullname.LastIndexOf(".")
        If i > 0 Then Extension = Fullname.Substring(i)

        If Extension = ".obj" Then
            Dim OBJ As New FileWriters.OBJ
            OBJ.Save(Fullname, myHF.TriangleList, myHF.Textures)
        End If

        If Extension = ".dae" Then MsgBox("Collada Export not yet implemented.")
    End Sub

    Private Sub RotateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RotateToolStripMenuItem.Click
        RotateToolStripMenuItem.Checked = Not RotateToolStripMenuItem.Checked
        render.WorldRotation(RotateToolStripMenuItem.Checked)
    End Sub

End Class