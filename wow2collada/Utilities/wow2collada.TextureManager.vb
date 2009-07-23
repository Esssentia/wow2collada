Imports OpenTK
Imports OpenTK.Graphics

Public Class TextureManager
    Implements IDisposable

    ''' <summary>
    ''' Structure to hold Texture information (Bitmap, TextureObject and Filename)
    ''' </summary>
    ''' <remarks>The TextureObject is dependent on the Direct3D device, so if the device changes, the textures have to be recalculated!</remarks>
    Public Class sTexture
        Public TextureMap As Bitmap
        Public OpenGLTexID As Integer

        Sub New(ByVal TextureBMP As Bitmap, ByVal OpenGLID As Integer)
            TextureMap = TextureBMP
            OpenGLTexID = OpenGLID
        End Sub
    End Class

    Private _Disposed As Boolean = False
    Public Textures As New Dictionary(Of String, sTexture)

    Sub New()
        'GL.EnableClientState(EnableCap.Texture2D)
    End Sub

    Function AddTexture(ByVal Name As String, ByVal Path As String) As String
        Dim Out As String = ""
        If Not TextureExists(Name) Then
            If myMPQ.Locate(Path) Then
                Dim BLP As New FileReaders.BLP
                Out = AddTexture(Name, BLP.LoadFromStream(myMPQ.LoadFile(Path), Path))

            End If
        Else
            Out = Name
        End If

        Return Out
    End Function

    Function AddTexture(ByVal Name As String, ByVal BMP As Bitmap) As String
        Dim Out As String = ""
        If Not TextureExists(Name) Then
            Dim OpenGLID As Integer = GL.GenTexture()
            Dim LayerData As Imaging.BitmapData = BMP.LockBits(New Rectangle(0, 0, BMP.Width, BMP.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
            GL.BindTexture(TextureTarget.Texture2D, OpenGLID)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, BMP.Width, BMP.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, LayerData.Scan0)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
            BMP.UnlockBits(LayerData)
            Textures.Add(Name, New sTexture(BMP, OpenGLID))
            Out = Name
        Else
            Out = Name
        End If

        Return Out
    End Function

    Function GetTextureOpenGLID(ByRef Name As String) As Integer
        If Textures.ContainsKey(Name) Then Return Textures(Name).OpenGLTexID
        Return -1
    End Function

    Function GetTexture(ByVal Name As String) As sTexture
        If Textures.ContainsKey(Name) Then Return Textures(Name)
        Return Nothing
    End Function

    Function TextureExists(ByVal Name As String) As Boolean
        Return Textures.ContainsKey(Name)
    End Function

    Sub Clear()
        For Each texture As sTexture In Textures.Values
            GL.DeleteTexture(texture.OpenGLTexID)
            texture.TextureMap.Dispose()
        Next
        Textures.Clear()
    End Sub

    Private Overloads Sub Dispose(ByVal disposing As Boolean)
        If Not Me._Disposed Then
            If disposing Then
                Clear()
            End If
        End If
        Me._Disposed = True
    End Sub

#Region "IDisposable Support"
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub
#End Region

End Class

