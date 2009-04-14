Imports System.Windows.Forms

Public Class HexViewer

    Private _FileName As String

    Sub New(ByVal FileName As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _FileName = FileName

    End Sub

    Private Sub HexViewer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim br As New System.IO.BinaryReader(myMPQ.LoadFile(_FileName))
        Dim FileProv As New Be.Windows.Forms.DynamicByteProvider(br.ReadBytes(br.BaseStream.Length))
        ToolStripStatusLabel1.Text = _FileName
        HexBox1.ByteProvider = FileProv
        HexBox1.BytesPerLine = 16
        HexBox1.StringViewVisible = True
        HexBox1.UseFixedBytesPerLine = True
    End Sub

    Private Sub GotoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GotoToolStripMenuItem.Click
        Dim f As New HexViewerGoto
        f.ShowDialog()
        Dim s As String = f.Value
        Dim h As Integer = Val("&H" & s)
        HexBox1.ScrollByteIntoView(h)
    End Sub

    Private Sub FindToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FindToolStripMenuItem1.Click
        Dim f As New HexViewerFind
        f.ShowDialog()
        HexBox1.Find(f.Value, 0)

    End Sub
End Class
