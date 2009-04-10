Namespace wow2collada

    Module Main
        Public frm As RenderForm
        Public abo As Splash
        Public render As render3d
        Public myHF As HelperFunctions
        Public myDBC As FileReaders.DBC
        Public myMPQ As FileReaders.MPQ
        Public CanvasTainted As Boolean = False

        Public Sub Main()
            abo = New Splash()
            abo.Show()
            Application.DoEvents()
            frm = New RenderForm()
            render = New render3d(frm.pic3d)
            myHF = New HelperFunctions
            Application.DoEvents()

            myMPQ = New FileReaders.MPQ
            Application.DoEvents()
            myMPQ.GenerateFileList()
            abo.UpdateProgress(100)

            If Not myMPQ.Locate("DBFilesClient\CreatureModelData.dbc") Or Not myMPQ.Locate("DBFilesClient\CreatureDisplayInfo.dbc") Then
                MsgBox("Could not load needed files... Aborting...")
                End
            End If

            myDBC = New FileReaders.DBC
            Dim DBC_MD As String = "DBFilesClient\CreatureModelData.dbc"
            Dim DBC_DI As String = "DBFilesClient\CreatureDisplayInfo.dbc"
            myDBC.LoadCreatureModelDataFromStream(myMPQ.LoadFile(DBC_MD), DBC_MD)
            myDBC.LoadCreatureDisplayInfoFromStream(myMPQ.LoadFile(DBC_DI), DBC_DI)

            ' Initialize Direct3D.
            If render.InitializeGraphics() Then
                frm.Show()

                ' While the form is valid, render the scene and process messages.
                Do While frm.Created
                    If Not CanvasTainted Then render.Render()
                    Application.DoEvents()
                Loop
            End If
        End Sub

    End Module

End Namespace
