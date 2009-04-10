Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports MpqReader

Namespace wow2collada.FileReaders

    Class BLP

        Public Function Load(ByVal FileName As String) As Bitmap
            Return LoadFromStream(File.OpenRead(FileName), FileName)
        End Function

        Public Function LoadFromStream(ByRef File As Stream, ByVal FileName As String) As Bitmap
            Dim br As New BinaryReader(File)

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
                Palette(i) = Color.FromArgb(br.ReadUInt32)
            Next

            br.BaseStream.Position = bOffsets(0) 'we only care about the first MIP and assume it is the most relevant one...

            Dim LastBLP = New Bitmap(bWidth, bHeight, Imaging.PixelFormat.Format32bppArgb)

            If (bType = 1 And bEnc = 1 And bAlphaDepth = 0) Then ' uncompressed, paletted, no alpha
                For y As Integer = 0 To bHeight - 1
                    For x As Integer = 0 To bWidth - 1
                        Dim RGBLUT As Byte = br.ReadByte
                        LastBLP.SetPixel(x, y, Color.FromArgb(255, Palette(RGBLUT).R, Palette(RGBLUT).G, Palette(RGBLUT).B))
                    Next
                Next

            ElseIf (bType = 1 And bEnc = 2 And (bAlphaDepth = 1 Or bAlphaDepth = 0)) Then ' DXT1
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
                        Dim _AlphaLUT As Byte() = New Byte() {br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte, br.ReadByte}
                        Dim _RGBValues As UInt16() = New UInt16(1) {br.ReadUInt16, br.ReadUInt16}
                        Dim _ColorLUT As UInt32 = br.ReadUInt32

                        Dim ColorLUT As Byte(,) = New Byte(3, 3) {{(_ColorLUT >> 30 And 3), (_ColorLUT >> 28 And 3), (_ColorLUT >> 26 And 3), (_ColorLUT >> 24 And 3)}, _
                                                                  {(_ColorLUT >> 22 And 3), (_ColorLUT >> 20 And 3), (_ColorLUT >> 18 And 3), (_ColorLUT >> 16 And 3)}, _
                                                                  {(_ColorLUT >> 14 And 3), (_ColorLUT >> 12 And 3), (_ColorLUT >> 10 And 3), (_ColorLUT >> 8 And 3)}, _
                                                                  {(_ColorLUT >> 6 And 3), (_ColorLUT >> 4 And 3), (_ColorLUT >> 2 And 3), (_ColorLUT And 3)}}

                        ' the LUT looks like this (I think):
                        ' b0        |b1         |b2         |b3        |b4         |b5        
                        ' 012 345 67|0 123 456 7|01 234 567 |012 345 67|0 123 456 7|01 234 567
                        ' a00 a01 a0|2 a03 a10 a|11 a12 a13 |a20 a21 a2|2 a23 a30 a|31 a32 a33

                        Dim AlphaLUT As Byte(,) = New Byte(3, 3) {}
                        AlphaLUT(0, 0) = _AlphaLUT(0) >> 0 And 7
                        AlphaLUT(0, 1) = _AlphaLUT(0) >> 3 And 7
                        AlphaLUT(0, 2) = (_AlphaLUT(0) >> 6 And 3) Or (_AlphaLUT(1) << 2 And 4)
                        AlphaLUT(0, 3) = _AlphaLUT(1) >> 1 And 7
                        AlphaLUT(1, 0) = _AlphaLUT(1) >> 4 And 7
                        AlphaLUT(1, 1) = (_AlphaLUT(1) >> 7 And 1) Or (_AlphaLUT(2) << 1 And 6)
                        AlphaLUT(1, 2) = _AlphaLUT(2) >> 2 And 7
                        AlphaLUT(1, 3) = _AlphaLUT(2) >> 5 And 7

                        AlphaLUT(2, 0) = _AlphaLUT(3) >> 0 And 7
                        AlphaLUT(2, 1) = _AlphaLUT(3) >> 3 And 7
                        AlphaLUT(2, 2) = (_AlphaLUT(3) >> 6 And 3) Or (_AlphaLUT(4) << 2 And 4)
                        AlphaLUT(2, 3) = _AlphaLUT(4) >> 1 And 7
                        AlphaLUT(3, 0) = _AlphaLUT(4) >> 4 And 7
                        AlphaLUT(3, 1) = (_AlphaLUT(4) >> 7 And 1) Or (_AlphaLUT(5) << 1 And 6)
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

