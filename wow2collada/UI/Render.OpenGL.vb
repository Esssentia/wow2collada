Imports System.Windows.Forms
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Imports System.Math
Imports wow2collada.HelperFunctions

Public Class RenderFormOpenGL

    Private m_DefaultTexture As Bitmap

    Private MouseIsDown As Boolean
    Private MousePosX As Integer
    Private MousePosY As Integer
    Private Rotate As Boolean = True
    Private DrawWireframe As Boolean = False
    Private DrawTextured As Boolean = True
    Private DrawBones As Boolean = False

    'camera stuff
    Private CAM_POSITION As sVector3 = New sVector3(0, 3, -24)
    Private VIEW_VECTOR As sVector3 = New sVector3(0, -0.6, 0.8)
    Private MOVE_VECTOR As sVector3 = New sVector3(0, 0, 1)
    Private ROTATION_VECTOR As sVector3 = New sVector3(0, 0, 180)
    Private RIGHT_VECTOR As sVector3 = New sVector3(1, 0, 0)
    Private UP_VECTOR As sVector3 = New sVector3(0.08715539, 0.7969557, 0.5977167)

    'FPS stuff
    Private LastTimeStamp As Integer
    Private FrameCounter As Integer

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
        CAM_POSITION = New sVector3(0, 3, -24)
        VIEW_VECTOR = New sVector3(0, -0.6, 0.8)
        MOVE_VECTOR = New sVector3(0, 0, 1)
        ROTATION_VECTOR = New sVector3(0, 0, 180)
        RIGHT_VECTOR = New sVector3(1, 0, 0)
        UP_VECTOR = New sVector3(0.08715539, 0.7969557, 0.5977167)
        SetupScene()
        ResizeScene()
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        m_DefaultTexture = My.Resources.DefaultTexture

        OpenGLControl.InitializeContexts()
        Gl.ReloadFunctions()
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
        'Gl.glCullFace(Gl.GL_BACK)
        'Gl.glEnable(Gl.GL_CULL_FACE)

        'Rotate
        Static Dim LastTicks As Integer = Environment.TickCount
        Dim NowTicks As Integer = Environment.TickCount()

        If (NowTicks - LastTimeStamp) > 1000 Then
            LastTimeStamp = NowTicks
            LabelFPS.Text = String.Format("{0} FPS", FrameCounter)
            FrameCounter = 0
        End If

        FrameCounter += 1

        If Rotate Then Gl.glRotatef((NowTicks - LastTicks) / 30.0F, 0, 1, 0)

        If DrawWireframe Then
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor4f(0.2F, 0.2F, 0.2F, 1.0F)
            For Each Model As sModel In Models
                For Each Submesh As sSubMesh In Model.Meshes
                    Gl.glCallList(Submesh.OpenGLMeshID)
                Next
            Next
        End If

        If DrawBones Then
            Gl.glPointSize(3)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glColor4f(0.3F, 0.3F, 0.5F, 1.0F)
            For Each Model As sModel In Models
                Gl.glCallList(Model.OpenGLBoneMeshID)
            Next
            Gl.glPointSize(1)
        End If

        If DrawTextured Then
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            For h As Integer = 0 To Models.Count - 1
                For i As Integer = 0 To Models(h).Meshes.Count - 1
                    With Models(h).Meshes(i)
                        If SubSets.Nodes(h).Checked And SubSets.Nodes(h).Nodes(i).Checked Then

                            For j As Integer = 0 To 3
                                If .TextureList.Count > j Then
                                    If Not .TextureList.ElementAt(j) Is Nothing Then
                                        Dim Tex As sTextureEntry = .TextureList.ElementAt(j)

                                        If j > 0 And Tex.AlphaMapID <> "" Then
                                            Gl.glEnable(Gl.GL_BLEND)
                                            'Gl.glColor4f(0.0F, 0.0F, 0.0F, 0.0F)
                                            
                                            'alphamap first
                                            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
                                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Models(h).Textures(Tex.AlphaMapID).OpenGLTexID)
                                            Gl.glCallList(.OpenGLMeshID)

                                            'texture afterwards
                                            Gl.glBlendFunc(Gl.GL_DST_ALPHA, Gl.GL_DST_COLOR) '  Gl.GL_ONE_MINUS_SRC_ALPHA)
                                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Models(h).Textures(Tex.TextureID).OpenGLTexID)
                                            'Gl.glCallList(.OpenGLMeshID)

                                        Else
                                            Select Case Tex.Blending1
                                                Case 0 'opaque
                                                    If Tex.Blending2 Then
                                                        Gl.glEnable(Gl.GL_BLEND)
                                                        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
                                                        Gl.glEnable(Gl.GL_ALPHA_TEST)
                                                        Gl.glAlphaFunc(Gl.GL_GREATER, 0.7F)
                                                    Else
                                                        Gl.glDisable(Gl.GL_BLEND)
                                                        Gl.glDisable(Gl.GL_ALPHA_TEST)
                                                    End If
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
                                            If Models(h).Textures.ContainsKey(Tex.TextureID) Then Gl.glBindTexture(Gl.GL_TEXTURE_2D, Models(h).Textures(Tex.TextureID).OpenGLTexID)
                                            Gl.glColor4f(1.0F, 1.0F, 1.0F, 1.0F)
                                            Gl.glCallList(.OpenGLMeshID)
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    End With
                Next
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
        Dim li As Integer = 1
        If Not myHF Is Nothing Then
            If Not Models.Count = 0 Then

                For h As Integer = 0 To Models.Count - 1
                    With Models(h)

                        Dim VT As List(Of sVertex) = Models(h).VerticesTransformed

                        'meshes
                        For i As Integer = 0 To .Meshes.Count - 1
                            Dim submesh As sSubMesh = .Meshes(i)

                            Gl.glNewList(li, Gl.GL_COMPILE)
                            Gl.glBegin(Gl.GL_TRIANGLES)
                            For j As Integer = 0 To submesh.TriangleList.Count - 1
                                Dim triangle As sTriangle = submesh.TriangleList(j)
                                For k As Integer = 0 To 2
                                    Gl.glTexCoord2f(VT(triangle.Vertices(k)).TextureCoords.U, VT(triangle.Vertices(k)).TextureCoords.V)
                                    Gl.glNormal3f(VT(triangle.Vertices(k)).Normal.X, VT(triangle.Vertices(k)).Normal.Z, -VT(triangle.Vertices(k)).Normal.Y)
                                    Gl.glVertex3f(VT(triangle.Vertices(k)).Position.X, VT(triangle.Vertices(k)).Position.Z, -VT(triangle.Vertices(k)).Position.Y)
                                Next
                            Next
                            Models(h).Meshes(i).OpenGLMeshID = li
                            Gl.glEnd()
                            Gl.glEndList()
                            li += 1
                        Next

                        'bones
                        If Not .Bones Is Nothing Then
                            Gl.glNewList(li, Gl.GL_COMPILE)
                            Gl.glBegin(Gl.GL_POINTS)
                            For i As Integer = 0 To .Bones.Count - 1
                                Dim Bone As sBone = .Bones(i)
                                Dim x As Single = Bone.PivotPoint.X
                                Dim y As Single = Bone.PivotPoint.Z
                                Dim z As Single = -Bone.PivotPoint.Y

                                If Bone.ParentBone <> -1 And Bone.ParentBone < 65535 Then
                                    x += .Bones(Bone.ParentBone).PivotPoint.X
                                    y += .Bones(Bone.ParentBone).PivotPoint.Z
                                    z -= .Bones(Bone.ParentBone).PivotPoint.Y
                                End If

                                Gl.glVertex3f(.Bones(i).PivotPoint.X, .Bones(i).PivotPoint.Z, -.Bones(i).PivotPoint.Y)
                            Next
                            .OpenGLBoneMeshID = li
                            Gl.glEnd()
                            Gl.glEndList()
                            li += 1
                        End If
                    End With
                Next
            End If
        End If

    End Sub

    Sub CreateTextureList()
        If Not myHF Is Nothing Then
            If Not Models.Count = 0 Then

                'dispose of the texture objects in OpenGL
                For h As Integer = 0 To Models.Count - 1
                    For Each texture As sTexture In Models(h).Textures.Values
                        Gl.glDeleteTextures(1, texture.OpenGLTexID)
                    Next
                Next

                'create OpenGL textures 
                For h As Integer = 0 To Models.Count - 1
                    For i As Integer = 0 To Models(h).Textures.Count - 1
                        Dim TexsID As String = Models(h).Textures.ElementAt(i).Key
                        Dim Tex As sTexture = Models(h).Textures.ElementAt(i).Value

                        If Not Models(h).Textures(TexsID).TextureMap Is Nothing Then
                            Dim TexiID As Integer
                            Dim bitmapdata As Imaging.BitmapData
                            Dim rect As Rectangle = New Rectangle(0, 0, Tex.TextureMap.Width, Tex.TextureMap.Height)

                            bitmapdata = Tex.TextureMap.LockBits(rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                            Gl.glGenTextures(1, TexiID)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, TexiID)
                            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, Tex.TextureMap.Width, Tex.TextureMap.Height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bitmapdata.Scan0)
                            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
                            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
                            'Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
                            Tex.TextureMap.UnlockBits(bitmapdata)
                            Tex.OpenGLTexID = TexiID
                            Models(h).SetOpenGLTextureID(TexsID, TexiID)
                            'Debug.Print(String.Format("Added Texture: {0} -> {1}", TexiID, TexsID))
                        Else
                            Debug.Print(String.Format("Problem with Texture: {0}", TexsID))
                        End If
                    Next
                Next
            End If
        End If
    End Sub

    Sub SetupScene()
        SubSets.Nodes.Clear()

        If Models.Count > 0 Then
            CreateTextureList()
            CreateVertexBuffer()

            For h As Integer = 0 To Models.Count - 1
                Dim n As TreeNode = SubSets.Nodes.Add(Models(h).Name)
                n.Checked = True
                For i As Integer = 0 To Models(h).Meshes.Count - 1
                    Dim sn As TreeNode = n.Nodes.Add(i)
                    sn.Checked = True
                Next
            Next

        End If
    End Sub

    Sub Camera_Move(ByVal Direction As sVector3)
        CAM_POSITION += Direction
        ResizeScene()
    End Sub

    Sub Camera_RotateX(ByVal Angle As Single)
        ROTATION_VECTOR.X += Angle

        'Rotate viewdir around the right vector:
        VIEW_VECTOR = sVector3.Normalize(VIEW_VECTOR * Math.Cos(Angle * Math.PI / 180) + UP_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new UP_VECTOR (by cross product)
        UP_VECTOR = sVector3.Cross(VIEW_VECTOR, RIGHT_VECTOR) * -1
        ResizeScene()
    End Sub

    Sub Camera_RotateY(ByVal Angle As Single)
        ROTATION_VECTOR.Y += Angle

        'Rotate viewdir around the up vector:
        VIEW_VECTOR = sVector3.Normalize(VIEW_VECTOR * Math.Cos(Angle * Math.PI / 180) - RIGHT_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new RIGHT_VECTOR (by cross product)
        RIGHT_VECTOR = sVector3.Cross(VIEW_VECTOR, UP_VECTOR)
        ResizeScene()
    End Sub

    Sub Camera_RotateZ(ByVal Angle As Single)
        ROTATION_VECTOR.Z += Angle

        'Rotate viewdir around the right vector:
        RIGHT_VECTOR = sVector3.Normalize(RIGHT_VECTOR * Math.Cos(Angle * Math.PI / 180) + UP_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new UP_VECTOR (by cross product)
        UP_VECTOR = sVector3.Cross(VIEW_VECTOR, RIGHT_VECTOR) * -1
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

    Public Sub SetFileName(ByVal FileName As String)
        LabelFile.Text = FileName
    End Sub

    Private Sub BonesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BonesToolStripMenuItem.Click
        BonesToolStripMenuItem.Checked = Not BonesToolStripMenuItem.Checked
        DrawBones = BonesToolStripMenuItem.Checked
    End Sub

End Class




