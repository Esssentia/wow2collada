Namespace wow2collada

    Module Main
        Public frm As RenderForm
        Public render As render3d
        Public hf As HelperFunctions
        Public myDBC As FileReaders.DBC

        Public Sub Main()
            frm = New RenderForm()
            render = New render3d(frm.pic3d)
            hf = New HelperFunctions

            myDBC = New FileReaders.DBC
            myDBC.LoadCreatureModelData("D:\temp\mpq\DBFilesClient\CreatureModelData.dbc")
            myDBC.LoadCreatureDisplayInfo("D:\temp\mpq\DBFilesClient\CreatureDisplayInfo.dbc")

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
