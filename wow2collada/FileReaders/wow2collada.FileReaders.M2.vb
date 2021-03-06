﻿Imports System.IO

Namespace FileReaders

    Public Class SKIN

        Public Structure sBoneIndices
            Public BoneIndex1 As Byte
            Public BoneIndex2 As Byte
            Public BoneIndex3 As Byte
            Public BoneIndex4 As Byte
        End Structure

        Public Structure sSubMesh
            Public ID As UInt32
            Public StartVertex As UInt16
            Public nVertices As UInt16
            Public StartTriangle As UInt16
            Public nTriangles As UInt16
            Public StartBones As UInt16
            Public nBones As UInt16
            Public RootBone As UInt16
            Public Position As sVector3
        End Structure

        Public Structure sTextureUnit
            Dim IsStatic As Boolean
            Dim RenderOrder As UInt16
            Dim SubmeshIndex1 As UInt16
            Dim SubmeshIndex2 As UInt16
            Dim ColorIndex As UInt16
            Dim RenderFlags As UInt16
            Dim TexUnitNumber As UInt16
            Dim Texture As UInt16
            Dim TexUnitNumber2 As UInt16
            Dim Transparency As UInt16
            Dim TextureAnim As UInt16
        End Structure

        ''' <summary>
        ''' Structure to hold Triangle information (three indices of vertices)
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure sTriangle
            Public VertexIndex() As UInt16
        End Structure

        Public Triangles As sTriangle()
        Public BoneIndices As sBoneIndices()
        Public SubMeshes As sSubMesh()
        Public TextureUnits As sTextureUnit()

        Public Function Load(ByVal FileName As String) As Boolean
            Return Load(File.OpenRead(FileName), FileName)
        End Function

        Public Function Load(ByVal File As Stream, ByVal FileName As String) As Boolean
            Dim br As New BinaryReader(File)

            Dim Magic As String = br.ReadChars(4)
            If Magic <> "SKIN" Then Return False

            Dim nIndices As UInt32 = br.ReadUInt32
            Dim ofsIndices As UInt32 = br.ReadUInt32
            Dim nTriangles As UInt32 = br.ReadUInt32 / 3 ' triangles are always counted as vertices (whyever...)
            Dim ofsTriangles As UInt32 = br.ReadUInt32
            Dim nProperties As UInt32 = br.ReadUInt32
            Dim ofsProperties As UInt32 = br.ReadUInt32
            Dim nSubmeshes As UInt32 = br.ReadUInt32
            Dim ofsSubmeshes As UInt32 = br.ReadUInt32
            Dim nTextureUnits As UInt32 = br.ReadUInt32
            Dim ofsTextureUnits As UInt32 = br.ReadUInt32
            Dim LOD As UInt32 = br.ReadUInt32

            Dim VertexIndices(nIndices - 1) As Integer
            br.BaseStream.Position = ofsIndices
            For i As Integer = 0 To nIndices - 1
                VertexIndices(i) = br.ReadUInt16
            Next

            ReDim Triangles(nTriangles - 1)
            br.BaseStream.Position = ofsTriangles
            For i As Integer = 0 To nTriangles - 1
                Triangles(i).VertexIndex = New UInt16() {VertexIndices(br.ReadUInt16), VertexIndices(br.ReadUInt16), VertexIndices(br.ReadUInt16)}
            Next

            ReDim BoneIndices(nProperties - 1)
            br.BaseStream.Position = ofsProperties
            For i As Integer = 0 To nProperties - 1
                BoneIndices(i).BoneIndex1 = br.ReadByte
                BoneIndices(i).BoneIndex2 = br.ReadByte
                BoneIndices(i).BoneIndex3 = br.ReadByte
                BoneIndices(i).BoneIndex4 = br.ReadByte
            Next

            ReDim SubMeshes(nSubmeshes - 1)
            br.BaseStream.Position = ofsSubmeshes
            For i As Integer = 0 To nSubmeshes - 1
                SubMeshes(i).ID = br.ReadUInt32
                SubMeshes(i).StartVertex = br.ReadUInt16
                SubMeshes(i).nVertices = br.ReadUInt16
                SubMeshes(i).StartTriangle = br.ReadUInt16 / 3 ' triangles are always counted as vertices (whyever...)
                SubMeshes(i).nTriangles = br.ReadUInt16 / 3 ' triangles are always counted as vertices (whyever...)
                SubMeshes(i).nBones = br.ReadUInt16
                SubMeshes(i).StartBones = br.ReadUInt16
                Dim Unknown0 As UInt16 = br.ReadUInt16
                SubMeshes(i).RootBone = br.ReadUInt16
                SubMeshes(i).Position = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                Dim Unknown1 As Single = br.ReadSingle
                Dim Unknown2 As Single = br.ReadSingle
                Dim Unknown3 As Single = br.ReadSingle
                Dim Unknown4 As Single = br.ReadSingle
            Next

            ReDim TextureUnits(nTextureUnits - 1)
            br.BaseStream.Position = ofsTextureUnits
            For i As Integer = 0 To nTextureUnits - 1
                TextureUnits(i).IsStatic = (br.ReadUInt16 = 16)
                TextureUnits(i).RenderOrder = br.ReadUInt16
                TextureUnits(i).SubmeshIndex1 = br.ReadUInt16
                TextureUnits(i).SubmeshIndex2 = br.ReadUInt16
                TextureUnits(i).ColorIndex = br.ReadUInt16
                TextureUnits(i).RenderFlags = br.ReadUInt16
                TextureUnits(i).TexUnitNumber = br.ReadUInt16
                Dim Unknown As UInt16 = br.ReadUInt16
                TextureUnits(i).Texture = br.ReadUInt16
                TextureUnits(i).TexUnitNumber2 = br.ReadUInt16
                TextureUnits(i).Transparency = br.ReadUInt16
                TextureUnits(i).TextureAnim = br.ReadUInt16
            Next
            Return True
        End Function

    End Class

    Public Class ANIM

        Public Function Load()
            Return True
        End Function

    End Class

    Public Class M2

        ''' <summary>
        ''' Structure to hold Triangle information (three indices of vertices)
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure sTriangle
            Public VertexIndex1 As UInt16
            Public VertexIndex2 As UInt16
            Public VertexIndex3 As UInt16
        End Structure

        Public Structure sTexture
            Public Type As UInt32
            Public Flags As UInt16
            Public lenFilename As UInt32
            Public ofsFilename As UInt32
            Public Filename As String
        End Structure

        Public Structure sAnimationSequence
            Public AnimationID As Integer
            Public SubAnimationID As Integer
            Public Length As Integer
            Public MovingSpeed As Single
            Public PlaybackSpeed As Integer
            Public BoundingBox1 As sVector3
            Public BoundingBox2 As sVector3
            Public Radius As Single
            Public NextAnimationID As Integer
            Public Index As Integer
        End Structure

        Public Structure sRenderFlag
            Dim Flags As Integer
            Dim Blending As Integer
        End Structure

        Public ModelName As String
        Public VersionInfo As String
        Public ModelType As UInt32
        Public Vertices As sVertex()
        Public Textures As sTexture()
        Public TextureLookup As UInt16()
        Public AnimationSequences As sAnimationSequence()
        Public AnimationLookup As Integer()
        Public Bones As sBone()
        Public BoneLookup As Integer()
        Public KeyBoneLookup As Integer()
        Public RenderFlags As sRenderFlag()

        Public Function Load(ByVal FileName As String) As Boolean
            Return Load(File.OpenRead(FileName), FileName)
        End Function

        Public Function Load(ByVal File As Stream, ByVal FileName As String) As Boolean
            Dim br As New BinaryReader(File)

            Dim Magic As String = br.ReadChars(4)

            If Magic <> "MD20" Then Return False

            Dim Version() As Byte = br.ReadBytes(4)
            Dim lName As UInt32 = br.ReadUInt32
            Dim ofsName As UInt32 = br.ReadUInt32
            Dim GlobalModelFlags As UInt32 = br.ReadUInt32
            Dim nGlobalSequences As UInt32 = br.ReadUInt32
            Dim ofsGlobalSequences As UInt32 = br.ReadUInt32
            Dim nAnimations As UInt32 = br.ReadUInt32
            Dim ofsAnimations As UInt32 = br.ReadUInt32
            Dim nAnimationLookup As UInt32 = br.ReadUInt32
            Dim ofsAnimationLookup As UInt32 = br.ReadUInt32
            Dim nBones As UInt32 = br.ReadUInt32
            Dim ofsBones As UInt32 = br.ReadUInt32
            Dim nKeyBoneLookup As UInt32 = br.ReadUInt32
            Dim ofsKeyBoneLookup As UInt32 = br.ReadUInt32
            Dim nVertices As UInt32 = br.ReadUInt32
            Dim ofsVertices As UInt32 = br.ReadUInt32
            Dim nViews As UInt32 = br.ReadUInt32
            Dim nColors As UInt32 = br.ReadUInt32
            Dim ofsColors As UInt32 = br.ReadUInt32
            Dim nTextures As UInt32 = br.ReadUInt32
            Dim ofsTextures As UInt32 = br.ReadUInt32
            Dim nTransparency As UInt32 = br.ReadUInt32
            Dim ofsTransparency As UInt32 = br.ReadUInt32
            Dim nTextureanimations As UInt32 = br.ReadUInt32
            Dim ofsTextureanimations As UInt32 = br.ReadUInt32
            Dim nTexReplace As UInt32 = br.ReadUInt32
            Dim ofsTexReplace As UInt32 = br.ReadUInt32
            Dim nRenderFlags As UInt32 = br.ReadUInt32
            Dim ofsRenderFlags As UInt32 = br.ReadUInt32
            Dim nBoneLookupTable As UInt32 = br.ReadUInt32
            Dim ofsBoneLookupTable As UInt32 = br.ReadUInt32
            Dim nTexLookup As UInt32 = br.ReadUInt32
            Dim ofsTexLookup As UInt32 = br.ReadUInt32
            Dim nTexUnits As UInt32 = br.ReadUInt32
            Dim ofsTexUnits As UInt32 = br.ReadUInt32
            Dim nTransLookup As UInt32 = br.ReadUInt32
            Dim ofsTransLookup As UInt32 = br.ReadUInt32
            Dim nTexAnimLookup As UInt32 = br.ReadUInt32
            Dim ofsTexAnimLookup As UInt32 = br.ReadUInt32
            br.BaseStream.Position = br.BaseStream.Position + 14 * 4 'skip 14 floats (unknown content)
            Dim nBoundingTriangles As UInt32 = br.ReadUInt32
            Dim ofsBoundingTriangles As UInt32 = br.ReadUInt32
            Dim nBoundingVertices As UInt32 = br.ReadUInt32
            Dim ofsBoundingVertices As UInt32 = br.ReadUInt32
            Dim nBoundingNormals As UInt32 = br.ReadUInt32
            Dim ofsBoundingNormals As UInt32 = br.ReadUInt32
            Dim nAttachments As UInt32 = br.ReadUInt32
            Dim ofsAttachments As UInt32 = br.ReadUInt32
            Dim nAttachLookup As UInt32 = br.ReadUInt32
            Dim ofsAttachLookup As UInt32 = br.ReadUInt32
            Dim nAttachments_2 As UInt32 = br.ReadUInt32
            Dim ofsAttachments_2 As UInt32 = br.ReadUInt32
            Dim nLights As UInt32 = br.ReadUInt32
            Dim ofsLights As UInt32 = br.ReadUInt32
            Dim nCameras As UInt32 = br.ReadUInt32
            Dim ofsCameras As UInt32 = br.ReadUInt32
            Dim nCameraLookup As UInt32 = br.ReadUInt32
            Dim ofsCameraLookup As UInt32 = br.ReadUInt32
            Dim nRibbonEmitters As UInt32 = br.ReadUInt32
            Dim ofsRibbonEmitters As UInt32 = br.ReadUInt32
            Dim nParticleEmitters As UInt32 = br.ReadUInt32
            Dim ofsParticleEmitters As UInt32 = br.ReadUInt32

            ' lookup values
            br.BaseStream.Position = ofsName
            Me.ModelName = br.ReadChars(lName - 1)

            ' vertices
            ReDim Vertices(nVertices - 1)
            br.BaseStream.Position = ofsVertices
            For i As Integer = 0 To nVertices - 1
                Vertices(i) = New sVertex()
                Vertices(i).Position = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                Vertices(i).BoneWeights = New Byte() {br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte}
                Vertices(i).Boneindices = New Byte() {br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte}
                Vertices(i).Normal = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                Vertices(i).TextureCoords = New sVector2(br.ReadSingle, br.ReadSingle)
                Dim Unknown1 As Single = br.ReadSingle
                Dim Unknown2 As Single = br.ReadSingle
            Next

            'textures
            ReDim Textures(nTextures - 1)
            br.BaseStream.Position = ofsTextures
            For i As Integer = 0 To nTextures - 1
                Textures(i).Type = br.ReadUInt32
                Dim Unknown As UInt16 = br.ReadUInt16
                Textures(i).Flags = br.ReadUInt16
                Textures(i).lenFilename = br.ReadUInt32
                Textures(i).ofsFilename = br.ReadUInt32
            Next i

            'texture lookups
            ReDim TextureLookup(nTexLookup - 1)
            br.BaseStream.Position = ofsTexLookup
            For i As Integer = 0 To nTexLookup - 1
                TextureLookup(i) = br.ReadUInt16
            Next i

            For i As Integer = 0 To nTextures - 1
                Select Case Textures(i).Type
                    Case 0 'texture defined by filename
                        br.BaseStream.Position = Textures(i).ofsFilename
                        Textures(i).Filename = br.ReadChars(Textures(i).lenFilename - 1) 'length includes trailing chr(0)
                        Textures(i).Filename = Textures(i).Filename.ToLower
                    Case 1 'Body and clothes
                        'Get From CharSections.dbc
                        Dim s As Dictionary(Of String, String()) = myDBC.GetTexturesByRaceGender(myDBC.M2PathToRaceID(FileName), myDBC.M2PathToGender(FileName))
                        Textures(i).Filename = s("skin")(0)
                    Case 2 'Cape
                    Case 6 'Hair and Beard
                        'Get From CharSections.dbc
                        Dim s As Dictionary(Of String, String()) = myDBC.GetTexturesByRaceGender(myDBC.M2PathToRaceID(FileName), myDBC.M2PathToGender(FileName))
                        Textures(i).Filename = s("hair")(0)
                    Case 8 'Tauren Fur
                    Case 11, 12, 13 'Skin for creatures
                        Dim ID As UInt32 = myDBC.GetIDFromModelFileName(FileName)
                        Dim Tex As String = myDBC.GetTextureFromCreatureModelID(ID, Textures(i).Type)
                        Dim s As String = (FileName.Substring(0, FileName.LastIndexOf("\") + 1) & Tex & ".blp").ToLower
                        Textures(i).Filename = s
                    Case Else
                        Debug.Print(Textures(i).Type)
                End Select
            Next

            'animation sequences
            ReDim AnimationSequences(nAnimations - 1)
            br.BaseStream.Position = ofsAnimations
            For i As Integer = 0 To nAnimations - 1
                With AnimationSequences(i)
                    .AnimationID = br.ReadUInt16
                    .SubAnimationID = br.ReadUInt16
                    .Length = br.ReadUInt32
                    .MovingSpeed = br.ReadSingle
                    br.BaseStream.Position = br.BaseStream.Position + 4 * 4 'skip for uint32 (unknown)
                    .PlaybackSpeed = br.ReadUInt32
                    .BoundingBox1 = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                    .BoundingBox2 = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                    .Radius = br.ReadSingle
                    .NextAnimationID = br.ReadUInt16
                    .Index = br.ReadUInt16
                End With
            Next i

            'animation lookup
            If nAnimationLookup > 0 Then
                ReDim AnimationLookup(nAnimationLookup - 1)
                br.BaseStream.Position = ofsAnimationLookup
                For i As Integer = 0 To nAnimationLookup - 1
                    AnimationLookup(i) = br.ReadUInt16
                Next
            End If

            'bones
            ReDim Bones(nBones - 1)
            br.BaseStream.Position = ofsBones
            For i As Integer = 0 To nBones - 1
                Dim Bone As New sBone
                With Bone
                    .AnimationSequence = br.ReadInt32
                    .Flags = br.ReadInt32
                    .ParentBone = br.ReadInt16
                    br.BaseStream.Position = br.BaseStream.Position + 2
                    .GeosetID = br.ReadInt16
                    br.BaseStream.Position = br.BaseStream.Position + 2
                    .Translation = New sAnimBlock(br.ReadInt16, br.ReadInt16, br.ReadInt32, br.ReadInt32, br.ReadInt32, br.ReadInt32)
                    '.Translation = New sAnimBlock(br.ReadUInt16, br.ReadUInt16, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32)
                    .Rotation = New sAnimBlock(br.ReadInt16, br.ReadInt16, br.ReadInt32, br.ReadInt32, br.ReadInt32, br.ReadInt32)
                    .Scaling = New sAnimBlock(br.ReadInt16, br.ReadInt16, br.ReadInt32, br.ReadInt32, br.ReadInt32, br.ReadInt32)
                    '.Translation = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                    '.Rotation = New sQuaternion(myHF.ShortToSingle(br.ReadUInt16), myHF.ShortToSingle(br.ReadUInt16), myHF.ShortToSingle(br.ReadUInt16), myHF.ShortToSingle(br.ReadUInt16))
                    '.Scaling = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                    .PivotPoint = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                End With
                Bones(i) = Bone
            Next i

            'bone lookup table
            ReDim BoneLookup(nBoneLookupTable - 1)
            br.BaseStream.Position = ofsBoneLookupTable
            For i As Integer = 0 To nBoneLookupTable - 1
                BoneLookup(i) = br.ReadUInt16
            Next

            'key bone lookup table
            ReDim KeyBoneLookup(nKeyBoneLookup - 1)
            br.BaseStream.Position = ofsKeyBoneLookup
            For i As Integer = 0 To nKeyBoneLookup - 1
                KeyBoneLookup(i) = br.ReadUInt16
            Next

            'render flags table
            ReDim RenderFlags(nRenderFlags - 1)
            br.BaseStream.Position = ofsRenderFlags
            For i As Integer = 0 To nRenderFlags - 1
                RenderFlags(i).Flags = br.ReadUInt16
                RenderFlags(i).Blending = br.ReadUInt16
            Next

            ' calculated values
            Me.VersionInfo = String.Format("{0}.{1}.{2}.{3}", Version(0), Version(1), Version(2), Version(3))

            Return True
        End Function

    End Class

End Namespace

