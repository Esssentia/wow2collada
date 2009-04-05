Namespace wow2collada

    Module Main
        Dim frm As RenderForm

        Public Sub Main()
            frm = New RenderForm()

            ' Initialize Direct3D.
            If frm.InitializeGraphics() Then
                frm.Show()

                ' While the form is valid,
                ' render the scene and process messages.
                Do While frm.Created
                    frm.Render()
                    Application.DoEvents()
                Loop
            End If
        End Sub

    End Module

End Namespace
