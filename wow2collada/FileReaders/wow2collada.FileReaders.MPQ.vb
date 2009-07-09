Imports System.IO
Imports System.Text
Imports MpqReader

Namespace FileReaders

    ''' <summary>
    ''' Class to deal with MPQ archives (encapsulates MpqTool, (C) 2006 Weichhold (oliver@weichhold.com))
    ''' </summary>
    ''' <remarks></remarks>
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

        Private _KnownMPQ As New List(Of String) 'list of mpq's relative to the DATA directory 
        Private _BasePath As String
        Public FileList As New System.Collections.Generic.Dictionary(Of String, FileListEntry)
        Public FileTree As New wow2collada.FileReaders.Node(Nothing, "ROOT")

        ''' <summary>
        ''' Initialize the MPQ Filereader (get location of the MPQ files and store the locations for later use
        ''' </summary>
        ''' <remarks></remarks>
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
            ' manual override for my computer because I have several WOW installations with different patchlevels
            If System.Environment.MachineName.ToLower = "remo-d2" Then _BasePath = "d:\temp\data\"

            ' I don't want to iterate through the directory reading arbitrary MPQs... (for now)
            _KnownMPQ.Add("common.mpq")
            _KnownMPQ.Add("patch.mpq")
            _KnownMPQ.Add("patch-2.mpq")
            _KnownMPQ.Add("expansion.mpq")
            _KnownMPQ.Add("lichking.mpq")
            _KnownMPQ.Add("deDE\base-deDE.mpq")
            _KnownMPQ.Add("deDE\expansion-locale-deDE.mpq")
            _KnownMPQ.Add("deDE\expansion-speech-deDE.mpq")
            _KnownMPQ.Add("deDE\lichking-locale-deDE.mpq")
            _KnownMPQ.Add("deDE\lichking-speech-deDE.mpq")
            _KnownMPQ.Add("deDE\locale-deDE.mpq")
            _KnownMPQ.Add("deDE\patch-deDE.mpq")
            _KnownMPQ.Add("deDE\patch-deDE-2.mpq")
            _KnownMPQ.Add("deDE\speech-deDE.mpq")
            '_KnownMPQ.Add("common-2.mpq")
            '_KnownMPQ.Add("deDE\backup-deDE.mpq")

        End Sub

        ''' <summary>
        ''' Get a list of all files in those MPQ files and store them for later
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub GenerateFileList()
            Dim mainProz As Single
            Dim subProz As Single
            Dim subTick As Single

            subTick = 99 / _KnownMPQ.Count

            For i As Integer = 0 To _KnownMPQ.Count - 1
                Dim archiveFile As String = _BasePath & _KnownMPQ.ElementAt(i)

                If File.Exists(archiveFile) Then

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
                                    Case ".m2", ".adt", ".blp", ".wav", ".mp3", ".trs"
                                        Dim Parts As String() = a.Path.Split("\")
                                        Dim parent As wow2collada.FileReaders.Node = FileTree
                                        For k As Integer = 0 To Parts.Count - 1
                                            parent = parent.Add(Parts(k))
                                        Next

                                    Case ".wmo"
                                        'filter out "sub-wmo's"

                                        Dim regex As New RegularExpressions.Regex("_\d{3}.wmo$")
                                        If Not regex.IsMatch(a.Path) Then
                                            Dim Parts As String() = a.Path.Split("\")
                                            Dim parent As wow2collada.FileReaders.Node = FileTree
                                            For k As Integer = 0 To Parts.Count - 1
                                                parent = parent.Add(Parts(k))
                                            Next
                                        End If

                                    Case ".anim", ".skin", ".lua", ".xml", ".sig", ".txt", ".exe", ".toc", ".zmp", ".ini", ".dll", ".sbt", ".ttf", ".dbc"
                                    Case ".xsd", ".wdl", ".wdt", ".icns", ".xib", ".nib", ".wtf", ".rsrc", ".bls", ".html", ".pdf", ".js", ".jpg", ".wfx"
                                    Case ".db", ".test", ".not", ".trs", ".plist", ".tiff", ".png", ".css", ".url", ".manifest", ".gif"
                                        'ignore... (don't allow user to select those directly as it would be pointless, kind of, we are NOT in the business of MPQ Explorer)
                                    Case Else
                                        'unknown file type? Oo
                                        Debug.Print(i)
                                End Select
                            End If
                        End If
                    Next

                    archive.Dispose()
                End If
            Next
        End Sub

        ''' <summary>
        ''' Locate a file in the MPQ file list
        ''' </summary>
        ''' <param name="path">The file to locate</param>
        ''' <returns>True if found, false otherwise</returns>
        ''' <remarks></remarks>
        Public Function Locate(ByVal path As String) As Boolean
            If path Is Nothing Then Return False
            Return FileList.ContainsKey(path.ToLower)
        End Function

        ''' <summary>
        ''' Return the MPQ that a specific file is in (mainly used for debugging
        ''' </summary>
        ''' <param name="path">The file to locate</param>
        ''' <returns>The name of the MPQ containing the file</returns>
        ''' <remarks></remarks>
        Public Function LocateMPQ(ByVal path As String) As String
            If path Is Nothing Then Return ""
            If Not Locate(path) Then Return ""
            Return FileList(path.ToLower).Archive
        End Function

        ''' <summary>
        ''' Load a file from the appropriate MPQ archive and return a stream to it
        ''' </summary>
        ''' <param name="path">The file to load</param>
        ''' <returns>A stream of the requested file</returns>
        ''' <remarks></remarks>
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
                    Return Nothing
                End If
            Else
                Return Nothing
            End If

        End Function

        ''' <summary>
        ''' Save a file from an MPQ archive and save it to disk
        ''' </summary>
        ''' <param name="MPQFileName">The filename within the MPQ archive</param>
        ''' <param name="DiskFileName">The filename on disk to write to</param>
        ''' <returns>True if found, false otherwise</returns>
        ''' <remarks></remarks>
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

