Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports MpqReader

Namespace wow2collada.FileReaders

    Class DBC

        Public Structure CreatureModelDataStructure
            Dim ModelID As UInt32
            Dim CreatureType As UInt32
            Dim ModelFilename As String
            Dim StringOffset As UInt32
        End Structure

        Public Structure CreatureDisplayInfoStructure
            Dim CreatureID As UInt32
            Dim CreatureModelID As UInt32
            Dim Scale As Single
            Dim StringOffset11 As UInt32
            Dim StringOffset12 As UInt32
            Dim StringOffset13 As UInt32
            Dim Texture11 As String
            Dim Texture12 As String
            Dim Texture13 As String
        End Structure

        Public CreatureModelData As CreatureModelDataStructure()
        Public CreatureDisplayInfo As CreatureDisplayInfoStructure()

        Public Function LoadCreatureModelData(ByVal FileName As String) As Boolean
            Return LoadCreatureModelDataFromStream(File.OpenRead(FileName), FileName)
        End Function

        Public Function LoadCreatureModelDataFromStream(ByRef File As Stream, ByVal FileName As String)
            Dim br As New BinaryReader(File)

            If br.ReadChars(4) <> "WDBC" Then
                File.Close()
                Return False
            End If


            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

            'Debug.Print(nRecords)
            'Debug.Print(nFields)
            'Debug.Print(recordSize)
            'Debug.Print(stringSize)

            ReDim CreatureModelData(nRecords - 1)

            For i As Integer = 0 To nRecords - 1
                br.BaseStream.Position = 20 + i * recordSize
                CreatureModelData(i).ModelID = br.ReadUInt32
                CreatureModelData(i).CreatureType = br.ReadUInt32
                CreatureModelData(i).StringOffset = br.ReadUInt32
            Next

            For i As Integer = 0 To nRecords - 1
                CreatureModelData(i).ModelFilename = myHF.GetZeroDelimitedStringFromBinaryReader(br, CreatureModelData(i).StringOffset + 20 + nRecords * recordSize).Replace(".mdx", ".m2")
                'Debug.Print(Creaturemodeldata(i).ModelFilename)
            Next

            File.Close()
            Return True
        End Function

        Public Function LoadCreatureDisplayInfo(ByVal FileName As String) As Boolean
            Return LoadCreatureDisplayInfoFromStream(File.OpenRead(FileName), FileName)
        End Function

        Public Function LoadCreatureDisplayInfoFromStream(ByRef File As Stream, ByVal FileName As String)
            Dim br As New BinaryReader(File)

            If br.ReadChars(4) <> "WDBC" Then
                File.Close()
                Return False
            End If

            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

            'Debug.Print(nRecords)
            'Debug.Print(nFields)
            'Debug.Print(recordSize)
            'Debug.Print(stringSize)

            ReDim CreatureDisplayInfo(nRecords - 1)

            For i As Integer = 0 To nRecords - 1
                br.BaseStream.Position = 20 + i * recordSize
                CreatureDisplayInfo(i).CreatureID = br.ReadUInt32
                CreatureDisplayInfo(i).CreatureModelID = br.ReadUInt32
                Dim Unknown0 As UInt32 = br.ReadUInt32
                Dim Unknown1 As UInt32 = br.ReadUInt32
                CreatureDisplayInfo(i).Scale = br.ReadSingle
                Dim Unknown2 As UInt32 = br.ReadUInt32
                CreatureDisplayInfo(i).StringOffset11 = br.ReadUInt32
                CreatureDisplayInfo(i).StringOffset12 = br.ReadUInt32
                CreatureDisplayInfo(i).StringOffset13 = br.ReadUInt32
            Next

            For i As Integer = 0 To nRecords - 1
                If CreatureDisplayInfo(i).StringOffset11 > 0 Then CreatureDisplayInfo(i).Texture11 = myHF.GetZeroDelimitedStringFromBinaryReader(br, CreatureDisplayInfo(i).StringOffset11 + 20 + nRecords * recordSize)
                If CreatureDisplayInfo(i).StringOffset12 > 0 Then CreatureDisplayInfo(i).Texture12 = myHF.GetZeroDelimitedStringFromBinaryReader(br, CreatureDisplayInfo(i).StringOffset12 + 20 + nRecords * recordSize)
                If CreatureDisplayInfo(i).StringOffset13 > 0 Then CreatureDisplayInfo(i).Texture13 = myHF.GetZeroDelimitedStringFromBinaryReader(br, CreatureDisplayInfo(i).StringOffset13 + 20 + nRecords * recordSize)
            Next

            File.Close()
            Return True
        End Function

        Public Function GetIDFromModelFileName(ByVal FileName As String) As UInt32
            Dim i As Integer = 0
            Dim found As Boolean = False
            Dim ID As UInt32 = 0

            While Not found And i < CreatureModelData.Length
                Dim s As String = CreatureModelData(i).ModelFilename.ToLower
                If s.LastIndexOf("\") >= 0 Then s = s.Substring(s.LastIndexOf("\"))
                If s = FileName.Substring(FileName.LastIndexOf("\")).ToLower Then
                    found = True
                    ID = CreatureModelData(i).ModelID
                End If
                i += 1
            End While

            Return ID
        End Function

        Public Function GetTextureFromCreatureModelID(ByVal ModelID As UInt32, ByVal Typ As Integer) As String
            Dim i As Integer = 0
            Dim found As Boolean = False
            Dim Tex As String = ""

            While Not found And i < CreatureDisplayInfo.Length
                If CreatureDisplayInfo(i).CreatureModelID = ModelID Then
                    found = True
                    Select Case Typ
                        Case 11
                            Tex = CreatureDisplayInfo(i).Texture11
                        Case 12
                            Tex = CreatureDisplayInfo(i).Texture12
                        Case 13
                            Tex = CreatureDisplayInfo(i).Texture13
                    End Select
                End If
                i += 1
            End While

            Return Tex
        End Function

    End Class

End Namespace

