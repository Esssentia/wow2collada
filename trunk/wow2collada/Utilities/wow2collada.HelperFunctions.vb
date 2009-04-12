''' <summary>
''' General functions that didn't really fit anywhere else...
''' </summary>
''' <remarks></remarks>
Class HelperFunctions

    Public m_Textures As New System.Collections.Generic.Dictionary(Of String, sTexture)
    Public m_TriangleList As New System.Collections.Generic.List(Of sTriangle)

    ''' <summary>
    ''' Structure to hold Texture information (Bitmap, TextureObject and Filename)
    ''' </summary>
    ''' <remarks>The TextureObject is dependent on the Direct3D device, so if the device changes, the textures have to be recalculated!</remarks>
    Public Structure sTexture
        Dim TexGra As Bitmap
        Dim TexObj As Texture
        Dim FileName As String
    End Structure

    ''' <summary>
    ''' Structure to hold Vertex information (Position, Normal, UV Coordinates)
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure sVertex
        Dim Position As Vector3
        Dim Normal As Vector3
        Dim UV As Vector2
    End Structure

    ''' <summary>
    ''' Structure to hold Triangle information (3 vertices and 1 texture id)
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure sTriangle
        Dim TextureID As String
        Dim P() As sVertex
    End Structure

    ''' <summary>
    ''' Reverses a string (i.e. ABC -> CBA)
    ''' </summary>
    ''' <param name="value">The string to reverse</param>
    ''' <returns>The reversed string</returns>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' Get a zero delimited string starting at a specified position from an array of bytes
    ''' </summary>
    ''' <param name="Stack">The array of bytes to look in</param>
    ''' <param name="Pos">The position with the array to start looking</param>
    ''' <returns>The string found</returns>
    ''' <remarks></remarks>
    Public Function GetZeroDelimitedString(ByVal Stack() As Byte, ByVal Pos As UInt32) As String
        Dim out As String = ""

        While Stack(Pos) <> 0
            out &= Chr(Stack(Pos))
            Pos += 1
        End While

        Return out.Trim
    End Function

    ''' <summary>
    ''' Returns a zero delimited string starting at a specified position from a binary reader. 
    ''' </summary>
    ''' <param name="br">The binary reader to use as the source</param>
    ''' <param name="Pos">The position to start the string at</param>
    ''' <returns>The extracted string</returns>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' Get all zero delimited string from an array of bytes. The amount of zero chars between strings do not matter.
    ''' </summary>
    ''' <param name="Stack">Array of bytes containing the data</param>
    ''' <returns>Array of strings containing the strings found</returns>
    ''' <remarks></remarks>
    Public Function GetAllZeroDelimitedStrings(ByVal Stack() As Byte) As String()
        Dim d As String = Chr(0)
        Return System.Text.Encoding.ASCII.GetString(Stack).Split(d, options:=System.StringSplitOptions.RemoveEmptyEntries)
    End Function

    ''' <summary>
    ''' Get the extension of a filename (i.e. d:\this\is\a\test.doc -> doc)
    ''' </summary>
    ''' <param name="Filename">The filename to get the extension from</param>
    ''' <returns>Extension (i.e. "doc") without a leading .</returns>
    ''' <remarks></remarks>
    Public Function GetExtension(ByVal Filename As String) As String
        Dim dotPos As Integer = Filename.LastIndexOf(".")
        If dotPos >= 0 Then Return Filename.Substring(dotPos + 1)
        Return ""
    End Function

    ''' <summary>
    ''' Get the basename of a filename (i.e. d:\this\is\a\test.doc -> test)
    ''' </summary>
    ''' <param name="Filename">The filename to get the basename from</param>
    ''' <returns>Basename (i.e. "test") without a leading \ or trailing .</returns>
    ''' <remarks></remarks>
    Public Function GetBaseName(ByVal Filename As String)
        Dim dotPos As Integer = Filename.LastIndexOf(".")
        If dotPos >= 0 Then Filename = Filename.Substring(0, dotPos)

        Dim slashPos As Integer = Filename.LastIndexOf("\")
        If slashPos >= 0 Then Filename = Filename.Substring(slashPos + 1)
        Return Filename
    End Function

    ''' <summary>
    ''' Get the basepath of a filename (i.e. d:\this\is\a\test.doc -> d:\this\is\a)
    ''' </summary>
    ''' <param name="Filename">The filename to get the basepath from</param>
    ''' <returns>Basepath (i.e. "d:\this\is\a") without a trailing \</returns>
    ''' <remarks></remarks>
    Public Function GetBasePath(ByVal Filename As String)
        Dim slashPos As Integer = Filename.LastIndexOf("\")
        If slashPos >= 0 Then Return Filename.Substring(0, slashPos)
        Return ""
    End Function

    ''' <summary>
    ''' Get the filename component of a filename (i.e. d:\this\is\a\test.doc -> test.doc)
    ''' </summary>
    ''' <param name="Filename">The filename to get the filename component from</param>
    ''' <returns>Filename component (i.e. "test.doc") without a leading \</returns>
    ''' <remarks></remarks>
    Public Function GetFileName(ByVal Filename As String)
        Return GetBaseName(Filename) & "." & GetExtension(Filename)
    End Function

    ''' <summary>
    ''' Returns a text with only a-z, A-Z and 0-0 characters in it, all other characters are removed (i.e. This_is-a%test 98 -> Thisisatest98)
    ''' </summary>
    ''' <param name="Text">The text to clean up</param>
    ''' <returns>Cleaned up text (i.e. "i.e. Thisisatest98test")</returns>
    ''' <remarks></remarks>
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