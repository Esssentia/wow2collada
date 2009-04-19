Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports MpqReader

Namespace FileReaders

    Class DBC

        Public Structure sCreatureModelData
            Dim ModelID As UInt32
            Dim CreatureType As UInt32
            Dim ModelFilename As String
            Dim StringOffset As UInt32
        End Structure

        Public Structure sCreatureDisplayInfo
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

        Public Structure sGroundEffectTexture
            Dim EffectDoodad1 As UInt32
            Dim EffectDoodad2 As UInt32
            Dim EffectDoodad3 As UInt32
            Dim EffectDoodad4 As UInt32
            Dim Density As Integer
        End Structure

        Public Structure sAnimationData
            Dim ID As Integer
            Dim Name As String
            Dim WeaponState As Integer
            Dim Flags As Integer
            Dim Preceeding As Integer
            Dim RealID As Integer
            Dim Group As Integer
        End Structure

        Public Structure sGroundEffectDoodad
            Dim DoodadID As Integer
            Dim DoodadModel As String
        End Structure

        Public CreatureModelData As sCreatureModelData()
        Public CreatureDisplayInfo As sCreatureDisplayInfo()
        Public GroundEffectTexture As sGroundEffectTexture()
        Public GroundEffectDoodad As sGroundEffectDoodad()
        Public AnimationData As sAnimationData()

        Public Function LoadBaseDBCs() As Boolean
            Dim Ret As Boolean = True
            Ret = Ret And LoadCreatureModelData()
            Ret = Ret And LoadCreatureDisplayInfo()
            Ret = Ret And LoadGroundEffectTexture()
            Ret = Ret And LoadGroundEffectDoodad()
            Ret = Ret And LoadAnimationData()
            Return Ret
        End Function

        Private Function LoadCreatureModelData()
            Dim FileName As String = "DBFilesClient\CreatureModelData.dbc"
            If Not myMPQ.Locate(FileName) Then Return False

            Dim br As New BinaryReader(myMPQ.LoadFile(FileName))

            If br.ReadChars(4) <> "WDBC" Then Return False

            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

            ReDim CreatureModelData(nRecords - 1)

            For i As Integer = 0 To nRecords - 1
                br.BaseStream.Position = 20 + i * recordSize
                CreatureModelData(i).ModelID = br.ReadUInt32
                CreatureModelData(i).CreatureType = br.ReadUInt32
                CreatureModelData(i).StringOffset = br.ReadUInt32
            Next

            For i As Integer = 0 To nRecords - 1
                CreatureModelData(i).ModelFilename = myHF.GetZeroDelimitedString(br, CreatureModelData(i).StringOffset + 20 + nRecords * recordSize).Replace(".mdx", ".m2")
                'Debug.Print(Creaturemodeldata(i).ModelFilename)
            Next

            Return True
        End Function

        Private Function LoadCreatureDisplayInfo()
            Dim FileName As String = "DBFilesClient\CreatureDisplayInfo.dbc"
            If Not myMPQ.Locate(FileName) Then Return False

            Dim br As New BinaryReader(myMPQ.LoadFile(FileName))

            If br.ReadChars(4) <> "WDBC" Then Return False

            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

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
                If CreatureDisplayInfo(i).StringOffset11 > 0 Then CreatureDisplayInfo(i).Texture11 = myHF.GetZeroDelimitedString(br, CreatureDisplayInfo(i).StringOffset11 + 20 + nRecords * recordSize)
                If CreatureDisplayInfo(i).StringOffset12 > 0 Then CreatureDisplayInfo(i).Texture12 = myHF.GetZeroDelimitedString(br, CreatureDisplayInfo(i).StringOffset12 + 20 + nRecords * recordSize)
                If CreatureDisplayInfo(i).StringOffset13 > 0 Then CreatureDisplayInfo(i).Texture13 = myHF.GetZeroDelimitedString(br, CreatureDisplayInfo(i).StringOffset13 + 20 + nRecords * recordSize)
            Next

            Return True
        End Function

        Private Function LoadGroundEffectTexture()
            Dim FileName As String = "DBFilesClient\GroundEffectTexture.dbc"
            If Not myMPQ.Locate(FileName) Then Return False

            Dim br As New BinaryReader(myMPQ.LoadFile(FileName))

            If br.ReadChars(4) <> "WDBC" Then Return False

            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

            ReDim GroundEffectTexture(nRecords - 1)

            For i As Integer = 0 To nRecords - 1
                br.BaseStream.Position = 20 + i * recordSize
                GroundEffectTexture(i).EffectDoodad1 = br.ReadUInt32
                GroundEffectTexture(i).EffectDoodad2 = br.ReadUInt32
                GroundEffectTexture(i).EffectDoodad3 = br.ReadUInt32
                GroundEffectTexture(i).EffectDoodad4 = br.ReadUInt32
                Dim Unknown0 As UInt32 = br.ReadUInt32
                Dim Unknown1 As UInt32 = br.ReadUInt32
                Dim Unknown2 As UInt32 = br.ReadUInt32
                Dim Unknown3 As UInt32 = br.ReadUInt32
                GroundEffectTexture(i).Density = br.ReadUInt32
            Next

            Return True
        End Function

        Private Function LoadGroundEffectDoodad()
            Dim FileName As String = "DBFilesClient\GroundEffectDoodad.dbc"
            If Not myMPQ.Locate(FileName) Then Return False

            Dim br As New BinaryReader(myMPQ.LoadFile(FileName))

            If br.ReadChars(4) <> "WDBC" Then Return False

            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

            ReDim GroundEffectDoodad(nRecords - 1)

            For i As Integer = 0 To nRecords - 1
                br.BaseStream.Position = 20 + i * recordSize
                GroundEffectDoodad(i).DoodadID = br.ReadUInt32
                Dim Unknown1 As UInt32 = br.ReadUInt32
                Dim offStr As UInt32 = br.ReadUInt32
                GroundEffectDoodad(i).DoodadModel = "World\NoDXT\Detail\" & myHF.GetZeroDelimitedString(br, 20 + nRecords * recordSize + offStr)
            Next

            Return True
        End Function

        Private Function LoadAnimationData()
            Dim FileName As String = "DBFilesClient\AnimationData.dbc"
            If Not myMPQ.Locate(FileName) Then Return False

            Dim br As New BinaryReader(myMPQ.LoadFile(FileName))

            If br.ReadChars(4) <> "WDBC" Then Return False

            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

            ReDim AnimationData(nRecords - 1)

            For i As Integer = 0 To nRecords - 1
                br.BaseStream.Position = 20 + i * recordSize

                AnimationData(i).ID = br.ReadUInt32
                Dim offStr As UInt32 = br.ReadUInt32
                AnimationData(i).WeaponState = br.ReadUInt32
                AnimationData(i).Flags = br.ReadUInt32
                Dim Unknown1 As UInt32 = br.ReadUInt32
                AnimationData(i).Preceeding = br.ReadUInt32
                AnimationData(i).RealID = br.ReadUInt32
                AnimationData(i).Group = br.ReadUInt32
                AnimationData(i).Name = myHF.GetZeroDelimitedString(br, 20 + nRecords * recordSize + offStr)
            Next

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

