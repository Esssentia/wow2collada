Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports MpqReader

Namespace wow2collada.FileReaders

    Public Class Node
        Dim _Data As String
        Dim _Nodes As System.Collections.Generic.Dictionary(Of String, wow2collada.FileReaders.Node)
        Dim _Parent As wow2collada.FileReaders.Node

        Public Sub New(ByVal parent As wow2collada.FileReaders.Node, ByVal text As String)
            _Parent = parent
            _Data = text
        End Sub

        Public Function Add(ByVal text As String)
            If _Nodes Is Nothing Then ' no children yet
                Dim NewNode As wow2collada.FileReaders.Node = New wow2collada.FileReaders.Node(Me, text)
                _Nodes = New System.Collections.Generic.Dictionary(Of String, wow2collada.FileReaders.Node)
                _Nodes.Add(text, NewNode)
                Return NewNode
            ElseIf Not _Nodes.ContainsKey(text) Then ' children but not found
                Dim NewNode As wow2collada.FileReaders.Node = New wow2collada.FileReaders.Node(Me, text)
                '_Nodes = New System.Collections.Generic.Dictionary(Of String, wow2collada.Node)
                _Nodes.Add(text, NewNode)
                Return NewNode
            Else
                Return _Nodes(text)
            End If
        End Function

        Public Function Nodes() As System.Collections.Generic.Dictionary(Of String, wow2collada.FileReaders.Node)
            Return _Nodes
        End Function

        Public Function Data() As String
            Return _Data
        End Function

    End Class

    Class MPQ

        ' All C# MPQ Functions are
        ' - (C) 2006 Weichhold (oliver@weichhold.com)
        ' - Uses code from the SCSharp project (http://scsharp.hungry.com/)
        ' - Uses SharpZipLib from IC#Code (http://www.icsharpcode.net/OpenSource/SharpZipLib/)

        Public Structure FileListEntry
            Dim Archive As String
            Dim Path As String
            Dim Size As Integer
        End Structure

        Private _KnownMPQ As String() 'list of mpq's relative to the DATA directory 
        Private _BasePath As String
        Public FileList As New System.Collections.Generic.Dictionary(Of String, FileListEntry)
        Public FileTree As New wow2collada.FileReaders.Node(Nothing, "ROOT")

        Public Sub New()
            Dim regKey1 As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Blizzard Entertainment\World of Warcraft")
            Dim regKey2 As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\Blizzard Entertainment\World of Warcraft")

            Dim p1 As String = regKey1.GetValue("InstallPath")
            Dim p2 As String = regKey2.GetValue("InstallPath")
            If p2 > "" Then
                _BasePath = p2 & "data\"
            Else
                _BasePath = p1 & "data\"
            End If
            ' manual override because I have several WOW installations with different patchlevels
            _BasePath = "d:\temp\data\"

            ReDim _KnownMPQ(15) ' I don't want to iterate through the directory reading arbitrary MPQs... (for now)
            _KnownMPQ(0) = "common.mpq"
            _KnownMPQ(1) = "common-2.mpq"
            _KnownMPQ(2) = "patch.mpq"
            _KnownMPQ(3) = "patch-2.mpq"
            _KnownMPQ(4) = "expansion.mpq"
            _KnownMPQ(5) = "lichking.mpq"
            _KnownMPQ(6) = "deDE\backup-deDE.mpq"
            _KnownMPQ(7) = "deDE\base-deDE.mpq"
            _KnownMPQ(8) = "deDE\expansion-locale-deDE.mpq"
            _KnownMPQ(9) = "deDE\expansion-speech-deDE.mpq"
            _KnownMPQ(10) = "deDE\lichking-locale-deDE.mpq"
            _KnownMPQ(11) = "deDE\lichking-speech-deDE.mpq"
            _KnownMPQ(12) = "deDE\locale-deDE.mpq"
            _KnownMPQ(13) = "deDE\patch-deDE.mpq"
            _KnownMPQ(14) = "deDE\patch-deDE-2.mpq"
            _KnownMPQ(15) = "deDE\speech-deDE.mpq"

        End Sub

        Public Sub GenerateFileList()
            Dim mainProz As Single
            Dim subProz As Single
            Dim subTick As Single

            subTick = 99 / _KnownMPQ.Length

            For i As Integer = 0 To _KnownMPQ.Length - 1
                Dim archiveFile As String = _BasePath & _KnownMPQ(i)
                Dim archive As MpqArchive = New MpqArchive(archiveFile)

                mainProz = i * subTick

                For j As Integer = 0 To archive.Files.Length - 1
                    subProz = j / archive.Files.Length * subTick
                    abo.UpdateProgress(mainProz + subProz)
                    Application.DoEvents()
                    Dim a As FileListEntry
                    a.Size = archive.Files(j).UncompressedSize
                    a.Archive = archiveFile
                    a.Path = archive.Files(j).Name.ToLower
                    FileList(a.Path) = a

                    If a.Path.LastIndexOf(".") > 0 Then
                        If a.Path.IndexOf("world of warcraft launcher.app") <> 0 And a.Path.IndexOf("world of warcraft.app") <> 0 And a.Path.IndexOf("background downloader.app") <> 0 Then
                            Select Case a.Path.Substring(a.Path.LastIndexOf("."))
                                Case ".m2", ".adt", ".wmo"
                                    Dim Parts As String() = a.Path.Split("\")
                                    Dim parent As wow2collada.FileReaders.Node = FileTree
                                    For k As Integer = 0 To Parts.Count - 1
                                        parent = parent.Add(Parts(k))
                                    Next
                                Case ".anim", ".skin", ".lua", ".xml", ".sig", ".txt", ".exe", ".toc", ".zmp", ".ini", ".dll", ".dbc", ".sbt", ".ttf"
                                Case ".xsd", ".wdl", ".wdt", ".icns", ".xib", ".nib", ".wtf", ".rsrc", ".bls", ".html", ".pdf", ".js", ".jpg", ".wfx"
                                Case ".db", ".test", ".not", ".trs", ".plist", ".tiff", ".png", ".css", ".url", ".manifest", ".gif", ".blp", ".wav", ".mp3"
                                    'ignore... (don't allow user to select those directly as it would be pointless, kind of, we are NOT in the business of MPQ Explorer)
                                Case Else
                                    'unknown file type? Oo
                                    Debug.Print(i)
                            End Select
                        End If
                    End If
                Next

                archive.Dispose()
            Next
        End Sub

        Public Function Locate(ByVal path As String) As Boolean
            If path Is Nothing Then Return False
            Return FileList.ContainsKey(path.ToLower)
        End Function

        Public Function LoadFile(ByVal path As String) As Stream
            If FileList.ContainsKey(path.ToLower) Then
                Dim fi As FileListEntry = FileList(path.ToLower)
                If System.IO.File.Exists(fi.Archive) Then
                    Dim archive As MpqArchive = New MpqArchive(fi.Archive)
                    Dim File As Stream = archive.OpenFile(fi.Path)
                    Dim buffer As Byte() = New Byte(fi.Size + 1000) {}
                    File.Read(buffer, 0, File.Length)
                    archive.Dispose()
                    Return New MemoryStream(buffer)
                Else
                    Return New MemoryStream()
                End If
            Else
                Return New MemoryStream()
            End If

        End Function

        Public Function SaveFileToDisk(ByVal MPQFileName, ByVal DiskFileName)
            If Locate(MPQFileName) Then
                Dim MPQStream As System.IO.Stream = LoadFile(MPQFileName)
                Dim DiskStream As New System.IO.FileStream(DiskFileName, IO.FileMode.Create)
                Dim Buffer(1000) As Byte

                Dim cnt As Integer = MPQStream.Read(Buffer, 0, Buffer.Length - 1)
                While cnt
                    DiskStream.Write(Buffer, 0, cnt)
                    cnt = MPQStream.Read(Buffer, 0, Buffer.Length - 1)
                End While

                MPQStream.Close()
                DiskStream.Close()

                Return True
            Else
                Return False
            End If

        End Function
    End Class

End Namespace

