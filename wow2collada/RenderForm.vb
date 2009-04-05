Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Public Class RenderForm
    ' The Direct3D device.
    Public m_Device As Device

    ' The material.
    Private m_Material As Material

    Private MouseIsDown As Boolean
    Private MousePosX As Integer
    Private MousePosY As Integer
    Private ModelOldX As Single
    Private ModelOldY As Single
    Private ModelOldZ As Single

#Region "D3D Setup Code"
    ' Data variables.
    Public NUM_TRIANGLES As Integer = 12
    Public NUM_POINTS As Integer = 3 * NUM_TRIANGLES
    Public DIS_VECTOR As Vector3 = New Vector3(0, 3, -4)
    Public LAT_VECTOR As Vector3 = New Vector3(0, 0, 0)

    ' The vertex buffer that holds drawing data.
    Public m_VertexBuffer As VertexBuffer = Nothing

    ' Initialize the graphics device. Return True if successful.
    Public Function InitializeGraphics() As Boolean
        Dim params As New PresentParameters
        params.Windowed = True
        params.SwapEffect = SwapEffect.Discard

        params.BackBufferFormat = Format.Unknown
        params.EnableAutoDepthStencil = True
        params.AutoDepthStencilFormat = DepthFormat.D16


        ' Best: Hardware device and hardware vertex processing.
        Try
            m_Device = New Device(0, DeviceType.Hardware, pic3d, CreateFlags.HardwareVertexProcessing, params)
            Debug.WriteLine("Hardware, HardwareVertexProcessing")
        Catch
        End Try

        ' Good: Hardware device and software vertex processing.
        If m_Device Is Nothing Then
            Try
                m_Device = New Device(0, DeviceType.Hardware, pic3d, CreateFlags.SoftwareVertexProcessing, params)
                Debug.WriteLine("Hardware, SoftwareVertexProcessing")
            Catch
            End Try
        End If

        ' Adequate?: Software device and software vertex processing.
        If m_Device Is Nothing Then
            Try
                m_Device = New Device(0, DeviceType.Reference, pic3d, CreateFlags.SoftwareVertexProcessing, params)
                Debug.WriteLine("Reference, SoftwareVertexProcessing")
            Catch ex As Exception
                ' If we still can't make a device, give up.
                MessageBox.Show("Error initializing Direct3D" & vbCrLf & vbCrLf & ex.Message, "Direct3D Error", MessageBoxButtons.OK)
                Return False
            End Try
        End If

        ' Turn on D3D lighting.
        m_Device.RenderState.Lighting = True

        ' Cull triangles that are oriented counter clockwise.
        m_Device.RenderState.CullMode = Cull.CounterClockwise

        ' Make points bigger so they're easy to see.
        m_Device.RenderState.PointSize = 1

        ' Use z-Buffer
        m_Device.RenderState.ZBufferEnable = True

        ' Create the vertex data.
        CreateVertexBuffer_Orig()

        ' Make the material.
        SetupMaterial()

        ' Make the lights.
        SetupLights()

        ' We succeeded.
        Return True
    End Function

    ' Create a vertex buffer for the device.
    Public Sub CreateVertexBuffer_Orig()

        Me.NUM_TRIANGLES = 12
        Me.NUM_POINTS = Me.NUM_TRIANGLES * 3

        ' Create a buffer.
        m_VertexBuffer = New VertexBuffer(GetType(CustomVertex.PositionNormalColored), NUM_POINTS, m_Device, 0, CustomVertex.PositionNormalColored.Format, Pool.Default)

        ' Lock the vertex buffer. 
        ' Lock returns an array of PositionNormalColored objects.
        Dim vertices As CustomVertex.PositionNormalColored() = CType(m_VertexBuffer.Lock(0, 0), CustomVertex.PositionNormalColored())

        ' Make the vertexes.
        Dim i As Integer = 0
        ' Top +Y.
        MakeRectanglePNC(vertices, i, _
            1, 1, -1, Color.Green.ToArgb, _
            -1, 1, -1, Color.Green.ToArgb, _
            -1, 1, 1, Color.Green.ToArgb, _
            1, 1, 1, Color.Green.ToArgb)

        ' Bottom. -Y
        MakeRectanglePNC(vertices, i, _
            1, -1, 1, Color.Green.ToArgb, _
            -1, -1, 1, Color.Green.ToArgb, _
            -1, -1, -1, Color.Green.ToArgb, _
            1, -1, -1, Color.Green.ToArgb)

        ' Right +X.
        MakeRectanglePNC(vertices, i, _
            1, 1, -1, Color.Red.ToArgb, _
            1, 1, 1, Color.Red.ToArgb, _
            1, -1, 1, Color.Red.ToArgb, _
            1, -1, -1, Color.Red.ToArgb)

        ' Left -X.
        MakeRectanglePNC(vertices, i, _
            -1, -1, -1, Color.Red.ToArgb, _
            -1, -1, 1, Color.Red.ToArgb, _
            -1, 1, 1, Color.Red.ToArgb, _
            -1, 1, -1, Color.Red.ToArgb)

        ' Front -Z.
        MakeRectanglePNC(vertices, i, _
            -1, 1, -1, Color.Blue.ToArgb, _
            1, 1, -1, Color.Blue.ToArgb, _
            1, -1, -1, Color.Blue.ToArgb, _
            -1, -1, -1, Color.Blue.ToArgb)

        ' Back +Z.
        MakeRectanglePNC(vertices, i, _
            -1, -1, 1, Color.Blue.ToArgb, _
            1, -1, 1, Color.Blue.ToArgb, _
            1, 1, 1, Color.Blue.ToArgb, _
            -1, 1, 1, Color.Blue.ToArgb)

        m_VertexBuffer.Unlock()
    End Sub

    ' Add two triangles to make a rectangle to the vertex buffer.
    Private Sub MakeRectanglePNC(ByVal vertices() As CustomVertex.PositionNormalColored, ByRef i As Integer, _
        ByVal x0 As Single, ByVal y0 As Single, ByVal z0 As Single, ByVal c0 As Integer, _
        ByVal x1 As Single, ByVal y1 As Single, ByVal z1 As Single, ByVal c1 As Integer, _
        ByVal x2 As Single, ByVal y2 As Single, ByVal z2 As Single, ByVal c2 As Integer, _
        ByVal x3 As Single, ByVal y3 As Single, ByVal z3 As Single, ByVal c3 As Integer)

        Dim vec0 As New Vector3(x1 - x0, y1 - y0, z1 - z0)
        Dim vec1 As New Vector3(x2 - x1, y2 - y1, z2 - z1)
        Dim n As Vector3 = Vector3.Cross(vec0, vec1)
        n.Normalize()

        vertices(i) = New CustomVertex.PositionNormalColored(x0, y0, z0, n.X, n.Y, n.Z, c0)
        i += 1
        vertices(i) = New CustomVertex.PositionNormalColored(x1, y1, z1, n.X, n.Y, n.Z, c1)
        i += 1
        vertices(i) = New CustomVertex.PositionNormalColored(x2, y2, z2, n.X, n.Y, n.Z, c2)
        i += 1

        vertices(i) = New CustomVertex.PositionNormalColored(x0, y0, z0, n.X, n.Y, n.Z, c0)
        i += 1
        vertices(i) = New CustomVertex.PositionNormalColored(x2, y2, z2, n.X, n.Y, n.Z, c2)
        i += 1
        vertices(i) = New CustomVertex.PositionNormalColored(x3, y3, z3, n.X, n.Y, n.Z, c3)
        i += 1
    End Sub

