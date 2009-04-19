Imports System.Windows.Forms
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Imports System.Math
Imports Tao.DevIl
Imports wow2collada.HelperFunctions

Public Class RenderFormOpenGL

    Private m_DefaultTexture As Bitmap

    Private MouseIsDown As Boolean
    Private MousePosX As Integer
    Private MousePosY As Integer
    Private Rotate As Boolean = True
    Private DrawWireframe As Boolean = False
    Private DrawTextured As Boolean = True

    'camera stuff
    Private CAM_POSITION As Vector3 = New Vector3(0, 3, -24)
    Private VIEW_VECTOR As Vector3 = New Vector3(0, -0.6, 0.8)
    Private MOVE_VECTOR As Vector3 = New Vector3(0, 0, 1)
    Private ROTATION_VECTOR As Vector3 = New Vector3(0, 0, 180)
    Private RIGHT_VECTOR As Vector3 = New Vector3(1, 0, 0)
    Private UP_VECTOR As Vector3 = New Vector3(0.08715539, 0.7969557, 0.5977167)

    Private Sub OpenGLControl_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles OpenGLControl.KeyDown
        Dim Mult As Integer = 1
        Dim AngleStep As Single = 5
        Dim PositionStep As Single = 0.1

        If e.Modifiers = Keys.Shift Then Mult = 10
        If e.Modifiers = Keys.Control Then Mult = 50

        Select Case e.KeyCode
            Case Keys.Up, Keys.W
                Camera_MoveForward(PositionStep * Mult)
            Case Keys.Down, Keys.S
                Camera_MoveForward(-PositionStep * Mult)
            Case Keys.Left, Keys.A
                Camera_RotateY(-AngleStep * Mult)
            Case Keys.Right, Keys.D
                Camera_RotateY(AngleStep * Mult)
            Case Keys.X
                Camera_MoveUpDown(-PositionStep * Mult)
            Case Keys.Space
                Camera_MoveUpDown(PositionStep * Mult)
            Case Keys.Q
                Camera_Strafe(PositionStep * Mult)
            Case Keys.E
                Camera_Strafe(-PositionStep * Mult)
            Case Keys.Y
                Camera_RotateZ(AngleStep * Mult)
            Case Keys.C
                Camera_RotateZ(-AngleStep * Mult)
            Case Else
                'Debug.Print(e.KeyCode)
        End Select

        e.Handled = True
    End Sub

    Private Sub TexturedToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TexturedToolStripMenuItem.Click
        TexturedToolStripMenuItem.Checked = Not TexturedToolStripMenuItem.Checked
        DrawTextured = TexturedToolStripMenuItem.Checked
    End Sub

    Private Sub WireframeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WireframeToolStripMenuItem.Click
        WireframeToolStripMenuItem.Checked = Not WireframeToolStripMenuItem.Checked
        DrawWireframe = WireframeToolStripMenuItem.Checked
    End Sub

    Private Sub RotateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RotateToolStripMenuItem.Click
        RotateToolStripMenuItem.Checked = Not RotateToolStripMenuItem.Checked
        Rotate = RotateToolStripMenuItem.Checked
    End Sub

    Private Sub ResetViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetViewToolStripMenuItem.Click
        ResetView()
    End Sub

    Public Sub ResetView()
        CAM_POSITION = New Vector3(0, 3, -24)
        VIEW_VECTOR = New Vector3(0, -0.6, 0.8)
        MOVE_VECTOR = New Vector3(0, 0, 1)
        ROTATION_VECTOR = New Vector3(0, 0, 180)
        RIGHT_VECTOR = New Vector3(1, 0, 0)
        UP_VECTOR = New Vector3(0.08715539, 0.7969557, 0.5977167)
        SetupScene()
        ResizeScene()
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        m_DefaultTexture = My.Resources.DefaultTexture

        OpenGLControl.InitializeContexts()
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.0F)
        Gl.glShadeModel(Gl.GL_SMOOTH)
        Gl.glClearDepth(1.0#)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST)
        Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1)
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glEnable(Gl.GL_ALPHA_TEST)
        Gl.glAlphaFunc(Gl.GL_GREATER, 0.5F)
        SetupScene()
    End Sub

    Public Sub RenderFrame()
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glLoadIdentity()
        Gl.glTranslatef(0.0F, 0.0F, -20.0F)
        Gl.glEnable(Gl.GL_TEXTURE_2D)

        'Rotate
        Static Dim LastTicks As Integer = Environment.TickCount
        Dim NowTicks As Integer = Environment.TickCount()
        If Rotate Then Gl.glRotatef((NowTicks - LastTicks) / 30.0F, 0, 1, 0)

        If DrawWireframe Then
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            For Each Submesh As sSubMesh In myHF.SubMeshes
                Gl.glCallList(Submesh.OpenGLMeshID)
            Next
        End If

        If DrawTextured Then
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            For Each li As ListViewItem In SubSets.Items
                If li.Checked Then
                    With myHF.SubMeshes(Val(li.Text))
                        For Each Tex As sTextureEntry In .TextureList
                            Select Case Tex.Blending
                                Case 0 'opaque
                                    Gl.glDisable(Gl.GL_BLEND) '?!
                                    Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
                                    Gl.glEnable(Gl.GL_ALPHA_TEST)
                                    Gl.glAlphaFunc(Gl.GL_GREATER, 0.7F)
                                    Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE)

                                Case 1 'BM_TRANSPARENT
                                    Gl.glEnable(Gl.GL_ALPHA_TEST)
                                    Gl.glAlphaFunc(Gl.GL_GEQUAL, 0.7F)
                                    Gl.glDisable(Gl.GL_BLEND)
                                    Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE)

                                Case 2 'BM_ALPHA_BLEND
                                    Gl.glDisable(Gl.GL_ALPHA_TEST)
                                    Gl.glEnable(Gl.GL_BLEND)
                                    Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
                                    Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE)

                                Case 3 'BM_ADDITIVE
                                    Gl.glDisable(Gl.GL_ALPHA_TEST)
                                    Gl.glEnable(Gl.GL_BLEND)
                                    Gl.glBlendFunc(Gl.GL_SRC_COLOR, Gl.GL_ONE)
                                    Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE)

                                Case 4 'BM_ADDITIVE_ALPHA
                                    Gl.glDisable(Gl.GL_ALPHA_TEST)
                                    Gl.glEnable(Gl.GL_BLEND)
                                    Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE)
                                    Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE)

                                Case 5 'BM_MODULATE
                                    Gl.glDisable(Gl.GL_ALPHA_TEST)
                                    Gl.glEnable(Gl.GL_BLEND)
                                    Gl.glBlendFunc(Gl.GL_DST_COLOR, Gl.GL_SRC_COLOR)
                                    Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE)

                                Case 6 'BM_MODULATEX2 (not sure if this is right)
                                    Gl.glDisable(Gl.GL_ALPHA_TEST)
                                    Gl.glEnable(Gl.GL_BLEND)
                                    Gl.glBlendFunc(Gl.GL_DST_COLOR, Gl.GL_SRC_COLOR)
                                    Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE)

                                Case Else
                                    Gl.glDisable(Gl.GL_ALPHA_TEST)
                                    Gl.glEnable(Gl.GL_BLEND)
                                    Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
                                    Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE)

                            End Select
                            'Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)

                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, myHF.Textures(Tex.TextureID).OpenGLTexID)
                            Gl.glCallList(.OpenGLMeshID)

                        Next
                    End With
                End If
            Next
        End If

        Gl.glFlush()
        OpenGLControl.Invalidate()
    End Sub

    Private Sub RenderFormOpenGL_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SplitContainer1.Panel2Collapsed = True
        ResizeScene()
    End Sub

    Private Sub ResizeScene()
        Dim w As Integer = Me.Width
        Dim h As Integer = Me.Height
        If h = 0 Then h = 1
        Gl.glViewport(0, 0, w, h)
        Gl.glMatrixMode(Gl.GL_PROJECTION)
        Gl.glLoadIdentity()
        Glu.gluPerspective(45.0#, w / h, 1.0#, 10000.0#)
        Glu.gluLookAt(CAM_POSITION.X, CAM_POSITION.Y, CAM_POSITION.Z, CAM_POSITION.X + VIEW_VECTOR.X, CAM_POSITION.Y + VIEW_VECTOR.Y, CAM_POSITION.Z + VIEW_VECTOR.Z, UP_VECTOR.X, UP_VECTOR.Y, UP_VECTOR.Z)
        Gl.glMatrixMode(Gl.GL_MODELVIEW)
        Gl.glLoadIdentity()
        Application.DoEvents()
        OpenGLControl.Invalidate()

        'Debug.Print(String.Format("{0}{1}{2}{3}", CAM_POSITION, VIEW_VECTOR, UP_VECTOR, ROTATION_VECTOR))
    End Sub

    Private Sub OpenGLControl_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles OpenGLControl.MouseDown
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

    Private Sub OpenGLControl_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles OpenGLControl.MouseLeave
        MouseIsDown = False
    End Sub

    Private Sub OpenGLControl_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles OpenGLControl.MouseMove
        If MouseIsDown Then
            Select Case e.Button
                Case Windows.Forms.MouseButtons.Left 'move view only (not implemented yet, might never be...)

                Case Windows.Forms.MouseButtons.Right 'move orientation
                    Camera_RotateX((e.Y - MousePosY) / 20)
                    Camera_RotateY((e.X - MousePosX) / 20)
                    MousePosX = e.X
                    MousePosY = e.Y

            End Select
        End If
    End Sub

    Private Sub OpenGLControl_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles OpenGLControl.MouseUp
        MouseIsDown = False
    End Sub

    Private Sub OpenGLControl_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles OpenGLControl.Resize
        ResizeScene()
    End Sub

    Sub CreateVertexBuffer()
        If Not myHF.SubMeshes Is Nothing Then
            Dim CurrIdx As Integer = 0

            For i As Integer = 0 To myHF.SubMeshes.Count - 1
                Dim submesh As sSubMesh = myHF.SubMeshes(i)
                CurrIdx += 1

                Gl.glNewList(CurrIdx, Gl.GL_COMPILE)
                Gl.glBegin(Gl.GL_TRIANGLES)
                For Each triangle As sTriangle In submesh.TriangleList
                    For j = 0 To 2
                        Gl.glColor4f(1.0F, 1.0F, 1.0F, 1.0F)
                        Gl.glTexCoord2f(triangle.P(j).UV.X, triangle.P(j).UV.Y)
                        Gl.glNormal3f(triangle.P(j).Normal.X, triangle.P(j).Normal.Z, -triangle.P(j).Normal.Y)
                        Gl.glVertex3f(triangle.P(j).Position.X, triangle.P(j).Position.Z, -triangle.P(j).Position.Y)
                    Next
                Next
                submesh.OpenGLMeshID = CurrIdx
                myHF.SubMeshes(i) = submesh
                Gl.glEnd()
                Gl.glEndList()

            Next
        End If
    End Sub

    Sub CreateTextureList()
        If Not myHF Is Nothing Then
            'dispose of the texture objects in OpenGL
            For Each texture As sTexture In myHF.Textures.Values
                Gl.glDeleteTextures(1, texture.OpenGLTexID)
            Next

            'create OpenGL textures 
            For i As Integer = 0 To myHF.Textures.Count - 1
                Dim TexsID As String = myHF.Textures.ElementAt(i).Key
                Dim Tex As sTexture = myHF.Textures.ElementAt(i).Value

                If Not myHF.Textures(TexsID).TexGra Is Nothing Then
                    Dim TexiID As Integer
                    Dim bitmapdata As Imaging.BitmapData
                    Dim rect As Rectangle = New Rectangle(0, 0, Tex.TexGra.Width, Tex.TexGra.Height)

                    bitmapdata = Tex.TexGra.LockBits(rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                    Gl.glGenTextures(1, TexiID)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, TexiID)
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, Tex.TexGra.Width, Tex.TexGra.Height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bitmapdata.Scan0)
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
                    'Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
                    Tex.TexGra.UnlockBits(bitmapdata)
                    Tex.OpenGLTexID = TexiID
                    myHF.Textures(TexsID) = Tex
                    Debug.Print(String.Format("Added Texture: {0} -> {1}", TexiID, TexsID))
                Else
                    Debug.Print(String.Format("Problem with Texture: {0}", TexsID))
                End If
            Next

        End If
    End Sub

    Sub SetupScene()
        CreateTextureList()
        CreateVertexBuffer()

        SubSets.Items.Clear()
        For i As Integer = 0 To myHF.SubMeshes.Count - 1
            SubSets.Items.Add(i)
            SubSets.Items(SubSets.Items.Count - 1).Checked = True
        Next

    End Sub

    Sub Camera_Move(ByVal Direction As Vector3)
        CAM_POSITION += Direction
        ResizeScene()
    End Sub

    Sub Camera_RotateX(ByVal Angle As Single)
        ROTATION_VECTOR.X += Angle

        'Rotate viewdir around the right vector:
        VIEW_VECTOR = Vector3.Normalize(VIEW_VECTOR * Math.Cos(Angle * Math.PI / 180) + UP_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new UP_VECTOR (by cross product)
        UP_VECTOR = Vector3.Cross(VIEW_VECTOR, RIGHT_VECTOR) * -1
        ResizeScene()
    End Sub

    Sub Camera_RotateY(ByVal Angle As Single)
        ROTATION_VECTOR.Y += Angle

        'Rotate viewdir around the up vector:
        VIEW_VECTOR = Vector3.Normalize(VIEW_VECTOR * Math.Cos(Angle * Math.PI / 180) - RIGHT_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new RIGHT_VECTOR (by cross product)
        RIGHT_VECTOR = Vector3.Cross(VIEW_VECTOR, UP_VECTOR)
        ResizeScene()
    End Sub

    Sub Camera_RotateZ(ByVal Angle As Single)
        ROTATION_VECTOR.Z += Angle

        'Rotate viewdir around the right vector:
        RIGHT_VECTOR = Vector3.Normalize(RIGHT_VECTOR * Math.Cos(Angle * Math.PI / 180) + UP_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new UP_VECTOR (by cross product)
        UP_VECTOR = Vector3.Cross(VIEW_VECTOR, RIGHT_VECTOR) * -1
        ResizeScene()
    End Sub

    Sub Camera_MoveForward(ByVal Distance As Single)
        CAM_POSITION += VIEW_VECTOR * Distance
        ResizeScene()
    End Sub

    Sub Camera_Strafe(ByVal Distance As Single)
        CAM_POSITION += RIGHT_VECTOR * Distance
        ResizeScene()
    End Sub

    Sub Camera_MoveUpDown(ByVal Distance As Single)
        CAM_POSITION += UP_VECTOR * Distance
        ResizeScene()
    End Sub

    Private Sub DebugModeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DebugModeToolStripMenuItem.Click
        DebugModeToolStripMenuItem.Checked = Not DebugModeToolStripMenuItem.Checked
        SplitContainer1.Panel2Collapsed = Not DebugModeToolStripMenuItem.Checked
    End Sub

End Class




