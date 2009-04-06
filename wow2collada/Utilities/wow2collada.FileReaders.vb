Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Namespace wow2collada.FileReaders

    Class SKIN
        Public Triangles As Triangle()
        Public BoneIndices As BoneIndices()
        Public SubMeshes As SubMesh()
        Public TextureUnits As TextureUnit()

        Public Function Load(ByVal FileName As String)
            Dim br As New BinaryReader(File.OpenRead(FileName))

            'WotLK only...

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
                Triangles(i).VertexIndex1 = VertexIndices(br.ReadUInt16)
                Triangles(i).VertexIndex2 = VertexIndices(br.ReadUInt16)
                Triangles(i).VertexIndex3 = VertexIndices(br.ReadUInt16)
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
                SubMeshes(i).StartTriangle = br.ReadUInt16
                SubMeshes(i).nTriangles = br.ReadUInt16 / 3 ' triangles are always counted as vertices (whyever...)
                SubMeshes(i).nBones = br.ReadUInt16
                SubMeshes(i).StartBones = br.ReadUInt16
                Dim sm_Unknown As UInt16 = br.ReadUInt16
                SubMeshes(i).RootBone = br.ReadUInt16
                SubMeshes(i).Position.X = br.ReadSingle
                SubMeshes(i).Position.Y = br.ReadSingle
                SubMeshes(i).Position.Z = br.ReadSingle
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

    Class M2

        Public ModelName As String
        Public VersionInfo As String
        Public ModelType As UInt32
        Public Vertices() As Vertex
        Public Textures() As M2Texture

        Public Function Load(ByVal FileName As String)
            Dim hf As New HelperFunctions
            Dim br As New BinaryReader(File.OpenRead(FileName))

            'WotLK only...

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
            Dim Unknown00 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown01 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown02 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown03 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown04 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown05 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown06 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown07 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown08 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown09 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown10 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown11 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown12 As Single = br.ReadSingle 'skip 14 floats (unknown content)
            Dim Unknown13 As Single = br.ReadSingle 'skip 14 floats (unknown content)
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

            ReDim Vertices(nVertices - 1)
            br.BaseStream.Position = ofsVertices
            For i As Integer = 0 To nVertices - 1
                Vertices(i).Position = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                Vertices(i).BoneWeights = New Byte() {br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte}
                Vertices(i).BoneIndices = New Byte() {br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte}
                Vertices(i).Normal = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                Vertices(i).TextureCoords = New Vector2(br.ReadSingle, br.ReadSingle)
                Dim Unknown1 As Single = br.ReadSingle
                Dim Unknown2 As Single = br.ReadSingle
            Next

            ReDim Textures(nTextures - 1)
            br.BaseStream.Position = ofsTextures
            For i As Integer = 0 To nTextures - 1
                Textures(i).Type = br.ReadUInt32
                Dim Unknown As UInt16 = br.ReadUInt16
                Textures(i).Flags = br.ReadUInt16
                Textures(i).lenFilename = br.ReadUInt32
                Textures(i).ofsFilename = br.ReadUInt32
            Next i

            For i As Integer = 0 To nTextures - 1
                If Textures(i).Type = 0 Then
                    br.BaseStream.Position = Textures(i).ofsFilename
                    Textures(i).Filename = br.ReadChars(Textures(i).lenFilename - 1) 'length includes trailing chr(0)
                End If
            Next

            ' calculated values
            Me.VersionInfo = String.Format("{0}.{1}.{2}.{3}", Version(0), Version(1), Version(2), Version(3))

            Return True
        End Function


    End Class

    Class ADT

        Public TextureFiles() As String
        Public ModelFiles() As String
        Public WMOFiles() As String
        Public M2Placements() As M2Placement
        Public WMOPlacements() As WMOPlacement
        Public MCNKs() As MCNK

        Public Function Load(ByVal FileName As String)
            Dim br As New BinaryReader(File.OpenRead(FileName))
            Dim hf As New HelperFunctions

            Dim ChunkId As String
            Dim ChunkLen As UInt32
            Dim FilePosition As UInt32 = 0
            Dim Version As UInt32

            While br.BaseStream.Position < br.BaseStream.Length
                ChunkId = br.ReadChars(4)
                ChunkId = hf.StrRev(ChunkId)
                ChunkLen = br.ReadUInt32

                Select Case ChunkId
                    Case "MHDR" 'Ignore the header chunk for now (redundant)
                    Case "MVER"
                        Version = br.ReadUInt32
                    Case "MCIN" 'No need for now
                    Case "MTEX" 'Texture files
                        TextureFiles = hf.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

                    Case "MMDX" 'Model files
                        ModelFiles = hf.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

                    Case "MMID" 'MMDX indices (ignore)
                    Case "MWMO" 'WMO files
                        WMOFiles = hf.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

                    Case "MWID" 'MWMO indices (ignore)
                    Case "MDDF" 'M2 Placements
                        ReDim M2Placements(ChunkLen / 36 - 1)
                        For i As Integer = 0 To ChunkLen / 36 - 1
                            M2Placements(i).MMDX_ID = br.ReadUInt32
                            M2Placements(i).ID = br.ReadUInt32
                            M2Placements(i).Position = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            M2Placements(i).Orientation = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            M2Placements(i).Scale = br.ReadUInt32 / 1024
                        Next

                    Case "MODF" 'WMO placements
                        ReDim WMOPlacements(ChunkLen / 64 - 1)
                        For i As Integer = 0 To ChunkLen / 64 - 1
                            WMOPlacements(i).MWMO_ID = br.ReadUInt32
                            WMOPlacements(i).ID = br.ReadUInt32
                            WMOPlacements(i).Position = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            WMOPlacements(i).Orientation = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            WMOPlacements(i).UpperExtents = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            WMOPlacements(i).LowerExtents = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            Dim Unknown As UInt16 = br.ReadUInt16
                            WMOPlacements(i).DoodadSetIndex = br.ReadUInt16
                            WMOPlacements(i).NameSetIndex = br.ReadUInt32
                        Next

                    Case "MCNK"
                        If MCNKs Is Nothing Then
                            ReDim MCNKs(0)
                        Else
                            ReDim Preserve MCNKs(MCNKs.Length)
                        End If

                        With MCNKs(MCNKs.Length - 1)
                            .flags = br.ReadUInt32
                            .IndexX = 32 * 533.3333 - br.ReadUInt32 'normalize to global coordinate grid (WMOs, M2s, ...)
                            .IndexY = 32 * 533.3333 - br.ReadUInt32 'normalize to global coordinate grid (WMOs, M2s, ...)
                            .nLayers = br.ReadUInt32
                            .nDoodadRefs = br.ReadUInt32
                            .offsHeight = br.ReadUInt32
                            .offsNormal = br.ReadUInt32
                            .offsLayer = br.ReadUInt32
                            .offsRefs = br.ReadUInt32
                            .offsAlpha = br.ReadUInt32
                            .sizeAlpha = br.ReadUInt32
                            .offsShadow = br.ReadUInt32
                            .sizeShadow = br.ReadUInt32
                            .areaid = br.ReadUInt32
                            .nMapObjRefs = br.ReadUInt32
                            .holes = br.ReadUInt32
                            Dim Unbekannt1 As UInt32 = br.ReadUInt32
                            Dim Unbekannt2 As UInt32 = br.ReadUInt32
                            Dim Unbekannt3 As UInt32 = br.ReadUInt32
                            Dim Unbekannt4 As UInt32 = br.ReadUInt32
                            .predTex = br.ReadUInt32
                            .noEffectDoodad = br.ReadUInt32
                            .offsSndEmitters = br.ReadUInt32
                            .nSndEmitters = br.ReadUInt32
                            .offsLiquid = br.ReadUInt32
                            .sizeLiquid = br.ReadUInt32
                            .Position = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            .offsColorValues = br.ReadUInt32
                            .props = br.ReadUInt32
                            .effectId = br.ReadUInt32

                            br.BaseStream.Position = FilePosition + .offsHeight
                            ReDim .HeightMap(144)

                            For i As Integer = 0 To 144
                                .HeightMap(i) = br.ReadSingle
                            Next
                            '.HeightMap8x8 = hf.GetHeightMap8x8FromHeightMap(.HeightMap)
                            '.HeightMap9x9 = hf.GetHeightMap9x9FromHeightMap(.HeightMap)
                        End With

                    Case "MH2O" 'Water and such...
                        ' do it :)

                    Case Else
                        MsgBox("Unknown Chunktype: " & ChunkId)
                End Select

                FilePosition += ChunkLen + 8
                'If (FilePosition Mod 4 <> 0) Then MsgBox("Argh...")
                br.BaseStream.Position = FilePosition
            End While

            Return True
        End Function
    End Class

    Class WMOROOT

        Public nMaterials As UInt32
        Public nGroups As UInt32
        Public nPortals As UInt32
        Public nLights As UInt32
        Public nModels As UInt32
        Public nDoodads As UInt32
        Public nSets As UInt32
        Public ambient_color As Color
        Public WMO_ID As UInt32
        Public BoundingBoxA As Vector3
        Public BoundingBoxB As Vector3
        Public Textures() As String
        Public DoodadSets() As DoodadSet
        Public Doodads() As Doodad

        Public Function Load(ByVal FileName As String)
            Dim br As New BinaryReader(File.OpenRead(FileName))
            Dim hf As New HelperFunctions

            Dim ChunkId As String
            Dim ChunkLen As UInt32
            Dim Version As UInt32
            Dim FilePosition As UInt32 = 0
            Dim MODN() As Byte

            While br.BaseStream.Position < br.BaseStream.Length
                ChunkId = br.ReadChars(4)
                ChunkId = hf.StrRev(ChunkId)
                ChunkLen = br.ReadUInt32

                Select Case ChunkId
                    Case "MVER"
                        Version = br.ReadUInt32
                    Case "MOHD"
                        nMaterials = br.ReadUInt32
                        nGroups = br.ReadUInt32
                        nPortals = br.ReadUInt32
                        nLights = br.ReadUInt32
                        nModels = br.ReadUInt32
                        nDoodads = br.ReadUInt32
                        nSets = br.ReadUInt32
                        ambient_color = Color.FromArgb(red:=br.ReadByte, green:=br.ReadByte, blue:=br.ReadByte, alpha:=br.ReadByte)
                        WMO_ID = br.ReadUInt32
                        BoundingBoxA = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                        BoundingBoxB = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                        Dim Unknown As UInt32 = br.ReadUInt32

                        'ReDim Materials(nMaterials)
                        ReDim DoodadSets(nSets)
                        ReDim Doodads(nDoodads)

                    Case "MOTX" ' Texture Files (List of zero-terminated strings)
                        Textures = hf.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))
                    Case "MOMT"
                        '??
                    Case "MOGN" 'We don't care about Group Names for now...
                    Case "MOGI" 'Wo don't care about Group Information for now...
                    Case "MOSB" 'We don't care about the sky box for now... It's in the *.dbc files anyway...
                    Case "MOPV" 'We don't care about portals...
                    Case "MOPT" 'We don't care about portals...
                    Case "MOPR" 'We don't care about portals...
                    Case "MOVV" 'We don't care abou the visible block vertices...
                    Case "MOVB" 'We don't care about the visible blocks...
                    Case "MOLT" 'We don't care about lights for now...
                    Case "MODS" 'Doodad Sets (32 Bytes each)
                        For i As Integer = 0 To nSets - 1
                            DoodadSets(i).name = br.ReadString(20)
                            DoodadSets(i).index = br.ReadUInt32
                            DoodadSets(i).count = br.ReadUInt32
                            Dim Unknown As UInt32 = br.ReadUInt32
                        Next

                    Case "MODN" 'Doodad Model Files (List of zero-terminated strings)
                        MODN = br.ReadBytes(ChunkLen)

                    Case "MODD" 'Doodads (40 Bytes each)
                        MsgBox(br.BaseStream.Position & " " & br.BaseStream.Length)
                        For i As Integer = 0 To (ChunkLen / 40) - 1 ' nDoodads contains ALL doodads, even the ones in subfiles... we'll get them later
                            Dim idx As UInt32 = br.ReadUInt32
                            Debug.Print(i & " " & idx & " " & br.BaseStream.Position)
                            Doodads(i).ModelFile = hf.GetZeroDelimitedString(MODN, idx)
                            Debug.Print(Doodads(i).ModelFile)

                            Doodads(i).Position = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            Doodads(i).Orientation = New Quaternion(br.ReadSingle, br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            Doodads(i).Scale = br.ReadSingle
                            Doodads(i).LightingColor = Color.FromArgb(blue:=br.ReadByte, green:=br.ReadByte, red:=br.ReadByte, alpha:=br.ReadByte)
                        Next

                    Case "MFOG" 'We don't care about fog for now...
                    Case Else
                        MsgBox("Unknown Chunktype: " & ChunkId)
                End Select

                FilePosition += ChunkLen + 8
                If (FilePosition Mod 4 <> 0) Then
                    MsgBox("Argh...")
                End If
                br.BaseStream.Position = FilePosition
            End While

            Return True
        End Function

    End Class

End Namespace