#End Region ' D3D Setup Code

#Region "D3D Drawing Code"
    ' Draw.
    Public Sub Render()
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
        m_Device.VertexFormat = CustomVertex.PositionNormalColored.Format

        ' Draw the primitives in the data stream.
        m_Device.DrawPrimitives(PrimitiveType.TriangleList, 0, NUM_TRIANGLES)

        ' End the scene and display.
        m_Device.EndScene()
        m_Device.Present()
    End Sub

    ' Setup the world, view, and projection matrices.
    Private Sub SetupMatrices()
        ' World Matrix:
        ' Rotate the object around the Y axis by
        ' 2 * Pi radians per 4000 ticks (4 seconds).
        Const TICKS_PER_REV As Integer = 8000
        Dim angle As Double = Environment.TickCount * (2 * Math.PI) / TICKS_PER_REV
        m_Device.Transform.World = Matrix.RotationY(CSng(angle))

        ' View Matrix:
        m_Device.Transform.View = Matrix.LookAtLH(DIS_VECTOR, LAT_VECTOR, New Vector3(0, 1, 0))

        ' Projection Matrix:
        ' Perspective transformation defined by:
        '       Field of view           Pi / 4
        '       Aspect ratio            1
        '       Near clipping plane     Z = 1
        '       Far clipping plane      Z = 100
        m_Device.Transform.Projection = _
            Matrix.PerspectiveFovLH(Math.PI / 4, 1, 1, 1000)
    End Sub

    ' Make the material.
    Private Sub SetupMaterial()
        m_Material = New Material()
        m_Material.Ambient = Color.FromArgb(255, 32, 32, 32)
        m_Material.Diffuse = Color.White
        m_Device.Material = m_Material
    End Sub

    ' Make the lights.
    Private Sub SetupLights()
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
#End Region ' D3D Drawing Code

    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        ToolStripStatusLabel1.Text = OpenFileDialog1.FileName
        LoadModel()
    End Sub

    Private Sub RenderForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ToolStripStatusLabel1.Text = "D:\temp\mpq\World\AZEROTH\WESTFALL\PASSIVEDOODADS\Crate\WestFallCrate.m2"
        LoadModel()
    End Sub

    Private Sub SolidToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SolidToolStripMenuItem.Click
        If m_Device Is Nothing Then Exit Sub
        m_Device.RenderState.FillMode = FillMode.Solid
        VerticesToolStripMenuItem.Checked = False
        WireframeToolStripMenuItem.Checked = False
        SolidToolStripMenuItem.Checked = True
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        OpenFileDialog1.Filter = "M2 Files (*.m2)|*.m2"
        OpenFileDialog1.InitialDirectory = "d:\temp\mpq"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub WireframeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WireframeToolStripMenuItem.Click
        If m_Device Is Nothing Then Exit Sub
        m_Device.RenderState.FillMode = FillMode.WireFrame
        VerticesToolStripMenuItem.Checked = False
        WireframeToolStripMenuItem.Checked = True
        SolidToolStripMenuItem.Checked = False
    End Sub

    Private Sub VerticesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VerticesToolStripMenuItem.Click
        If m_Device Is Nothing Then Exit Sub
        m_Device.RenderState.FillMode = FillMode.Point
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
        ModelOldX = LAT_VECTOR.X
        ModelOldY = LAT_VECTOR.Y
        ModelOldZ = DIS_VECTOR.Z
    End Sub

    Private Sub pic3d_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles pic3d.MouseLeave
        MouseIsDown = False
    End Sub

    Private Sub pic3d_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic3d.MouseMove
        If MouseIsDown Then
            If (ModifierKeys And Keys.Shift) = Keys.Shift Then
                DIS_VECTOR.Z = Math.Max(Math.Min(ModelOldZ - 20 * (e.X - MousePosX) / 600, -1), -100)
            Else
                LAT_VECTOR.X = Math.Max(Math.Min(ModelOldX - 20 * (e.X - MousePosX) / 600, 50), -50)
                LAT_VECTOR.Y = Math.Max(Math.Min(ModelOldY + 20 * (e.Y - MousePosY) / 600, 50), -50)
            End If
        End If
    End Sub

    Private Sub pic3d_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic3d.MouseUp
        MouseIsDown = False
    End Sub

    Public Sub LoadModel()

        'temp
        Dim hf As New wow2collada.HelperFunctions
        'open.MPQ or something like it would be nice :)
        'hf.LoadBLP("D:\temp\mpq\Textures\BloodSplats\BloodSplatBlack01.blp")

        'reset view
        DIS_VECTOR = New Vector3(0, 3, -4)
        LAT_VECTOR = New Vector3(0, 0, 0)

        ListBox1.Items.Clear()
        Dim MD20 As New wow2collada.FileReaders.M2
        Dim SKIN As New wow2collada.FileReaders.SKIN
        'Dim WMOROOT As New WMOtoCollada.FileReaders.WMOROOT
        'Dim ADT As New WMOtoCollada.FileReaders.ADT

        Dim FileName As String = ToolStripStatusLabel1.Text
        Dim FileNameMD20 As String = FileName.Substring(0, FileName.LastIndexOf(".")) + ".m2"
        Dim FileNameSKIN As String = FileName.Substring(0, FileName.LastIndexOf(".")) + "00.skin"
        'Dim FileNameWMOROOT As String = "D:\temp\mpq\World\wmo\Azeroth\Buildings\GoldshireInn\GoldshireInn.wmo"
        'Dim FileNameADT As String = "D:\temp\mpq\World\maps\Azeroth\Azeroth_35_35.adt"
        MD20.Load(FileNameMD20)
        SKIN.Load(FileNameSKIN)
        'WMOROOT.Load(FileNameWMOROOT)
        'ADT.Load(FileNameADT)

        hf.CreateVertexBuffer(Me, MD20, SKIN)

        ListBox1.Items.Add("M2 File: " & FileNameMD20)
        ListBox1.Items.Add("M2 Model: " & MD20.ModelName)
        ListBox1.Items.Add("M2 Version: " & MD20.VersionInfo)
        ListBox1.Items.Add("M2 Vertices: " & MD20.Vertices.Length)
        'For i As Integer = 0 To MD20.Vertices.Length - 1
        'ListBox1.Items.Add(MD20.Vertices(i).toString)
        'Next
        ListBox1.Items.Add("SKIN File: " & FileNameSKIN)
        ListBox1.Items.Add("SKIN Triangles: " & SKIN.Triangles.Length)
        ListBox1.Items.Add("SKIN Boneindices: " & SKIN.BoneIndices.Length)
        ListBox1.Items.Add("SKIN Submeshes: " & SKIN.SubMeshes.Length)

        For i As Integer = 0 To SKIN.SubMeshes.Length - 1
            ListBox1.Items.Add("  Submesh [" & SKIN.SubMeshes(i).ID & "]: (Bones:" & SKIN.SubMeshes(i).nBones & ", Tris: " & SKIN.SubMeshes(i).nTriangles & ", Verts: " & SKIN.SubMeshes(i).nVertices & ")")
        Next

        For i As Integer = 0 To SKIN.TextureUnits.Length - 1
            ListBox1.Items.Add("  Textures [" & SKIN.TextureUnits(i).SubmeshIndex1 & " - " & SKIN.TextureUnits(i).Texture & "]")
            ListBox1.Items.Add("     File: " & MD20.Textures(SKIN.TextureUnits(i).Texture).Type & " - " & MD20.Textures(SKIN.TextureUnits(i).Texture).Filename & "]")
        Next
        'For i As Integer = 0 To SKIN.Triangles.Length - 1
        '    ListBox1.Items.Add("[" & MD20.Vertices(SKIN.Triangles(i).VertexIndex1).toString & ", " & MD20.Vertices(SKIN.Triangles(i).VertexIndex2).toString & ", " & MD20.Vertices(SKIN.Triangles(i).VertexIndex3).toString & "]")
        'Next

        'ListBox1.Items.Add("WMO Root File: " & FileNameWMOROOT)
        'ListBox1.Items.Add("WMO Textures: " & WMOROOT.Textures.Length)
        'For i As Integer = 0 To WMOROOT.Textures.Length - 1
        '    ListBox1.Items.Add(WMOROOT.Textures(i))
        'Next
        'ListBox1.Items.Add("WMO Doodads: " & WMOROOT.Doodads.Length)
        'For i As Integer = 0 To WMOROOT.Doodads.Length - 1
        '    ListBox1.Items.Add(WMOROOT.Doodads(i).ModelFile)
        'Next

        'ListBox1.Items.Add("ADT Root File: " & FileNameADT)
        'ListBox1.Items.Add("ADT Textures: " & ADT.TextureFiles.Length)
        'For i As Integer = 0 To ADT.TextureFiles.Length - 1
        '    ListBox1.Items.Add(ADT.TextureFiles(i))
        'Next
        'ListBox1.Items.Add("ADT Models: " & ADT.ModelFiles.Length)
        'For i As Integer = 0 To ADT.ModelFiles.Length - 1
        '    ListBox1.Items.Add(ADT.ModelFiles(i))
        'Next
        'ListBox1.Items.Add("ADT WMOs: " & ADT.WMOFiles.Length)
        'For i As Integer = 0 To ADT.WMOFiles.Length - 1
        '    ListBox1.Items.Add(ADT.WMOFiles(i))
        'Next
        'ListBox1.Items.Add("MCNK Chunks: " & ADT.MCNKs.Length)
        'For i As Integer = 0 To ADT.MCNKs.Length - 1
        '    ListBox1.Items.Add(ADT.MCNKs(i).offsHeight)
        'Next
    End Sub

End Class