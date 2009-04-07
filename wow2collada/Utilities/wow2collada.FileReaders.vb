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
                SubMeshes(i).StartTriangle = br.ReadUInt16 / 3 ' triangles are always counted as vertices (whyever...)
                SubMeshes(i).nTriangles = br.ReadUInt16 / 3 ' triangles are always counted as vertices (whyever...)
                SubMeshes(i).nBones = br.ReadUInt16
                SubMeshes(i).StartBones = br.ReadUInt16
                Dim Unknown0 As UInt16 = br.ReadUInt16
                SubMeshes(i).RootBone = br.ReadUInt16
                SubMeshes(i).Position.X = br.ReadSingle
                SubMeshes(i).Position.Y = br.ReadSingle
                SubMeshes(i).Position.Z = br.ReadSingle
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

    Class M2

        Public ModelName As String
        Public VersionInfo As String
        Public ModelType As UInt32
        Public Vertices() As Vertex
        Public Textures() As M2Texture

        Public Function Load(ByVal FileName As String)
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
                Select Case Textures(i).Type
                    Case 0
                        br.BaseStream.Position = Textures(i).ofsFilename
                        Textures(i).Filename = br.ReadChars(Textures(i).lenFilename - 1) 'length includes trailing chr(0)
                    Case 11, 12, 13
                        Dim bp As String = "d:\temp\mpq\"
                        Dim ID As UInt32 = myDBC.GetIDFromModelFileName(FileName)
                        Dim Tex As String = myDBC.GetTextureFromCreatureModelID(ID, Textures(i).Type)
                        Dim s As String = (FileName.Substring(0, FileName.LastIndexOf("\") + 1) & Tex & ".blp").ToLower
                        If (s.IndexOf(bp) = 0) Then s = s.Substring(bp.Length)
                        Textures(i).Filename = s
                End Select
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
        Public MCNKs(,) As MCNK

        Public Function Load(ByVal FileName As String)
            Dim br As New BinaryReader(File.OpenRead(FileName))

            Dim ChunkId As String
            Dim ChunkLen As UInt32
            Dim FilePosition As UInt32 = 0
            Dim Version As UInt32
            ReDim MCNKs(15, 15)

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
                        Dim Flags As UInt32 = br.ReadUInt32
                        Dim IndexX As UInt32 = br.ReadUInt32
                        Dim IndexY As UInt32 = br.ReadUInt32
                        With MCNKs(IndexX, IndexY)
                            .flags = Flags
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
                            .Position = New Vector3(valueZ:=br.ReadSingle, valueX:=br.ReadSingle, valueY:=br.ReadSingle)
                            .offsColorValues = br.ReadUInt32
                            .props = br.ReadUInt32
                            .effectId = br.ReadUInt32

                            br.BaseStream.Position = FilePosition + .offsHeight
                            Dim SubChunkId As String = br.ReadChars(4)
                            SubChunkId = hf.StrRev(SubChunkId)
                            Dim SubChunkLen As UInt32 = br.ReadUInt32
                            If SubChunkId <> "MCVT" Then Debug.Print("Argh...: Expected MCVT sub-chunk, got: " & SubChunkId)
                            If SubChunkLen <> 145 * 4 Then Debug.Print("Argh...: Expected MCVT sub-chunk of length " & (145 * 4) & ", got: " & SubChunkLen)
                            ReDim .HeightMap(144)

                            For i As Integer = 0 To 144
                                .HeightMap(i) = br.ReadSingle
                            Next
                            .HeightMap8x8 = hf.GetHeightMap8x8FromHeightMap(.HeightMap)
                            .HeightMap9x9 = hf.GetHeightMap9x9FromHeightMap(.HeightMap)
                        End With

                    Case "MH2O" 'Water and such...
                        ' do it :)

                    Case Else
                        Debug.Print("Unknown Chunktype: " & ChunkId)
                End Select

                FilePosition += ChunkLen + 8
                If (FilePosition Mod 4 <> 0) Then Debug.Print("Argh...:" & ChunkId & "-" & ChunkLen)
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

    Class DBC
        Public Structure CreatureModelDataStructure
            Dim ModelID As UInt32
            Dim CreatureType As UInt32
            Dim ModelFilename As String
            Dim StringOffset As UInt32
        End Structure

        Public Structure CreatureDisplayInfoStructure
            Dim CreatureID As UInt32
            Dim CreatureModelID As UInt32
            Dim Scale As Single
            Dim StringOffset11 As UInt32
            Dim StringOffset12 As UInt32
            Dim StringOffset13 As UInt32
            Dim Texture11 As String
            Dim Texture12 As String
            Dim Texture13 As String
        End Structure

        Public CreatureModelData As CreatureModelDataStructure()
        Public CreatureDisplayInfo As CreatureDisplayInfoStructure()

        Public Function LoadCreatureModelData(ByVal FileName As String)
            Dim br As New BinaryReader(File.OpenRead(FileName))

            If br.ReadChars(4) <> "WDBC" Then Return False

            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

            'Debug.Print(nRecords)
            'Debug.Print(nFields)
            'Debug.Print(recordSize)
            'Debug.Print(stringSize)

            ReDim CreatureModelData(nRecords - 1)

            For i As Integer = 0 To nRecords - 1
                br.BaseStream.Position = 20 + i * recordSize
                CreatureModelData(i).ModelID = br.ReadUInt32
                CreatureModelData(i).CreatureType = br.ReadUInt32
                CreatureModelData(i).StringOffset = br.ReadUInt32
            Next

            For i As Integer = 0 To nRecords - 1
                CreatureModelData(i).ModelFilename = hf.GetZeroDelimitedStringFromBinaryReader(br, CreatureModelData(i).StringOffset + 20 + nRecords * recordSize).Replace(".mdx", ".m2")
                'Debug.Print(Creaturemodeldata(i).ModelFilename)
            Next

            Return True
        End Function

        Public Function LoadCreatureDisplayInfo(ByVal FileName As String)
            Dim br As New BinaryReader(File.OpenRead(FileName))

            If br.ReadChars(4) <> "WDBC" Then Return False

            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

            'Debug.Print(nRecords)
            'Debug.Print(nFields)
            'Debug.Print(recordSize)
            'Debug.Print(stringSize)

            ReDim CreatureDisplayInfo(nRecords - 1)

            For i As Integer = 0 To nRecords - 1
                br.BaseStream.Position = 20 + i * recordSize
                CreatureDisplayInfo(i).CreatureID = br.ReadUInt32
                CreatureDisplayInfo(i).CreatureModelID = br.ReadUInt32
                Dim Unknown0 As UInt32 = br.ReadUInt32
                Dim Unknown1 As UInt32 = br.ReadUInt32
                CreatureDisplayInfo(i).Scale = br.ReadSingle
                Dim Unknown2 As UInt32 = br.ReadUInt32
                CreatureDisplayInfo(i).StringOffset11 = br.ReadUInt32
                CreatureDisplayInfo(i).StringOffset12 = br.ReadUInt32
                CreatureDisplayInfo(i).StringOffset13 = br.ReadUInt32
            Next

            For i As Integer = 0 To nRecords - 1
                If CreatureDisplayInfo(i).StringOffset11 > 0 Then CreatureDisplayInfo(i).Texture11 = hf.GetZeroDelimitedStringFromBinaryReader(br, CreatureDisplayInfo(i).StringOffset11 + 20 + nRecords * recordSize)
                If CreatureDisplayInfo(i).StringOffset12 > 0 Then CreatureDisplayInfo(i).Texture12 = hf.GetZeroDelimitedStringFromBinaryReader(br, CreatureDisplayInfo(i).StringOffset12 + 20 + nRecords * recordSize)
                If CreatureDisplayInfo(i).StringOffset13 > 0 Then CreatureDisplayInfo(i).Texture13 = hf.GetZeroDelimitedStringFromBinaryReader(br, CreatureDisplayInfo(i).StringOffset13 + 20 + nRecords * recordSize)
            Next

            Return True
        End Function

        Public Function GetIDFromModelFileName(ByVal FileName As String) As UInt32
            Dim i As Integer = 0
            Dim found As Boolean = False
            Dim ID As UInt32 = 0

            While Not found And i < CreatureModelData.Length
                Dim s As String = CreatureModelData(i).ModelFilename.ToLower
                If s.LastIndexOf("\") >= 0 Then s = s.Substring(s.LastIndexOf("\"))
                If s = FileName.Substring(FileName.LastIndexOf("\")).ToLower Then
                    found = True
                    ID = CreatureModelData(i).ModelID
                End If
                i += 1
            End While

            Return ID
        End Function

        Public Function GetTextureFromCreatureModelID(ByVal ModelID As UInt32, ByVal Typ As Integer) As String
            Dim i As Integer = 0
            Dim found As Boolean = False
            Dim Tex As String = ""

            While Not found And i < CreatureDisplayInfo.Length
                If CreatureDisplayInfo(i).CreatureModelID = ModelID Then
                    found = True
                    Select Case Typ
                        Case 11
                            Tex = CreatureDisplayInfo(i).Texture11
                        Case 12
                            Tex = CreatureDisplayInfo(i).Texture12
                        Case 13
                            Tex = CreatureDisplayInfo(i).Texture13
                    End Select
                End If
                i += 1
            End While

            Return Tex
        End Function

    End Class

    Class BLP

        Public Function Load(ByVal FileName As String) As Bitmap
            Dim br As New BinaryReader(File.OpenRead(FileName))

            If br.ReadChars(4) <> "BLP2" Then Return Nothing

            Dim bType As UInt32 = br.ReadUInt32
            Dim bEnc As Byte = br.ReadByte
            Dim bAlphaDepth As Byte = br.ReadByte
            Dim bAlphaEnc As Byte = br.ReadByte
            Dim bHasMips As Byte = br.ReadByte
            Dim bWidth As UInt32 = br.ReadUInt32
            Dim bHeight As UInt32 = br.ReadUInt32
            Dim bOffsets As UInt32() = New UInt32(15) {br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32}
            Dim bLengths As UInt32() = New UInt32(15) {br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, _
                                                       br.ReadUInt32, br.ReadUInt32, br.ReadUInt32, br.ReadUInt32}
            Dim Palette(255) As Color
            For i As Integer = 0 To 255
                Palette(i) = Color.FromArgb(br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte)
            Next

            br.BaseStream.Position = bOffsets(0) 'we only care about the first MIP and assume it is the most relevant one...

            Dim LastBLP = New Bitmap(bWidth, bHeight, Imaging.PixelFormat.Format32bppArgb)

            If (bType = 1 And bEnc = 2 And (bAlphaDepth = 1 Or bAlphaDepth = 0)) Then ' DXT1
                For y As Integer = 0 To bHeight - 1 Step 4
                    For x As Integer = 0 To bWidth - 1 Step 4
                        Dim _RGBValues As UInt16() = New UInt16(1) {br.ReadUInt16, br.ReadUInt16}
                        Dim _ColorLUT As UInt32 = br.ReadUInt32

                        Dim ColorLUT As Byte(,) = New Byte(3, 3) {{(_ColorLUT >> 30 And 3), (_ColorLUT >> 28 And 3), (_ColorLUT >> 26 And 3), (_ColorLUT >> 24 And 3)}, _
                                                                  {(_ColorLUT >> 22 And 3), (_ColorLUT >> 20 And 3), (_ColorLUT >> 18 And 3), (_ColorLUT >> 16 And 3)}, _
                                                                  {(_ColorLUT >> 14 And 3), (_ColorLUT >> 12 And 3), (_ColorLUT >> 10 And 3), (_ColorLUT >> 8 And 3)}, _
                                                                  {(_ColorLUT >> 6 And 3), (_ColorLUT >> 4 And 3), (_ColorLUT >> 2 And 3), (_ColorLUT And 3)}}
                        Dim RGBValues As Color() = New Color(3) {}
                        RGBValues(0) = Color.FromArgb(255, _RGBValues(0) >> 8 And 255, _RGBValues(0) >> 3 And 255, _RGBValues(0) << 3 And 255)
                        RGBValues(1) = Color.FromArgb(255, _RGBValues(1) >> 8 And 255, _RGBValues(1) >> 3 And 255, _RGBValues(1) << 3 And 255)

                        If RGBValues(0).ToArgb > RGBValues(1).ToArgb Then
                            RGBValues(2) = Color.FromArgb(255, 2 / 3 * RGBValues(0).R + 1 / 3 * RGBValues(1).R, 2 / 3 * RGBValues(0).G + 1 / 3 * RGBValues(1).G, 2 / 3 * RGBValues(0).B + 1 / 3 * RGBValues(1).B)
                            RGBValues(3) = Color.FromArgb(255, 1 / 3 * RGBValues(0).R + 2 / 3 * RGBValues(1).R, 1 / 3 * RGBValues(0).G + 2 / 3 * RGBValues(1).G, 1 / 3 * RGBValues(0).B + 2 / 3 * RGBValues(1).B)
                        Else
                            RGBValues(2) = Color.FromArgb(255, 1 / 2 * RGBValues(0).R + 1 / 2 * RGBValues(1).R, 1 / 2 * RGBValues(0).G + 1 / 2 * RGBValues(1).G, 1 / 2 * RGBValues(0).B + 1 / 2 * RGBValues(1).B)
                            RGBValues(3) = Color.FromArgb(0, 0, 0, 0)
                        End If

                        For y1 As Integer = 0 To 3
                            For x1 As Integer = 0 To 3
                                Dim ci As Integer = ColorLUT(3 - y1, 3 - x1)
                                LastBLP.SetPixel(x + x1, y + y1, Color.FromArgb(RGBValues(ci).A, RGBValues(ci).R, RGBValues(ci).G, RGBValues(ci).B))
                            Next
                        Next

                    Next
                Next

            ElseIf (bType = 1 And bEnc = 2 And bAlphaDepth = 8 And bAlphaEnc = 1) Then ' DXT3
                For y As Integer = 0 To bHeight - 1 Step 4
                    For x As Integer = 0 To bWidth - 1 Step 4
                        Dim _AlphaMap As Byte() = New Byte(7) {br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte}
                        Dim _RGBValues As UInt16() = New UInt16(1) {br.ReadUInt16, br.ReadUInt16}
                        Dim _ColorLUT As UInt32 = br.ReadUInt32

                        Dim ColorLUT As Byte(,) = New Byte(3, 3) {{(_ColorLUT >> 30 And 3), (_ColorLUT >> 28 And 3), (_ColorLUT >> 26 And 3), (_ColorLUT >> 24 And 3)}, _
                                                                  {(_ColorLUT >> 22 And 3), (_ColorLUT >> 20 And 3), (_ColorLUT >> 18 And 3), (_ColorLUT >> 16 And 3)}, _
                                                                  {(_ColorLUT >> 14 And 3), (_ColorLUT >> 12 And 3), (_ColorLUT >> 10 And 3), (_ColorLUT >> 8 And 3)}, _
                                                                  {(_ColorLUT >> 6 And 3), (_ColorLUT >> 4 And 3), (_ColorLUT >> 2 And 3), (_ColorLUT And 3)}}

                        Dim AlphaMap As Byte(,) = New Byte(3, 3) {{_AlphaMap(0) << 4 Or 15, _AlphaMap(0) Or 15, _AlphaMap(1) << 4 Or 15, _AlphaMap(1) Or 15}, _
                                                                  {_AlphaMap(2) << 4 Or 15, _AlphaMap(2) Or 15, _AlphaMap(3) << 4 Or 15, _AlphaMap(3) Or 15}, _
                                                                  {_AlphaMap(4) << 4 Or 15, _AlphaMap(4) Or 15, _AlphaMap(5) << 4 Or 15, _AlphaMap(5) Or 15}, _
                                                                  {_AlphaMap(6) << 4 Or 15, _AlphaMap(6) Or 15, _AlphaMap(7) << 4 Or 15, _AlphaMap(7) Or 15}}

                        Dim RGBValues As Color() = New Color(3) {}
                        RGBValues(0) = Color.FromArgb(0, _RGBValues(0) >> 8 And 255, _RGBValues(0) >> 3 And 255, _RGBValues(0) << 3 And 255)
                        RGBValues(1) = Color.FromArgb(0, _RGBValues(1) >> 8 And 255, _RGBValues(1) >> 3 And 255, _RGBValues(1) << 3 And 255)

                        If _RGBValues(0) > _RGBValues(1) Then
                            RGBValues(2) = Color.FromArgb(0, 2 / 3 * RGBValues(0).R + 1 / 3 * RGBValues(1).R, 2 / 3 * RGBValues(0).G + 1 / 3 * RGBValues(1).G, 2 / 3 * RGBValues(0).B + 1 / 3 * RGBValues(1).B)
                            RGBValues(3) = Color.FromArgb(0, 1 / 3 * RGBValues(0).R + 2 / 3 * RGBValues(1).R, 1 / 3 * RGBValues(0).G + 2 / 3 * RGBValues(1).G, 1 / 3 * RGBValues(0).B + 2 / 3 * RGBValues(1).B)
                        Else
                            Debug.Print("Weird Values found in file...")
                        End If

                        For y1 As Integer = 0 To 3
                            For x1 As Integer = 0 To 3
                                Dim ci As Integer = ColorLUT(3 - y1, 3 - x1)
                                Dim al As Integer = AlphaMap(y1, x1)
                                LastBLP.SetPixel(x + x1, y + y1, Color.FromArgb(al, RGBValues(ci).R, RGBValues(ci).G, RGBValues(ci).B))
                            Next
                        Next

                    Next
                Next

            ElseIf (bType = 1 And bEnc = 2 And bAlphaDepth = 8 And bAlphaEnc = 7) Then ' DXT5
                For y As Integer = 0 To bHeight - 1 Step 4
                    For x As Integer = 0 To bWidth - 1 Step 4
                        Dim a0 As Byte = br.ReadByte
                        Dim a1 As Byte = br.ReadByte
                        Dim _AlphaLUT As Byte() = New Byte(5) {br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte}
                        Dim _RGBValues As UInt16() = New UInt16(1) {br.ReadUInt16, br.ReadUInt16}
                        Dim _ColorLUT As UInt32 = br.ReadUInt32

                        Dim ColorLUT As Byte(,) = New Byte(3, 3) {{(_ColorLUT >> 30 And 3), (_ColorLUT >> 28 And 3), (_ColorLUT >> 26 And 3), (_ColorLUT >> 24 And 3)}, _
                                                                  {(_ColorLUT >> 22 And 3), (_ColorLUT >> 20 And 3), (_ColorLUT >> 18 And 3), (_ColorLUT >> 16 And 3)}, _
                                                                  {(_ColorLUT >> 14 And 3), (_ColorLUT >> 12 And 3), (_ColorLUT >> 10 And 3), (_ColorLUT >> 8 And 3)}, _
                                                                  {(_ColorLUT >> 6 And 3), (_ColorLUT >> 4 And 3), (_ColorLUT >> 2 And 3), (_ColorLUT And 3)}}

                        ' the LUT looks like this:
                        ' b0        b1         b2         b3        b4         b5        
                        ' 012 345 670 123 456 701 234 567 012 345 670 123 456 701 234 567
                        ' a00 a01 a02 a03 a10 a11 a12 a13 a20 a21 a22 a23 a30 a31 a32 a33

                        Dim AlphaLUT As Byte(,) = New Byte(3, 3) {}
                        AlphaLUT(0, 0) = _AlphaLUT(0) And 7
                        AlphaLUT(0, 1) = _AlphaLUT(0) >> 3 And 7
                        AlphaLUT(0, 2) = _AlphaLUT(0) >> 6 And _AlphaLUT(1) >> 2 And 7
                        AlphaLUT(0, 3) = _AlphaLUT(1) >> 1 And 7
                        AlphaLUT(1, 0) = _AlphaLUT(1) >> 4 And 7
                        AlphaLUT(1, 1) = _AlphaLUT(1) >> 7 And _AlphaLUT(2) >> 1 And 7
                        AlphaLUT(1, 2) = _AlphaLUT(2) >> 2 And 7
                        AlphaLUT(1, 3) = _AlphaLUT(2) >> 5 And 7

                        AlphaLUT(2, 0) = _AlphaLUT(3) And 7
                        AlphaLUT(2, 1) = _AlphaLUT(3) >> 3 And 7
                        AlphaLUT(2, 2) = _AlphaLUT(3) >> 6 And _AlphaLUT(4) >> 2 And 7
                        AlphaLUT(2, 3) = _AlphaLUT(4) >> 1 And 7
                        AlphaLUT(3, 0) = _AlphaLUT(4) >> 4 And 7
                        AlphaLUT(3, 1) = _AlphaLUT(4) >> 7 And _AlphaLUT(5) >> 1 And 7
                        AlphaLUT(3, 2) = _AlphaLUT(5) >> 2 And 7
                        AlphaLUT(3, 3) = _AlphaLUT(5) >> 5 And 7

                        Dim RGBValues As Color() = New Color(3) {}
                        RGBValues(0) = Color.FromArgb(0, _RGBValues(0) >> 8 And 255, _RGBValues(0) >> 3 And 255, _RGBValues(0) << 3 And 255)
                        RGBValues(1) = Color.FromArgb(0, _RGBValues(1) >> 8 And 255, _RGBValues(1) >> 3 And 255, _RGBValues(1) << 3 And 255)

                        If _RGBValues(0) > _RGBValues(1) Then
                            RGBValues(2) = Color.FromArgb(0, 2 / 3 * RGBValues(0).R + 1 / 3 * RGBValues(1).R, 2 / 3 * RGBValues(0).G + 1 / 3 * RGBValues(1).G, 2 / 3 * RGBValues(0).B + 1 / 3 * RGBValues(1).B)
                            RGBValues(3) = Color.FromArgb(0, 1 / 3 * RGBValues(0).R + 2 / 3 * RGBValues(1).R, 1 / 3 * RGBValues(0).G + 2 / 3 * RGBValues(1).G, 1 / 3 * RGBValues(0).B + 2 / 3 * RGBValues(1).B)
                        Else
                            Debug.Print("Weird Values found in file...")
                        End If

                        Dim AlphaValues As Byte() = New Byte(7) {}
                        AlphaValues(0) = a0
                        AlphaValues(1) = a1
                        If AlphaValues(0) > AlphaValues(1) Then
                            AlphaValues(2) = (6.0 * a0 + 1.0 * a1) / 7.0
                            AlphaValues(3) = (5.0 * a0 + 2.0 * a1) / 7.0
                            AlphaValues(4) = (4.0 * a0 + 3.0 * a1) / 7.0
                            AlphaValues(5) = (3.0 * a0 + 4.0 * a1) / 7.0
                            AlphaValues(6) = (2.0 * a0 + 5.0 * a1) / 7.0
                            AlphaValues(7) = (1.0 * a0 + 6.0 * a1) / 7.0
                        Else
                            AlphaValues(2) = (4.0 * a0 + 1.0 * a1) / 5.0
                            AlphaValues(3) = (3.0 * a0 + 2.0 * a1) / 5.0
                            AlphaValues(4) = (2.0 * a0 + 3.0 * a1) / 5.0
                            AlphaValues(5) = (1.0 * a0 + 4.0 * a1) / 5.0
                            AlphaValues(6) = 0
                            AlphaValues(7) = 255
                        End If

                        For y1 As Integer = 0 To 3
                            For x1 As Integer = 0 To 3
                                Dim ci As Integer = ColorLUT(3 - y1, 3 - x1)
                                Dim al As Integer = AlphaValues(AlphaLUT(y1, x1))
                                LastBLP.SetPixel(x + x1, y + y1, Color.FromArgb(al, RGBValues(ci).R, RGBValues(ci).G, RGBValues(ci).B))
                            Next
                        Next

                    Next
                Next

            Else
                Debug.Print("File with unknown properties found: (bType = " & bType & ", bEnc = " & bEnc & ", bAlphaDepth = " & bAlphaDepth & ", bAlphaEnc = " & bAlphaEnc & ")")
                Return Nothing 'we don't support this yet...
            End If

            Return LastBLP
        End Function


    End Class

End Namespace

