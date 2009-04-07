Namespace wow2collada

    Module Main
        Public frm As RenderForm
        Public abo As Splash
        Public render As render3d
        Public hf As HelperFunctions
        Public myDBC As FileReaders.DBC
        Public myMPQ As FileReaders.MPQ

        Public Sub Main()
            abo = New Splash()
            abo.Show()
            Application.DoEvents()
            frm = New RenderForm()
            render = New render3d(frm.pic3d)
            hf = New HelperFunctions
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
            myDBC.LoadCreatureModelDataFromStream(myMPQ.LoadFile("DBFilesClient\CreatureModelData.dbc"))
            myDBC.LoadCreatureDisplayInfoFromStream(myMPQ.LoadFile("DBFilesClient\CreatureDisplayInfo.dbc"))

            ' Initialize Direct3D.
            If render.InitializeGraphics() Then
                frm.Show()

                ' While the form is valid, render the scene and process messages.
                Do While frm.Created
                    render.Render()
                    Application.DoEvents()
                Loop
            End If
        End Sub

    End Module

End Namespace
