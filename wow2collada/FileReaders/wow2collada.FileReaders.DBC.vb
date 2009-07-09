Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
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

        Public Structure sCharSections
            Dim ID As Integer
            Dim Race As Integer
            Dim Gender As Integer
            Dim GeneralType As Integer
            Dim Texture1 As String
            Dim Texture2 As String
            Dim Texture3 As String
            Dim Flags As Integer
            Dim Type As Integer
            Dim Variation As Integer
        End Structure

        Public CreatureModelData As sCreatureModelData()
        Public CreatureDisplayInfo As sCreatureDisplayInfo()
        Public GroundEffectTexture As sGroundEffectTexture()
        Public GroundEffectDoodad As sGroundEffectDoodad()
        Public AnimationData As sAnimationData()
        Public CharSections As sCharSections()

        Public Function LoadBaseDBCs() As Boolean
            Dim Ret As Boolean = True
            Ret = Ret And LoadCreatureModelData()
            Ret = Ret And LoadCreatureDisplayInfo()
            Ret = Ret And LoadGroundEffectTexture()
            Ret = Ret And LoadGroundEffectDoodad()
            Ret = Ret And LoadAnimationData()
            Ret = Ret And LoadCharSections()
            Return Ret
        End Function

        Private Function LoadCharSections()
            Dim FileName As String = "DBFilesClient\CharSections.dbc"
            If Not myMPQ.Locate(FileName) Then Return False

            Dim br As New BinaryReader(myMPQ.LoadFile(FileName))

            If br.ReadChars(4) <> "WDBC" Then Return False

            Dim nRecords As UInt32 = br.ReadUInt32
            Dim nFields As UInt32 = br.ReadUInt32
            Dim recordSize As UInt32 = br.ReadUInt32
            Dim stringSize As UInt32 = br.ReadUInt32

            ReDim CharSections(nRecords - 1)

            For i As Integer = 0 To nRecords - 1
                br.BaseStream.Position = 20 + i * recordSize

                CharSections(i).ID = br.ReadUInt32
                CharSections(i).Race = br.ReadUInt32
                CharSections(i).Gender = br.ReadUInt32
                CharSections(i).GeneralType = br.ReadUInt32
                Dim offStr1 As UInt32 = br.ReadUInt32
                Dim offStr2 As UInt32 = br.ReadUInt32
                Dim offStr3 As UInt32 = br.ReadUInt32
                CharSections(i).Flags = br.ReadUInt32
                CharSections(i).Type = br.ReadUInt32
                CharSections(i).Variation = br.ReadUInt32

                CharSections(i).Texture1 = myHF.GetZeroDelimitedString(br, 20 + nRecords * recordSize + offStr1)
                CharSections(i).Texture2 = myHF.GetZeroDelimitedString(br, 20 + nRecords * recordSize + offStr2)
                CharSections(i).Texture3 = myHF.GetZeroDelimitedString(br, 20 + nRecords * recordSize + offStr3)
            Next

            Return True
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

        Public Function GetTexturesByRaceGender(ByVal RaceID As Integer, ByVal Gender As Integer) As Dictionary(Of String, String())
            Dim Out As New Dictionary(Of String, String())

            For i As Integer = 0 To CharSections.Count - 1
                With CharSections(i)
                    If .Race = RaceID And .Gender = Gender And .Variation = 0 Then
                        Select Case .GeneralType
                            Case 0 'base skin
                                If Not Out.ContainsKey("skin") Then Out.Add("skin", New String() {.Texture1, .Texture2, .Texture3})
                            Case 1 'face
                                If Not Out.ContainsKey("face") Then Out.Add("face", New String() {.Texture1, .Texture2, .Texture3})
                            Case 2 'facial hair
                                If Not Out.ContainsKey("facialhair") Then Out.Add("facialhair", New String() {.Texture1, .Texture2, .Texture3})
                            Case 3 'hair
                                If Not Out.ContainsKey("hair") Then Out.Add("hair", New String() {.Texture1, .Texture2, .Texture3})
                            Case 4 'underwear
                                Out.Add("underwear", New String() {.Texture1, .Texture2, .Texture3})
                        End Select
                    End If
                End With
            Next
            Return Out
        End Function

        Public Function M2PathToRaceID(ByVal M2Path As String) As Integer
            Dim s As String = M2Path.ToLower
            If InStr(s, "human") Then Return 1 '01 Human
            If InStr(s, "orc") Then Return 2 '02 Orc
            If InStr(s, "dwarf") Then Return 3 '03 Dwarf
            If InStr(s, "nightelf") Then Return 4 '04 NightElf
            If InStr(s, "scourge") Then Return 5 '05 Scourge
            If InStr(s, "tauren") Then Return 6 '06 Tauren
            If InStr(s, "gnome") Then Return 7 '07 Gnome
            If InStr(s, "goblin") Then Return 9 '09 Goblin
            If InStr(s, "bloodelf") Then Return 10 '10 BloodElf
            If InStr(s, "draenei") Then Return 11 '11 Draenei
            If InStr(s, "felorc") Then Return 12 '12 FelOrc
            If InStr(s, "naga_") Then Return 13 '13 Naga_
            If InStr(s, "broken") Then Return 14 '14 Broken
            If InStr(s, "vrykul") Then Return 16 '16 Vrykul
            If InStr(s, "tuskarr") Then Return 17 '17 Tuskarr
            If InStr(s, "taunka") Then Return 19 '19 Taunka
            If InStr(s, "foresttroll") Then Return 18 '18 ForestTroll
            If InStr(s, "icetroll") Then Return 21 '21 IceTroll
            If InStr(s, "troll") Then Return 8 '08 Troll
            If InStr(s, "northrendskeleton") Then Return 20 '20 NorthrendSkeleton
            If InStr(s, "skeleton") Then Return 15 '15 Skeleton
        End Function

        Public Function M2PathToGender(ByVal M2Path As String) As Integer
            Dim s As String = M2Path.ToLower
            If InStr(s, "female") Then Return 1 '01 female
            Return 0 '00 male
        End Function


        Public Sub DumpRaceList()
            Dim id As Integer = -1
            For i As Integer = 0 To CharSections.Count - 1
                With CharSections(i)
                    If .Race <> id Then
                        id = .Race
                        Debug.Print(id & " " & .Texture1)
                    End If
                End With
            Next
        End Sub

    End Class

End Namespace

