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

        If chkTexturelist.Checked Then
            Lines.Clear()
            For i As Integer = 0 To _ADT.TextureFiles.Count - 1
                Lines.Add(i.ToString("000") & " " & _ADT.TextureFiles(i))
            Next
            IO.File.WriteAllLines(Directory & "/textures.txt", Lines.ToArray)
        End If

        If chkDepthmap.Checked Then myHF.DepthmapFromADT(_ADT, Directory & "/")

        For x As Integer = 0 To 15
            For y As Integer = 0 To 15
                Dim ChunkID As String = "(" & x.ToString("00") & "," & y.ToString("00") & ")"
                With _ADT.MCNKs(x, y)

                    If chkAlphamaps.Checked Then
                        For i As Integer = 0 To .AlphaMaps.Count - 1
                            If Not .AlphaMaps(i) Is Nothing Then
                                .AlphaMaps(i).Save(Directory & "/" & ChunkID & "_alpha_orig" & i.ToString("00") & ".png", Imaging.ImageFormat.Png)
                                'Using Map As Bitmap = NormalizeAlphaMap(.AlphaMaps(i))
                                'Map.Save(Directory & "/" & ChunkID & "_alpha_norm" & i.ToString("00") & ".png", ImageFormat.Png)
                                'End Using
                            End If
                        Next
                    End If

                    If chkTexturelist.Checked Then
                        Lines.Clear()
                        For i As Integer = 0 To .Layer.Count - 1
                            If i = 0 Or Not .AlphaMaps(i) Is Nothing Then Lines.Add(i.ToString("000") & " " & _ADT.TextureFiles(.Layer(i).TextureID))
                        Next
                        IO.File.WriteAllLines(Directory & "/" & ChunkID & "_layers.txt", Lines.ToArray)
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

        If chkTexturelist.Checked Then
            For i As Integer = 0 To _ADT.TextureFiles.Length - 1
                Dim TexFi As String = _ADT.TextureFiles(i)
                If myMPQ.Locate(TexFi) Then
                    Dim TexImg As Bitmap = BLP.LoadFromStream(myMPQ.LoadFile(TexFi), TexFi)
                    If Not TexImg Is Nothing Then
                        TexImg.Save(Directory & "/" & myHF.GetBaseName(TexFi) & "_orig.png", System.Drawing.Imaging.ImageFormat.Png)
                        myHF.NormalizeTexture(TexImg).Save(Directory & "/" & myHF.GetBaseName(TexFi) & "_norm.png", System.Drawing.Imaging.ImageFormat.Png)
                    End If
                End If
            Next
        End If

        Button1.Enabled = True
        Me.Close()
    End Sub

End Class
