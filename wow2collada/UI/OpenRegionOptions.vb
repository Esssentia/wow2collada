Imports System.Windows.Forms

Public Class OpenRegionOptions

    Public Class sRegion
        Public NiceName As String
        Public ADTs As Vector2()

        Sub New(ByVal Name As String, ByVal ADTList As Vector2())
            NiceName = Name
            ADTs = ADTList
        End Sub

    End Class

    Public Class sContinent
        Public NiceName As String
        Public FilePrefix As String
        Public Regions As New Dictionary(Of String, sRegion)

        Sub New(ByVal Name As String, ByVal Prefix As String)
            NiceName = Name
            FilePrefix = Prefix
        End Sub

        Sub AddRegion(ByVal Region As sRegion)
            Regions.Add(Region.NiceName, Region)
        End Sub
    End Class

    Dim Continents As Dictionary(Of String, sContinent)

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub OpenRegionOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ComboBox1.Items.Clear()

        Dim xmlfile As New Xml.XmlDocument()

        xmlfile.Load(New System.IO.StringReader(My.Resources.Regions))

        Dim xmlnodes As Xml.XmlNodeList = xmlfile.DocumentElement.ChildNodes

        For Each xmlnode As Xml.XmlNode In xmlnodes
            Select Case xmlnode.Name
                Case "continent"
                    ComboBox1.Items.Add(xmlnode.Attributes("name").Value)
                    


            End Select
        Next

        ComboBox2.Items.Clear()
        ComboBox2.Enabled = False
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        ComboBox2.Items.Clear()
        ComboBox2.Text = ""

        Dim xmlfile As New Xml.XmlDocument()

        xmlfile.Load(New System.IO.StringReader(My.Resources.Regions))

        Dim xmlnodes As Xml.XmlNodeList = xmlfile.DocumentElement.ChildNodes

        For Each xmlnode As Xml.XmlNode In xmlnodes
            Select Case xmlnode.Name
                Case "continent"
                    If xmlnode.Attributes("name").Value = ComboBox1.SelectedItem Then
                        For Each xmlsubnode As Xml.XmlNode In xmlnode.ChildNodes
                            Select Case xmlsubnode.Name
                                Case "region"
                                    ComboBox2.Items.Add(xmlsubnode.Attributes("name").Value)
                            End Select
                        Next
                    End If
            End Select
        Next

        ComboBox2.Enabled = ComboBox2.Items.Count > 0
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim xmlfile As New Xml.XmlDocument()

        xmlfile.Load(New System.IO.StringReader(My.Resources.Regions))

        Dim xmlnodes As Xml.XmlNodeList = xmlfile.DocumentElement.ChildNodes

        For Each xmlnode As Xml.XmlNode In xmlnodes
            Select Case xmlnode.Name
                Case "continent"
                    If xmlnode.Attributes("name").Value = ComboBox1.SelectedItem Then
                        For Each xmlsubnode As Xml.XmlNode In xmlnode.ChildNodes
                            Select Case xmlsubnode.Name
                                Case "region"
                                    If xmlsubnode.Attributes("name").Value = ComboBox2.SelectedItem Then
                                        Dim File As String = xmlnode.Attributes("file").Value
                                        Dim xMin As Integer = xmlsubnode.Attributes("xmin").Value
                                        Dim xMax As Integer = xmlsubnode.Attributes("xmax").Value
                                        Dim yMin As Integer = xmlsubnode.Attributes("ymin").Value
                                        Dim yMax As Integer = xmlsubnode.Attributes("ymax").Value

                                        'do tha thaeng...
                                        MsgBox("plöpp")

                                    End If

                            End Select
                        Next
                    End If
            End Select
        Next
    End Sub

End Class
