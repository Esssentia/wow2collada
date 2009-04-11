Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports MpqReader
Imports wow2collada.wow2collada

Namespace wow2collada.FileWriters

    Class OBJ

        Public Function Save(ByVal Filename As String, ByRef Triangles As System.Collections.Generic.List(Of wow2collada.HelperFunctions.sTriangle), ByRef Textures As System.Collections.Generic.Dictionary(Of String, wow2collada.HelperFunctions.sTexture)) As Boolean
            'Save everything as OBJ...
            Dim OBJFile As String
            Dim MTLFile As String
            Dim TEXFile As String
            Dim BasePath As String
            Dim Lines As New System.Collections.Generic.List(Of String)
            Dim CurrMat As String = ""
            Dim CurrTexW As Single = 0
            Dim CurrTexH As Single = 0

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


            Lines.Clear()
            Lines.Add("# Exported from wow2collada")
            Lines.Add("")
            Lines.Add(String.Format("mtllib {0}", myHF.GetFileName(MTLFile)))
            Lines.Add("")

            'all vertices first
            For i As Integer = 0 To Triangles.Count - 1
                With Triangles(i)
                    Lines.Add(String.Format("v {0:f6} {1:f6} {2:f6}", .P1.Position.X, .P1.Position.Y, .P1.Position.Z))
                    Lines.Add(String.Format("v {0:f6} {1:f6} {2:f6}", .P2.Position.X, .P2.Position.Y, .P2.Position.Z))
                    Lines.Add(String.Format("v {0:f6} {1:f6} {2:f6}", .P3.Position.X, .P3.Position.Y, .P3.Position.Z))
                End With
            Next

            Lines.Add("")

            'now vertex normals
            For i As Integer = 0 To Triangles.Count - 1
                With Triangles(i)
                    Lines.Add(String.Format("vn {0:f6} {1:f6} {2:f6}", .P1.Normal.X, .P1.Normal.Y, .P1.Normal.Z))
                    Lines.Add(String.Format("vn {0:f6} {1:f6} {2:f6}", .P2.Normal.X, .P2.Normal.Y, .P2.Normal.Z))
                    Lines.Add(String.Format("vn {0:f6} {1:f6} {2:f6}", .P3.Normal.X, .P3.Normal.Y, .P3.Normal.Z))
                End With
            Next

            Lines.Add("")

            'now texture coordinates
            For i As Integer = 0 To Triangles.Count - 1
                With Triangles(i)
                    Lines.Add(String.Format("vt {0:f6} {1:f6}", .P1.UV.X, 1 - .P1.UV.Y))
                    Lines.Add(String.Format("vt {0:f6} {1:f6}", .P2.UV.X, 1 - .P2.UV.Y))
                    Lines.Add(String.Format("vt {0:f6} {1:f6}", .P3.UV.X, 1 - .P3.UV.Y))
                End With
            Next

            Lines.Add("")

            'now triangles with groups and materials
            For i As Integer = 0 To Triangles.Count - 1
                With Triangles(i)
                    If myHF.GetBaseName(.TextureID) <> CurrMat Then
                        Lines.Add("")
                        Lines.Add(String.Format("g {0}", myHF.StringToPureAscii(myHF.GetBaseName(.TextureID))))
                        Lines.Add(String.Format("usemtl {0}", myHF.StringToPureAscii(myHF.GetBaseName(.TextureID))))
                        Lines.Add("s 1")
                        CurrMat = myHF.GetBaseName(.TextureID)
                    End If
                    Lines.Add(String.Format("f {0:d}/{0:d}/{0:d} {1:d}/{1:d}/{1:d} {2:d}/{2:d}/{2:d}", (i * 3 + 1), (i * 3 + 2), (i * 3 + 3)))
                End With
            Next

            File.WriteAllLines(OBJFile, Lines.ToArray)

        End Function

    End Class

End Namespace

