Imports System.Windows.Forms

Public Class HexViewer

    Public FileName As String

    Private Sub HexViewer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim br As New System.IO.BinaryReader(wow2collada.myMPQ.LoadFile(FileName))

        While br.BaseStream.Position < (br.BaseStream.Length - 16)
            Dim Out As String = Hex(br.BaseStream.Position).ToString.PadLeft(8, "0") & ": "
            Dim Line As Byte() = br.ReadBytes(16)
            Dim LeftSide As String = ""
            Dim RightSide As String = ""


            For i As Integer = 0 To 15
                LeftSide &= Hex(Line(i)).ToString.PadLeft(2, "0") & " "
                If Line(i) > 31 And Line(1) < 128 Then
                    RightSide &= Chr(Line(i))
                Else
                    RightSide &= "."
                End If

                If (i + 1) Mod 4 = 0 Then
                    LeftSide &= " "
                    RightSide &= " "
                End If
            Next

            ListBox1.Items.Add(Out & LeftSide & "  " & RightSide)
        End While

    End Sub

End Class
