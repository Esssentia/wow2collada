Imports System
Imports System.Text
Imports System.IO

Namespace FileWriters

    Class OBJ

        Public Function Save(ByVal Filename As String, ByRef SubMeshes As List(Of HelperFunctions.sSubMesh), ByRef Textures As Dictionary(Of String, HelperFunctions.sTexture)) As Boolean
            'Save everything as OBJ...
            Dim OBJFile As String
            Dim MTLFile As String
            Dim TEXFile As String
            Dim BasePath As String
            Dim Lines As New List(Of String)
            Dim CurrIdx As Integer
            Dim CurrGrp As Integer

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

                    TEXFile = BasePath & "\" & myHF.StringToPureAscii(myHF.GetBaseName(.ID)) & ".png"
                    .TexGra.Save(TEXFile)

                    Lines.Add(String.Format("newmtl {0}", myHF.StringToPureAscii(myHF.GetBaseName(.ID))))
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
            For Each submesh As HelperFunctions.sSubMesh In SubMeshes
                For Each triangle As HelperFunctions.sTriangle In submesh.TriangleList
                    For j As Integer = 0 To 2
                        Lines.Add(String.Format("v {0:f6} {1:f6} {2:f6}", triangle.P(j).Position.X, triangle.P(j).Position.Y, triangle.P(j).Position.Z))
                    Next
                Next
            Next

            Lines.Add("")

            'now vertex normals
            For Each submesh As HelperFunctions.sSubMesh In SubMeshes
                For Each triangle As HelperFunctions.sTriangle In submesh.TriangleList
                    For j As Integer = 0 To 2
                        Lines.Add(String.Format("vn {0:f6} {1:f6} {2:f6}", triangle.P(j).Normal.X, triangle.P(j).Normal.Y, triangle.P(j).Normal.Z))
                    Next
                Next
            Next

            Lines.Add("")

            'now texture coordinates
            For Each submesh As HelperFunctions.sSubMesh In SubMeshes
                For Each triangle As HelperFunctions.sTriangle In submesh.TriangleList
                    For j As Integer = 0 To 2
                        Lines.Add(String.Format("vt {0:f6} {1:f6}", triangle.P(j).UV.X, 1 - triangle.P(j).UV.Y)) ' don't ask^^
                    Next
                Next
            Next

            Lines.Add("")

            'now triangles with groups and materials
            For Each submesh As HelperFunctions.sSubMesh In SubMeshes

                CurrGrp += 1

                Lines.Add("")
                Lines.Add(String.Format("g {0}", myHF.StringToPureAscii(myHF.GetBaseName(submesh.TextureList(0).TextureID))))
                Lines.Add(String.Format("usemtl {0}", myHF.StringToPureAscii(myHF.GetBaseName(submesh.TextureList(0).TextureID))))
                Lines.Add(String.Format("s {0}", CurrGrp))

                For Each triangle As HelperFunctions.sTriangle In submesh.TriangleList
                    CurrIdx += 1
                    Lines.Add(String.Format("f {0:d}/{0:d}/{0:d} {1:d}/{1:d}/{1:d} {2:d}/{2:d}/{2:d}", CurrIdx * 3, CurrIdx * 3 + 1, CurrIdx * 3 + 2))
                Next

            Next
            File.WriteAllLines(OBJFile, Lines.ToArray)

        End Function

    End Class

End Namespace

