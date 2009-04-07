Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Namespace wow2collada

    Public Structure DBC
        Dim nRecords As UInt32
        Dim nFields As UInt32
        Dim recordSize As UInt32
        Dim stringSize As UInt32

    End Structure

    Public Structure MCNK
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

    Public Structure M2Placement
        Public MMDX_ID As UInt32
        Public ID As UInt32
        Public Position As Vector3
        Public Orientation As Vector3
        Public Scale As Single
    End Structure

    Public Structure WMOPlacement
        Public MWMO_ID As UInt32
        Public ID As UInt32
        Public Position As Vector3
        Public Orientation As Vector3
        Public UpperExtents As Vector3
        Public LowerExtents As Vector3
        Public DoodadSetIndex As UInt16
        Public NameSetIndex As UInt32
    End Structure

    Public Structure DoodadSet
        Public name As String
        Public index As UInt32
        Public count As UInt32
    End Structure

    Public Structure Doodad
        Public ModelFile As String
        Public Position As Vector3
        Public Orientation As Quaternion
        Public Scale As Single
        Public LightingColor As Color
    End Structure

    Public Structure BoneIndices
        Public BoneIndex1 As Byte
        Public BoneIndex2 As Byte
        Public BoneIndex3 As Byte
        Public BoneIndex4 As Byte
    End Structure

    Public Structure Triangle
        Public VertexIndex1 As UInt16
        Public VertexIndex2 As UInt16
        Public VertexIndex3 As UInt16
    End Structure

    Public Structure SubMesh
        Public ID As UInt32
        Public StartVertex As UInt16
        Public nVertices As UInt16
        Public StartTriangle As UInt16
        Public nTriangles As UInt16
        Public StartBones As UInt16
        Public nBones As UInt16
        Public RootBone As UInt16
        Public Position As Vector3
    End Structure

    Public Structure TextureUnit
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

    Public Structure Vertex
        Public Position As Vector3
        Public BoneWeights As Byte()
        Public BoneIndices As Byte()
        Public Normal As Vector3
        Public TextureCoords As Vector2
    End Structure

    Public Structure M2Texture
        Public Type As UInt32
        Public Flags As UInt16
        Public lenFilename As UInt32
        Public ofsFilename As UInt32
        Public Filename As String
    End Structure

    Class HelperFunctions

        Public Function StrRev(ByVal value As String) As String
            If value.Length > 1 Then
                Dim workingValue As New System.Text.StringBuilder
                For position As Int32 = value.Length - 1 To 0 Step -1
                    workingValue.Append(value.Chars(position))
                Next
                Return workingValue.ToString
            Else
                Return value
            End If
        End Function

        Public Function GetZeroDelimitedString(ByVal Stack() As Byte, ByVal Pos As UInt32) As String
            Dim out As String = ""

            While Stack(Pos) <> 0
                out &= Chr(Stack(Pos))
                Pos += 1
            End While

            Return out.Trim
        End Function

        Public Function GetZeroDelimitedStringFromBinaryReader(ByRef br As BinaryReader, ByVal Pos As UInt32) As String
            Dim out As String = ""
            Dim c As Char

            br.BaseStream.Position = Pos
            c = br.ReadChar
            While Asc(c) <> 0
                out &= c
                c = br.ReadChar
            End While

            Return out.Trim
        End Function

        Public Function GetAllZeroDelimitedStrings(ByVal Stack() As Byte) As String()
            Dim d As String = Chr(0)
            Return Encoding.ASCII.GetString(Stack).Split(d, options:=System.StringSplitOptions.RemoveEmptyEntries)
        End Function

        Public Function GetHeightMap9x9FromHeightMap(ByVal HeightMap() As Single) As Single(,)
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

        Public Function GetHeightMap8x8FromHeightMap(ByVal HeightMap() As Single) As Single(,)
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
