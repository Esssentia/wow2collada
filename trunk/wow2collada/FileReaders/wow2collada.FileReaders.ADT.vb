Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports MpqReader

Namespace FileReaders

    Class ADT

        Public Structure sM2Placement
            Public MMDX_ID As UInt32
            Public ID As UInt32
            Public Position As Vector3
            Public Orientation As Vector3
            Public Scale As Single
        End Structure

        Public Structure sWMOPlacement
            Public MWMO_ID As UInt32
            Public ID As UInt32
            Public Position As Vector3
            Public Orientation As Vector3
            Public UpperExtents As Vector3
            Public LowerExtents As Vector3
            Public DoodadSetIndex As UInt16
            Public NameSetIndex As UInt32
        End Structure

        Public Structure sMCNK
            Dim flags As UInt32
            Dim nLayers As UInt32
            Dim nDoodadRefs As UInt32
            Dim offsHeight As UInt32
            Dim offsNormal As UInt32
            Dim offsLayer As UInt32
            Dim offsRefs As UInt32
            Dim offsAlpha As UInt32
            Dim sizeAlpha As UInt32
            Dim offsShadow As UInt32
            Dim sizeShadow As UInt32
            Dim areaid As UInt32
            Dim nMapObjRefs As UInt32
            Dim holes As UInt32
            Dim predTex As UInt32
            Dim noEffectDoodad As UInt32
            Dim offsSndEmitters As UInt32
            Dim nSndEmitters As UInt32
            Dim offsLiquid As UInt32
            Dim sizeLiquid As UInt32
            Dim Position As Vector3
            Dim offsColorValues As UInt32
            Dim props As UInt32
            Dim effectId As UInt32
            Dim HeightMap As Single()
            Dim HeightMap9x9 As Single(,)
            Dim HeightMap8x8 As Single(,)
        End Structure

        Public TextureFiles() As String
        Public ModelFiles() As String
        Public WMOFiles() As String
        Public M2Placements() As sM2Placement
        Public WMOPlacements() As sWMOPlacement
        Public MCNKs(,) As sMCNK

        Public Function Load(ByVal FileName As String) As Boolean
            Return LoadFromStream(File.OpenRead(FileName), FileName)
        End Function

        Public Function LoadFromStream(ByVal File As Stream, ByVal FileName As String) As Boolean
            Dim br As New BinaryReader(File)

            Dim ChunkId As String
            Dim ChunkLen As UInt32
            Dim FilePosition As UInt32 = 0
            Dim Version As UInt32
            ReDim MCNKs(15, 15)

            While br.BaseStream.Position < br.BaseStream.Length
                ChunkId = br.ReadChars(4)
                ChunkId = myHF.StrRev(ChunkId)
                ChunkLen = br.ReadUInt32

                Select Case ChunkId
                    Case "MHDR" 'Ignore the header chunk for now (redundant)
                    Case "MVER"
                        Version = br.ReadUInt32
                    Case "MCIN" 'No need for now
                    Case "MTEX" 'Texture files
                        TextureFiles = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

                    Case "MMDX" 'Model files
                        ModelFiles = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

                    Case "MMID" 'MMDX indices (ignore)
                    Case "MWMO" 'WMO files
                        WMOFiles = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

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
                            SubChunkId = myHF.StrRev(SubChunkId)
                            Dim SubChunkLen As UInt32 = br.ReadUInt32
                            If SubChunkId <> "MCVT" Then Debug.Print("Argh...: Expected MCVT sub-chunk, got: " & SubChunkId)
                            If SubChunkLen <> 145 * 4 Then Debug.Print("Argh...: Expected MCVT sub-chunk of length " & (145 * 4) & ", got: " & SubChunkLen)
                            ReDim .HeightMap(144)

                            For i As Integer = 0 To 144
                                .HeightMap(i) = br.ReadSingle
                            Next
                            .HeightMap8x8 = GetHeightMap8x8FromHeightMap(.HeightMap)
                            .HeightMap9x9 = GetHeightMap9x9FromHeightMap(.HeightMap)
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

        Private Function GetHeightMap9x9FromHeightMap(ByVal HeightMap() As Single) As Single(,)
            Dim Out(8, 8) As Single

            '  1    2    3    4    5    6    7    8    9       Row 0
            '    10   11   12   13   14   15   16   17         Row 1
            '  18   19   20   21   22   23   24   25   26      Row 2
            '    27   28   29   30   31   32   33   34         Row 3
            '  35   36   37   38   39   40   41   42   43      Row 4
            '    44   45   46   47   48   49   50   51         Row 5
            '  52   53   54   55   56   57   58   59   60      Row 6
            '    61   62   63   64   65   66   67   68         Row 7
            '  69   70   71   72   73   74   75   76   77      Row 8
            '    78   79   80   81   82   83   84   85         Row 9
            '  86   87   88   89   90   91   92   93   94      Row 10
            '    95   96   97   98   99   100  101  102        Row 11
            ' 103  104  105  106  107  108  109  110  111      Row 12
            '   112  113  114  115  116  117  118  119         Row 13
            ' 120  121  122  123  124  125  126  127  128      Row 14
            '   129  130  131  132  133  134  135  136         Row 15
            ' 137  138  139  140  141  142  143  144  145      Row 16

            For r As Integer = 0 To 8
                For c As Integer = 0 To 8
                    Out(r, c) = HeightMap(r * 17 + c)
                Next
            Next

            Return Out
        End Function

        Private Function GetHeightMap8x8FromHeightMap(ByVal HeightMap() As Single) As Single(,)
            Dim Out(7, 7) As Single

            '  1    2    3    4    5    6    7    8    9       Row 0
            '    10   11   12   13   14   15   16   17         Row 1
            '  18   19   20   21   22   23   24   25   26      Row 2
            '    27   28   29   30   31   32   33   34         Row 3
            '  35   36   37   38   39   40   41   42   43      Row 4
            '    44   45   46   47   48   49   50   51         Row 5
            '  52   53   54   55   56   57   58   59   60      Row 6
            '    61   62   63   64   65   66   67   68         Row 7
            '  69   70   71   72   73   74   75   76   77      Row 8
            '    78   79   80   81   82   83   84   85         Row 9
            '  86   87   88   89   90   91   92   93   94      Row 10
            '    95   96   97   98   99   100  101  102        Row 11
            ' 103  104  105  106  107  108  109  110  111      Row 12
            '   112  113  114  115  116  117  118  119         Row 13
            ' 120  121  122  123  124  125  126  127  128      Row 14
            '   129  130  131  132  133  134  135  136         Row 15
            ' 137  138  139  140  141  142  143  144  145      Row 16

            For r As Integer = 0 To 7
                For c As Integer = 0 To 7
                    Out(r, c) = HeightMap(9 + r * 17 + c)
                Next
            Next

            Return Out
        End Function

    End Class

End Namespace

