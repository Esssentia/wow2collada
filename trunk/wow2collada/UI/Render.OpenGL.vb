Imports System.Windows.Forms
Imports System.Math
Imports wow2collada.HelperFunctions
Imports System.Runtime.InteropServices.Marshal

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
    Private EYE_POS As New Vector3(6, 7, 8)
    Private LOOK_AT As New Vector3(0, 0, 0)
    Private EYE_DIST As Single = 1
    Private ROTATION_VECTOR As Vector3 = New Vector3(0, 0, 180)
    Private RIGHT_VECTOR As Vector3 = New Vector3(1, 0, 0)
    Private UP_VECTOR As Vector3 = New Vector3(0, 0, 1)

    'FPS stuff
    Private LastTimeStamp As Integer
    Private FrameCounter As Integer

    'splatting stuff


    'shader stuff
    Private FragmentShaderID As Integer
    Private ShaderProgramID As Integer

    Private OpenGLLoaded As Boolean = False

    Private Sub GLControl1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles GlControl1.KeyDown
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
        EYE_POS = New Vector3(6, 7, 8)
        EYE_DIST = 1
        LOOK_AT = New Vector3(0, 0, 0)
        ROTATION_VECTOR = New Vector3(0, 0, 180)
        RIGHT_VECTOR = New Vector3(1, 0, 0)
        UP_VECTOR = New Vector3(0, 0, 1)
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Public Sub GLInit()
        GL.ClearColor(0.0F, 0.0F, 0.0F, 0.0F)
        GL.ShadeModel(ShadingModel.Smooth)
        GL.ClearDepth(1.0#)
        GL.Enable(EnableCap.DepthTest)
        GL.DepthFunc(DepthFunction.Lequal)
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)
        GL.Viewport(0, 0, GlControl1.Width, GlControl1.Height)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadIdentity()
        Glu.Perspective(45.0, GlControl1.Width / GlControl1.Height, 0.1, 10000.0)
        Glu.LookAt(EYE_POS, LOOK_AT, UP_VECTOR)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)

        GL.Enable(EnableCap.Texture2D)
        GL.Enable(EnableCap.Blend)
        GL.Enable(EnableCap.AlphaTest)

        OpenGLLoaded = True
        SetupScene()

    End Sub

    Public Sub RenderFrame()
        If SuspendRender Then Return

        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadIdentity()
        Glu.Perspective(45.0, GlControl1.Width / GlControl1.Height, 0.1, 10000.0)
        Glu.LookAt(LOOK_AT + (EYE_POS - LOOK_AT) * EYE_DIST, LOOK_AT, UP_VECTOR)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
        
        'Rotate
        Static Dim LastTicks As Integer = Environment.TickCount
        Dim NowTicks As Integer = Environment.TickCount()

        If (NowTicks - LastTimeStamp) > 1000 Then
            LastTimeStamp = NowTicks
            LabelFPS.Text = String.Format("{0} FPS", FrameCounter)
            FrameCounter = 0
        End If

        FrameCounter += 1

        If Rotate Then GL.Rotate((NowTicks - LastTicks) / 30.0F, 0, 0, 1)

        If DrawWireframe Then
            GL.Disable(EnableCap.Texture2D)
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line)
            GL.Color4(0.2F, 0.2F, 0.2F, 1.0F)
            For Each Model As ModelManager.sModel In ModelMgr.Models.Values
                For Each Submesh As sSubMesh In ModelMgr.ModelData(Model.ModelDataID).Meshes

                    GL.EnableClientState(EnableCap.VertexArray)
                    GL.EnableClientState(EnableCap.NormalArray)
                    GL.EnableClientState(EnableCap.TextureCoordArray)
                    GL.BindBuffer(BufferTarget.ArrayBuffer, Model.OpenGLVBOVerticesID)
                    GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero)
                    GL.BindBuffer(BufferTarget.ArrayBuffer, Model.OpenGLVBONormalsID)
                    GL.NormalPointer(3, VertexPointerType.Float, 0, IntPtr.Zero)
                    GL.BindBuffer(BufferTarget.ArrayBuffer, ModelMgr.ModelData(Model.ModelDataID).OpenGLVBOTexCoordsID)
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero)
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, Submesh.OpenGLVBOIndicesID)
                    GL.DrawElements(BeginMode.Triangles, Submesh.OpenGLVBOIndicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero)

                    'GL.CallList(Submesh.OpenGLMeshID)
                Next
            Next
        End If

        If DrawBones Then
            GL.PointSize(3)
            GL.Disable(EnableCap.Texture2D)
            GL.Color4(0.3F, 0.3F, 0.5F, 1.0F)
            For Each Model As ModelManager.sModel In ModelMgr.Models.Values
                GL.CallList(Model.OpenGLBoneMeshID)
            Next
            GL.PointSize(1)
        End If

        If DrawTextured Then
            GL.Enable(EnableCap.Texture2D)
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)
            For h As Integer = 0 To ModelMgr.Models.Count - 1
                For i As Integer = 0 To ModelMgr.ModelData(ModelMgr.Models(h).ModelDataID).Meshes.Count - 1
                    With ModelMgr.ModelData(ModelMgr.Models(h).ModelDataID).Meshes(i)
                        If SubSets.Nodes(h).Checked And SubSets.Nodes(h).Nodes(i).Checked Then

                            GL.Enable(EnableCap.Texture2D)
                            If .isADT Then
                                GL.UseProgram(ShaderProgramID)

                                Dim DoLayer1 As Boolean = False
                                Dim DoLayer2 As Boolean = False
                                Dim DoLayer3 As Boolean = False
                                Dim ID As String

                                GL.ActiveTexture(TextureUnit.Texture0)
                                ID = .GetTextureIDByName("Layer0")
                                If TextureMgr.TextureExists(ID) Then
                                    GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(ID))
                                End If

                                GL.ActiveTexture(TextureUnit.Texture1)
                                ID = .GetTextureIDByName("Layer1")
                                If TextureMgr.TextureExists(ID) Then GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(ID))

                                GL.ActiveTexture(TextureUnit.Texture2)
                                ID = .GetTextureIDByName("Layer2")
                                If TextureMgr.TextureExists(ID) Then GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(ID))

                                GL.ActiveTexture(TextureUnit.Texture3)
                                ID = .GetTextureIDByName("Layer3")
                                If TextureMgr.TextureExists(ID) Then GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(ID))

                                GL.ActiveTexture(TextureUnit.Texture4)
                                ID = .GetTextureIDByName("Alpha1")
                                If TextureMgr.TextureExists(ID) Then
                                    GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(ID))
                                    DoLayer1 = True
                                End If

                                GL.ActiveTexture(TextureUnit.Texture5)
                                ID = .GetTextureIDByName("Alpha2")
                                If TextureMgr.TextureExists(ID) Then
                                    GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(ID))
                                    DoLayer2 = True
                                End If

                                GL.ActiveTexture(TextureUnit.Texture6)
                                ID = .GetTextureIDByName("Alpha3")
                                If TextureMgr.TextureExists(ID) Then
                                    GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(ID))
                                    DoLayer3 = True
                                End If

                                GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "WotLK"), IIf(.isWotLK, 1, 0))
                                GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "DoLayer1"), IIf(DoLayer1, 1, 0))
                                GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "DoLayer2"), IIf(DoLayer2, 1, 0))
                                GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "DoLayer3"), IIf(DoLayer3, 1, 0))

                                GL.Enable(EnableCap.Texture2D)
                                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)

                                GL.EnableClientState(EnableCap.VertexArray)
                                GL.EnableClientState(EnableCap.NormalArray)
                                GL.EnableClientState(EnableCap.TextureCoordArray)
                                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelMgr.Models(h).OpenGLVBOVerticesID)
                                GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero)
                                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelMgr.Models(h).OpenGLVBONormalsID)
                                GL.NormalPointer(3, VertexPointerType.Float, 0, IntPtr.Zero)
                                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelMgr.ModelData(ModelMgr.Models(h).ModelDataID).OpenGLVBOTexCoordsID)
                                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero)
                                GL.BindBuffer(BufferTarget.ElementArrayBuffer, .OpenGLVBOIndicesID)
                                GL.DrawElements(BeginMode.Triangles, .OpenGLVBOIndicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero)
                                'GL.CallList(.OpenGLMeshID)

                            Else
                                GL.UseProgram(0)
                                For j As Integer = 0 To 3
                                    If .TextureList.Count > j Then
                                        If Not .TextureList.ElementAt(j) Is Nothing Then
                                            Dim Tex As sTextureEntry = .TextureList.ElementAt(j)

                                            If j > 0 And Tex.AlphaMapID <> "" Then
                                                GL.Enable(EnableCap.Blend)
                                                'Gl.glColor4f(0.0F, 0.0F, 0.0F, 0.0F)

                                                'alphamap first
                                                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
                                                GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(Tex.AlphaMapID))
                                                GL.CallList(.OpenGLMeshID)

                                                'texture afterwards
                                                GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.DstColor) '  Gl.GL_ONE_MINUS_SRC_ALPHA)
                                                GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(Tex.TextureID))
                                                'Gl.glCallList(.OpenGLMeshID)

                                            Else
                                                Select Case Tex.Blending1
                                                    Case 0 'opaque
                                                        If Tex.Blending2 Then
                                                            GL.Enable(EnableCap.Blend)
                                                            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
                                                            GL.Enable(EnableCap.AlphaTest)
                                                            GL.AlphaFunc(AlphaFunction.Greater, 0.7F)
                                                        Else
                                                            GL.Disable(EnableCap.Blend)
                                                            GL.Disable(EnableCap.AlphaTest)
                                                        End If
                                                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Combine)

                                                    Case 1 'BM_TRANSPARENT
                                                        GL.Enable(EnableCap.AlphaTest)
                                                        GL.AlphaFunc(AlphaFunction.Greater, 0.7F)
                                                        GL.Disable(EnableCap.Blend)
                                                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Combine)

                                                    Case 2 'BM_ALPHA_BLEND
                                                        GL.Disable(EnableCap.AlphaTest)
                                                        GL.Enable(EnableCap.Blend)
                                                        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
                                                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Combine)

                                                    Case 3 'BM_ADDITIVE
                                                        GL.Disable(EnableCap.AlphaTest)
                                                        GL.Enable(EnableCap.Blend)
                                                        GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.One)
                                                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Combine)

                                                    Case 4 'BM_ADDITIVE_ALPHA
                                                        GL.Disable(EnableCap.AlphaTest)
                                                        GL.Enable(EnableCap.Blend)
                                                        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One)
                                                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Combine)

                                                    Case 5 'BM_MODULATE
                                                        GL.Disable(EnableCap.AlphaTest)
                                                        GL.Enable(EnableCap.Blend)
                                                        GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.SrcColor)
                                                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Combine)

                                                    Case 6 'BM_MODULATEX2 (not sure if this is right)
                                                        GL.Disable(EnableCap.AlphaTest)
                                                        GL.Enable(EnableCap.Blend)
                                                        GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.SrcColor)
                                                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Combine)

                                                    Case Else
                                                        GL.Disable(EnableCap.AlphaTest)
                                                        GL.Enable(EnableCap.Blend)
                                                        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
                                                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Combine)

                                                End Select
                                                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Replace)
                                                If TextureMgr.TextureExists(Tex.TextureID) Then
                                                    GL.ActiveTexture(TextureUnit.Texture0)
                                                    GL.BindTexture(TextureTarget.Texture2D, TextureMgr.GetTextureOpenGLID(Tex.TextureID))
                                                End If

                                                GL.Color4(1.0F, 1.0F, 1.0F, 1.0F)

                                                GL.EnableClientState(EnableCap.VertexArray)
                                                GL.EnableClientState(EnableCap.NormalArray)
                                                GL.EnableClientState(EnableCap.TextureCoordArray)
                                                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelMgr.Models(h).OpenGLVBOVerticesID)
                                                GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero)
                                                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelMgr.Models(h).OpenGLVBONormalsID)
                                                GL.NormalPointer(3, VertexPointerType.Float, 0, IntPtr.Zero)
                                                GL.BindBuffer(BufferTarget.ArrayBuffer, ModelMgr.ModelData(ModelMgr.Models(h).ModelDataID).OpenGLVBOTexCoordsID)
                                                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero)
                                                GL.BindBuffer(BufferTarget.ElementArrayBuffer, .OpenGLVBOIndicesID)
                                                GL.DrawElements(BeginMode.Triangles, .OpenGLVBOIndicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero)

                                                'GL.CallList(.OpenGLMeshID)
                                            End If
                                        End If
                                    End If
                                Next

                            End If


                        End If
                    End With
                Next
            Next

        End If


        GL.Flush()

        If GlControl1.Created Then GlControl1.SwapBuffers()
    End Sub

    Private Sub RenderFormOpenGL_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SplitContainer1.Panel2Collapsed = True
    End Sub

    Private Sub GlControl1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles GlControl1.Resize
        If Not OpenGLLoaded Then Return
        GL.Viewport(0, 0, GlControl1.Width, GlControl1.Height)
    End Sub

    Private Sub GlControl1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles GlControl1.Load
        OpenGLLoaded = True
    End Sub

    Private Sub GlControl1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles GlControl1.Paint
        If Not OpenGLLoaded Then Return
        RenderFrame()
    End Sub

    Private Sub GLControl1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GlControl1.MouseDown
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

    Private Sub GLControl1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles GlControl1.MouseLeave
        MouseIsDown = False
    End Sub

    Private Sub GLControl1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GlControl1.MouseMove
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

    Private Sub GLControl1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GlControl1.MouseUp
        MouseIsDown = False
    End Sub

    Sub CreateVertexBuffer()
        Dim li As Integer = 1
        If Not myHF Is Nothing Then
            For Each item In ModelMgr.Models

                Dim VT As List(Of sVertex) = ModelMgr.VerticesTransformed(item.Key)

                Dim VBOVertices As Single() = ModelMgr.VBOVertices(item.Key)
                Dim VBOVerticesID As Integer

                Dim VBONormals As Single() = ModelMgr.VBONormals(item.Key)
                Dim VBONormalsID As Integer

                Dim VBOTexCoords As Single() = ModelMgr.VBOTexCoords(item.Key)
                Dim VBOTexCoordsID As Integer

                'create buffers
                GL.GenBuffers(1, VBOVerticesID)
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOVerticesID)
                GL.BufferData(BufferTarget.ArrayBuffer, SizeOf(GetType(Single)) * VBOVertices.Count, VBOVertices, BufferUsageHint.StaticDraw)
                ModelMgr.Models(item.Key).OpenGLVBOVerticesID = VBOVerticesID

                GL.GenBuffers(1, VBONormalsID)
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBONormalsID)
                GL.BufferData(BufferTarget.ArrayBuffer, SizeOf(GetType(Single)) * VBONormals.Count, VBONormals, BufferUsageHint.StaticDraw)
                ModelMgr.Models(item.Key).OpenGLVBONormalsID = VBONormalsID

                GL.GenBuffers(1, VBOTexCoordsID)
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOTexCoordsID)
                GL.BufferData(BufferTarget.ArrayBuffer, SizeOf(GetType(Single)) * VBOTexCoords.Count, VBOTexCoords, BufferUsageHint.StaticDraw)
                ModelMgr.ModelData(item.Value.ModelDataID).OpenGLVBOTexCoordsID = VBOTexCoordsID


                'meshes
                For Each mesh In ModelMgr.ModelData(item.Value.ModelDataID).Meshes

                    Dim VBOIndices As Integer() = mesh.VBOIndices
                    Dim VBOIndicesID As Integer

                    GL.GenBuffers(1, VBOIndicesID)
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOIndicesID)
                    GL.BufferData(BufferTarget.ElementArrayBuffer, SizeOf(GetType(Integer)) * VBOIndices.Count, VBOIndices, BufferUsageHint.StaticDraw)
                    mesh.OpenGLVBOIndicesID = VBOIndicesID

                    'GL.NewList(li, ListMode.Compile)
                    'GL.Begin(BeginMode.Triangles)
                    'For j As Integer = 0 To .Meshes(i).TriangleList.Count - 1
                    '    Dim triangle As sTriangle = .Meshes(i).TriangleList(j)
                    '    For k As Integer = 0 To 2
                    '        GL.TexCoord2(VT(triangle.Vertices(k)).TextureCoords.U, VT(triangle.Vertices(k)).TextureCoords.V)
                    '        GL.Normal3(VT(triangle.Vertices(k)).Normal.X, VT(triangle.Vertices(k)).Normal.Y, VT(triangle.Vertices(k)).Normal.Z)
                    '        GL.Vertex3(VT(triangle.Vertices(k)).Position.X, VT(triangle.Vertices(k)).Position.Y, VT(triangle.Vertices(k)).Position.Z)
                    '    Next
                    'Next
                    '.Meshes(i).OpenGLMeshID = li
                    'GL.End()
                    'GL.EndList()
                    'li += 1
                Next

                'bones
                'If Not .Bones Is Nothing Then
                '    GL.NewList(li, ListMode.Compile)
                '    GL.Begin(BeginMode.Points)
                '    For i As Integer = 0 To .Bones.Count - 1
                '        Dim Bone As sBone = .Bones(i)
                '        Dim x As Single = Bone.PivotPoint.X
                '        Dim y As Single = Bone.PivotPoint.Y
                '        Dim z As Single = Bone.PivotPoint.Z

                '        If Bone.ParentBone <> -1 And Bone.ParentBone < 65535 Then
                '            x += .Bones(Bone.ParentBone).PivotPoint.X
                '            y += .Bones(Bone.ParentBone).PivotPoint.Y
                '            z += .Bones(Bone.ParentBone).PivotPoint.Z
                '        End If

                '        GL.Vertex3(.Bones(i).PivotPoint.X, .Bones(i).PivotPoint.Y, .Bones(i).PivotPoint.Z)
                '    Next
                '    .OpenGLBoneMeshID = li
                '    GL.End()
                '    GL.EndList()
                '    li += 1
                'End If
            Next
        End If

    End Sub

    Sub CreateShader()
        FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader)

        Dim FragShad As String = FragmentShaderADT()
        GL.ShaderSource(FragmentShaderID, 1, New String() {FragShad}, FragShad.Length - 1)
        GL.CompileShader(FragmentShaderID)

        Dim status As Integer
        GL.GetShader(FragmentShaderID, ShaderParameter.CompileStatus, status)
        If status = 0 Then
            Dim s As New System.Text.StringBuilder(10000)
            GL.GetShaderInfoLog(FragmentShaderID, 10000, vbNull, s)
            'Debug.Print(FragShad)
            MsgBox(s.ToString)
        End If

        ShaderProgramID = GL.CreateProgram()
        GL.AttachShader(ShaderProgramID, FragmentShaderID)
        GL.LinkProgram(ShaderProgramID)
        GL.UseProgram(ShaderProgramID)

        GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "Layer0"), 0)
        GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "Layer1"), 1)
        GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "Layer2"), 2)
        GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "Layer3"), 3)
        GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "Alpha1"), 4)
        GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "Alpha2"), 5)
        GL.Uniform1(GL.GetUniformLocation(ShaderProgramID, "Alpha3"), 6)

    End Sub

    Sub SetupScene()
        SubSets.Nodes.Clear()
        If ModelMgr.Models.Count > 0 Then
            CreateShader()
            CreateVertexBuffer()

            For Each model In ModelMgr.Models
                Dim n As TreeNode = SubSets.Nodes.Add(model.Value.Name)
                n.Checked = True
                For i As Integer = 0 To ModelMgr.ModelData(model.Value.ModelDataID).Meshes.Count - 1
                    Dim sn As TreeNode = n.Nodes.Add(i)
                    sn.Checked = True
                Next
            Next
        End If
    End Sub

