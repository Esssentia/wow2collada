Imports System
Imports System.Text
Imports System.IO

Namespace FileWriters

    Class OBJ

        Public Function Save(ByVal Filename As String, ByRef Triangles As List(Of HelperFunctions.sTriangle), ByRef Textures As Dictionary(Of String, HelperFunctions.sTexture)) As Boolean
            'Save everything as OBJ...
            Dim OBJFile As String
            Dim MTLFile As String
            Dim TEXFile As String
            Dim BasePath As String
            Dim Lines As New List(Of String)
            Dim CurrMat As String = ""
            Dim GroupNumber As Integer = 1

            BasePath = myHF.GetBasePath(Filename)
            OBJFile = BasePath & "\" & myHF.GetBaseName(Filename) & ".obj"
            MTLFile = BasePath & "\" & myHF.GetBaseName(Filename) & ".mtl"

            'Create MTL File

            '# comment
            '
            'newmtl(MAT_NAME)
            'illum 2
            'Kd 1.000000 1.000000 1.000000
            'Ka 1.000000 1.000000 1.000000
            'Ks 1.000000 1.000000 1.000000
            'Ke 0.000000 0.000000 0.000000
            'Ns 140.625
            'map_Kd MAT_FILE
            '
            '...

            Lines.Clear()
            Lines.Add("# Exported from wow2collada")
            Lines.Add("")

            For i As Integer = 0 To Textures.Count - 1
                With (Textures.ElementAt(i).Value)

                    TEXFile = BasePath & "\" & myHF.StringToPureAscii(myHF.GetBaseName(.FileName)) & ".png"
                    .TexGra.Save(TEXFile)

                    Lines.Add(String.Format("newmtl {0}", myHF.StringToPureAscii(myHF.GetBaseName(.FileName))))
                    Lines.Add("Kd 1.000000 1.000000 1.000000")
                    Lines.Add("Ka 1.000000 1.000000 1.000000")
                    Lines.Add("Ks 1.000000 1.000000 1.000000")
                    Lines.Add("Ke 0.000000 0.000000 0.000000")
                    Lines.Add("Ns 0.000000")
                    Lines.Add("illum 1")
                    Lines.Add(String.Format("map_Kd {0}", TEXFile))
                    Lines.Add("")
                End With
            Next

            File.WriteAllLines(MTLFile, Lines.ToArray)


            'Create OBJ File

            '# comment
            '
            'mtllib MTL_FILE_NAME
            '
            'g GROUP_NAME
            'usemtl MAT_NAME
            'v -1.00 -1.00 1.00
            'v 1.00 -1.00 1.00
            'v 1.00 1.00 1.00
            'f 1 3 4
            'f 4 2 1
            'f 5 6 8
            'f 8 7 5
            '
            '...


            'tried to get rid of duplicate vertices, normals and UVs with the use of dictionaries (getting a list of unique vertices/normals/..)
            'but the performance was extremely crappy...
            'probably have to find some clever way to avoid all those dictionary lookups
            '
            ' Filling the lists like this is very slow: if not x.containskey(y) then x.add(y)
            ' Reading the lists back like this is very slow as well: x.elementat(i)
            '
            'there has to be a better way, but I haven't found it yet...

            Lines.Clear()
            Lines.Add("# Exported from wow2collada")
            Lines.Add("")
            Lines.Add(String.Format("mtllib {0}", myHF.GetFileName(MTLFile)))
            Lines.Add("")

            'all vertices first
            For i As Integer = 0 To Triangles.Count - 1
                For j As Integer = 0 To 2
                    Lines.Add(String.Format("v {0:f6} {1:f6} {2:f6}", Triangles(i).P(j).Position.X, Triangles(i).P(j).Position.Y, Triangles(i).P(j).Position.Z))
                Next
            Next

            Lines.Add("")

            'now vertex normals
            For i As Integer = 0 To Triangles.Count - 1
                For j As Integer = 0 To 2
                    Lines.Add(String.Format("vn {0:f6} {1:f6} {2:f6}", Triangles(i).P(j).Normal.X, Triangles(i).P(j).Normal.Y, Triangles(i).P(j).Normal.Z))
                Next
            Next

            Lines.Add("")

            'now texture coordinates
            For i As Integer = 0 To Triangles.Count - 1
                For j As Integer = 0 To 2
                    Lines.Add(String.Format("vt {0:f6} {1:f6}", Triangles(i).P(j).UV.X, 1 - Triangles(i).P(j).UV.Y)) ' don't ask^^
                Next
            Next

            Lines.Add("")

            'now triangles with groups and materials
            For i As Integer = 0 To Triangles.Count - 1
                With Triangles(i)
                    If myHF.GetBaseName(.TextureID) <> CurrMat Then
                        Lines.Add("")
                        Lines.Add(String.Format("g {0}", myHF.StringToPureAscii(myHF.GetBaseName(.TextureID))))
                        Lines.Add(String.Format("usemtl {0}", myHF.StringToPureAscii(myHF.GetBaseName(.TextureID))))
                        Lines.Add(String.Format("s {0}", GroupNumber))
                        CurrMat = myHF.GetBaseName(.TextureID)
                        GroupNumber += 1
                    End If

                    Lines.Add(String.Format("f {0:d}/{0:d}/{0:d} {1:d}/{1:d}/{1:d} {2:d}/{2:d}/{2:d}", i * 3 + 1, i * 3 + 2, i * 3 + 3))
                End With
            Next

            File.WriteAllLines(OBJFile, Lines.ToArray)

        End Function

    End Class

End Namespace

