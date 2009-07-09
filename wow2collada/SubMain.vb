Module Main
    Public frm As MainForm
    Public frmOG As RenderFormOpenGL
    Public abo As Splash
    Public myHF As HelperFunctions
    Public myDBC As FileReaders.DBC
    Public myMPQ As FileReaders.MPQ
    Public Models As New List(Of sModel)

    Public Sub Main()
        'must be first, will be used by all other functions...
        myHF = New HelperFunctions


        'Dim Va As sVector3 = New sVector3(1, 1, 1)
        'Dim Ra As sQuaternion = sQuaternion.FromRotationAnglesDEG(0, 0, 0)
        'Dim Sa As Single = 1
        'Dim Ma As sMatrix4 = New sMatrix4(Ra, Va, Sa)

        'Dim Vb As sVector3 = New sVector3(1, 1, 1)
        'Dim Rb As sQuaternion = sQuaternion.FromRotationAnglesDEG(0, 0, 0)
        'Dim Sb As Single = 1
        'Dim Mb As sMatrix4 = New sMatrix4(Rb, Vb, Sb)

        'Dim Vi As sVector3 = New sVector3(1, 1, 1)

        'Dim Vo As sVector3 = (Ma * Mb) * Vi

        'MsgBox(String.Format("{0} {1} {2}", Vo.X, Vo.Y, Vo.Z))
        'End















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