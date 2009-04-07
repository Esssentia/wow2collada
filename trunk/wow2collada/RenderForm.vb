Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Public Class RenderForm

    Private MouseIsDown As Boolean
    Private MousePosX As Integer
    Private MousePosY As Integer
    Private ModelOldX As Single
    Private ModelOldY As Single
    Private ModelOldZ As Single

    Public Structure TextureListItem
        Dim Img As Bitmap
        Dim Path As String
    End Structure

    Public TextureImages As New System.Collections.Generic.List(Of TextureListItem)

    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        ToolStripStatusLabel1.Text = OpenFileDialog1.FileName
        LoadModel()
    End Sub

    Private Sub RenderForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ToolStripStatusLabel1.Text = "D:\temp\mpq\World\AZEROTH\WESTFALL\PASSIVEDOODADS\Crate\WestFallCrate.m2"
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
    End Sub

    Private Sub ResetViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetViewToolStripMenuItem.Click
        wow2collada.render.ResetView()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Static pos As Integer = -1

        If TextureImages.Count > 0 Then
            pos += 1
            If pos >= TextureImages.Count Then pos = 0
            TextureBox1.Image = TextureImages.Item(pos).Img
            ToolStripStatusLabel2.Text = TextureImages.Item(pos).Path
        End If
    End Sub

    Private Sub TextureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextureBox1.Click
        Dim a As New ImageViewer
        a.PictureBox1.Image = TextureBox1.Image
        a.ToolStripStatusLabel1.Text = ToolStripStatusLabel2.Text
        a.Show()
    End Sub
End Class