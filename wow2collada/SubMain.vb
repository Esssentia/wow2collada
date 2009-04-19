Module Main
    Public frm As MainForm
    'Public frm3D As RenderFormDirect3D
    Public frmOG As RenderFormOpenGL
    Public abo As Splash
    Public myHF As HelperFunctions
    Public myDBC As FileReaders.DBC
    Public myMPQ As FileReaders.MPQ

    Public Sub Main()
        'must be first, will be used by all other functions...
        myHF = New HelperFunctions

        abo = New Splash()
        abo.Show()

        frm = New MainForm()
        frmOG = New RenderFormOpenGL()


        myMPQ = New FileReaders.MPQ
        Application.DoEvents()
        myMPQ.GenerateFileList()
        abo.UpdateProgress(100)

        myDBC = New FileReaders.DBC
        If Not myDBC.LoadBaseDBCs() Then RequiredFileNotFound()

        frm.Show()
        frmOG.Show()

        ' While the form is valid, render the scene and process messages.
        Do While frm.Created
            frmOG.RenderFrame()
            Application.DoEvents()
        Loop

    End Sub

    Private Sub RequiredFileNotFound()
        MsgBox("Could not open a file required for operation...")
        End
    End Sub

End Module