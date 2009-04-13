''' <summary>
''' General functions that didn't really fit anywhere else...
''' </summary>
''' <remarks></remarks>
Class HelperFunctions

    Public Textures As New System.Collections.Generic.Dictionary(Of String, sTexture)
    Public TriangleList As New System.Collections.Generic.List(Of sTriangle)

    ''' <summary>
    ''' Structure to hold Texture information (Bitmap, TextureObject and Filename)
    ''' </summary>
    ''' <remarks>The TextureObject is dependent on the Direct3D device, so if the device changes, the textures have to be recalculated!</remarks>
    Public Structure sTexture
        Dim TexGra As Bitmap
        Dim TexObj As Texture
        Dim ID As String
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
    ''' Create a dummy triangle to make sure that we have at least something to display/initiate the 3D subsystem with
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        Dim Tri As New sTriangle
        Tri.P = New sVertex() {New sVertex, New sVertex, New sVertex}
        Tri.P(0).Position = New Vector3(0, 0, 1)
        Tri.P(1).Position = New Vector3(0, 1, 0)
        Tri.P(2).Position = New Vector3(1, 0, 0)
        Tri.P(0).Normal = New Vector3(1, 1, 1)
        Tri.P(1).Normal = New Vector3(1, 1, 1)
        Tri.P(2).Normal = New Vector3(1, 1, 1)
        TriangleList.Add(Tri)
    End Sub

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

    ''' <summary>
    ''' Loads a model from the MPQ archive, can be either of the following: M2, WMO, ADT
    ''' </summary>
    ''' <param name="FileName">Name of the file to load</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadModelFromMPQ(ByVal FileName As String) As System.Collections.Generic.List(Of String)
        Dim out As New System.Collections.Generic.List(Of String)

        Select Case FileName.Substring(FileName.LastIndexOf(".") + 1).ToLower
            Case "m2", "mdx"
                'clear triangle and texture lists
                Textures.Clear()
                TriangleList.Clear()

                Dim MD20 As New FileReaders.M2
                Dim SKIN As New FileReaders.SKIN
                Dim FileNameMD20 As String = FileName.Substring(0, FileName.LastIndexOf(".")) + ".m2"
                Dim FileNameSKIN As String = FileName.Substring(0, FileName.LastIndexOf(".")) + "00.skin"
                MD20.LoadFromStream(myMPQ.LoadFile(FileNameMD20), FileNameMD20)
                SKIN.LoadFromStream(myMPQ.LoadFile(FileNameSKIN), FileNameSKIN)

                CreateVertexBufferFromM2(MD20, SKIN, New Vector3(0, 0, 0), New Quaternion(0, 0, 0, 0), 1) 'textured

                out.Add("M2 File: " & FileNameMD20)
                out.Add("M2 Model: " & MD20.ModelName)
                out.Add("M2 Version: " & MD20.VersionInfo)
                out.Add("M2 Vertices: " & MD20.Vertices.Length)

                For i As Integer = 0 To MD20.TextureLookup.Length - 1
                    out.Add(" Texture Map: " & i & " -> " & MD20.TextureLookup(i))
                Next

                out.Add("SKIN File: " & FileNameSKIN)
                out.Add("SKIN Triangles: " & SKIN.Triangles.Length)
                out.Add("SKIN Boneindices: " & SKIN.BoneIndices.Length)
                out.Add("SKIN Submeshes: " & SKIN.SubMeshes.Length)

                For i As Integer = 0 To SKIN.SubMeshes.Length - 1
                    out.Add(" Submesh " & i & " [SubID=" & SKIN.SubMeshes(i).ID & "]: Bones:" & SKIN.SubMeshes(i).nBones & ", Tris: " & SKIN.SubMeshes(i).nTriangles & ", Verts: " & SKIN.SubMeshes(i).nVertices & ")")
                Next

                For i As Integer = 0 To Textures.Count - 1
                    With Textures.ElementAt(i).Value
                        out.Add(" Texture [" & .ID & "]")
                    End With
                Next

            Case "wmo"
                Dim FileNameWMO As String = FileName
                Dim WMO As OpenWMOOptions
                WMO = New OpenWMOOptions(FileNameWMO)
                WMO.ShowDialog()

            Case "adt"
                Dim FileNameADT As String = FileName
                Dim ADT As OpenADTOptions
                ADT = New OpenADTOptions(FileNameADT)
                ADT.ShowDialog()

            Case Else
                out.Add("Unknown file format: " & FileName.Substring(FileName.LastIndexOf(".") + 1).ToLower)
        End Select

        ' tranfer the data to the 3D subsystem
        render.ResetView()
        render.SetupScene()

        Return out
    End Function

    ''' <summary>
    ''' Take a M2/SKIN filereader and turn the model into a list of vertices/textures
    ''' </summary>
    ''' <param name="MD20">The M2 filereader to use</param>
    ''' <param name="SKIN">The SKIN filereader to use</param>
    ''' <param name="Position">The position of the model (if part of a WMO)</param>
    ''' <param name="Orientation">The orientation of the model (if part of a WMO)</param>
    ''' <param name="Scale">The scale of the model (if part of a WMO)</param>
    ''' <remarks></remarks>
    Public Sub CreateVertexBufferFromM2(ByVal MD20 As FileReaders.M2, ByVal SKIN As FileReaders.SKIN, ByVal Position As Vector3, ByVal Orientation As Quaternion, ByVal Scale As Single)
        Dim BLP As New FileReaders.BLP

        'scale (done below) -> rot (done through matrix) -> trans (done below)
        Dim rMat As Matrix = Matrix.RotationQuaternion(Orientation)

        'Debug.Print(String.Format("{0} {1} {2} {3}", Orientation.X, Orientation.Y, Orientation.Z, Orientation.W))

        If Not SKIN.SubMeshes Is Nothing Then
            For i As Integer = 0 To SKIN.SubMeshes.Length - 1
                Dim TexID As Integer = -1
                For j As Integer = 0 To SKIN.TextureUnits.Length - 1
                    If SKIN.TextureUnits(j).SubmeshIndex1 = i Then TexID = MD20.TextureLookup(SKIN.TextureUnits(j).Texture)
                Next

                Dim TexFi As String = MD20.Textures(TexID).Filename
                If myMPQ.Locate(TexFi) Then
                    If Not Textures.ContainsKey(TexFi) Then
                        Dim Tex As New HelperFunctions.sTexture
                        Dim TexImg As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(TexFi), TexFi)
                        If Not TexImg Is Nothing Then
                            Tex.ID = TexFi
                            Tex.TexGra = TexImg
                            Textures(TexFi) = Tex
                        End If
                    End If

                    For j As Integer = 0 To SKIN.SubMeshes(i).nTriangles - 1
                        Dim k As Integer = SKIN.SubMeshes(i).StartTriangle + j
                        With SKIN.Triangles(k)
                            Dim Tri As New HelperFunctions.sTriangle
                            Dim V As HelperFunctions.sVertex() = New HelperFunctions.sVertex() {New HelperFunctions.sVertex, New HelperFunctions.sVertex, New HelperFunctions.sVertex}

                            For m = 0 To 2
                                V(m).Position = Vector3.TransformCoordinate(MD20.Vertices(.VertexIndex(m)).Position * Scale, rMat) + Position
                                V(m).Normal = Vector3.TransformCoordinate(MD20.Vertices(.VertexIndex(m)).Normal, rMat)
                                V(m).UV = MD20.Vertices(.VertexIndex(m)).TextureCoords
                                'Debug.Print("pre {0} {1} {2} -- pos {3} {4} {5}", MD20.Vertices(.VertexIndex(m)).Position.X, MD20.Vertices(.VertexIndex(m)).Position.Y, MD20.Vertices(.VertexIndex(m)).Position.Z, V(m).Position.X, V(m).Position.Y, V(m).Position.Z)
                            Next

                            Tri.P = V
                            Tri.TextureID = TexFi
                            TriangleList.Add(Tri)
                        End With
                    Next
                End If
            Next
        End If
    End Sub

End Class