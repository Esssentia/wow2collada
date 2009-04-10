Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports MpqReader

Namespace wow2collada.FileWriters

    Class OBJ

        Public Sub Save(ByVal Filename As String, ByVal Triangles As System.Collections.Generic.List(Of wow2collada.HelperFunctions.sTriangle), ByVal Textures As System.Collections.Generic.Dictionary(Of String, wow2collada.HelperFunctions.sTexture))
            'Save everything as OBJ...

            '# comment
            'v -1.00 -1.00 1.00
            'v 1.00 -1.00 1.00
            'v 1.00 1.00 1.00
            '
            'f 1 3 4
            'f 4 2 1
            'f 5 6 8
            'f 8 7 5

            'Dim sw As New StringWriter(File.OpenWrite(Filename))
            Dim s(Triangles.Count * 10) As String


            s(0) = "# Exported from wow2collada"
            For i As Integer = 0 To Triangles.Count - 1
                With Triangles(i)
                    s(i * 10 + 1) = String.Format("v  {0:f6} {1:f6} {2:f6}", .P1.Position.X, .P1.Position.Y, .P1.Position.Z)
                    s(i * 10 + 2) = String.Format("vn {0:f6} {1:f6} {2:f6}", .P1.Normal.X, .P1.Normal.Y, .P1.Normal.Z)
                    s(i * 10 + 3) = String.Format("vt {0:f6} {1:f6}", .P1.UV.X, .P1.UV.Y)

                    s(i * 10 + 4) = String.Format("v  {0:f6} {1:f6} {2:f6}", .P2.Position.X, .P2.Position.Y, .P2.Position.Z)
                    s(i * 10 + 5) = String.Format("vn {0:f6} {1:f6} {2:f6}", .P2.Normal.X, .P2.Normal.Y, .P2.Normal.Z)
                    s(i * 10 + 6) = String.Format("vt {0:f6} {1:f6}", .P2.UV.X, .P2.UV.Y)

                    s(i * 10 + 7) = String.Format("v  {0:f6} {1:f6} {2:f6}", .P3.Position.X, .P3.Position.Y, .P3.Position.Z)
                    s(i * 10 + 8) = String.Format("vn {0:f6} {1:f6} {2:f6}", .P3.Normal.X, .P3.Normal.Y, .P3.Normal.Z)
                    s(i * 10 + 9) = String.Format("vt {0:f6} {1:f6}", .P3.UV.X, .P3.UV.Y)

                    s(i * 10 + 10) = String.Format("f  {0:d} {1:d} {2:d}", (i + 1), (i + 2), (i + 3))
                End With
            Next

            File.WriteAllLines(Filename, s)

        End Sub

    End Class

End Namespace

