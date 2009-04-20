Imports System.IO

Namespace FileReaders

    Class WMO

        Public Structure sDoodadSet
            Public name As String
            Public index As UInt32
            Public count As UInt32
        End Structure

        Public Structure sDoodad
            Public ModelFile As String
            Public Position As Vector3
            Public Orientation As Quaternion
            Public Scale As Single
            Public LightingColor As Color
        End Structure

        ''' <summary>
        ''' Structure to hold Triangle information (three indices of vertices)
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure sTriangle
            Public VertexIndex1 As UInt16
            Public VertexIndex2 As UInt16
            Public VertexIndex3 As UInt16
        End Structure

        ''' <summary>
        ''' Structure to hold Vertex information (Position, Normal, UV Coordinates and Bone-Information)
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure sVertex
            Public Position As Vector3
            Public BoneWeights As Byte()
            Public BoneIndices As Byte()
            Public Normal As Vector3
            Public TextureCoords As Vector2
        End Structure

        ''' <summary>
        ''' Structure to hold SubSet information (Triangles, Vertices, Material Indices)
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure sSubSet
            Public Triangles As sTriangle()
            Public Vertices As sVertex() ' Position, Normal, UV
            Public Materials As Byte()
        End Structure

        Public Structure sMaterial
            Dim TexID As String
            Dim Flags As Integer
            Dim Blending As Integer
        End Structure

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
        Public DoodadSets As sDoodadSet()
        Public Doodads As sDoodad()
        Public Groups As String()
        Public Textures As sMaterial()
        Public SubSets As sSubSet()

        Public Sub LoadRoot(ByVal File As Byte())
            LoadRoot(New MemoryStream(File))
        End Sub

        Public Sub LoadRoot(ByVal File As Stream)
            Dim br As New BinaryReader(File)

            Dim ChunkId As String
            Dim ChunkLen As UInt32
            Dim Version As UInt32
            Dim FilePosition As UInt32 = 0
            Dim MODN() As Byte = New Byte() {} 'pointless, but I don't want warnings :)
            Dim MOTX() As Byte = New Byte() {} 'pointless, but I don't want warnings :)
            Dim Done As Boolean = False

            While br.BaseStream.Position < br.BaseStream.Length And Not Done
                ChunkId = br.ReadChars(4)
                ChunkId = myHF.StrRev(ChunkId)
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

                        ReDim Textures(nMaterials - 1)
                        ReDim DoodadSets(nSets - 1)
                        ReDim Doodads(nDoodads - 1)

                    Case "MOTX" ' Texture Files (List of zero-terminated strings)
                        'We get them through Materials instead of directly...
                        'Textures = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))
                        MOTX = br.ReadBytes(ChunkLen)
                    Case "MOMT" ' Materials
                        For i As Integer = 0 To nMaterials - 1
                            br.BaseStream.Position = br.BaseStream.Position + 4
                            Textures(i).Flags = br.ReadUInt32
                            Textures(i).Blending = br.ReadUInt32
                            Dim StartID As UInt32 = br.ReadUInt32 'Texture String Start
                            br.BaseStream.Position = br.BaseStream.Position + 48
                            Textures(i).TexID = myHF.GetZeroDelimitedString(MOTX, StartID)
                        Next
                    Case "MOGN" 'Group Names (lets get them just in case...
                        Groups = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

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
                            DoodadSets(i).name = br.ReadChars(20)
                            DoodadSets(i).name = DoodadSets(i).name.Trim(Chr(0)) 'null padded strings suck :)
                            DoodadSets(i).index = br.ReadUInt32
                            DoodadSets(i).count = br.ReadUInt32
                            Dim Unknown As UInt32 = br.ReadUInt32
                        Next

                    Case "MODN" 'Doodad Model Files (List of zero-terminated strings)
                        MODN = br.ReadBytes(ChunkLen)

                    Case "MODD" 'Doodads (40 Bytes each)
                        'MsgBox(br.BaseStream.Position & " " & br.BaseStream.Length)
                        For i As Integer = 0 To (ChunkLen / 40) - 1 ' nDoodads contains ALL doodads, even the ones in subfiles... we'll get them later
                            Dim idx As UInt32 = br.ReadUInt32
                            'Debug.Print(i & " " & idx & " " & br.BaseStream.Position)
                            Doodads(i).ModelFile = myHF.GetZeroDelimitedString(MODN, idx)
                            Doodads(i).Position = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            Doodads(i).Orientation = New Quaternion(br.ReadSingle, br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            Doodads(i).Scale = br.ReadSingle
                            Doodads(i).LightingColor = Color.FromArgb(blue:=br.ReadByte, green:=br.ReadByte, red:=br.ReadByte, alpha:=br.ReadByte)
                        Next

                    Case "MFOG" 'We don't care about fog for now...
                    Case "", Chr(0) & Chr(0) & Chr(0) & Chr(0)
                        Done = True
                    Case Else
                        Debug.Print("Unknown Chunktype: " & ChunkId)
                End Select

                FilePosition += ChunkLen + 8
                If (FilePosition Mod 4 <> 0) Then
                    'MsgBox("Argh...")
                End If
                br.BaseStream.Position = FilePosition
                Application.DoEvents()
            End While
        End Sub

        Public Sub LoadSub(ByVal File As Byte())
            LoadSub(New MemoryStream(File))
        End Sub

        Public Sub LoadSub(ByVal File As Stream)
            Dim br As New BinaryReader(File)

            Dim Version As Integer = 0
            Dim FilePosition As Integer = 0
            Dim Done As Boolean = False
            Dim ChunkId As String
            Dim ChunkLen As UInt32

            If SubSets Is Nothing Then
                ReDim SubSets(0)
            Else
                ReDim Preserve SubSets(SubSets.Length)
            End If

            While br.BaseStream.Position < br.BaseStream.Length And Not Done
                ChunkId = br.ReadChars(4)
                ChunkId = myHF.StrRev(ChunkId)
                ChunkLen = br.ReadUInt32

                Select Case ChunkId
                    Case "MVER"
                        Version = br.ReadUInt32
                    Case "MOGP"
                        ChunkLen = 17 * 4
                    Case "MOPY" ' Materials
                        ReDim SubSets(SubSets.Length - 1).Materials(ChunkLen / 2 - 1)
                        For k As Integer = 0 To ChunkLen / 2 - 1
                            Dim Unknown1 As Byte = br.ReadByte
                            SubSets(SubSets.Length - 1).Materials(k) = br.ReadByte
                        Next

                    Case "MOVI" ' Triangle Vertex Indices
                        ReDim SubSets(SubSets.Length - 1).Triangles(ChunkLen / 6 - 1)
                        For k As Integer = 0 To ChunkLen / 6 - 1
                            SubSets(SubSets.Length - 1).Triangles(k).VertexIndex1 = br.ReadUInt16
                            SubSets(SubSets.Length - 1).Triangles(k).VertexIndex2 = br.ReadUInt16
                            SubSets(SubSets.Length - 1).Triangles(k).VertexIndex3 = br.ReadUInt16
                        Next

                    Case "MOVT" ' Vertices
                        ReDim SubSets(SubSets.Length - 1).Vertices(ChunkLen / 12 - 1)
                        For k As Integer = 0 To ChunkLen / 12 - 1
                            SubSets(SubSets.Length - 1).Vertices(k).Position = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                        Next
                    Case "MONR" ' Normals
                        If SubSets(SubSets.Length - 1).Vertices.Length <> ChunkLen / 12 Then
                            Debug.Print("Vertex Count Normals doesn't match others... urgh...")
                        Else
                            For k As Integer = 0 To ChunkLen / 12 - 1
                                SubSets(SubSets.Length - 1).Vertices(k).Normal = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            Next
                        End If

                    Case "MOTV" ' Texture Coordinates
                        If SubSets(SubSets.Length - 1).Vertices.Length <> ChunkLen / 8 Then
                            Debug.Print("Vertex Count Texture Coordinates doesn't match others... urgh...")
                        Else
                            For k As Integer = 0 To ChunkLen / 8 - 1
                                SubSets(SubSets.Length - 1).Vertices(k).TextureCoords = New Vector2(br.ReadSingle, br.ReadSingle)
                            Next
                        End If

                    Case "MOBA"
                        'Debug.Print("MOBA")
                    Case "MOLR"
                        'Debug.Print("MOLR")
                    Case "MODR"
                        'Debug.Print("MODR")
                    Case "MOBN"
                        'Debug.Print("MOBN")
                    Case "MOBR"
                        'Debug.Print("MOBR")
                    Case "MOCV"
                        'Debug.Print("MOCV")
                    Case "MLIQ"
                        'Debug.Print("MLIQ")
                    Case "", Chr(0) & Chr(0) & Chr(0) & Chr(0)
                        Done = True
                    Case Else
                        Debug.Print("Unknown Chunktype: " & ChunkId)
                End Select

                FilePosition += ChunkLen + 8
                br.BaseStream.Position = FilePosition
            End While
        End Sub

        Public Function LoadFromStream(ByVal File As Stream, ByVal FileName As String) As Boolean
            Dim br As New BinaryReader(File)

            Dim ChunkId As String
            Dim ChunkLen As UInt32
            Dim Version As UInt32
            Dim FilePosition As UInt32 = 0
            Dim MODN() As Byte = New Byte() {} 'pointless, but I don't want warnings :)
            Dim MOTX() As Byte = New Byte() {} 'pointless, but I don't want warnings :)
            Dim Done As Boolean = False

            frm.StatusLabel1.Text = "Loading WMO Basefile..."
            Application.DoEvents()

            While br.BaseStream.Position < br.BaseStream.Length And Not Done
                ChunkId = br.ReadChars(4)
                ChunkId = myHF.StrRev(ChunkId)
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

                        ReDim Textures(nMaterials - 1)
                        ReDim DoodadSets(nSets - 1)
                        ReDim Doodads(nDoodads - 1)

                    Case "MOTX" ' Texture Files (List of zero-terminated strings)
                        'We get them through Materials instead of directly...
                        'Textures = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))
                        MOTX = br.ReadBytes(ChunkLen)

                    Case "MOMT" ' Materials
                        For i As Integer = 0 To nMaterials - 1
                            br.BaseStream.Position = br.BaseStream.Position + 4
                            Textures(i).Flags = br.ReadUInt32
                            Textures(i).Blending = br.ReadUInt32
                            Dim StartID As UInt32 = br.ReadUInt32 'Texture String Start
                            br.BaseStream.Position = br.BaseStream.Position + 48
                            Textures(i).TexID = myHF.GetZeroDelimitedString(MOTX, StartID)
                        Next

                    Case "MOGN" 'Group Names (lets get them just in case...
                        Groups = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

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
                        'MsgBox(br.BaseStream.Position & " " & br.BaseStream.Length)
                        For i As Integer = 0 To (ChunkLen / 40) - 1 ' nDoodads contains ALL doodads, even the ones in subfiles... we'll get them later
                            Dim idx As UInt32 = br.ReadUInt32
                            'Debug.Print(i & " " & idx & " " & br.BaseStream.Position)
                            Doodads(i).ModelFile = myHF.GetZeroDelimitedString(MODN, idx)
                            Doodads(i).Position = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            Doodads(i).Orientation = New Quaternion(br.ReadSingle, br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            Doodads(i).Scale = br.ReadSingle
                            Doodads(i).LightingColor = Color.FromArgb(blue:=br.ReadByte, green:=br.ReadByte, red:=br.ReadByte, alpha:=br.ReadByte)
                        Next

                    Case "MFOG" 'We don't care about fog for now...
                    Case "", Chr(0) & Chr(0) & Chr(0) & Chr(0)
                        Done = True
                    Case Else
                        Debug.Print("Unknown Chunktype: " & ChunkId)
                End Select

                FilePosition += ChunkLen + 8
                If (FilePosition Mod 4 <> 0) Then
                    'MsgBox("Argh...")
                End If
                br.BaseStream.Position = FilePosition
                Application.DoEvents()
            End While

            frm.StatusLabel1.Text = "Loading WMO Subfile(s)..."
            Application.DoEvents()

            ' now try to get the WMO Subfiles... (same directory, same name but with _xxx.wmo instead of just .wmo)
            Dim n As Integer = 0
            Dim SubFile As String = FileName.Substring(0, FileName.LastIndexOf(".")) & "_" & n.ToString.PadLeft(3, "0") & ".wmo"
            While myMPQ.Locate(SubFile)
                'Debug.Print(SubFile)

                If SubSets Is Nothing Then
                    ReDim SubSets(0)
                Else
                    ReDim Preserve SubSets(SubSets.Length)
                End If

                br = New BinaryReader(myMPQ.LoadFile(SubFile))

                Version = 0
                FilePosition = 0
                Done = False

                While br.BaseStream.Position < br.BaseStream.Length And Not Done
                    ChunkId = br.ReadChars(4)
                    ChunkId = myHF.StrRev(ChunkId)
                    ChunkLen = br.ReadUInt32

                    Select Case ChunkId
                        Case "MVER"
                            Version = br.ReadUInt32
                        Case "MOGP"
                            ChunkLen = 17 * 4
                        Case "MOPY" ' Materials
                            ReDim SubSets(SubSets.Length - 1).Materials(ChunkLen / 2 - 1)
                            For k As Integer = 0 To ChunkLen / 2 - 1
                                Dim Unknown1 As Byte = br.ReadByte
                                SubSets(SubSets.Length - 1).Materials(k) = br.ReadByte
                            Next

                        Case "MOVI" ' Triangle Vertex Indices
                            ReDim SubSets(SubSets.Length - 1).Triangles(ChunkLen / 6 - 1)
                            For k As Integer = 0 To ChunkLen / 6 - 1
                                SubSets(SubSets.Length - 1).Triangles(k).VertexIndex1 = br.ReadUInt16
                                SubSets(SubSets.Length - 1).Triangles(k).VertexIndex2 = br.ReadUInt16
                                SubSets(SubSets.Length - 1).Triangles(k).VertexIndex3 = br.ReadUInt16
                            Next

                        Case "MOVT" ' Vertices
                            ReDim SubSets(SubSets.Length - 1).Vertices(ChunkLen / 12 - 1)
                            For k As Integer = 0 To ChunkLen / 12 - 1
                                SubSets(SubSets.Length - 1).Vertices(k).Position = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                            Next
                        Case "MONR" ' Normals
                            If SubSets(SubSets.Length - 1).Vertices.Length <> ChunkLen / 12 Then
                                Debug.Print("Vertex Count Normals doesn't match others... urgh...")
                            Else
                                For k As Integer = 0 To ChunkLen / 12 - 1
                                    SubSets(SubSets.Length - 1).Vertices(k).Normal = New Vector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                                Next
                            End If

                        Case "MOTV" ' Texture Coordinates
                            If SubSets(SubSets.Length - 1).Vertices.Length <> ChunkLen / 8 Then
                                Debug.Print("Vertex Count Texture Coordinates doesn't match others... urgh...")
                            Else
                                For k As Integer = 0 To ChunkLen / 8 - 1
                                    SubSets(SubSets.Length - 1).Vertices(k).TextureCoords = New Vector2(br.ReadSingle, br.ReadSingle)
                                Next
                            End If

                        Case "MOBA"
                            'Debug.Print("MOBA")
                        Case "MOLR"
                            'Debug.Print("MOLR")
                        Case "MODR"
                            'Debug.Print("MODR")
                        Case "MOBN"
                            'Debug.Print("MOBN")
                        Case "MOBR"
                            'Debug.Print("MOBR")
                        Case "MOCV"
                            'Debug.Print("MOCV")
                        Case "MLIQ"
                            'Debug.Print("MLIQ")
                        Case "", Chr(0) & Chr(0) & Chr(0) & Chr(0)
                            Done = True
                        Case Else
                            Debug.Print("Unknown Chunktype: " & ChunkId)
                    End Select

                    FilePosition += ChunkLen + 8
                    If (FilePosition Mod 4 <> 0) Then
                        'MsgBox("Argh...")
                    End If
                    br.BaseStream.Position = FilePosition
                End While

                n += 1
                SubFile = FileName.Substring(0, FileName.LastIndexOf(".")) & "_" & n.ToString.PadLeft(3, "0") & ".wmo"
                Application.DoEvents()
            End While
            Return True
        End Function

    End Class

End Namespace

