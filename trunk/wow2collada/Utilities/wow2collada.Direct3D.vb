Imports System.IO
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Namespace wow2collada


    Class render3d

        ' The Direct3D device.
        Public m_Device As Device

        ' The canvas
        Private _canvas As System.Windows.Forms.PictureBox

        ' The material.
        Private m_Material As Material

        ' Data variables.
        Public NUM_POINTS As Integer
        Public DIS_VECTOR As Vector3 = New Vector3(0, 3, -4)
        Public LAT_VECTOR As Vector3 = New Vector3(0, 0, 0)
        Public ROT_VECTOR As Vector3 = New Vector3(0, 0, 0)

        ' The vertex buffer that holds drawing data.
        Private m_VertexBuffer As VertexBuffer = Nothing
        Private DeviceLost As Boolean = False
        Private PresentParams As PresentParameters

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
            Debug.Print(m_Device.RenderState.ZBufferWriteEnable)

            ' Enable Transparency (needed for Textures)
            m_Device.RenderState.AlphaBlendEnable = True
            m_Device.RenderState.DestinationBlend = Direct3D.Blend.InvSourceAlpha
            m_Device.RenderState.SourceBlend = Direct3D.Blend.SourceAlpha

            ' Create the initial vertex data.
            CreateVertexBufferInit()

            ' Make the material.
            SetupMaterial()

            ' Make the lights.
            SetupLights()

            ' We succeeded.
            Return True
        End Function

        ' Create a (dummy) vertex buffer for the device.
        Public Sub CreateVertexBufferInit()
            Me.NUM_POINTS = 3
            m_VertexBuffer = New VertexBuffer(GetType(CustomVertex.PositionNormalColored), NUM_POINTS, m_Device, 0, CustomVertex.PositionNormalColored.Format, Pool.Default)
            Dim vertices As CustomVertex.PositionNormalColored() = CType(m_VertexBuffer.Lock(0, 0), CustomVertex.PositionNormalColored())
            vertices(0) = New CustomVertex.PositionNormalColored(0, 1, 0, 0, 1, 0, Color.Blue.ToArgb)
            vertices(1) = New CustomVertex.PositionNormalColored(1, 1, 0, 0, 1, 0, Color.Red.ToArgb)
            vertices(2) = New CustomVertex.PositionNormalColored(1, 0, 0, 0, 1, 0, Color.Green.ToArgb)
            m_VertexBuffer.Unlock()
        End Sub

        ' Draw.
        Public Sub Render()
            'Do we still have a device?
            If DeviceLost Then
                Try
                    m_Device.Reset(PresentParams)
                    ResumeScene()
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
            If wow2collada.myHF.m_Textures Is Nothing Then
                m_Device.VertexFormat = CustomVertex.PositionNormalColored.Format
                m_Device.DrawPrimitives(PrimitiveType.TriangleList, 0, NUM_POINTS / 3)
            Else
                m_Device.VertexFormat = CustomVertex.PositionNormalTextured.Format
                For i As Integer = 0 To wow2collada.myHF.m_TriangleList.Count - 1
                    With wow2collada.myHF.m_TriangleList(i)
                        'Debug.Print(.TextureID)
                        If .TextureID > "" Then If wow2collada.myHF.m_Textures.ContainsKey(.TextureID) Then m_Device.SetTexture(0, wow2collada.myHF.m_Textures(.TextureID).TexObj)
                        m_Device.DrawPrimitives(PrimitiveType.TriangleList, i * 3, 1)
                    End With
                Next
            End If

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
            ' Rotate the object around the Y axis by
            ' 2 * Pi radians per 4000 ticks (4 seconds).
            Const TICKS_PER_REV As Integer = 8000
            Dim angle As Double = Environment.TickCount * (2 * Math.PI) / TICKS_PER_REV
            m_Device.Transform.World = Matrix.RotationY(CSng(angle))

            ' View Matrix:
            Dim r As New Matrix
            r = Matrix.RotationX(ROT_VECTOR.X)
            r.Multiply(Matrix.RotationY(ROT_VECTOR.Y))
            r.Multiply(Matrix.RotationZ(ROT_VECTOR.Z))

            Dim v As Vector4 = Vector3.Transform(DIS_VECTOR, r)
            m_Device.Transform.View = Matrix.LookAtLH(New Vector3(v.X, v.Y, v.Z), LAT_VECTOR, New Vector3(0, 1, 0))

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

        Sub ResetView()
            DIS_VECTOR = New Vector3(0, 3, -4)
            LAT_VECTOR = New Vector3(0, 0, 0)
            ROT_VECTOR = New Vector3(0, 0, 0)
        End Sub

        Sub ResetScene()
            CanvasTainted = True ' stop rendering
            wow2collada.myHF.m_Textures.Clear()
            wow2collada.myHF.m_TriangleList.Clear()
            NUM_POINTS = 0
        End Sub

        Sub ResumeScene()
            For i As Integer = 0 To wow2collada.myHF.m_Textures.Count - 1
                Dim Tex As New wow2collada.HelperFunctions.sTexture
                Tex.FileName = wow2collada.myHF.m_Textures.ElementAt(i).Value.FileName
                Tex.TexGra = wow2collada.myHF.m_Textures.ElementAt(i).Value.TexGra
                Tex.TexObj = Texture.FromBitmap(m_Device, Tex.TexGra, Usage.None, Pool.Managed)
                wow2collada.myHF.m_Textures(Tex.FileName) = Tex
            Next

            NUM_POINTS = wow2collada.myHF.m_TriangleList.Count * 3
            m_VertexBuffer = New VertexBuffer(GetType(CustomVertex.PositionNormalTextured), NUM_POINTS, m_Device, 0, CustomVertex.PositionNormalTextured.Format, Pool.Default)

            Dim vertices As CustomVertex.PositionNormalTextured() = CType(m_VertexBuffer.Lock(0, 0), CustomVertex.PositionNormalTextured())

            'also translate from Blizzard-Coordinate-System (Z-up) to Direct3D-Coordinate-System (Y-up)
            Dim tMat As New Matrix

            tMat.M11 = 1
            tMat.M23 = -1
            tMat.M32 = 1
            tMat.M44 = 1

            For i = 0 To wow2collada.myHF.m_TriangleList.Count - 1
                vertices(i * 3 + 0).Position = Vector3.TransformCoordinate(wow2collada.myHF.m_TriangleList(i).P1.Position, tMat)
                vertices(i * 3 + 0).Normal = Vector3.TransformCoordinate(wow2collada.myHF.m_TriangleList(i).P1.Normal, tMat)
                vertices(i * 3 + 0).Tu = wow2collada.myHF.m_TriangleList(i).P1.UV.X
                vertices(i * 3 + 0).Tv = wow2collada.myHF.m_TriangleList(i).P1.UV.Y
                vertices(i * 3 + 1).Position = Vector3.TransformCoordinate(wow2collada.myHF.m_TriangleList(i).P2.Position, tMat)
                vertices(i * 3 + 1).Normal = Vector3.TransformCoordinate(wow2collada.myHF.m_TriangleList(i).P2.Normal, tMat)
                vertices(i * 3 + 1).Tu = wow2collada.myHF.m_TriangleList(i).P2.UV.X
                vertices(i * 3 + 1).Tv = wow2collada.myHF.m_TriangleList(i).P2.UV.Y
                vertices(i * 3 + 2).Position = Vector3.TransformCoordinate(wow2collada.myHF.m_TriangleList(i).P3.Position, tMat)
                vertices(i * 3 + 2).Normal = Vector3.TransformCoordinate(wow2collada.myHF.m_TriangleList(i).P3.Normal, tMat)
                vertices(i * 3 + 2).Tu = wow2collada.myHF.m_TriangleList(i).P3.UV.X
                vertices(i * 3 + 2).Tv = wow2collada.myHF.m_TriangleList(i).P3.UV.Y
            Next

            m_VertexBuffer.Unlock()
            CanvasTainted = False 'resume rendering
        End Sub

        Public Function LoadModelFromMPQ(ByVal FileName As String) As System.Collections.Generic.List(Of String)
            Dim out As New System.Collections.Generic.List(Of String)

            ResetView()
            ResetScene()

            Select Case FileName.Substring(FileName.LastIndexOf(".") + 1).ToLower
                Case "m2", "mdx"

                    Dim MD20 As New wow2collada.FileReaders.M2
                    Dim SKIN As New wow2collada.FileReaders.SKIN
                    Dim FileNameMD20 As String = FileName.Substring(0, FileName.LastIndexOf(".")) + ".m2"
                    Dim FileNameSKIN As String = FileName.Substring(0, FileName.LastIndexOf(".")) + "00.skin"
                    MD20.LoadFromStream(myMPQ.LoadFile(FileNameMD20), FileNameMD20)
                    SKIN.LoadFromStream(myMPQ.LoadFile(FileNameSKIN), FileNameSKIN)

                    CreateVertexBufferFromM2(MD20, SKIN, New Vector3(0, 0, 0), New Quaternion(0, 0, 0, 0), 1) 'textured

                    out.Add("M2 File: " & FileNameMD20)
                    out.Add("M2 Model: " & MD20.ModelName)
                    out.Add("M2 Version: " & MD20.VersionInfo)
                    out.Add("M2 Vertices: " & MD20.Vertices.Length)

                    For i As Integer = 0 To MD20.TextureLookup.Length - 1
                        out.Add(" Texture Map: " & i & " -> " & MD20.TextureLookup(i))
                    Next

                    out.Add("SKIN File: " & FileNameSKIN)
                    out.Add("SKIN Triangles: " & SKIN.Triangles.Length)
                    out.Add("SKIN Boneindices: " & SKIN.BoneIndices.Length)
                    out.Add("SKIN Submeshes: " & SKIN.SubMeshes.Length)

                    For i As Integer = 0 To SKIN.SubMeshes.Length - 1
                        out.Add(" Submesh " & i & " [SubID=" & SKIN.SubMeshes(i).ID & "]: Bones:" & SKIN.SubMeshes(i).nBones & ", Tris: " & SKIN.SubMeshes(i).nTriangles & ", Verts: " & SKIN.SubMeshes(i).nVertices & ")")
                    Next

                    For i As Integer = 0 To wow2collada.myHF.m_Textures.Count - 1
                        With wow2collada.myHF.m_Textures.ElementAt(i).Value
                            out.Add(" Texture [" & .FileName & "]")
                        End With
                    Next

                Case "wmo"
                    Dim WMO As New wow2collada.FileReaders.WMO
                    Dim FileNameWMO As String = FileName
                    WMO.LoadFromStream(myMPQ.LoadFile(FileNameWMO), FileNameWMO)
                    CreateVertexBufferFromWMO(WMO)

                    out.Add("WMO Root File: " & FileNameWMO)
                    If Not WMO.Textures Is Nothing Then
                        out.Add("WMO Textures: " & WMO.Textures.Length)
                        For i As Integer = 0 To WMO.Textures.Length - 1
                            out.Add(WMO.Textures(i))
                        Next
                    End If

                    If WMO.nDoodads > 0 Then

                        frm.StatusLabel1.Text = "Loading Doodads... 0/" & WMO.Doodads.Length
                        frm.ProgressBar1.Value = 0
                        frm.ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)
                        Application.DoEvents()

                        out.Add("WMO Doodads: " & WMO.Doodads.Length)
                        For i As Integer = 0 To WMO.Doodads.Length - 1
                            If WMO.Doodads(i).ModelFile > "" Then

                                frm.StatusLabel1.Text = "Loading Doodads... " & i & "/" & WMO.Doodads.Length
                                frm.ProgressBar1.Value = 100 * (i / WMO.Doodads.Length)
                                frm.ProgressBar1.ForeColor = Color.FromArgb(255, 255 - 255 * frm.ProgressBar1.Value / 100, 255 * frm.ProgressBar1.Value / 100, 0)
                                Application.DoEvents()

                                out.Add(WMO.Doodads(i).ModelFile)
                                Dim MD20 As New wow2collada.FileReaders.M2
                                Dim SKIN As New wow2collada.FileReaders.SKIN
                                Dim FileNameMD20 As String = WMO.Doodads(i).ModelFile.Substring(0, WMO.Doodads(i).ModelFile.LastIndexOf(".")) + ".m2"
                                Dim FileNameSKIN As String = WMO.Doodads(i).ModelFile.Substring(0, WMO.Doodads(i).ModelFile.LastIndexOf(".")) + "00.skin"
                                MD20.LoadFromStream(myMPQ.LoadFile(FileNameMD20), FileNameMD20)
                                SKIN.LoadFromStream(myMPQ.LoadFile(FileNameSKIN), FileNameSKIN)
                                CreateVertexBufferFromM2(MD20, SKIN, WMO.Doodads(i).Position, WMO.Doodads(i).Orientation, WMO.Doodads(i).Scale) 'textured
                            End If
                        Next
                    End If

                    frm.StatusLabel1.Text = frm.currentFile
                    frm.ProgressBar1.Value = 0
                    frm.ProgressBar1.ForeColor = Color.FromArgb(255, 255, 0, 0)

                Case "adt"
                    Dim ADT As New wow2collada.FileReaders.ADT
                    Dim FileNameADT As String = FileName
                    ADT.LoadFromStream(myMPQ.LoadFile(FileNameADT), FileNameADT)
                    CreateVertexBufferFromADT(ADT)

                    out.Add("ADT Root File: " & FileNameADT)
                    out.Add("ADT Textures: " & ADT.TextureFiles.Length)
                    For i As Integer = 0 To ADT.TextureFiles.Length - 1
                        out.Add(ADT.TextureFiles(i))
                    Next
                    out.Add("ADT Models: " & ADT.ModelFiles.Length)
                    For i As Integer = 0 To ADT.ModelFiles.Length - 1
                        out.Add(ADT.ModelFiles(i))
                    Next
                    out.Add("ADT WMOs: " & ADT.WMOFiles.Length)
                    For i As Integer = 0 To ADT.WMOFiles.Length - 1
                        out.Add(ADT.WMOFiles(i))
                    Next
                Case Else
                    out.Add("Unknown file format: " & FileName.Substring(FileName.LastIndexOf(".") + 1).ToLower)
            End Select

            ResumeScene()

            Return out
        End Function

        Public Sub CreateVertexBufferFromM2(ByVal MD20 As wow2collada.FileReaders.M2, ByVal SKIN As wow2collada.FileReaders.SKIN, ByVal Position As Vector3, ByVal Orientation As Quaternion, ByVal Scale As Single)
            Dim BLP As New wow2collada.FileReaders.BLP

            'scale -> rot -> trans
            Dim rMat As New Matrix
            rMat = Matrix.Transformation(New Vector3(0, 0, 0), Quaternion.Identity, New Vector3(Scale, Scale, Scale), New Vector3(0, 0, 0), Orientation, Position)

            For i As Integer = 0 To SKIN.SubMeshes.Length - 1
                Dim TexID As Integer = -1
                For j As Integer = 0 To SKIN.TextureUnits.Length - 1
                    If SKIN.TextureUnits(j).SubmeshIndex1 = i Then TexID = MD20.TextureLookup(SKIN.TextureUnits(j).Texture)
                Next

                Dim TexFi As String = MD20.Textures(TexID).Filename
                If myMPQ.Locate(TexFi) Then
                    If Not wow2collada.myHF.m_Textures.ContainsKey(TexFi) Then
                        Dim Tex As New wow2collada.HelperFunctions.sTexture
                        Dim TexImg As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(TexFi), TexFi)
                        If Not TexImg Is Nothing Then
                            Tex.FileName = TexFi
                            Tex.TexGra = TexImg
                            Tex.TexObj = Texture.FromBitmap(m_Device, TexImg, Usage.None, Pool.Managed)
                            wow2collada.myHF.m_Textures(TexFi) = Tex
                        End If
                    End If

                    For j As Integer = 0 To SKIN.SubMeshes(i).nTriangles - 1
                        Dim k As Integer = SKIN.SubMeshes(i).StartTriangle + j
                        With SKIN.Triangles(k)
                            Dim Tri As New wow2collada.HelperFunctions.sTriangle
                            Dim V1 As wow2collada.HelperFunctions.sVertex
                            Dim V2 As wow2collada.HelperFunctions.sVertex
                            Dim V3 As wow2collada.HelperFunctions.sVertex

                            V1.Position = Vector3.TransformCoordinate(MD20.Vertices(.VertexIndex1).Position, rMat)
                            V1.Normal = Vector3.TransformCoordinate(MD20.Vertices(.VertexIndex1).Normal, rMat)
                            V1.UV = MD20.Vertices(.VertexIndex1).TextureCoords

                            V2.Position = Vector3.TransformCoordinate(MD20.Vertices(.VertexIndex2).Position, rMat)
                            V2.Normal = Vector3.TransformCoordinate(MD20.Vertices(.VertexIndex2).Normal, rMat)
                            V2.UV = MD20.Vertices(.VertexIndex2).TextureCoords

                            V3.Position = Vector3.TransformCoordinate(MD20.Vertices(.VertexIndex3).Position, rMat)
                            V3.Normal = Vector3.TransformCoordinate(MD20.Vertices(.VertexIndex3).Normal, rMat)
                            V3.UV = MD20.Vertices(.VertexIndex3).TextureCoords

                            Tri.P1 = V1
                            Tri.P2 = V2
                            Tri.P3 = V3
                            Tri.TextureID = TexFi
                            wow2collada.myHF.m_TriangleList.Add(Tri)
                        End With
                    Next
                End If
            Next
        End Sub

        Public Sub CreateVertexBufferFromWMO(ByVal WMO As wow2collada.FileReaders.WMO)
            Dim BLP As New wow2collada.FileReaders.BLP

            frm.StatusLabel1.Text = "Loading Textures..."
            Application.DoEvents()

            For i As Integer = 0 To WMO.Textures.Length - 1
                Dim TexFi As String = WMO.Textures(i)
                If myMPQ.Locate(TexFi) Then
                    If Not wow2collada.myHF.m_Textures.ContainsKey(TexFi) Then
                        Dim Tex As New wow2collada.HelperFunctions.sTexture
                        Dim TexImg As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(TexFi), TexFi)
                        If Not TexImg Is Nothing Then
                            Tex.FileName = TexFi
                            Tex.TexGra = TexImg
                            Tex.TexObj = Texture.FromBitmap(m_Device, TexImg, Usage.None, Pool.Managed)
                            wow2collada.myHF.m_Textures(TexFi) = Tex
                        End If
                    End If
                End If
            Next

            frm.StatusLabel1.Text = "Loading Subsets..."
            Application.DoEvents()

            Dim vi As Integer = 0
            Dim ti As Integer = 0
            For i As Integer = 0 To WMO.SubSets.Count - 1
                Application.DoEvents()
                For j As Integer = 0 To WMO.SubSets(i).Triangles.Length - 1
                    Dim MatID As Byte = WMO.SubSets(i).Materials(j)
                    If MatID < WMO.Textures.Length Then
                        Dim Tri As New wow2collada.HelperFunctions.sTriangle
                        Dim V1 As wow2collada.HelperFunctions.sVertex
                        Dim V2 As wow2collada.HelperFunctions.sVertex
                        Dim V3 As wow2collada.HelperFunctions.sVertex

                        With WMO.SubSets(i).Triangles(j)
                            V1.Position = WMO.SubSets(i).Vertices(.VertexIndex1).Position
                            V1.Normal = WMO.SubSets(i).Vertices(.VertexIndex1).Normal
                            V1.UV = WMO.SubSets(i).Vertices(.VertexIndex1).TextureCoords

                            V2.Position = WMO.SubSets(i).Vertices(.VertexIndex2).Position
                            V2.Normal = WMO.SubSets(i).Vertices(.VertexIndex2).Normal
                            V2.UV = WMO.SubSets(i).Vertices(.VertexIndex2).TextureCoords

                            V3.Position = WMO.SubSets(i).Vertices(.VertexIndex3).Position
                            V3.Normal = WMO.SubSets(i).Vertices(.VertexIndex3).Normal
                            V3.UV = WMO.SubSets(i).Vertices(.VertexIndex3).TextureCoords

                            Tri.P1 = V1
                            Tri.P2 = V2
                            Tri.P3 = V3
                            Tri.TextureID = WMO.Textures(MatID)
                            wow2collada.myHF.m_TriangleList.Add(Tri)
                        End With
                    End If
                Next
            Next
        End Sub

        Public Sub CreateVertexBufferFromADT(ByVal ADT As wow2collada.FileReaders.ADT)
            'do the 9x9 and 8x8 matrix 
            '  0/0   0/1   0/2   0/3   0/4   0/5   0/6   0/7   0/8
            '     0/0   0/1   0/2   0/3   0/4   0/5   0/6   0/7
            '  1/0   1/1   1/2   1/3   1/4   1/5   1/6   1/7   1/8
            '     1/0   1/1   1/2   1/3   1/4   1/5   1/6   1/7
            '  2/0   2/1   2/2   2/3   2/4   2/5   2/6   2/7   2/8
            '     2/0   2/1   2/2   2/3   2/4   2/5   2/6   2/7
            '  3/0   3/1   3/2   3/3   3/4   3/5   3/6   3/7   3/8
            '     3/0   3/1   3/2   3/3   3/4   3/5   3/6   3/7
            '  4/0   4/1   4/2   4/3   4/4   4/5   4/6   4/7   4/8
            '     4/0   4/1   4/2   4/3   4/4   4/5   4/6   4/7
            '  5/0   5/1   5/2   5/3   5/4   5/5   5/6   5/7   5/8
            '     5/0   5/1   5/2   5/3   5/4   5/5   5/6   5/7
            '  6/0   6/1   6/2   6/3   6/4   6/5   6/6   6/7   6/8
            '     6/0   6/1   6/2   6/3   6/4   6/5   6/6   6/7
            '  7/0   7/1   7/2   7/3   7/4   7/5   7/6   7/7   7/8
            '     7/0   7/1   7/2   7/3   7/4   7/5   7/6   7/7
            '  8/0   8/1   8/2   8/3   8/4   8/5   8/6   8/7   8/8

            NUM_POINTS = 8 * 8 * 4 * 3 * 256
            wow2collada.myHF.m_Textures = Nothing 'no textures for now
            m_VertexBuffer = New VertexBuffer(GetType(CustomVertex.PositionNormalColored), NUM_POINTS, m_Device, 0, CustomVertex.PositionNormalColored.Format, Pool.Default)

            ' Lock the vertex buffer. 
            ' Lock returns an array of PositionNormalColored objects.
            Dim vertices As CustomVertex.PositionNormalColored() = CType(m_VertexBuffer.Lock(0, 0), CustomVertex.PositionNormalColored())

            Dim dx As Single = (533.3333F / 16) / 8
            Dim dy As Single = (533.3333F / 16) / 8

            For i As Integer = 0 To 7
                For j As Integer = 0 To 7
                    With ADT.MCNKs(j, i)
                        Dim vi As Integer = (i * 8 + j) * 64 * 12
                        Dim xo As Single = (i - 4) * 8 * dx
                        Dim yo As Single = (j - 4) * 8 * dy
                        Dim zo As Single = .Position.Z - 68
                        For x As Integer = 0 To 7
                            Dim xr As Single = xo + x * dx
                            For y As Integer = 0 To 7
                                Dim yr As Single = yo + y * dy
                                Dim vi2 As Integer = (x * 8 + y) * 12
                                vertices(vi + vi2 + 0) = New CustomVertex.PositionNormalColored(xr + 0.0 * dx, zo + .HeightMap9x9(x + 0, y + 0), yr + 0.0 * dy, 0, 1, 0, Color.Green.ToArgb)
                                vertices(vi + vi2 + 1) = New CustomVertex.PositionNormalColored(xr + 0.0 * dx, zo + .HeightMap9x9(x + 0, y + 1), yr + 1.0 * dy, 0, 1, 0, Color.Green.ToArgb)
                                vertices(vi + vi2 + 2) = New CustomVertex.PositionNormalColored(xr + 0.5 * dx, zo + .HeightMap8x8(x + 0, y + 0), yr + 0.5 * dy, 0, 1, 0, Color.Green.ToArgb)

                                vertices(vi + vi2 + 3) = New CustomVertex.PositionNormalColored(xr + 0.0 * dx, zo + .HeightMap9x9(x + 0, y + 1), yr + 1.0 * dy, 0, 1, 0, Color.Green.ToArgb)
                                vertices(vi + vi2 + 4) = New CustomVertex.PositionNormalColored(xr + 1.0 * dx, zo + .HeightMap9x9(x + 1, y + 1), yr + 1.0 * dy, 0, 1, 0, Color.Green.ToArgb)
                                vertices(vi + vi2 + 5) = New CustomVertex.PositionNormalColored(xr + 0.5 * dx, zo + .HeightMap8x8(x + 0, y + 0), yr + 0.5 * dy, 0, 1, 0, Color.Green.ToArgb)

                                vertices(vi + vi2 + 6) = New CustomVertex.PositionNormalColored(xr + 1.0 * dx, zo + .HeightMap9x9(x + 1, y + 1), yr + 1.0 * dy, 0, 1, 0, Color.Green.ToArgb)
                                vertices(vi + vi2 + 7) = New CustomVertex.PositionNormalColored(xr + 1.0 * dx, zo + .HeightMap9x9(x + 1, y + 0), yr + 0.0 * dy, 0, 1, 0, Color.Green.ToArgb)
                                vertices(vi + vi2 + 8) = New CustomVertex.PositionNormalColored(xr + 0.5 * dx, zo + .HeightMap8x8(x + 0, y + 0), yr + 0.5 * dy, 0, 1, 0, Color.Green.ToArgb)

                                vertices(vi + vi2 + 9) = New CustomVertex.PositionNormalColored(xr + 1.0 * dx, zo + .HeightMap9x9(x + 1, y + 0), yr + 0.0 * dy, 0, 1, 0, Color.Green.ToArgb)
                                vertices(vi + vi2 + 10) = New CustomVertex.PositionNormalColored(xr + 0.0 * dx, zo + .HeightMap9x9(x + 0, y + 0), yr + 0.0 * dy, 0, 1, 0, Color.Green.ToArgb)
                                vertices(vi + vi2 + 11) = New CustomVertex.PositionNormalColored(xr + 0.5 * dx, zo + .HeightMap8x8(x + 0, y + 0), yr + 0.5 * dy, 0, 1, 0, Color.Green.ToArgb)
                            Next
                        Next
                    End With
                Next
            Next

            m_VertexBuffer.Unlock()
        End Sub

    End Class

End Namespace