Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports MpqReader
Imports wow2collada.HelperFunctions

Namespace FileReaders

    Public Class ADT

        Public Structure sLayer
            Public TextureID As UInt32
            Public Flags As UInt32
            Public AlphaOffset As UInt32
            Public DetailTextureID As UInt32
        End Structure

        Public Structure sM2Placement
            Public MMDX_ID As UInt32
            Public ID As UInt32
            Public Position As sVector3
            Public Orientation As sVector3
            Public Scale As Single
        End Structure

        Public Structure sWMOPlacement
            Public MWMO_ID As UInt32
            Public ID As UInt32
            Public Position As sVector3
            Public Orientation As sVector3
            Public UpperExtents As sVector3
            Public LowerExtents As sVector3
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
            Dim Position As sVector3
            Dim offsColorValues As UInt32
            Dim props As UInt32
            Dim effectId As UInt32
            Dim HeightMap As Single()
            Dim NormalMap As sVector3()
            Dim Layer As sLayer()
            Dim AlphaMaps As Bitmap()
            Dim HeightMap9x9 As Single(,)
            Dim HeightMap8x8 As Single(,)
            Dim preWotLK As Boolean
        End Structure

        Public TextureFiles As String()
        Public ModelFiles As String()
        Public WMOFiles As String()
        Public M2Placements As sM2Placement()
        Public WMOPlacements As sWMOPlacement()
        Public MCNKs As sMCNK(,)
        Public Version As UInt32

        Public Sub Load(ByVal File As Byte())
            Load(New MemoryStream(File))
        End Sub

        Public Function Load(ByVal FileName As String) As Boolean
            Return Load(File.OpenRead(FileName))
        End Function

        Public Function Load(ByVal File As Stream, Optional ByVal HeightOnly As Boolean = False) As Boolean
            Dim br As New BinaryReader(File)

            Dim ChunkId As String
            Dim ChunkLen As UInt32
            Dim FilePosition As UInt32 = 0
            ReDim MCNKs(15, 15)
            Dim Done As Boolean = False

            While br.BaseStream.Position < br.BaseStream.Length And Not Done
                ChunkId = br.ReadChars(4)
                ChunkId = myHF.StrRev(ChunkId)
                ChunkLen = br.ReadUInt32

                Select Case ChunkId
                    Case "MHDR" 'Ignore the header chunk for now (redundant)
                    Case "MVER"
                        Version = br.ReadUInt32
                    Case "MCIN" 'No need for now
                    Case "MTEX" 'Texture files
                        If Not HeightOnly Then TextureFiles = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

                    Case "MMDX" 'Model files
                        If Not HeightOnly Then ModelFiles = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

                    Case "MMID" 'MMDX indices (ignore)
                    Case "MWMO" 'WMO files
                        If Not HeightOnly Then WMOFiles = myHF.GetAllZeroDelimitedStrings(br.ReadBytes(ChunkLen))

                    Case "MWID" 'MWMO indices (ignore)
                    Case "MDDF" 'M2 Placements
                        If Not HeightOnly Then
                            ReDim M2Placements(ChunkLen / 36 - 1)
                            For i As Integer = 0 To ChunkLen / 36 - 1
                                M2Placements(i).MMDX_ID = br.ReadUInt32
                                M2Placements(i).ID = br.ReadUInt32
                                M2Placements(i).Position = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                                M2Placements(i).Orientation = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                                M2Placements(i).Scale = br.ReadUInt16 / 1024.0F
                                Dim Unknown As UInt16 = br.ReadUInt16
                            Next
                        End If

                    Case "MODF" 'WMO placements
                        If Not HeightOnly Then
                            ReDim WMOPlacements(ChunkLen / 64 - 1)
                            For i As Integer = 0 To ChunkLen / 64 - 1
                                WMOPlacements(i).MWMO_ID = br.ReadUInt32
                                WMOPlacements(i).ID = br.ReadUInt32
                                WMOPlacements(i).Position = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                                WMOPlacements(i).Orientation = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                                WMOPlacements(i).UpperExtents = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                                WMOPlacements(i).LowerExtents = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                                Dim Unknown As UInt16 = br.ReadUInt16
                                WMOPlacements(i).DoodadSetIndex = br.ReadUInt16
                                WMOPlacements(i).NameSetIndex = br.ReadUInt32
                            Next
                        End If

                    Case "MCNK"
                        LoadMCNK(br, HeightOnly)


                    Case "MH2O" 'Water and such...
                        ' do it :)
                    Case "", Chr(0) & Chr(0) & Chr(0) & Chr(0)
                        Done = True
                    Case Else
                        Debug.Print("Unknown Chunktype: " & ChunkId)
                End Select

                FilePosition += ChunkLen + 8
                br.BaseStream.Position = FilePosition
            End While

            Return True
        End Function

        Private Sub LoadMCNK(ByRef br As BinaryReader, ByVal HeightOnly As Boolean)
            Dim FilePosition As Integer = br.BaseStream.Position - 8
            Dim Flags As UInt32 = br.ReadUInt32
            Dim IndexX As UInt32 = br.ReadUInt32
            Dim IndexY As UInt32 = br.ReadUInt32

            Dim SubChunkId As String
            Dim SubChunkLen As UInt32

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
                .Position = New sVector3(br.ReadSingle, br.ReadSingle, br.ReadSingle)
                .offsColorValues = br.ReadUInt32
                .props = br.ReadUInt32
                .effectId = br.ReadUInt32

                'MCVT subchunk
                br.BaseStream.Position = FilePosition + .offsHeight
                SubChunkId = br.ReadChars(4)
                SubChunkId = myHF.StrRev(SubChunkId)
                SubChunkLen = br.ReadUInt32
                If SubChunkId <> "MCVT" Then Debug.Print("Argh...: Expected MCVT subchunk, got: " & SubChunkId)
                If SubChunkLen <> 145 * 4 Then Debug.Print("Argh...: Expected MCVT subchunk of length " & (145 * 4) & ", got: " & SubChunkLen)
                ReDim .HeightMap(144)

                For i As Integer = 0 To 144
                    .HeightMap(i) = br.ReadSingle
                Next
                .HeightMap8x8 = GetHeightMap8x8FromHeightMap(.HeightMap)
                .HeightMap9x9 = GetHeightMap9x9FromHeightMap(.HeightMap)

                If Not HeightOnly Then
                    'MCNR subchunk
                    br.BaseStream.Position = FilePosition + .offsNormal
                    SubChunkId = br.ReadChars(4)
                    SubChunkId = myHF.StrRev(SubChunkId)
                    SubChunkLen = br.ReadUInt32
                    If SubChunkId <> "MCNR" Then Debug.Print("Argh...: Expected MCNR subchunk, got: " & SubChunkId)
                    If SubChunkLen <> (145 * 3) Then Debug.Print("Argh...: Expected MCNR subchunk of length " & (145 * 3) & ", got: " & SubChunkLen)
                    ReDim .NormalMap(144)

                    For i As Integer = 0 To 144
                        .NormalMap(i) = New sVector3(br.ReadSByte / 127, br.ReadSByte / 127, br.ReadSByte / 127)
                    Next

                    'MCLY subchunk
                    br.BaseStream.Position = FilePosition + .offsLayer
                    SubChunkId = br.ReadChars(4)
                    SubChunkId = myHF.StrRev(SubChunkId)
                    SubChunkLen = br.ReadUInt32
                    If SubChunkId <> "MCLY" Then Debug.Print("Argh...: Expected MCLY subchunk, got: " & SubChunkId)
                    If SubChunkLen Mod 16 <> 0 Then Debug.Print("Argh...: Expected MCLY subchunk of length multiple 16, got: " & SubChunkLen)
                    ReDim .Layer(3)

                    For i As Integer = 0 To SubChunkLen / 16 - 1
                        .Layer(i).TextureID = br.ReadInt32
                        .Layer(i).Flags = br.ReadInt32
                        .Layer(i).AlphaOffset = br.ReadInt32
                        .Layer(i).DetailTextureID = br.ReadInt32
                    Next

                    'MCAL subchunk
                    br.BaseStream.Position = FilePosition + .offsAlpha
                    SubChunkId = br.ReadChars(4)
                    SubChunkId = myHF.StrRev(SubChunkId)
                    SubChunkLen = br.ReadUInt32
                    If SubChunkId <> "MCAL" Then Debug.Print("Argh...: Expected MCAL subchunk, got: " & SubChunkId)
                    'If SubChunkLen Mod 16 <> 0 Then Debug.Print("Argh...: Expected MCAL subchunk of length multiple 16, got: " & SubChunkLen)
                    ReDim .AlphaMaps(3)

                    .preWotLK = (SubChunkLen Mod 2048 = 0) And SubChunkLen > 0

                    For i As Integer = 0 To 3
                        'If .Layer(i).TextureID > 0 Then
                        If .Layer(i).Flags And &H100 Then 'use alpha map
                            If .Layer(i).Flags And &H200 Then 'compressed alpha
                                Dim Buffer(4095) As Byte
                                Dim offO As Integer = 0
                                While offO < 4096
                                    Dim b1 As Byte = br.ReadByte
                                    Dim b2 As Byte = br.ReadByte
                                    Dim count As Integer = b1 And &H7F
                                    Dim fill As Boolean = b1 And &H80

                                    'Debug.Print(br.BaseStream.Position.ToString("0000000") & " " & offO.ToString("0000") & " " & count.ToString("000") & " " & fill)

                                    For k As Integer = 0 To count - 1
                                        If offO < 4096 Then
                                            Buffer(offO) = b2
                                            offO += 1
                                        Else
                                            Debug.Print(String.Format("Compressed Alpha Weirdness: Bufferpos {0} / Bufferval {1}", offO, b2))
                                            k = count
                                        End If
                                        If Not fill And k < (count - 1) Then b2 = br.ReadByte
                                    Next

                                End While
                                .AlphaMaps(i) = BytesToAlphaBitmap4096(Buffer)
                            Else 'uncompressed alpha
                                If .preWotLK Then
                                    Dim Buffer(2047) As Byte
                                    Buffer = br.ReadBytes(2048)
                                    .AlphaMaps(i) = BytesToAlphaBitmap2048(Buffer)
                                Else
                                    Dim Buffer(4095) As Byte
                                    Buffer = br.ReadBytes(4096)
                                    .AlphaMaps(i) = BytesToAlphaBitmap4096(Buffer)
                                End If
                            End If
                        End If
                        'End If
                    Next
                End If

            End With
        End Sub

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
                    Out(r, c) = HeightMap(c * 17 + r)
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
                    Out(r, c) = HeightMap(9 + c * 17 + r)
                Next
            Next

            Return Out
        End Function

        Private Function BytesToAlphaBitmap2048(ByVal buffer As Byte())
            Dim Out As New Bitmap(64, 64, Imaging.PixelFormat.Format32bppArgb)
            Dim B1 As Byte
            Dim B2 As Byte

            For y As Integer = 0 To 63
                For x As Integer = 0 To 31
                    B1 = buffer(x + y * 32) And &HF0
                    B2 = buffer(x + y * 32) << 4 And &HF0
                    Out.SetPixel(x * 2 + 1, y, Color.FromArgb(B1, B1, B1, B1))
                    Out.SetPixel(x * 2, y, Color.FromArgb(B2, B2, B2, B2))
                Next
            Next

            Return Out
        End Function

        Private Function BytesToAlphaBitmap4096(ByVal buffer As Byte())
            Dim Out As New Bitmap(64, 64, Imaging.PixelFormat.Format32bppArgb)
            Dim B As Byte

            For y As Integer = 0 To 63
                For x As Integer = 0 To 63
                    B = buffer(x + y * 64)
                    Out.SetPixel(x, y, Color.FromArgb(B, B, B, B))
                Next
            Next

            Return Out
        End Function

    End Class

End Namespace

