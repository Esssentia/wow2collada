
''' <summary>
''' Class to manage all models and their positioning data
''' </summary>
''' <remarks></remarks>
Public Class ModelManager

    Public Class sModelData
        Public Name As String
        Public Meshes As New List(Of sSubMesh)
        Public Bones As New List(Of sBone)
        Public Vertices As New List(Of sVertex)
        Public OpenGLVBOTexCoordsID As Integer

    End Class

    Public Class sModel
        Public Name As String
        Public ModelDataID As String
        Public ScaleRotTrans As sMatrix4
        Public OpenGLBoneMeshID As Integer
        Public OpenGLVBOVerticesID As Integer
        Public OpenGLVBONormalsID As Integer
    End Class

    Public Models As New Dictionary(Of Integer, sModel)
    Public ModelData As New Dictionary(Of Integer, sModelData)

    Public ModelsTopID As Integer = -1
    Public ModelDataTopID As Integer = -1

    Public ReadOnly Property Meshes(ByVal ModelID As Integer) As List(Of sSubMesh)
        Get
            Return ModelData(Models(ModelID).ModelDataID).Meshes
        End Get
    End Property

    Public ReadOnly Property Bones(ByVal ModelID As Integer) As List(Of sBone)
        Get
            Return ModelData(Models(ModelID).ModelDataID).Bones
        End Get
    End Property

    Public ReadOnly Property Vertices(ByVal ModelID As Integer) As List(Of sVertex)
        Get
            Return ModelData(Models(ModelID).ModelDataID).Vertices
        End Get
    End Property

    Public ReadOnly Property VerticesTransformed(ByVal ModelID As Integer) As List(Of sVertex)
        Get
            Dim VT As New List(Of sVertex)
            For Each VO As sVertex In ModelData(Models(ModelID).ModelDataID).Vertices
                'scale -> rot -> trans
                'new.Position = sVector3.Rotate(_Vertices(.VertexIndex(m)).Position * Scale, Orientation) + Position
                'new.Normal = sVector3.Rotate(_Vertices(.VertexIndex(m)).Normal, Orientation)
                VT.Add(New sVertex(Models(ModelID).ScaleRotTrans * VO.Position, Models(ModelID).ScaleRotTrans * VO.Normal, VO.TextureCoords, VO.BoneWeights, VO.Boneindices))
            Next
            Return VT
        End Get

    End Property

    Public ReadOnly Property VBOVertices(ByVal ModelID As Integer) As Single()
        Get
            Dim VT(ModelData(Models(ModelID).ModelDataID).Vertices.Count * 3 - 1) As Single
            For i As Integer = 0 To ModelData(Models(ModelID).ModelDataID).Vertices.Count - 1
                Dim pos As sVector3 = Models(ModelID).ScaleRotTrans * ModelData(Models(ModelID).ModelDataID).Vertices(i).Position
                VT(3 * i + 0) = pos.X
                VT(3 * i + 1) = pos.Y
                VT(3 * i + 2) = pos.Z
            Next
            Return VT
        End Get
    End Property

    Public ReadOnly Property VBONormals(ByVal ModelID As Integer) As Single()
        Get
            Dim VT(ModelData(Models(ModelID).ModelDataID).Vertices.Count * 3 - 1) As Single
            For i As Integer = 0 To ModelData(Models(ModelID).ModelDataID).Vertices.Count - 1
                Dim pos As sVector3 = Models(ModelID).ScaleRotTrans * ModelData(Models(ModelID).ModelDataID).Vertices(i).Normal
                VT(3 * i + 0) = pos.X
                VT(3 * i + 1) = pos.Y
                VT(3 * i + 2) = pos.Z
            Next
            Return VT
        End Get
    End Property

    Public ReadOnly Property VBOTexCoords(ByVal ModelID As Integer) As Single()
        Get
            Dim VT(ModelData(Models(ModelID).ModelDataID).Vertices.Count * 2 - 1) As Single
            For i As Integer = 0 To ModelData(Models(ModelID).ModelDataID).Vertices.Count - 1
                VT(2 * i + 0) = ModelData(Models(ModelID).ModelDataID).Vertices(i).TextureCoords.U
                VT(2 * i + 1) = ModelData(Models(ModelID).ModelDataID).Vertices(i).TextureCoords.V
            Next
            Return VT
        End Get
    End Property

    Public Property OpenGLVBOVerticesID(ByVal ModelID As Integer) As Integer
        Get
            Return Models(ModelID).OpenGLVBOVerticesID
        End Get
        Set(ByVal value As Integer)
            Models(ModelID).OpenGLVBOVerticesID = value
        End Set
    End Property

    Public Property OpenGLVBONormalsID(ByVal ModelID As Integer) As Integer
        Get
            Return Models(ModelID).OpenGLVBONormalsID
        End Get
        Set(ByVal value As Integer)
            Models(ModelID).OpenGLVBONormalsID = value
        End Set
    End Property

    Public Property OpenGLVBOTexCoordsID(ByVal ModelID As Integer) As Integer
        Get
            Return ModelData(Models(ModelID).ModelDataID).OpenGLVBOTexCoordsID
        End Get
        Set(ByVal value As Integer)
            ModelData(Models(ModelID).ModelDataID).OpenGLVBOTexCoordsID = value
        End Set
    End Property

    Public ReadOnly Property Name(ByVal ModelID As Integer) As String
        Get
            Return Models(ModelID).Name
        End Get
    End Property

    Public Property ScaleRotTrans(ByVal ModelID As Integer) As sMatrix4
        Get
            Return Models(ModelID).ScaleRotTrans
        End Get
        Set(ByVal value As sMatrix4)
            Models(ModelID).ScaleRotTrans = value
        End Set
    End Property

    Public Property OpenGLBoneMeshID(ByVal ModelID As Integer) As Integer
        Get
            Return Models(ModelID).OpenGLBoneMeshID
        End Get
        Set(ByVal value As Integer)
            Models(ModelID).OpenGLBoneMeshID = value
        End Set
    End Property

    Public Sub AddModelFromM2(ByVal Name As String, ByVal MD20 As FileReaders.M2, ByVal SKIN As FileReaders.SKIN, ByVal ANIM As FileReaders.ANIM, ByVal ScaleRotTrans As sMatrix4)
        Dim ID As Integer = AddModel(Name, ScaleRotTrans)
        If Models(ID).ModelDataID = -1 Then Models(ID).ModelDataID = ModelDataFromM2(Name, MD20, SKIN, ANIM)
    End Sub

    Public Sub AddModelFromM2(ByVal Name As String, ByVal MD20 As FileReaders.M2, ByVal SKIN As FileReaders.SKIN, ByVal ANIM As FileReaders.ANIM, ByVal Position As sVector3, ByVal Orientation As sQuaternion, ByVal Scale As Single)
        Dim ID As Integer = AddModel(Name, New sMatrix4(Orientation, Position, Scale))
        If Models(ID).ModelDataID = -1 Then Models(ID).ModelDataID = ModelDataFromM2(Name, MD20, SKIN, ANIM)
    End Sub

    Public Sub AddModelFromM2(ByVal Name As String, ByVal MD20 As FileReaders.M2, ByVal SKIN As FileReaders.SKIN, ByVal ANIM As FileReaders.ANIM)
        Dim ID As Integer = AddModel(Name, New sMatrix4())
        If Models(ID).ModelDataID = -1 Then Models(ID).ModelDataID = ModelDataFromM2(Name, MD20, SKIN, ANIM)
    End Sub

    Public Sub AddModelFromM2NoData(ByVal Name As String, ByVal ScaleRotTrans As sMatrix4)
        Dim ID As Integer = AddModel(Name, ScaleRotTrans)
        If Models(ID).ModelDataID = -1 Then Models(ID).ModelDataID = GetModelDataIDFromName(Name)
    End Sub

    Public Sub AddModelFromWMO(ByVal Name As String, ByVal WMO As FileReaders.WMO, ByVal ScaleRotTrans As sMatrix4)
        Dim ID As Integer = AddModel(Name, ScaleRotTrans)
        If Models(ID).ModelDataID = -1 Then Models(ID).ModelDataID = ModelDataFromWMO(Name, WMO)
    End Sub

    Public Sub AddModelFromWMO(ByVal Name As String, ByVal WMO As FileReaders.WMO, ByVal Position As sVector3, ByVal Orientation As sQuaternion)
        Dim ID As Integer = AddModel(Name, New sMatrix4(Orientation, Position, 1))
        If Models(ID).ModelDataID = -1 Then Models(ID).ModelDataID = ModelDataFromWMO(Name, WMO)
    End Sub

    Public Sub AddModelFromWMO(ByVal Name As String, ByVal WMO As FileReaders.WMO)
        Dim ID As Integer = AddModel(Name, New sMatrix4())
        If Models(ID).ModelDataID = -1 Then Models(ID).ModelDataID = ModelDataFromWMO(Name, WMO)
    End Sub

    Public Function ModelDataFromM2(ByVal Name As String, ByVal MD20 As FileReaders.M2, ByVal SKIN As FileReaders.SKIN, ByVal ANIM As FileReaders.ANIM) As Integer
        Dim ID As Integer = GetModelDataIDFromName(Name)
        If ID = -1 Then
            ModelDataTopID += 1
            ID = ModelDataTopID

            Dim MD As New sModelData()
            MD.Name = Name

            If Not MD20.Bones Is Nothing Then MD.Bones = MD20.Bones.ToList
            If Not MD20.Vertices Is Nothing Then MD.Vertices = MD20.Vertices.ToList

            If Not SKIN.SubMeshes Is Nothing Then

                For i As Integer = 0 To SKIN.SubMeshes.Length - 1

                    Dim mesh As New sSubMesh()

                    For j As Integer = 0 To SKIN.TextureUnits.Length - 1
                        If SKIN.TextureUnits(j).SubmeshIndex1 = i Then
                            Dim TexID As String = MD20.TextureLookup(SKIN.TextureUnits(j).Texture)
                            Dim TexFi As String = MD20.Textures(TexID).Filename

                            If TextureMgr.AddTexture(TexFi, TexFi) <> "" Then
                                mesh.TextureList.Add(New sTextureEntry(TexFi, "", MD20.RenderFlags(SKIN.TextureUnits(j).RenderFlags).Flags, 0, MD20.RenderFlags(SKIN.TextureUnits(j).RenderFlags).Blending, 0))
                            End If
                        End If
                    Next

                    For j As Integer = 0 To SKIN.SubMeshes(i).nTriangles - 1
                        Dim k As Integer = SKIN.SubMeshes(i).StartTriangle + j
                        With SKIN.Triangles(k)
                            mesh.TriangleList.Add(New sTriangle(.VertexIndex(0), .VertexIndex(1), .VertexIndex(2)))
                        End With
                    Next

                    MD.Meshes.Add(mesh)

                Next
            End If

            ModelData.Add(ID, MD)
        End If

        Return ID
    End Function

    Public Function ModelDataFromWMO(ByVal Name As String, ByVal WMO As FileReaders.WMO)
        Dim ID As Integer = GetModelDataIDFromName(Name)
        If ID = -1 Then
            ModelDataTopID += 1
            ID = ModelDataTopID

            Dim MD As New sModelData()

            'load textures
            For i As Integer = 0 To WMO.Textures.Length - 1
                Dim TexFi As String = WMO.Textures(i).TexID
                TextureMgr.AddTexture(TexFi, TexFi)
            Next

            'load subsets

            For i As Integer = 0 To WMO.SubSets.Count - 1
                Dim CurrMatID As Integer = -1
                Dim submesh As New sSubMesh
                Dim vi As Integer = MD.Vertices.Count
                MD.Vertices.AddRange(WMO.SubSets(i).Vertices)

                For j As Integer = 0 To WMO.SubSets(i).Triangles.Length - 1
                    Dim MatID As Byte = WMO.SubSets(i).Materials(j)
                    If MatID < WMO.Textures.Length Then

                        If MatID <> CurrMatID Then
                            If CurrMatID <> -1 Then MD.Meshes.Add(submesh)
                            CurrMatID = MatID
                            submesh = New sSubMesh
                            submesh.TextureList.Add(New sTextureEntry(WMO.Textures(MatID).TexID, "", 0, WMO.Textures(MatID).Flags, 0, WMO.Textures(MatID).Blending))

                        End If

                        With WMO.SubSets(i).Triangles(j)
                            submesh.TriangleList.Add(New sTriangle(vi + .V1, vi + .V2, vi + .V3))
                        End With
                    End If
                Next

                MD.Meshes.Add(submesh)
            Next
            ModelData.Add(ID, MD)
        End If
        Return ID
    End Function

    Public Function AddVerticesToModelData(ByVal ModelDataID As Integer, ByVal Vertices As sVertex()) As Integer
        Dim ID As Integer = ModelData(ModelDataID).Vertices.Count
        ModelData(ModelDataID).Vertices.AddRange(Vertices)
        Return ID
    End Function

    Public Function AddModel(ByVal Name As String, ByVal ScaleRotTrans As sMatrix4) As Integer
        ModelsTopID += 1

        Dim model As New sModel
        model.ScaleRotTrans = ScaleRotTrans
        model.Name = Name
        model.ModelDataID = GetModelDataIDFromName(Name)

        Models.Add(ModelsTopID, model)
        Return ModelsTopID
    End Function

    Public Function AddModel(ByVal Name As String) As Integer
        Return AddModel(Name, New sMatrix4)
    End Function

    Public Function AddModelData(ByVal Name As String) As Integer
        ModelDataTopID += 1

        Dim ModelDat As New sModelData

        ModelDat.Name = Name

        ModelData.Add(ModelDataTopID, ModelDat)
        Return ModelDataTopID
    End Function

    Public Function GetModelIDFromName(ByVal Name As String) As Integer
        Dim Out As Integer = -1
        For Each item In Models
            If item.Value.Name = Name Then Out = item.Key
        Next
        Return Out
    End Function

    Public Function GetModelDataIDFromName(ByVal Name As String) As Integer
        Dim Out As Integer = -1
        For Each item In ModelData
            If item.Value.Name = Name Then Out = item.Key
        Next
        Return Out
    End Function

    Public Sub Clear()
        Models = New Dictionary(Of Integer, sModel)
        ModelData = New Dictionary(Of Integer, sModelData)
        ModelsTopID = -1
        ModelDataTopID = -1
    End Sub

End Class
