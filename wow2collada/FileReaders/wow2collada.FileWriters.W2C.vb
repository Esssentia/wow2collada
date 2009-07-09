Imports System
Imports System.Text
Imports System.IO

Namespace FileWriters

    Class W2C

        Public Function Save(ByVal Filename As String, ByRef Models As List(Of sModel)) As Boolean
            'Save everything as W2C (wow2collada intermediate format for blender import)
            Dim W2CFile As String
            Dim BasePath As String
            Dim Lines As New List(Of String)
            Dim TextureList As New Dictionary(Of String, Integer)
            Dim MatIdx As Integer = 0
            Dim MesIdx As Integer = 0

            BasePath = myHF.GetBasePath(Filename)
            W2CFile = BasePath & "\" & myHF.GetBaseName(Filename) & ".w2c"

            Lines.Clear()
            'format:
            '
            'material 1 t1 (material number, texturefilename)
            'material 2 t2
            '...
            'mesh 1 1 b1 b2 (mesh number, material number, b1/b2 = blending flags)
            '[
            'px1 py1 pz1 nx1 ny1 nz1 u1 v1 px2 py2 pz2 nx2 ny2 nz2 u2 v2 px3 py3 pz3 nx3 ny3 nz3 u3 v3 (first triangle -> position, normal, UV for each of the three vertices)
            '...
            ']
            'mesh 2 x b1 b2
            '[
            '...
            ']
            '...


            'materials
            For h As Integer = 0 To Models.Count - 1
                For i As Integer = 0 To Models(h).Textures.Count - 1
                    With Models(h).Textures.ElementAt(i)
                        Dim TEXFile As String = myHF.StringToPureAscii(myHF.GetBaseName(.Key))
                        If Not TextureList.ContainsKey(TEXFile) Then
                            .Value.TextureMap.Save(BasePath & "\" & TEXFile & ".png")
                            MatIdx += 1
                            TextureList.Add(TEXFile, MatIdx)
                            Lines.Add(String.Format("mat {0} {1}", MatIdx, BasePath & "\" & TEXFile & ".png"))
                        End If

                    End With
                Next
            Next
            
            'submeshes:
            For h As Integer = 0 To Models.Count - 1
                With Models(h)
                    For i As Integer = 0 To Models(h).Meshes.Count - 1
                        MesIdx += 1
                        Dim MatID As Integer = TextureList(myHF.StringToPureAscii(myHF.GetBaseName(.Meshes(i).TextureList(0).TextureID)))
                        Dim B1 As Integer = .Meshes(i).TextureList(0).Blending1
                        Dim B2 As Integer = .Meshes(i).TextureList(0).Blending2
                        Lines.Add(String.Format("mesh {0} {1} {2} {3}", MesIdx, MatID, B1, B2))
                        For Each triangle As sTriangle In .Meshes(i).TriangleList
                            Lines.Add(String.Format("t {0:f6} {1:f6} {2:f6} {3:f6} {4:f6} {5:f6} {6:f6} {7:f6} {8:f6} {9:f6} {10:f6} {11:f6} {12:f6} {13:f6} {14:f6} {15:f6} {16:f6} {17:f6} {18:f6} {19:f6} {20:f6} {21:f6} {22:f6} {23:f6}", _
                                .Vertices(triangle.V1).Position.X, .Vertices(triangle.V1).Position.Y, .Vertices(triangle.V1).Position.Z, .Vertices(triangle.V1).Normal.X, .Vertices(triangle.V1).Normal.Y, .Vertices(triangle.V1).Normal.Z, .Vertices(triangle.V1).TextureCoords.U, .Vertices(triangle.V1).TextureCoords.V, _
                                .Vertices(triangle.V2).Position.X, .Vertices(triangle.V2).Position.Y, .Vertices(triangle.V2).Position.Z, .Vertices(triangle.V2).Normal.X, .Vertices(triangle.V2).Normal.Y, .Vertices(triangle.V2).Normal.Z, .Vertices(triangle.V2).TextureCoords.U, .Vertices(triangle.V2).TextureCoords.V, _
                                .Vertices(triangle.V3).Position.X, .Vertices(triangle.V3).Position.Y, .Vertices(triangle.V3).Position.Z, .Vertices(triangle.V3).Normal.X, .Vertices(triangle.V3).Normal.Y, .Vertices(triangle.V3).Normal.Z, .Vertices(triangle.V3).TextureCoords.U, .Vertices(triangle.V3).TextureCoords.V))
                        Next
                    Next
                End With
            Next

            File.WriteAllLines(W2CFile, Lines.ToArray)

        End Function

    End Class

End Namespace

