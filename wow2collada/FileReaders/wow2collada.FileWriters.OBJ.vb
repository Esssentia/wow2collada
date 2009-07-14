Imports System
Imports System.Text
Imports System.IO

Namespace FileWriters

    Public Class OBJ

        Public Function Save(ByVal Filename As String, ByRef Models As List(Of sModel)) As Boolean
            'Save everything as OBJ...
            Dim OBJFile As String
            Dim MTLFile As String
            Dim TEXFile As String
            Dim BasePath As String
            Dim Lines As New List(Of String)
            Dim CurrIdx As Integer
            Dim CurrGrp As Integer
            Dim TextureList As New Dictionary(Of String, Integer)

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

            For h As Integer = 0 To Models.Count - 1
                For i As Integer = 0 To Models(h).Textures.Count - 1
                    With Models(h).Textures.ElementAt(i)
                        TEXFile = BasePath & "\" & myHF.StringToPureAscii(myHF.GetBaseName(.Key)) & ".png"
                        If Not TextureList.ContainsKey(TEXFile) Then
                            .Value.TextureMap.Save(TEXFile)
                            TextureList.Add(TEXFile, 1)

                            Lines.Add(String.Format("newmtl {0}", myHF.StringToPureAscii(myHF.GetBaseName(.Key))))
                            Lines.Add("Kd 1.000000 1.000000 1.000000")
                            Lines.Add("Ka 1.000000 1.000000 1.000000")
                            Lines.Add("Ks 1.000000 1.000000 1.000000")
                            Lines.Add("Ke 0.000000 0.000000 0.000000")
                            Lines.Add("Ns 0.000000")
                            Lines.Add("illum 1")
                            Lines.Add(String.Format("map_Kd {0}", TEXFile))
                            Lines.Add("")
                        End If

                    End With
                Next

                File.WriteAllLines(MTLFile, Lines.ToArray)
            Next

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
            For h As Integer = 0 To Models.Count - 1
                With Models(h)
                    For i As Integer = 0 To .Meshes.Count - 1
                        For Each triangle As sTriangle In .Meshes(i).TriangleList
                            For j As Integer = 0 To 2
                                Dim vi As Integer = triangle.Vertices(j)
                                Lines.Add(String.Format("v {0:f6} {1:f6} {2:f6}", .Vertices(vi).Position.X, .Vertices(vi).Position.Y, .Vertices(vi).Position.Z))
                            Next
                        Next

                    Next
                End With
            Next

            Lines.Add("")

            'now vertex normals
            For h As Integer = 0 To Models.Count - 1
                With Models(h)
                    For i As Integer = 0 To .Meshes.Count - 1
                        For Each triangle As sTriangle In .Meshes(i).TriangleList
                            For j As Integer = 0 To 2
                                Dim vi As Integer = triangle.Vertices(j)
                                Lines.Add(String.Format("vn {0:f6} {1:f6} {2:f6}", .Vertices(vi).Normal.X, .Vertices(vi).Normal.Y, .Vertices(vi).Normal.Z))
                            Next
                        Next
                    Next
                End With
            Next

            Lines.Add("")

            'now texture coordinates
            For h As Integer = 0 To Models.Count - 1
                With Models(h)
                    For i As Integer = 0 To .Meshes.Count - 1
                        For Each triangle As sTriangle In .Meshes(i).TriangleList
                            For j As Integer = 0 To 2
                                Dim vi As Integer = triangle.Vertices(j)
                                Lines.Add(String.Format("vt {0:f6} {1:f6}", .Vertices(vi).TextureCoords.U, 1 - .Vertices(vi).TextureCoords.V)) ' don't ask^^
                            Next
                        Next
                    Next
                End With
            Next

            Lines.Add("")

            'now triangles with groups and materials
            For h As Integer = 0 To Models.Count - 1
                For i As Integer = 0 To Models(h).Meshes.Count - 1

                    CurrGrp += 1

                    Lines.Add("")
                    Lines.Add(String.Format("g {0}", myHF.StringToPureAscii(myHF.GetBaseName(Models(h).Meshes(i).TextureList(0).TextureID))))
                    Lines.Add(String.Format("usemtl {0}", myHF.StringToPureAscii(myHF.GetBaseName(Models(h).Meshes(i).TextureList(0).TextureID))))
                    Lines.Add(String.Format("s {0}", CurrGrp))

                    For Each triangle As sTriangle In Models(h).Meshes(i).TriangleList
                        Lines.Add(String.Format("f {0:d}/{0:d}/{0:d} {1:d}/{1:d}/{1:d} {2:d}/{2:d}/{2:d}", CurrIdx * 3 + 1, CurrIdx * 3 + 2, CurrIdx * 3 + 3))
                        CurrIdx += 1
                    Next

                Next
            Next
            File.WriteAllLines(OBJFile, Lines.ToArray)

        End Function

    End Class

End Namespace

