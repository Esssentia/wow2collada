Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Public Class RenderForm

    Private MouseIsDown As Boolean
    Private MousePosX As Integer
    Private MousePosY As Integer
    Private ModelOldX As Single
    Private ModelOldY As Single
    Private ModelOldZ As Single
    Private myThread As System.Threading.Thread

    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        ToolStripStatusLabel1.Text = OpenFileDialog1.FileName
        LoadModel()
    End Sub

    Private Sub RenderForm_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        myThread.Abort()
    End Sub

    Private Sub RenderForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ToolStripStatusLabel1.Text = "D:\temp\mpq\World\AZEROTH\WESTFALL\PASSIVEDOODADS\Crate\WestFallCrate.m2"
        ToolStripProgressBar1.Minimum = 0
        ToolStripProgressBar1.Maximum = 1000
        ToolStripProgressBar1.Value = 0

        myThread = New System.Threading.Thread(AddressOf Me.PopulateList)
        myThread.Start()

        LoadModel()
    End Sub

    Private Sub SolidToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SolidToolStripMenuItem.Click
        wow2collada.render.AlterFillMode(FillMode.Solid)
        VerticesToolStripMenuItem.Checked = False
        WireframeToolStripMenuItem.Checked = False
        SolidToolStripMenuItem.Checked = True
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        OpenFileDialog1.Filter = "M2 Files (*.m2)|*.m2|WMO Files (*.wmo)|*.wmo|ADT Files (*.adt)|*.adt"
        OpenFileDialog1.InitialDirectory = "d:\temp\mpq"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.ShowDialog()
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
        ModelOldX = wow2collada.render.LAT_VECTOR.X
        ModelOldY = wow2collada.render.LAT_VECTOR.Y
        ModelOldZ = wow2collada.render.DIS_VECTOR.Z
    End Sub

    Private Sub pic3d_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles pic3d.MouseLeave
        MouseIsDown = False
    End Sub

    Private Sub pic3d_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic3d.MouseMove
        If MouseIsDown Then
            If (ModifierKeys And Keys.Shift) = Keys.Shift Then
                wow2collada.render.DIS_VECTOR.Z = Math.Max(Math.Min(ModelOldZ - 20 * (e.X - MousePosX) / 600, -1), -100)
            Else
                wow2collada.render.LAT_VECTOR.X = Math.Max(Math.Min(ModelOldX - 20 * (e.X - MousePosX) / 600, 50), -50)
                wow2collada.render.LAT_VECTOR.Y = Math.Max(Math.Min(ModelOldY + 20 * (e.Y - MousePosY) / 600, 50), -50)
            End If
        End If
    End Sub

    Private Sub pic3d_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic3d.MouseUp
        MouseIsDown = False
    End Sub

    Private Sub LoadModel()
        Dim Retval As System.Collections.Generic.List(Of String)
        ListBox1.Items.Clear()
        Retval = wow2collada.render.LoadModel(ToolStripStatusLabel1.Text)
        For i As Integer = 0 To Retval.Count - 1
            ListBox1.Items.Add(Retval(i))
        Next

        Button1.PerformClick()
    End Sub

    Private Sub ResetViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetViewToolStripMenuItem.Click
        wow2collada.render.ResetView()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Static pos As Integer = -1

        If wow2collada.render.m_Textures.Count > 0 Then
            pos += 1
            If pos >= wow2collada.render.m_Textures.Count Then pos = 0
            TextureBox1.Image = wow2collada.render.m_Textures(pos).TexGra
            ToolStripStatusLabel2.Text = wow2collada.render.m_Textures(pos).FileName
        End If
    End Sub

    Private Sub TextureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextureBox1.Click
        Dim a As New ImageViewer
        a.PictureBox1.Image = TextureBox1.Image
        a.ToolStripStatusLabel1.Text = ToolStripStatusLabel2.Text
        a.Show()
    End Sub

    Delegate Function AddNodeCallback(ByVal pn As TreeNode, ByVal da As String)

    Function _AddNode(ByVal pn As TreeNode, ByVal da As String) As TreeNode
        Return pn.Nodes.Add(da, da)
    End Function

    Delegate Sub SetProgressCallback(ByVal per As Single)

    Sub _SetProgress(ByVal per As Single)
        ToolStripProgressBar1.Value = Math.Min(Math.Max(per, 0), 1000)
        ToolStripProgressBar1.ForeColor = Color.FromArgb(255, 255 * (100 - per) / 100, 255 * per / 100, 0)
    End Sub

    Private Sub PopulateList()
        ' ugly... feel free to improve on it :)
        Dim cnt As Integer = wow2collada.myMPQ._FileList.Count
        Dim cur As Integer = 0

        For Each i As String In wow2collada.myMPQ._FileList.Keys
            Select Case i.Substring(i.LastIndexOf("."))
                Case ".m2", ".adt", ".wmo"
                    Dim Parts As String() = i.Split("\")
                    Dim parent As TreeNode = TreeView1.Nodes(0)
                    Dim per As Single = cur / cnt * 1000
                    cur += 1

                    Invoke(New SetProgressCallback(AddressOf _SetProgress), New Object() {per})

                    For j As Integer = 0 To Parts.Count - 1
                        Dim found As TreeNode() = parent.Nodes.Find(Parts(j), False)
                        If found.Count > 0 Then
                            parent = found(0)
                        Else
                            parent = Invoke(New AddNodeCallback(AddressOf _AddNode), New Object() {parent, Parts(j)})
                        End If
                    Next
                Case Else
                    'ignore... (don't list .anim, .skin, ...)
            End Select
            
        Next
    End Sub

End Class