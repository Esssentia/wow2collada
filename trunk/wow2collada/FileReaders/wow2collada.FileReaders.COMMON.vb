Namespace FileReaders

    ''' <summary>
    ''' Class to manage and manipulate a tree of nodes (very simplistic)
    ''' </summary>
    ''' <remarks></remarks>
    Class Node
        Dim _Data As String
        Dim _Nodes As Dictionary(Of String, Node)
        Dim _Parent As Node

        ''' <summary>
        ''' Create a new node
        ''' </summary>
        ''' <param name="parent">The parent of the new node</param>
        ''' <param name="text">The content of the new node</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal parent As Node, ByVal text As String)
            _Parent = parent
            _Data = text
        End Sub

        ''' <summary>
        ''' Add a child node to a node if it doesn't exist yet
        ''' </summary>
        ''' <param name="text">The content of the new node</param>
        ''' <returns>Returns the newly added node (or the existing one if one was already there)</returns>
        ''' <remarks></remarks>
        Public Function Add(ByVal text As String) As Node
            If _Nodes Is Nothing Then ' no children yet
                Dim NewNode As Node = New Node(Me, text)
                _Nodes = New Dictionary(Of String, Node)
                _Nodes.Add(text, NewNode)
                Return NewNode
            ElseIf Not _Nodes.ContainsKey(text) Then ' children but not found
                Dim NewNode As Node = New Node(Me, text)
                _Nodes.Add(text, NewNode)
                Return NewNode
            Else
                Return _Nodes(text)
            End If
        End Function

        ''' <summary>
        ''' Returns all child nodes of a node
        ''' </summary>
        ''' <returns>Child nodes</returns>
        ''' <remarks></remarks>
        Public Function Nodes() As Dictionary(Of String, Node)
            Return _Nodes
        End Function

        ''' <summary>
        ''' Returns the data of a node
        ''' </summary>
        ''' <returns>Data of the node</returns>
        ''' <remarks></remarks>
        Public Function Data() As String
            Return _Data
        End Function

    End Class

End Namespace
