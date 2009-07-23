Imports System.Windows.Forms


Public Class DumpADT
    Private _Filename As String
    Private _ADT As FileReaders.ADT

    Public Property Filename() As String
        Get
            Return _Filename
        End Get
        Set(ByVal value As String)
            _Filename = value
        End Set
    End Property

    Public Property ADT() As FileReaders.ADT
        Get
            Return _ADT
        End Get
        Set(ByVal value As FileReaders.ADT)
            _ADT = value
        End Set
    End Property

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim BLP As New FileReaders.BLP
        Dim Directory As String = myHF.GetBasePath(_Filename)
        Dim Lines As New List(Of String)

        Button1.Enabled = False

        ToolStripProgressBar1.Minimum = 0
        ToolStripProgressBar1.Maximum = 256
        ToolStripProgressBar1.Value = 0

        If chkXML.Checked Then
            Dim xmlfile As System.Xml.XmlWriter = Xml.XmlWriter.Create(Directory & "/adt.xml")

            xmlfile.WriteStartDocument(True)
            xmlfile.WriteStartElement("adt")

            'global stuff
            '--------------

            'texture list
            xmlfile.WriteStartElement("textures")
            For i As Integer = 0 To _ADT.TextureFiles.Count - 1
                xmlfile.WriteStartElement("texture")
                xmlfile.WriteAttributeString("id", i)
                xmlfile.WriteValue(_ADT.TextureFiles(i))
                xmlfile.WriteEndElement()
            Next
            xmlfile.WriteEndElement()

            'chunk wide stuff
            '----------------

            For tx As Integer = 0 To 15
                For ty As Integer = 0 To 15
                    With _ADT.MCNKs(tx, ty)
                        xmlfile.WriteStartElement("tile")
                        xmlfile.WriteAttributeString("x", tx)
                        xmlfile.WriteAttributeString("y", ty)

                        'depthmap
                        xmlfile.WriteStartElement("depthmap")
                        For i As Integer = 0 To 8
                            For j As Integer = 0 To 8
                                xmlfile.WriteStartElement("point")
                                xmlfile.WriteAttributeString("x", i)
                                xmlfile.WriteAttributeString("y", j)
                                xmlfile.WriteAttributeString("z", .HeightMap9x9(i, j) + .Position.Z)
                                xmlfile.WriteEndElement()
                                If i < 8 And j < 8 Then
                                    xmlfile.WriteStartElement("point")
                                    xmlfile.WriteAttributeString("x", i + 0.5)
                                    xmlfile.WriteAttributeString("y", j + 0.5)
                                    xmlfile.WriteAttributeString("z", .HeightMap8x8(i, j) + .Position.Z)
                                    xmlfile.WriteEndElement()
                                End If
                            Next
                        Next
                        xmlfile.WriteEndElement()

                        'tile texture info
                        xmlfile.WriteStartElement("textures")
                        For i As Integer = 0 To .Layer.Count - 1
                            If i = 0 Or Not .AlphaMaps(i) Is Nothing Then
                                xmlfile.WriteStartElement("texture")
                                xmlfile.WriteAttributeString("layer", i)
                                xmlfile.WriteAttributeString("id", .Layer(i).TextureID)
                                xmlfile.WriteValue(_ADT.TextureFiles(.Layer(i).TextureID))
                                xmlfile.WriteEndElement()
                            End If
                        Next
                        xmlfile.WriteEndElement()


                        xmlfile.WriteEndElement()
                    End With
                Next
            Next

            xmlfile.WriteEndDocument()
            xmlfile.Close()
        End If

        If chkDepthmap.Checked Then myHF.DepthmapFromADT(_ADT, Directory & "/")

        For x As Integer = 0 To 15
            For y As Integer = 0 To 15
                Dim ChunkID As String = "(" & x.ToString("00") & "," & y.ToString("00") & ")"
                With _ADT.MCNKs(x, y)

                    If chkAlphamaps.Checked Then
                        For i As Integer = 0 To .AlphaMaps.Count - 1
                            If Not .AlphaMaps(i) Is Nothing Then
                                .AlphaMaps(i).Save(Directory & "/" & ChunkID & "_alpha" & i.ToString("00") & ".png", Imaging.ImageFormat.Png)
                            End If
                        Next
                    End If

                    Application.DoEvents()
                    ToolStripProgressBar1.Value = x * 16 + y

                    If chkCombined.Checked Then
                        'Dim timer As Long = Now.Ticks
                        Using Map As Bitmap = myHF.Blend(_ADT, x, y, chkLayermaps.Checked, chkAlphamaps.Checked, Directory & "/" & ChunkID)
                            Map.Save(Directory & "/" & ChunkID & "_combined.png", Imaging.ImageFormat.Png)
                        End Using
                        'Debug.Print("(" & x.ToString("00") & "/" & y.ToString("00") & "): " & CSng(Now.Ticks - timer) / 10000000)
                    End If

                End With
            Next
        Next

        If chkTextures.Checked Then
            For i As Integer = 0 To _ADT.TextureFiles.Length - 1
                Dim TexFi As String = _ADT.TextureFiles(i)
                If myMPQ.Locate(TexFi) Then
                    Dim TexImg As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(TexFi), TexFi)
                    If Not TexImg Is Nothing Then
                        TexImg.Save(Directory & "/" & myHF.GetBaseName(TexFi) & ".png", System.Drawing.Imaging.ImageFormat.Png)
                        'myHF.NormalizeTexture(TexImg).Save(Directory & "/" & myHF.GetBaseName(TexFi) & "_norm.png", System.Drawing.Imaging.ImageFormat.Png)
                    End If
                End If
            Next
        End If

        Button1.Enabled = True
        Me.Close()
    End Sub

End Class
