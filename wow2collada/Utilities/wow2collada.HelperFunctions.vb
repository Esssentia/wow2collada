Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Namespace wow2collada

    Class HelperFunctions

        Public Structure sTexture
            Dim TexGra As Bitmap
            Dim TexObj As Texture
            Dim FileName As String
        End Structure

        Public Structure sVertex
            Dim Position As Vector3
            Dim Normal As Vector3
            Dim UV As Vector2
        End Structure

        Public Structure sTriangle
            Dim TextureID As String
            Dim P1 As sVertex
            Dim P2 As sVertex
            Dim P3 As sVertex
        End Structure

        Public m_Textures As New System.Collections.Generic.Dictionary(Of String, sTexture)
        Public m_TriangleList As New System.Collections.Generic.List(Of sTriangle)

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

        Public Function GetZeroDelimitedStringFromBinaryReader(ByRef br As System.IO.BinaryReader, ByVal Pos As UInt32) As String
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
            Return System.Text.Encoding.ASCII.GetString(Stack).Split(d, options:=System.StringSplitOptions.RemoveEmptyEntries)
        End Function

        Public Function GetExtension(ByVal Filename As String)
            Dim dotPos As Integer = Filename.LastIndexOf(".")
            If dotPos >= 0 Then Return Filename.Substring(dotPos + 1)
            Return ""
        End Function

        Public Function GetBaseName(ByVal Filename As String)
            Dim dotPos As Integer = filename.LastIndexOf(".")
            If dotPos >= 0 Then Filename = Filename.Substring(0, dotPos)

            Dim slashPos As Integer = filename.LastIndexOf("\")
            If slashPos >= 0 Then Filename = Filename.Substring(slashPos + 1)
            Return Filename
        End Function

        Public Function GetBasePath(ByVal Filename As String)
            Dim slashPos As Integer = Filename.LastIndexOf("\")
            If slashPos >= 0 Then Return Filename.Substring(0, slashPos)
            Return ""
        End Function

        Public Function GetFileName(ByVal Filename As String)
            Return GetBaseName(Filename) & "." & GetExtension(Filename)
        End Function

        Public Function StringToPureAscii(ByVal Text As String)
            Dim Out As String = ""
            Dim CE As CharEnumerator = Text.GetEnumerator()
            Dim CleanChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

            While CE.MoveNext()
                If CleanChars.IndexOf(CE.Current) <> -1 Then
                    Out &= CE.Current
                End If
            End While

            Return Out
        End Function
    End Class

End Namespace