#Region "Camera Management"
    Sub Camera_Move(ByVal Direction As Vector3)
        EYE_POS += Direction
        LOOK_AT += Direction
    End Sub

    Sub Camera_RotateX(ByVal Angle As Single)
        Dim VIEW_VECTOR As Vector3 = LOOK_AT - EYE_POS

        'Rotate viewdir around the right vector:
        VIEW_VECTOR = Vector3.Normalize(VIEW_VECTOR * Math.Cos(Angle * Math.PI / 180) + UP_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new UP_VECTOR (by cross product)
        UP_VECTOR = Vector3.Cross(VIEW_VECTOR, RIGHT_VECTOR) * -1
        LOOK_AT = EYE_POS + VIEW_VECTOR
    End Sub

    Sub Camera_RotateY(ByVal Angle As Single)
        Dim VIEW_VECTOR As Vector3 = LOOK_AT - EYE_POS

        'Rotate viewdir around the up vector:
        VIEW_VECTOR = Vector3.Normalize(VIEW_VECTOR * Math.Cos(Angle * Math.PI / 180) - RIGHT_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new RIGHT_VECTOR (by cross product)
        RIGHT_VECTOR = Vector3.Cross(VIEW_VECTOR, UP_VECTOR)
        LOOK_AT = EYE_POS + VIEW_VECTOR
    End Sub

    Sub Camera_RotateZ(ByVal Angle As Single)
        Dim VIEW_VECTOR As Vector3 = LOOK_AT - EYE_POS
        'Rotate viewdir around the right vector:
        RIGHT_VECTOR = Vector3.Normalize(RIGHT_VECTOR * Math.Cos(Angle * Math.PI / 180) + UP_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new UP_VECTOR (by cross product)
        UP_VECTOR = Vector3.Cross(VIEW_VECTOR, RIGHT_VECTOR) * -1
    End Sub

    Sub Camera_MoveForward(ByVal Distance As Single)
        Dim VIEW_VECTOR As Vector3 = LOOK_AT - EYE_POS
        EYE_POS += VIEW_VECTOR * Distance
    End Sub

    Sub Camera_Strafe(ByVal Distance As Single)
        EYE_POS += RIGHT_VECTOR * Distance
    End Sub

    Sub Camera_MoveUpDown(ByVal Distance As Single)
        EYE_POS += UP_VECTOR * Distance
    End Sub

#End Region

    Private Sub DebugModeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DebugModeToolStripMenuItem.Click
        DebugModeToolStripMenuItem.Checked = Not DebugModeToolStripMenuItem.Checked
        SplitContainer1.Panel2Collapsed = Not DebugModeToolStripMenuItem.Checked
    End Sub

    Public Sub SetFileName(ByVal FileName As String)
        LabelFile.Text = FileName
        SetupScene()
    End Sub

    Private Sub BonesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BonesToolStripMenuItem.Click
        BonesToolStripMenuItem.Checked = Not BonesToolStripMenuItem.Checked
        DrawBones = BonesToolStripMenuItem.Checked
    End Sub

    ''' <summary>
    ''' Return a GLSL fragment shader that will blend according to the blending rules in WotLK
    ''' </summary>
    ''' <returns>A string containing the shader</returns>
    ''' <remarks></remarks>
    Private Function FragmentShaderADT() As String
        Dim Out As New List(Of String)
        Dim Txt As String = ""

        Out.Add("uniform bool WotLK;")
        Out.Add("uniform sampler2D Layer0;")
        Out.Add("uniform sampler2D Layer1;")
        Out.Add("uniform sampler2D Alpha1;")
        Out.Add("uniform bool DoLayer1;")
        Out.Add("uniform sampler2D Layer2;")
        Out.Add("uniform sampler2D Alpha2;")
        Out.Add("uniform bool DoLayer2;")
        Out.Add("uniform sampler2D Layer3;")
        Out.Add("uniform sampler2D Alpha3;")
        Out.Add("uniform bool DoLayer3;")
        Out.Add("varying vec4 texCoord;")

        Out.Add("void main (void)")
        Out.Add("{")
        Out.Add("   vec4 l0 = texture2D( Layer0, texCoord.xy * 8.0);")
        Out.Add("   vec4 l1 = texture2D( Layer1, texCoord.xy * 8.0);")
        Out.Add("   vec4 l2 = texture2D( Layer2, texCoord.xy * 8.0);")
        Out.Add("   vec4 l3 = texture2D( Layer3, texCoord.xy * 8.0);")

        Out.Add("   float a1 = 0.0;")
        Out.Add("   float a2 = 0.0;")
        Out.Add("   float a3 = 0.0;")

        Out.Add("   if (DoLayer1)")
        Out.Add("      a1 = texture2D( Alpha1, texCoord.xy).a;")

        Out.Add("   if (DoLayer2)")
        Out.Add("      a2 = texture2D( Alpha2, texCoord.xy).a;")

        Out.Add("   if (DoLayer3)")
        Out.Add("      a3 = texture2D( Alpha3, texCoord.xy).a;")

        Out.Add("   if (WotLK)")
        Out.Add("      gl_FragColor = l0 * (1.0 - a1 - a2 - a3) + l1 * a1 + l2 * a2 + l3 * a3;")
        Out.Add("   else")
        Out.Add("      gl_FragColor = ((l0 * (1.0 - a1) + l1 * a1) * (1.0 - a2) + l2 * a2) * (1.0 - a3) + l3 * a3;")
        Out.Add("}")

        For Each s As String In Out
            Txt &= s & vbCrLf
        Next

        Txt &= Chr(0)

        Return Txt

    End Function

    Private Sub GlControl1_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GlControl1.MouseWheel
        If e.Delta > 0 Then EYE_DIST = Math.Max(EYE_DIST / 1.1, 0.1)
        If e.Delta < 0 Then EYE_DIST = Math.Min(EYE_DIST * 1.1, 80)
    End Sub

End Class




