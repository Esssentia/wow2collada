''' <summary>
''' Class to deal with all Direct3D related activities (rendering, camera, lights, ...)
''' </summary>
''' <remarks></remarks>
Class render3d

    ' The Direct3D device.
    Private m_Device As Device

    ' The canvas
    Private _canvas As System.Windows.Forms.PictureBox

    ' The material.
    Private m_Material As Material

    ' Camera stuff
    Private CAM_POSITION As Vector3 = New Vector3(0, 3, -4)
    Private VIEW_VECTOR As Vector3 = Vector3.Normalize(New Vector3(0, -3, 4))
    Private MOVE_VECTOR As Vector3 = Vector3.Normalize(New Vector3(0, 0, 1))
    Private ROTATION_VECTOR As Vector3 = New Vector3(0, 0, 0)
    Private RIGHT_VECTOR As Vector3 = New Vector3(1, 0, 0)
    Private UP_VECTOR As Vector3 = New Vector3(0, 1, 0)

    ' World stuff
    Private ROTATE As Boolean = True

    ' The vertex buffer that holds drawing data.
    Private m_VertexBuffer As VertexBuffer = Nothing
    Private DeviceLost As Boolean = False
    Private PresentParams As PresentParameters

    ' Intermediate data structures to keep 3D subsystem and program separate
    Private Structure m_Vertex
        Dim Position As Vector3
        Dim Normal As Vector3
        Dim UV As Vector2
    End Structure

    Private Structure m_Triangle
        Dim TextureID As String
        Dim P() As m_Vertex
    End Structure

    Private m_Textures As New Dictionary(Of String, Texture)
    Private m_Triangles As New List(Of m_Triangle)

    Sub Camera_Move(ByVal Direction As Vector3)
        CAM_POSITION += Direction
    End Sub

    Sub Camera_RotateX(ByVal Angle As Single)
        ROTATION_VECTOR.X += Angle

        'Rotate viewdir around the right vector:
        VIEW_VECTOR = Vector3.Normalize(VIEW_VECTOR * Math.Cos(Angle * Math.PI / 180) + UP_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new UP_VECTOR (by cross product)
        UP_VECTOR = Vector3.Cross(VIEW_VECTOR, RIGHT_VECTOR) * -1
    End Sub

    Sub Camera_RotateY(ByVal Angle As Single)
        ROTATION_VECTOR.Y += Angle

        'Rotate viewdir around the up vector:
        VIEW_VECTOR = Vector3.Normalize(VIEW_VECTOR * Math.Cos(Angle * Math.PI / 180) - RIGHT_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new RIGHT_VECTOR (by cross product)
        RIGHT_VECTOR = Vector3.Cross(VIEW_VECTOR, UP_VECTOR)
    End Sub

    Sub Camera_RotateZ(ByVal Angle As Single)
        ROTATION_VECTOR.Z += Angle

        'Rotate viewdir around the right vector:
        RIGHT_VECTOR = Vector3.Normalize(RIGHT_VECTOR * Math.Cos(Angle * Math.PI / 180) + UP_VECTOR * Math.Sin(Angle * Math.PI / 180))

        'Now compute the new UP_VECTOR (by cross product)
        UP_VECTOR = Vector3.Cross(VIEW_VECTOR, RIGHT_VECTOR) * -1
    End Sub

    Sub Camera_MoveForward(ByVal Distance As Single)
        CAM_POSITION += VIEW_VECTOR * Distance
    End Sub

    Sub Camera_Strafe(ByVal Distance As Single)
        CAM_POSITION += RIGHT_VECTOR * Distance
    End Sub

    Sub Camera_MoveUpDown(ByVal Distance As Single)
        CAM_POSITION += UP_VECTOR * Distance
    End Sub

    Public Sub New(ByVal canvas As System.Windows.Forms.PictureBox)
        _canvas = canvas
    End Sub

    ' Initialize the graphics device. Return True if successful.
    Public Function InitializeGraphics() As Boolean
        PresentParams = New PresentParameters

        'set some presentation parameters (mainly todo with depth stuff)
        PresentParams.Windowed = True
        PresentParams.SwapEffect = SwapEffect.Discard
        PresentParams.BackBufferFormat = Format.Unknown
        PresentParams.EnableAutoDepthStencil = True
        PresentParams.AutoDepthStencilFormat = DepthFormat.D16

        ' Best: Hardware device and hardware vertex processing.
        Try
            m_Device = New Device(0, DeviceType.Hardware, _canvas, CreateFlags.HardwareVertexProcessing, PresentParams)
        Catch
        End Try

        ' Good: Hardware device and software vertex processing.
        If m_Device Is Nothing Then
            Try
                m_Device = New Device(0, DeviceType.Hardware, _canvas, CreateFlags.SoftwareVertexProcessing, PresentParams)
            Catch
            End Try
        End If

        ' Adequate?: Software device and software vertex processing.
        If m_Device Is Nothing Then
            Try
                m_Device = New Device(0, DeviceType.Reference, _canvas, CreateFlags.SoftwareVertexProcessing, PresentParams)
            Catch ex As Exception
                ' If we still can't make a device, give up.
                MessageBox.Show("Error initializing Direct3D" & vbCrLf & vbCrLf & ex.Message, "Direct3D Error", MessageBoxButtons.OK)
                Return False
            End Try
        End If

        ' Turn on D3D lighting.
        m_Device.RenderState.Lighting = True

        ' Cull triangles that are oriented counter clockwise.
        m_Device.RenderState.CullMode = Cull.None

        ' Make points bigger so they're easy to see.
        m_Device.RenderState.PointSize = 1

        ' Use z-Buffer
        m_Device.RenderState.ZBufferEnable = True

        ' Enable Transparency (needed for Textures)
        m_Device.RenderState.AlphaBlendEnable = True
        m_Device.RenderState.DestinationBlend = Direct3D.Blend.InvSourceAlpha
        m_Device.RenderState.SourceBlend = Direct3D.Blend.SourceAlpha

        ' Create the initial vertex data.
        CreateTriangleList()
        CreateVertexBuffer()

        ' Make the material.
        SetupMaterial()

        ' Make the lights.
        SetupLights()

        ' We succeeded.
        Return True
    End Function

    ' Draw.
    Public Sub Render()
        'Do we still have a device?
        If DeviceLost Then
            Try
                m_Device.Reset(PresentParams)
                SetupScene()
                DeviceLost = False
            Catch
                DeviceLost = True
            End Try
        End If
        ' Clear the back buffer.
        m_Device.Clear(ClearFlags.Target Or ClearFlags.ZBuffer, Color.Black, 1, 0)

        ' Make a scene.
        m_Device.BeginScene()

        ' Draw stuff here...
        ' Setup the world, view, and projection matrices.
        SetupMatrices()

        ' Set the device's data stream source (the vertex buffer).
        m_Device.SetStreamSource(0, m_VertexBuffer, 0)

        ' Tell the device the format of the vertices.
        ' And draw the primitives in the data stream.
        m_Device.VertexFormat = CustomVertex.PositionNormalTextured.Format

        Dim lastTex As String = "{--}"

        For i As Integer = 0 To m_Triangles.Count - 1
            With m_Triangles(i)
                If .TextureID <> lastTex Then
                    If .TextureID > "" Then If Not m_Textures Is Nothing Then If m_Textures.ContainsKey(.TextureID) Then m_Device.SetTexture(0, m_Textures(.TextureID))
                    lastTex = .TextureID
                End If

                m_Device.DrawPrimitives(PrimitiveType.TriangleList, i * 3, 1)
            End With
        Next


        ' End the scene and display.
        m_Device.EndScene()
        Try
            m_Device.Present()
        Catch
            DeviceLost = True
        End Try
    End Sub

    ' Setup the world, view, and projection matrices.
    Private Sub SetupMatrices()
        ' World Matrix:
        ' Rotate the object around the Y axis by 2 * Pi radians per 8000 ticks (8 seconds).
        Const TICKS_PER_REV As Integer = 8000
        Dim angle As Double = Environment.TickCount * (2 * Math.PI) / TICKS_PER_REV
        If ROTATE Then m_Device.Transform.World = Matrix.RotationY(CSng(angle))

        ' View Matrix:

        m_Device.Transform.View = Matrix.LookAtLH(CAM_POSITION, CAM_POSITION + VIEW_VECTOR, UP_VECTOR)

        ' Projection Matrix:
        ' Perspective transformation defined by:
        '       Field of view           Pi / 4
        '       Aspect ratio            1
        '       Near clipping plane     Z = 1
        '       Far clipping plane      Z = 100
        m_Device.Transform.Projection = Matrix.PerspectiveFovLH(Math.PI / 4, _canvas.Width / _canvas.Height, 1, 1000)
    End Sub

    ' Make the material.
    Public Sub SetupMaterial()
        m_Material = New Material()
        m_Material.Ambient = Color.FromArgb(255, 32, 32, 32)
        m_Material.Diffuse = Color.White
        m_Device.Material = m_Material
    End Sub

    ' Make the lights.
    Public Sub SetupLights()
        ' Make a light.
        m_Device.Lights(0).Type = LightType.Directional
        m_Device.Lights(0).Diffuse = Color.White
        m_Device.Lights(0).Ambient = Color.White
        m_Device.Lights(0).Direction = New Vector3(0, -1, 0)
        m_Device.Lights(0).Enabled = True

        m_Device.Lights(1).Type = LightType.Directional
        m_Device.Lights(1).Diffuse = Color.White
        m_Device.Lights(1).Ambient = Color.White
        m_Device.Lights(1).Direction = New Vector3(1, -1, 1)
        m_Device.Lights(1).Enabled = True

        ' Add some ambient light.
        m_Device.RenderState.Ambient = Color.Gray
    End Sub

    Sub AlterFillMode(ByVal fi As FillMode)
        If m_Device Is Nothing Then Exit Sub
        m_Device.RenderState.FillMode = fi
    End Sub

    Public Sub ResetView()
        CAM_POSITION = New Vector3(0, 3, -4)
        VIEW_VECTOR = Vector3.Normalize(New Vector3(0, -3, 4))
        MOVE_VECTOR = Vector3.Normalize(New Vector3(0, 0, 1))
        ROTATION_VECTOR = New Vector3(0, 0, 0)
        UP_VECTOR = New Vector3(0, 1, 0)
        RIGHT_VECTOR = New Vector3(1, 0, 0)
    End Sub

    Sub CreateTextureList()
        m_Textures.Clear()
        For Each Tex As HelperFunctions.sTexture In myHF.Textures.Values
            m_Textures(Tex.ID) = Texture.FromBitmap(m_Device, Tex.TexGra, Usage.None, Pool.Managed)
        Next
    End Sub

    ''' <summary>
    ''' Copies the global triangle list to the local one
    ''' Also transforms to Direct3D coordinate sytem
    ''' from Blizzard-Coordinate-System (Z-up) to Direct3D-Coordinate-System (Y-up)
    ''' </summary>
    ''' <remarks></remarks>
    Sub CreateTriangleList()
        m_Triangles.Clear()
        For Each Tri As HelperFunctions.sTriangle In myHF.TriangleList
            Dim mTri As New m_Triangle
            mTri.TextureID = Tri.TextureID
            mTri.P = New m_Vertex() {New m_Vertex(), New m_Vertex, New m_Vertex}
            For j As Integer = 0 To 2
                mTri.P(j).Position.X = Tri.P(j).Position.x
                mTri.P(j).Position.Y = Tri.P(j).Position.Z
                mTri.P(j).Position.Z = -Tri.P(j).Position.Y
                mTri.P(j).Normal.X = Tri.P(j).Normal.x
                mTri.P(j).Normal.Y = Tri.P(j).Normal.Z
                mTri.P(j).Normal.Z = -Tri.P(j).Normal.Y
                mTri.P(j).UV = Tri.P(j).UV
            Next
            m_Triangles.Add(mTri)
        Next
    End Sub

    Public Sub SetupScene()
        CreateTextureList()
        CreateTriangleList()
        CreateVertexBuffer()
        CanvasTainted = False 'resume rendering
    End Sub

    Sub CreateVertexBuffer()
        'lock vertex buffer
        m_VertexBuffer = New VertexBuffer(GetType(CustomVertex.PositionNormalTextured), m_Triangles.Count * 3, m_Device, 0, CustomVertex.PositionNormalTextured.Format, Pool.Default)

        Dim vertices As CustomVertex.PositionNormalTextured() = CType(m_VertexBuffer.Lock(0, 0), CustomVertex.PositionNormalTextured())

        For i = 0 To m_Triangles.Count - 1
            With m_Triangles(i)
                For j = 0 To 2
                    vertices(i * 3 + j).Position = .P(j).Position
                    vertices(i * 3 + j).Normal = .P(j).Normal
                    vertices(i * 3 + j).Tu = .P(j).UV.X
                    vertices(i * 3 + j).Tv = .P(j).UV.Y
                Next
            End With
        Next

        m_VertexBuffer.Unlock()
    End Sub

    Public Sub WorldRotation(ByVal Value As Boolean)
        ROTATE = Value
    End Sub

End Class