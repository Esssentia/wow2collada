﻿Imports System.Math
Imports System.IO
Imports System.Runtime.InteropServices

''' <summary>
''' Class to hold AnimationBlock data
''' </summary>
''' <remarks></remarks>
Public Class sAnimBlock
    Private _InterpolationType As Integer
    Private _GlobalSequenceID As Integer
    Private _nTimestampPairs As Integer
    Private _ofsTimestampPairs As Integer
    Private _nKeyframePairs As Integer
    Private _ofsKeyframePairs As Integer

    Sub New()

    End Sub

    Sub New(ByVal InterpolationType As Integer, ByVal GlobalSequenceID As Integer, ByVal nTimestampPairs As Integer, ByVal ofsTimestampPairs As Integer, ByVal nKeyframePairs As Integer, ByVal ofsKeyframePairs As Integer)
        _InterpolationType = InterpolationType
        _GlobalSequenceID = GlobalSequenceID
        _nTimestampPairs = nTimestampPairs
        _ofsTimestampPairs = ofsTimestampPairs
        _nKeyframePairs = nKeyframePairs
        _ofsKeyframePairs = ofsKeyframePairs
    End Sub

End Class

''' <summary>
''' Class to hold Bone data
''' </summary>
''' <remarks></remarks>
Public Class sBone
    Public AnimationSequence As Integer
    Public Flags As Integer
    Public ParentBone As Integer
    Public GeosetID As Integer
    Public Translation As New sAnimBlock
    Public Rotation As New sAnimBlock
    Public Scaling As New sAnimBlock
    Public PivotPoint As New sVector3(0, 0, 0)
End Class

''' <summary>
''' Class to hold 4x4 matrices
''' </summary>
''' <remarks>A huge thanks to Genscher from the Blender community for talking me through using 4x4 matrices to do scale/rot/trans in one go and across multiple objects</remarks>
Public Class sMatrix4
    Private _m(,) As Single
    
    Public Shared Operator *(ByVal M1 As sMatrix4, ByVal M2 As sMatrix4) As sMatrix4
        Dim Mo As New sMatrix4()

        Mo(0, 0) = M1(0, 0) * M2(0, 0) + M1(0, 1) * M2(1, 0) + M1(0, 2) * M2(2, 0) + M1(0, 3) * M2(3, 0)
        Mo(0, 1) = M1(0, 0) * M2(0, 1) + M1(0, 1) * M2(1, 1) + M1(0, 2) * M2(2, 1) + M1(0, 3) * M2(3, 1)
        Mo(0, 2) = M1(0, 0) * M2(0, 2) + M1(0, 1) * M2(1, 2) + M1(0, 2) * M2(2, 2) + M1(0, 3) * M2(3, 2)
        Mo(0, 3) = M1(0, 0) * M2(0, 3) + M1(0, 1) * M2(1, 3) + M1(0, 2) * M2(2, 3) + M1(0, 3) * M2(3, 3)

        Mo(1, 0) = M1(1, 0) * M2(0, 0) + M1(1, 1) * M2(1, 0) + M1(1, 2) * M2(2, 0) + M1(1, 3) * M2(3, 0)
        Mo(1, 1) = M1(1, 0) * M2(0, 1) + M1(1, 1) * M2(1, 1) + M1(1, 2) * M2(2, 1) + M1(1, 3) * M2(3, 1)
        Mo(1, 2) = M1(1, 0) * M2(0, 2) + M1(1, 1) * M2(1, 2) + M1(1, 2) * M2(2, 2) + M1(1, 3) * M2(3, 2)
        Mo(1, 3) = M1(1, 0) * M2(0, 3) + M1(1, 1) * M2(1, 3) + M1(1, 2) * M2(2, 3) + M1(1, 3) * M2(3, 3)

        Mo(2, 0) = M1(2, 0) * M2(0, 0) + M1(2, 1) * M2(1, 0) + M1(2, 2) * M2(2, 0) + M1(2, 3) * M2(3, 0)
        Mo(2, 1) = M1(2, 0) * M2(0, 1) + M1(2, 1) * M2(1, 1) + M1(2, 2) * M2(2, 1) + M1(2, 3) * M2(3, 1)
        Mo(2, 2) = M1(2, 0) * M2(0, 2) + M1(2, 1) * M2(1, 2) + M1(2, 2) * M2(2, 2) + M1(2, 3) * M2(3, 2)
        Mo(2, 3) = M1(2, 0) * M2(0, 3) + M1(2, 1) * M2(1, 3) + M1(2, 2) * M2(2, 3) + M1(2, 3) * M2(3, 3)

        Mo(3, 0) = M1(3, 0) * M2(0, 0) + M1(3, 1) * M2(1, 0) + M1(3, 2) * M2(2, 0) + M1(3, 3) * M2(3, 0)
        Mo(3, 1) = M1(3, 0) * M2(0, 1) + M1(3, 1) * M2(1, 1) + M1(3, 2) * M2(2, 1) + M1(3, 3) * M2(3, 1)
        Mo(3, 2) = M1(3, 0) * M2(0, 2) + M1(3, 1) * M2(1, 2) + M1(3, 2) * M2(2, 2) + M1(3, 3) * M2(3, 2)
        Mo(3, 3) = M1(3, 0) * M2(0, 3) + M1(3, 1) * M2(1, 3) + M1(3, 2) * M2(2, 3) + M1(3, 3) * M2(3, 3)

        Return Mo
    End Operator

    Public Shared Operator *(ByVal M As sMatrix4, ByVal V As sVector3) As sVector3
        Dim Vo As New sVector3()

        Vo.X = V.X * M(0, 0) + V.Y * M(1, 0) + V.Z * M(2, 0) + M(3, 0)
        Vo.Y = V.X * M(0, 1) + V.Y * M(1, 1) + V.Z * M(2, 1) + M(3, 1)
        Vo.Z = V.X * M(0, 2) + V.Y * M(1, 2) + V.Z * M(2, 2) + M(3, 2)

        Return Vo
    End Operator

    Default Public Property m(ByVal x As Integer, ByVal y As Integer) As Single
        Get
            Return _m(x, y)
        End Get
        Set(ByVal value As Single)
            _m(x, y) = value
        End Set
    End Property

    Private Sub FromQuaternion(ByVal Q As sQuaternion)
        'https://svn.blender.org/svnroot/bf-blender/trunk/blender/source/blender/blenlib/intern/arithb.c
        Dim q0 As Single
        Dim q1 As Single
        Dim q2 As Single
        Dim q3 As Single
        Dim qda As Single
        Dim qdb As Single
        Dim qdc As Single
        Dim qaa As Single
        Dim qab As Single
        Dim qac As Single
        Dim qbb As Single
        Dim qbc As Single
        Dim qcc As Single
        Dim SQRT2 As Single = Math.Sqrt(2)
        Dim Qn As sQuaternion = sQuaternion.Normalize(Q)

        q0 = SQRT2 * Q.W
        q1 = SQRT2 * Q.X
        q2 = SQRT2 * Q.Y
        q3 = SQRT2 * Q.Z

        qda = q0 * q1
        qdb = q0 * q2
        qdc = q0 * q3
        qaa = q1 * q1
        qab = q1 * q2
        qac = q1 * q3
        qbb = q2 * q2
        qbc = q2 * q3
        qcc = q3 * q3

        _m(0, 0) = 1 - qbb - qcc
        _m(0, 1) = qdc + qab
        _m(0, 2) = -qdb + qac
        _m(0, 3) = 0

        _m(1, 0) = -qdc + qab
        _m(1, 1) = 1 - qaa - qcc
        _m(1, 2) = qda + qbc
        _m(1, 3) = 0

        _m(2, 0) = qdb + qac
        _m(2, 1) = -qda + qbc
        _m(2, 2) = 1 - qaa - qbb
        _m(2, 3) = 0

        _m(3, 0) = 0
        _m(3, 1) = 0
        _m(3, 2) = 0
        _m(3, 3) = 1

    End Sub

    Sub New()
        ReDim _m(3, 3)
        _m(0, 0) = 1
        _m(1, 1) = 1
        _m(2, 2) = 1
        _m(3, 3) = 1
    End Sub

    Sub New(ByVal rotation As sQuaternion, ByVal translation As sVector3, ByVal scale As Single)
        ReDim _m(3, 3)
        FromQuaternion(rotation)
        _m(3, 0) = translation.X
        _m(3, 1) = translation.Y
        _m(3, 2) = translation.Z
        _m(3, 3) = scale
    End Sub

End Class

''' <summary>
''' Class to hold Quaternions
''' </summary>
''' <remarks></remarks>
Public Class sQuaternion
    Private _X As Single
    Private _Y As Single
    Private _Z As Single
    Private _W As Single

    Public Property X() As Single
        Get
            Return _X
        End Get
        Set(ByVal value As Single)
            _X = value
        End Set
    End Property

    Public Property Y() As Single
        Get
            Return _Y
        End Get
        Set(ByVal value As Single)
            _Y = value
        End Set
    End Property

    Public Property Z() As Single
        Get
            Return _Z
        End Get
        Set(ByVal value As Single)
            _Z = value
        End Set
    End Property

    Public Property W() As Single
        Get
            Return _W
        End Get
        Set(ByVal value As Single)
            _W = value
        End Set
    End Property

    Public Shared Operator *(ByVal q1 As sQuaternion, ByVal q2 As sQuaternion) As sQuaternion
        Dim w As Single = (-q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z + q1.W * q2.W)
        Dim z As Single = (q1.X * q2.Y + q1.Y * q2.X + q1.Z * q2.W - q1.W * q2.Z)
        Dim y As Single = (-q1.X * q2.Z + q1.Y * q2.W - q1.Z * q2.X - q1.W * q2.Y)
        Dim x As Single = (q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y + q1.W * q2.X)
        Return New sQuaternion(x, y, z, w)
    End Operator

    Public Sub New(ByVal X As Single, ByVal Y As Single, ByVal Z As Single, ByVal W As Single)
        _X = X
        _Y = Y
        _Z = Z
        _W = W
    End Sub

    Public Shared Function Normalize(ByVal Q As sQuaternion) As sQuaternion
        Dim l As Single = Q.Length
        If l = 0 Then Return Q
        Return New sQuaternion(Q.X / l, Q.Y / l, Q.Z / l, Q.W / l)
    End Function

    Public ReadOnly Property Length()
        Get
            Return Sqrt(_X ^ 2 + _Y ^ 2 + _Z ^ 2 + _W ^ 2)
        End Get
    End Property

    Public Shared Function FromRotationAnglesRAD(ByVal xRot As Single, ByVal yRot As Single, ByVal zRot As Single) As sQuaternion
        Dim sx As Single = Sin(xRot / 2)
        Dim sy As Single = Sin(yRot / 2)
        Dim sz As Single = Sin(zRot / 2)
        Dim cx As Single = Cos(xRot / 2)
        Dim cy As Single = Cos(yRot / 2)
        Dim cz As Single = Cos(zRot / 2)
        Return New sQuaternion(sx * cy * cz - cx * sy * sz, _
                               cx * sy * cz + sx * cy * sz, _
                               cx * cy * sz - sx * sy * cz, _
                               cx * cy * cz + sx * sy * sz)
    End Function

    Public Shared Function FromRotationAnglesDEG(ByVal xRot As Single, ByVal yRot As Single, ByVal zRot As Single) As sQuaternion
        Return FromRotationAnglesRAD(xRot * PI / 180, yRot * PI / 180, zRot * PI / 180)
    End Function

    Public Overrides Function toString() As String
        Return String.Format("X:{0} Y:{1} Z:{2} W:{3}", Me.X, Me.Y, Me.Z, Me.W)
    End Function

End Class

''' <summary>
''' Class to hold 3D vectors
''' </summary>
''' <remarks></remarks>
Public Class sVector3
    Private _X As Single
    Private _Y As Single
    Private _Z As Single

    Public Shared Operator +(ByVal v1 As sVector3, ByVal v2 As sVector3) As sVector3
        Return New sVector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z)
    End Operator

    Public Shared Operator *(ByVal V As sVector3, ByVal S As Single) As sVector3
        Return New sVector3(V.X * S, V.Y * S, V.Z * S)
    End Operator

    Public Shared Operator -(ByVal V1 As sVector3, ByVal V2 As sVector3) As sVector3
        Return New sVector3(V1.X - V2.X, V1.Y - V2.Y, V1.Z - V2.Z)
    End Operator

    Public Property X() As Single
        Get
            Return _X
        End Get
        Set(ByVal value As Single)
            _X = value
        End Set
    End Property

    Public Property Y() As Single
        Get
            Return _Y
        End Get
        Set(ByVal value As Single)
            _Y = value
        End Set
    End Property

    Public Property Z() As Single
        Get
            Return _Z
        End Get
        Set(ByVal value As Single)
            _Z = value
        End Set
    End Property

    Public ReadOnly Property Length()
        Get
            Return Sqrt(_X ^ 2 + _Y ^ 2 + _Z ^ 2)
        End Get
    End Property

    Sub New()
        _X = 0
        _Y = 0
        _Z = 0
    End Sub

    Sub New(ByVal X As Single, ByVal Y As Single, ByVal Z As Single)
        _X = X
        _Y = Y
        _Z = Z
    End Sub

    Public Shared Function Normalize(ByVal V As sVector3) As sVector3
        Dim l As Single = V.Length
        If l = 0 Then Return V
        Return New sVector3(V.X / l, V.Y / l, V.Z / l)
    End Function

    Public Shared Function Cross(ByVal V1 As sVector3, ByVal V2 As sVector3) As sVector3
        Return New sVector3(V1.Y * V2.Z - V1.Z * V2.Y, V1.Z * V2.X - V1.X * V2.Z, V1.X * V2.Y - V1.Y * V2.X)
    End Function

    Public Shared Function Rotate(ByVal V As sVector3, ByVal Q As sQuaternion) As sVector3
        Dim t2 As Single = Q.W * Q.X
        Dim t3 As Single = Q.W * Q.Y
        Dim t4 As Single = Q.W * Q.Z
        Dim t5 As Single = -Q.X * Q.X
        Dim t6 As Single = Q.X * Q.Y
        Dim t7 As Single = Q.X * Q.Z
        Dim t8 As Single = -Q.Y * Q.Y
        Dim t9 As Single = Q.Y * Q.Z
        Dim t1 As Single = -Q.Z * Q.Z

        Return New sVector3(2 * ((t8 + t1) * V.X + (t6 - t4) * V.Y + (t3 + t7) * V.Z) + V.X, _
                            2 * ((t4 + t6) * V.X + (t5 + t1) * V.Y + (t9 - t2) * V.Z) + V.Y, _
                            2 * ((t7 - t3) * V.X + (t2 + t9) * V.Y + (t5 + t8) * V.Z) + V.Z)
    End Function

    Public Overrides Function toString() As String
        Return String.Format("X:{0} Y:{1} Z:{2}", Me.X, Me.Y, Me.Z)
    End Function

End Class

''' <summary>
''' Class to hold 2D vectors
''' </summary>
''' <remarks></remarks>
Public Class sVector2
    Private _U As Single
    Private _V As Single

    Public Property U() As Single
        Get
            Return _U
        End Get
        Set(ByVal value As Single)
            _U = value
        End Set
    End Property

    Public Property V() As Single
        Get
            Return _V
        End Get
        Set(ByVal value As Single)
            _V = value
        End Set
    End Property

    Sub New(ByVal U As Single, ByVal V As Single)
        _U = U
        _V = V
    End Sub

End Class

''' <summary>
''' Structure to hold Vertex information (Position, Normal, UV Coordinates and Bone-Information)
''' </summary>
''' <remarks></remarks>
Public Class sVertex
    Dim _Position As sVector3
    Dim _BoneWeights As Byte()
    Dim _BoneIndices As Byte()
    Dim _Normal As sVector3
    Dim _TextureCoords As sVector2

    Public Property Position() As sVector3
        Get
            Return _Position
        End Get
        Set(ByVal value As sVector3)
            _Position = value
        End Set
    End Property

    Public Property Normal() As sVector3
        Get
            Return _Normal
        End Get
        Set(ByVal value As sVector3)
            _Normal = value
        End Set
    End Property

    Public Property TextureCoords() As sVector2
        Get
            Return _TextureCoords
        End Get
        Set(ByVal value As sVector2)
            _TextureCoords = value
        End Set
    End Property

    Public Property BoneWeights() As Byte()
        Get
            Return _BoneWeights
        End Get
        Set(ByVal value As Byte())
            _BoneWeights = value
        End Set
    End Property

    Public Property Boneindices() As Byte()
        Get
            Return _BoneIndices
        End Get
        Set(ByVal value As Byte())
            _BoneIndices = value
        End Set
    End Property

    Sub New()
        _Position = New sVector3(0, 0, 0)
        _Normal = New sVector3(0, 0, 0)
        _TextureCoords = New sVector2(0, 0)
        _BoneWeights = New Byte() {0, 0, 0, 0}
        _BoneIndices = New Byte() {0, 0, 0, 0}
    End Sub

    Sub New(ByVal p1 As Single, ByVal p2 As Single, ByVal p3 As Single, ByVal n1 As Single, ByVal n2 As Single, ByVal n3 As Single, ByVal t1 As Single, ByVal t2 As Single)
        _Position = New sVector3(p1, p2, p3)
        _Normal = New sVector3(n1, n2, n3)
        _TextureCoords = New sVector2(t1, t2)
        _BoneWeights = New Byte() {0, 0, 0, 0}
        _BoneIndices = New Byte() {0, 0, 0, 0}
    End Sub

    Sub New(ByVal p1 As Single, ByVal p2 As Single, ByVal p3 As Single, ByVal n1 As Single, ByVal n2 As Single, ByVal n3 As Single, ByVal t1 As Single, ByVal t2 As Single, ByVal bw1 As Byte, ByVal bw2 As Byte, ByVal bw3 As Byte, ByVal bw4 As Byte, ByVal bi1 As Byte, ByVal bi2 As Byte, ByVal bi3 As Byte, ByVal bi4 As Byte)
        _Position = New sVector3(p1, p2, p3)
        _Normal = New sVector3(n1, n2, n3)
        _TextureCoords = New sVector2(t1, t2)
        _BoneWeights = New Byte() {bw1, bw2, bw3, bw4}
        _BoneIndices = New Byte() {bi1, bi2, bi3, bi4}
    End Sub

    Sub New(ByVal Position As sVector3, ByVal Normal As sVector3, ByVal TextureCoords As sVector2, ByVal BoneWeights As Byte(), ByVal BoneIndices As Byte())
        _Position = Position
        _Normal = Normal
        _TextureCoords = TextureCoords
        _BoneWeights = BoneWeights
        _BoneIndices = BoneIndices
    End Sub

    Sub New(ByVal Position As sVector3, ByVal Normal As sVector3, ByVal TextureCoords As sVector2)
        _Position = Position
        _Normal = Normal
        _TextureCoords = TextureCoords
        _BoneWeights = New Byte() {0, 0, 0, 0}
        _BoneIndices = New Byte() {0, 0, 0, 0}
    End Sub

End Class

''' <summary>
''' Structure to hold Triangle information (3 vertices)
''' </summary>
''' <remarks></remarks>
Public Class sTriangle
    Private _V1 As Integer
    Private _V2 As Integer
    Private _V3 As Integer

    Public Property V1() As Integer
        Get
            Return _V1
        End Get
        Set(ByVal value As Integer)
            _V1 = value
        End Set
    End Property

    Public Property V2() As Integer
        Get
            Return _V2
        End Get
        Set(ByVal value As Integer)
            _V2 = value
        End Set
    End Property

    Public Property V3() As Integer
        Get
            Return _V3
        End Get
        Set(ByVal value As Integer)
            _V3 = value
        End Set
    End Property

    Sub New()
        _V1 = -1
        _V2 = -1
        _V3 = -1
    End Sub

    Sub New(ByVal V1 As Integer, ByVal V2 As Integer, ByVal V3 As Integer)
        _V1 = V1
        _V2 = V2
        _V3 = V3
    End Sub

    Public Property Vertices() As Integer()
        Get
            Return New Integer() {_V1, _V2, _V3}
        End Get
        Set(ByVal value As Integer())
            _V1 = value(0)
            _V2 = value(1)
            _V3 = value(2)
        End Set
    End Property

End Class

Public Class sTextureEntry
    Dim _TextureID As String
    Dim _AlphaMapID As String
    Dim _Flags1 As Integer
    Dim _Flags2 As Integer
    Dim _Blending1 As Integer
    Dim _Blending2 As Integer
    Dim _TextureName As String

    Public Property TextureName() As String
        Get
            Return _TextureName
        End Get
        Set(ByVal value As String)
            _TextureName = value
        End Set
    End Property

    Public Property TextureID() As String
        Get
            Return _TextureID
        End Get
        Set(ByVal value As String)
            _TextureID = value
        End Set
    End Property

    Public Property AlphaMapID() As String
        Get
            Return _AlphaMapID
        End Get
        Set(ByVal value As String)
            _AlphaMapID = AlphaMapID
        End Set
    End Property

    Public Property Flags1() As Integer
        Get
            Return _Flags1
        End Get
        Set(ByVal value As Integer)
            _Flags1 = value
        End Set
    End Property

    Public Property Flags2() As Integer
        Get
            Return _Flags2
        End Get
        Set(ByVal value As Integer)
            _Flags2 = value
        End Set
    End Property

    Public Property Blending1() As Integer
        Get
            Return _Blending1
        End Get
        Set(ByVal value As Integer)
            _Blending1 = value
        End Set
    End Property

    Public Property Blending2() As Integer
        Get
            Return _Blending2
        End Get
        Set(ByVal value As Integer)
            _Blending2 = value
        End Set
    End Property

    Sub New(ByVal TextureID As String, ByVal AlphaMapID As String, ByVal Flags1 As Integer, ByVal Flags2 As Integer, ByVal Blending1 As Integer, ByVal Blending2 As Integer, Optional ByVal TextureName As String = "")
        _TextureID = TextureID
        _AlphaMapID = AlphaMapID
        _Flags1 = Flags1
        _Flags2 = Flags2
        _Blending1 = Blending1
        _Blending2 = Blending2
        _TextureName = TextureName
    End Sub

End Class

''' <summary>
''' Structure to hold Submesh information (Trianglelist + Textures)
''' </summary>
''' <remarks></remarks>
Public Class sSubMesh
    Dim _TextureList As List(Of sTextureEntry)
    Dim _OpenGLMeshID As Integer
    Dim _OpenGLBoneMeshID As Integer
    Dim _TriangleList As List(Of sTriangle)
    Dim _isADT As Boolean = False
    Dim _isWotLK As Boolean = False
    Dim _OpenGLVBOIndicesID As Integer

    Public Property isADT() As Boolean
        Get
            Return _isADT
        End Get
        Set(ByVal value As Boolean)
            _isADT = value
        End Set
    End Property

    Public Property isWotLK() As Boolean
        Get
            Return _isWotLK
        End Get
        Set(ByVal value As Boolean)
            _isWotlK = value
        End Set
    End Property

    Public Property TextureList() As List(Of sTextureEntry)
        Get
            Return _TextureList
        End Get
        Set(ByVal value As List(Of sTextureEntry))
            _TextureList = value
        End Set
    End Property

    Public Property TriangleList() As List(Of sTriangle)
        Get
            Return _TriangleList
        End Get
        Set(ByVal value As List(Of sTriangle))
            _TriangleList = value
        End Set
    End Property

    Public Property OpenGLVBOIndicesID() As Integer
        Get
            Return _OpenGLVBOIndicesID
        End Get
        Set(ByVal value As Integer)
            _OpenGLVBOIndicesID = value
        End Set
    End Property

    Public ReadOnly Property OpenGLVBOIndicesCount() As Integer
        Get
            Return _TriangleList.Count * 3
        End Get
    End Property

    Public ReadOnly Property VBOIndices() As Integer()
        Get
            Dim VT(_TriangleList.Count * 3 - 1) As Integer
            For i As Integer = 0 To _TriangleList.Count - 1
                VT(3 * i + 0) = _TriangleList(i).V1
                VT(3 * i + 1) = _TriangleList(i).V2
                VT(3 * i + 2) = _TriangleList(i).V3
            Next
            Return VT
        End Get
    End Property

    Public Property OpenGLMeshID() As Integer
        Get
            Return _OpenGLMeshID
        End Get
        Set(ByVal value As Integer)
            _OpenGLMeshID = value
        End Set
    End Property

    Public Property OpenGLBoneMeshID() As Integer
        Get
            Return _OpenGLBoneMeshID
        End Get
        Set(ByVal value As Integer)
            _OpenGLBoneMeshID = value
        End Set
    End Property

    Sub New()
        _TextureList = New List(Of sTextureEntry)
        _TriangleList = New List(Of sTriangle)
    End Sub

    Function GetTextureIDByName(ByVal Name As String) As String
        Dim Out As String = ""

        For Each te As sTextureEntry In _TextureList
            If te.TextureName = Name Then Out = te.TextureID
        Next

        Return Out
    End Function

End Class

''' <summary>
''' General functions that didn't really fit anywhere else...
''' </summary>
''' <remarks></remarks>
Public Class HelperFunctions

    ''' <summary>
    ''' Reverses a string (i.e. ABC -> CBA)
    ''' </summary>
    ''' <param name="value">The string to reverse</param>
    ''' <returns>The reversed string</returns>
    ''' <remarks></remarks>
    Public Function StrRev(ByVal value As String) As String
        If value.Length > 1 Then
            Dim workingValue As New System.Text.StringBuilder
            For position As Int32 = value.Length - 1 To 0 Step -1
                workingValue.Append(value.Chars(position))
            Next
            Return workingValue.ToString
        Else
            Return value
        End If
    End Function

    ''' <summary>
    ''' Get a zero delimited string starting at a specified position from an array of bytes
    ''' </summary>
    ''' <param name="Stack">The array of bytes to look in</param>
    ''' <param name="Pos">The position with the array to start looking</param>
    ''' <returns>The string found</returns>
    ''' <remarks></remarks>
    Public Function GetZeroDelimitedString(ByVal Stack() As Byte, ByVal Pos As UInt32) As String
        Dim out As String = ""

        While Stack(Pos) <> 0
            out &= Chr(Stack(Pos))
            Pos += 1
        End While

        Return out.Trim
    End Function

    ''' <summary>
    ''' Returns a zero delimited string starting at a specified position from a binary reader. 
    ''' </summary>
    ''' <param name="br">The binary reader to use as the source</param>
    ''' <param name="Pos">The position to start the string at</param>
    ''' <returns>The extracted string</returns>
    ''' <remarks></remarks>
    Public Function GetZeroDelimitedString(ByRef br As System.IO.BinaryReader, ByVal Pos As UInt32) As String
        Dim out As String = ""
        Dim c As Char

        br.BaseStream.Position = Pos
        c = br.ReadChar
        While Asc(c) <> 0
            out &= c
            c = br.ReadChar
        End While

        Return out.Trim
    End Function

    ''' <summary>
    ''' Returns a zero delimited string from a stream starting at the current position
    ''' </summary>
    ''' <param name="st">The stream to use as the source</param>
    ''' <returns>The extracted string</returns>
    ''' <remarks></remarks>
    Public Function GetZeroDelimitedString(ByRef st As IO.Stream) As String
        Dim out As String = ""
        Dim c As Byte


        c = st.ReadByte
        While c <> 0
            out &= Chr(c)
            c = st.ReadByte
        End While

        Return out.Trim
    End Function

    ''' <summary>
    ''' Get all zero delimited string from an array of bytes. The amount of zero chars between strings do not matter.
    ''' </summary>
    ''' <param name="Stack">Array of bytes containing the data</param>
    ''' <returns>Array of strings containing the strings found</returns>
    ''' <remarks></remarks>
    Public Function GetAllZeroDelimitedStrings(ByVal Stack() As Byte) As String()
        Dim d As String = Chr(0)
        Return System.Text.Encoding.ASCII.GetString(Stack).Split(d, options:=System.StringSplitOptions.RemoveEmptyEntries)
    End Function

    ''' <summary>
    ''' Get the extension of a filename (i.e. d:\this\is\a\test.doc -> doc)
    ''' </summary>
    ''' <param name="Filename">The filename to get the extension from</param>
    ''' <returns>Extension (i.e. "doc") without a leading .</returns>
    ''' <remarks></remarks>
    Public Function GetExtension(ByVal Filename As String) As String
        Dim dotPos As Integer = Filename.LastIndexOf(".")
        If dotPos >= 0 Then Return Filename.Substring(dotPos + 1)
        Return ""
    End Function

    ''' <summary>
    ''' Get the basename of a filename (i.e. d:\this\is\a\test.doc -> test)
    ''' </summary>
    ''' <param name="Filename">The filename to get the basename from</param>
    ''' <returns>Basename (i.e. "test") without a leading \ or trailing .</returns>
    ''' <remarks></remarks>
    Public Function GetBaseName(ByVal Filename As String)
        Dim dotPos As Integer = Filename.LastIndexOf(".")
        If dotPos >= 0 Then Filename = Filename.Substring(0, dotPos)

        Dim slashPos As Integer = Filename.LastIndexOf("\")
        If slashPos >= 0 Then Filename = Filename.Substring(slashPos + 1)
        Return Filename
    End Function

    ''' <summary>
    ''' Get the basepath of a filename (i.e. d:\this\is\a\test.doc -> d:\this\is\a)
    ''' </summary>
    ''' <param name="Filename">The filename to get the basepath from</param>
    ''' <returns>Basepath (i.e. "d:\this\is\a") without a trailing \</returns>
    ''' <remarks></remarks>
    Public Function GetBasePath(ByVal Filename As String)
        Dim slashPos As Integer = Filename.LastIndexOf("\")
        If slashPos >= 0 Then Return Filename.Substring(0, slashPos)
        Return ""
    End Function

    ''' <summary>
    ''' Get the filename component of a filename (i.e. d:\this\is\a\test.doc -> test.doc)
    ''' </summary>
    ''' <param name="Filename">The filename to get the filename component from</param>
    ''' <returns>Filename component (i.e. "test.doc") without a leading \</returns>
    ''' <remarks></remarks>
    Public Function GetFileName(ByVal Filename As String)
        Return GetBaseName(Filename) & "." & GetExtension(Filename)
    End Function

    ''' <summary>
    ''' Returns a text with only a-z, A-Z and 0-0 characters in it, all other characters are removed (i.e. This_is-a%test 98 -> Thisisatest98)
    ''' </summary>
    ''' <param name="Text">The text to clean up</param>
    ''' <returns>Cleaned up text (i.e. "i.e. Thisisatest98test")</returns>
    ''' <remarks></remarks>
    Public Function StringToPureAscii(ByVal Text As String)
        Dim Out As String = ""
        Dim CE As CharEnumerator = Text.GetEnumerator()
        Dim CleanChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

        While CE.MoveNext()
            If CleanChars.IndexOf(CE.Current) <> -1 Then
                Out &= CE.Current
            End If
        End While

        Return Out
    End Function

    ''' <summary>
    ''' Loads a model from the MPQ archive, can be either of the following: M2, WMO, ADT
    ''' </summary>
    ''' <param name="FileName">Name of the file to load</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadModelFromMPQ(ByVal FileName As String) As System.Collections.Generic.List(Of String)
        Dim out As New System.Collections.Generic.List(Of String)

        Select Case FileName.Substring(FileName.LastIndexOf(".") + 1).ToLower
            Case "m2", "mdx"
                ModelMgr.Clear()
                TextureMgr.Clear()
                AddM2Model(FileName, New sVector3(0, 0, 0), New sQuaternion(0, 0, 0, 1), 1.0F)

            Case "wmo"
                Dim FileNameWMO As String = FileName
                Dim WMO As OpenWMOOptions
                WMO = New OpenWMOOptions(FileNameWMO)
                WMO.ShowDialog()

            Case "adt"
                Dim FileNameADT As String = FileName
                Dim ADT As OpenADTOptions
                ADT = New OpenADTOptions(FileNameADT)
                ADT.ShowDialog()

            Case Else
                out.Add("Unknown file format: " & FileName.Substring(FileName.LastIndexOf(".") + 1).ToLower)
        End Select

        out.Add("Models:")
        For Each model In ModelMgr.Models
            out.Add(String.Format("Model: {0}", model.Value.Name))
            out.Add(String.Format("   Bones: {0}", ModelMgr.ModelData(model.Value.ModelDataID).Bones.Count))
            out.Add(String.Format("   Vertices: {0}", ModelMgr.ModelData(model.Value.ModelDataID).Vertices.Count))
            out.Add(String.Format("   Meshes: {0}", ModelMgr.ModelData(model.Value.ModelDataID).Meshes.Count))
            For Each mesh In ModelMgr.ModelData(model.Value.ModelDataID).Meshes
                out.Add(String.Format("      Tris: {0}, Textures: {1}", mesh.TriangleList.Count, mesh.TextureList.Count))
            Next
        Next

        out.Add("Textures:")
        For Each texture In TextureMgr.Textures
            out.Add(String.Format("   Texture [{0}] -> [{1}]", texture.Key, myMPQ.LocateMPQ(texture.Key)))
        Next

        frmOG.ResetView()
        Return out
    End Function

    Public Sub AddM2Model(ByVal Modelfile As String, ByVal Pos As sVector3, ByVal Rot As sQuaternion, ByVal Scale As Single)
        AddM2Model(Modelfile, New sMatrix4(Rot, Pos, Scale))
    End Sub

    Public Sub AddM2Model(ByVal Modelfile As String, ByVal ScaleRotTrans As sMatrix4)
        If ModelMgr.GetModelDataIDFromName(myHF.GetBaseName(Modelfile)) <> -1 Then
            ModelMgr.AddModelFromM2NoData(myHF.GetBaseName(Modelfile), ScaleRotTrans)
        Else
            Dim MD20 As New FileReaders.M2
            Dim SKIN As New FileReaders.SKIN
            Dim ANIM As New FileReaders.ANIM
            Dim FileNameMD20 As String = Modelfile.Substring(0, Modelfile.LastIndexOf(".")) + ".m2"
            Dim FileNameSKIN As String = Modelfile.Substring(0, Modelfile.LastIndexOf(".")) + "00.skin"
            Dim FileNameANIM As String = Modelfile.Substring(0, Modelfile.LastIndexOf(".")) + ".anim"

            MD20.Load(myMPQ.LoadFile(FileNameMD20), FileNameMD20)
            SKIN.Load(myMPQ.LoadFile(FileNameSKIN), FileNameSKIN)

            ModelMgr.AddModelFromM2(myHF.GetBaseName(Modelfile), MD20, SKIN, ANIM, ScaleRotTrans)
        End If
    End Sub

    Public Sub AddWMOModel(ByVal Modelfile As String, ByVal wPos As sVector3, ByVal wRot As sQuaternion)

        Dim WMO As New FileReaders.WMO

        'load base WMO
        WMO.LoadRoot(myMPQ.LoadFile(Modelfile))

        'load sub WMOs
        Dim BaseName As String = myHF.GetBasePath(Modelfile) & "\" & myHF.GetBaseName(Modelfile) & "_"
        Dim n As Integer = 0
        Dim SubFile As String = BaseName & n.ToString.PadLeft(3, "0") & ".wmo"
        While myMPQ.Locate(SubFile)
            WMO.LoadSub(myMPQ.LoadFile(SubFile))
            n += 1
            SubFile = BaseName & n.ToString.PadLeft(3, "0") & ".wmo"
        End While

        Dim mWMO As New sMatrix4(wRot, wPos, 1)
        ModelMgr.AddModelFromWMO(myHF.GetBaseName(Modelfile), WMO, mWMO)

        If WMO.Doodads.Count > 0 Then
            Dim id As Integer = WMO.DoodadSets(0).index
            Dim cn As Integer = WMO.DoodadSets(0).count

            For i As Integer = id To id + cn - 1
                With WMO.Doodads(i)
                    If .ModelFile > "" Then
                        'correct the position of the M2 by the WMO operations
                        Dim mM2 As New sMatrix4(.Orientation, .Position, .Scale)
                        myHF.AddM2Model(.ModelFile, mM2 * mWMO)
                    End If
                End With
            Next
        End If
    End Sub

    Public Function ShortToSingle(ByVal i As UInt16) As Single
        Dim sign As Integer = (i >> 15 And 1) * 2 - 1
        Dim exponent As Integer = (i >> 10 And &H1F) - 15
        Dim significand As Integer = (i And &H3FF)
        Dim out As Single = 1.0F * sign * significand * 10 ^ exponent
        'Debug.Print(String.Format("{0} {1} {2} {3}", sign, exponent, significand, out))

    End Function

    ''' <summary>
    ''' Scales a 64x64 alphamap to 2048x2048 using bicubic interpolation and edgecorrection
    ''' </summary>
    ''' <param name="Alphamap">The 64x64 alphamap to scale up</param>
    ''' <returns>The 2048x2048 scaled up alphamap</returns>
    ''' <remarks></remarks>
    Public Function NormalizeAlphaMap(ByVal Alphamap As Bitmap) As Bitmap
        Dim Out As New Bitmap(2048, 2048, Imaging.PixelFormat.Format32bppArgb)
        Dim Map As New Bitmap(256, 256, Imaging.PixelFormat.Format32bppArgb)
        Dim Gra1 As Graphics = Graphics.FromImage(Map)
        Dim Gra2 As Graphics = Graphics.FromImage(Out)
        Dim Bru As New TextureBrush(Alphamap)

        ' interpolation screws up on borders because it assumes black continuation...
        ' that's why I tile the image first and then use the rectangle I acutally want

        Bru.WrapMode = Drawing2D.WrapMode.TileFlipXY
        Gra1.FillRectangle(Bru, New Rectangle(0, 0, 256, 256))
        Gra2.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        Gra2.DrawImage(Map, New Rectangle(0, 0, 2048, 2048), New Rectangle(128, 128, 64, 64), GraphicsUnit.Pixel)

        Bru.Dispose()
        Gra2.Dispose()
        Gra1.Dispose()
        Map.Dispose()

        Return Out
    End Function

    ''' <summary>
    ''' Tiles a 256x256 texture to a new 2048x2048 bitmap
    ''' </summary>
    ''' <param name="Texture">The 256x256 texture to tile</param>
    ''' <returns>The 2048x2048 bitmap with the tiled texture</returns>
    ''' <remarks></remarks>
    Public Function NormalizeTexture(ByVal Texture As Bitmap) As Bitmap
        Dim Out As New Bitmap(2048, 2048, Imaging.PixelFormat.Format32bppArgb)
        Using Gra As Graphics = Graphics.FromImage(Out)
            Using Bru As New TextureBrush(Texture)
                Gra.FillRectangle(Bru, New Rectangle(0, 0, 2048, 2048))
            End Using
        End Using
        Return Out
    End Function

    ''' <summary>
    ''' Create a depthmap from an ADT
    ''' </summary>
    ''' <param name="ADT">The ADT to use as the data source</param>
    ''' <param name="Path">The path to save the depthmap to (optional)</param>
    ''' <returns>The depthmap (128x128)</returns>
    ''' <remarks></remarks>
    Public Function DepthmapFromADT(ByVal ADT As FileReaders.ADT, Optional ByVal Path As String = "") As Bitmap
        Dim dm(144, 144) As Single
        Dim zh As Single = -99999
        Dim zl As Single = 99999
        Dim z As Single


        '  0/0   0/1   0/2   0/3   0/4   0/5   0/6   0/7   0/8    <- 9x9 matrix
        '     0/0   0/1   0/2   0/3   0/4   0/5   0/6   0/7       <- 8x8 matrix
        '  1/0   1/1   1/2   1/3   1/4   1/5   1/6   1/7   1/8
        '     1/0   1/1   1/2   1/3   1/4   1/5   1/6   1/7
        '  2/0   2/1   2/2   2/3   2/4   2/5   2/6   2/7   2/8
        '     2/0   2/1   2/2   2/3   2/4   2/5   2/6   2/7
        '  3/0   3/1   3/2   3/3   3/4   3/5   3/6   3/7   3/8
        '     3/0   3/1   3/2   3/3   3/4   3/5   3/6   3/7
        '  4/0   4/1   4/2   4/3   4/4   4/5   4/6   4/7   4/8
        '     4/0   4/1   4/2   4/3   4/4   4/5   4/6   4/7
        '  5/0   5/1   5/2   5/3   5/4   5/5   5/6   5/7   5/8
        '     5/0   5/1   5/2   5/3   5/4   5/5   5/6   5/7
        '  6/0   6/1   6/2   6/3   6/4   6/5   6/6   6/7   6/8
        '     6/0   6/1   6/2   6/3   6/4   6/5   6/6   6/7
        '  7/0   7/1   7/2   7/3   7/4   7/5   7/6   7/7   7/8
        '     7/0   7/1   7/2   7/3   7/4   7/5   7/6   7/7
        '  8/0   8/1   8/2   8/3   8/4   8/5   8/6   8/7   8/8



        For x As Integer = 0 To 15
            For y As Integer = 0 To 15
                For i As Integer = 0 To 8
                    For j As Integer = 0 To 8
                        z = ADT.MCNKs(x, y).HeightMap9x9(i, j) + ADT.MCNKs(x, y).Position.Z
                        dm(x * 9 + i, y * 9 + j) = z
                        zl = IIf(z < zl, z, zl)
                        zh = IIf(z > zh, z, zh)
                    Next
                Next
            Next
        Next

        Dim Depthm As New Bitmap(144, 144, Imaging.PixelFormat.Format32bppArgb)
        Dim f1 As Single
        Dim f2 As Single = 255 / (zh - zl)

        For x As Integer = 0 To 143
            For y As Integer = 0 To 143
                f1 = (dm(x, y) - zl) * f2
                Depthm.SetPixel(x, y, Color.FromArgb(255, f1, f1, f1))
            Next
        Next

        If Path > "" Then Depthm.Save(Path + "Depthmap.png", Imaging.ImageFormat.Png)
        Return Depthm
    End Function

    ''' <summary>
    ''' Blend the textures of an ADT tile into one 2048x2048 bitmap
    ''' </summary>
    ''' <param name="ADT">The ADT to use as the data source</param>
    ''' <param name="x">The tile x coordinate</param>
    ''' <param name="y">The tile y coordinate</param>
    ''' <param name="Path">The path to save the intermediate results to (optional)</param>
    ''' <returns>The blended bitmap (2048x2048)</returns>
    ''' <remarks></remarks>
    Public Function Blend(ByVal ADT As FileReaders.ADT, ByVal x As Integer, ByVal y As Integer, Optional ByVal DumpLayers As Boolean = False, Optional ByVal DumpAlphas As Boolean = False, Optional ByVal Path As String = "") As Bitmap
        Dim BLP As New FileReaders.BLP

        Dim Alpha1 As Bitmap
        Dim Alpha2 As Bitmap
        Dim Alpha3 As Bitmap

        Dim Layer0 As Bitmap
        Dim Layer1 As Bitmap
        Dim Layer2 As Bitmap
        Dim Layer3 As Bitmap

        With ADT.MCNKs(x, y)

            'First iteration (create pure layers)
            Dim Tex0 As String = ADT.TextureFiles(.Layer(0).TextureID)
            Dim Tex1 As String = ADT.TextureFiles(.Layer(1).TextureID)
            Dim Tex2 As String = ADT.TextureFiles(.Layer(2).TextureID)
            Dim Tex3 As String = ADT.TextureFiles(.Layer(3).TextureID)
            Dim Do0 As Boolean = myMPQ.Locate(Tex0)
            Dim Do1 As Boolean = myMPQ.Locate(Tex1) And Not .AlphaMaps(1) Is Nothing
            Dim Do2 As Boolean = myMPQ.Locate(Tex2) And Not .AlphaMaps(2) Is Nothing
            Dim Do3 As Boolean = myMPQ.Locate(Tex3) And Not .AlphaMaps(3) Is Nothing

            If Do0 Then
                Layer0 = NormalizeTexture(BLP.LoadFromStream(myMPQ.LoadFile(Tex0), Tex0))
                If DumpLayers And Path > "" Then Layer0.Save(Path & "_layer0.png", Imaging.ImageFormat.Png)
            End If

            If Do1 Then
                Layer1 = NormalizeTexture(BLP.LoadFromStream(myMPQ.LoadFile(Tex1), Tex1))
                Alpha1 = NormalizeAlphaMap(.AlphaMaps(1))
                If DumpLayers And Path > "" Then Layer1.Save(Path & "_layer1.png", Imaging.ImageFormat.Png)
                If DumpAlphas And Path > "" Then Alpha1.Save(Path & "_alpha1.png", Imaging.ImageFormat.Png)
            End If

            If Do2 Then
                Layer2 = NormalizeTexture(BLP.LoadFromStream(myMPQ.LoadFile(Tex2), Tex2))
                Alpha2 = NormalizeAlphaMap(.AlphaMaps(2))
                If DumpLayers And Path > "" Then Layer2.Save(Path & "_layer2.png", Imaging.ImageFormat.Png)
                If DumpAlphas And Path > "" Then Alpha2.Save(Path & "_alpha2.png", Imaging.ImageFormat.Png)
            End If

            If Do3 Then
                Layer3 = NormalizeTexture(BLP.LoadFromStream(myMPQ.LoadFile(Tex3), Tex3))
                Alpha3 = NormalizeAlphaMap(.AlphaMaps(3))
                If DumpLayers And Path > "" Then Layer3.Save(Path & "_layer3.png", Imaging.ImageFormat.Png)
                If DumpAlphas And Path > "" Then Alpha3.Save(Path & "_alpha3.png", Imaging.ImageFormat.Png)
            End If

            'now combine the layers into ONE texture

            Dim Combined As New Bitmap(2048, 2048, Imaging.PixelFormat.Format32bppArgb)

            Dim c0 As Integer
            Dim c0r As Integer
            Dim c0g As Integer
            Dim c0b As Integer
            Dim c0a As Single
            Dim c1 As Integer
            Dim c1r As Integer
            Dim c1g As Integer
            Dim c1b As Integer
            Dim c1a As Single
            Dim c2 As Integer
            Dim c2r As Integer
            Dim c2g As Integer
            Dim c2b As Integer
            Dim c2a As Single
            Dim c3 As Integer
            Dim c3r As Integer
            Dim c3g As Integer
            Dim c3b As Integer
            Dim c3a As Single

            Dim stco As Integer
            Dim scco As IntPtr

            Dim stl0 As Integer
            Dim scl0 As IntPtr
            Dim stl1 As Integer
            Dim scl1 As IntPtr
            Dim stl2 As Integer
            Dim scl2 As IntPtr
            Dim stl3 As Integer
            Dim scl3 As IntPtr

            Dim sta1 As Integer
            Dim sca1 As IntPtr
            Dim sta2 As Integer
            Dim sca2 As IntPtr
            Dim sta3 As Integer
            Dim sca3 As IntPtr

            Dim CombinedData As Imaging.BitmapData

            Dim Layer0Data As Imaging.BitmapData
            Dim Layer1Data As Imaging.BitmapData
            Dim Layer2Data As Imaging.BitmapData
            Dim Layer3Data As Imaging.BitmapData

            Dim Alpha1Data As Imaging.BitmapData
            Dim Alpha2Data As Imaging.BitmapData
            Dim Alpha3Data As Imaging.BitmapData

            Dim Rect As New Rectangle(0, 0, 2048, 2048)

            CombinedData = Combined.LockBits(Rect, Imaging.ImageLockMode.WriteOnly, Imaging.PixelFormat.Format32bppArgb)
            stco = CombinedData.Stride
            scco = CombinedData.Scan0

            If Do0 Then
                Layer0Data = Layer0.LockBits(Rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                stl0 = Layer0Data.Stride
                scl0 = Layer0Data.Scan0
            End If
            If Do1 Then
                Layer1Data = Layer1.LockBits(Rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                stl1 = Layer1Data.Stride
                scl1 = Layer1Data.Scan0
                Alpha1Data = Alpha1.LockBits(Rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                sta1 = Alpha1Data.Stride
                sca1 = Alpha1Data.Scan0
            End If
            If Do2 Then
                Layer2Data = Layer2.LockBits(Rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                stl2 = Layer2Data.Stride
                scl2 = Layer2Data.Scan0
                Alpha2Data = Alpha2.LockBits(Rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                sta2 = Alpha2Data.Stride
                sca2 = Alpha2Data.Scan0
            End If
            If Do3 Then
                Layer3Data = Layer3.LockBits(Rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                stl3 = Layer3Data.Stride
                scl3 = Layer3Data.Scan0
                Alpha3Data = Alpha3.LockBits(Rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                sta3 = Alpha3Data.Stride
                sca3 = Alpha3Data.Scan0
            End If

            For lx As Integer = 0 To 2047
                For ly As Integer = 0 To 2047
                    c0 = Marshal.ReadInt32(scl0, stl0 * ly + 4 * lx)
                    c0r = c0 >> 16 And 255
                    c0g = c0 >> 8 And 255
                    c0b = c0 And 255

                    If .preWotLK Then
                        'color = ((layer0 * (1- alpha1) + layer1 * alpha1) * (1 - alpha2) + layer2 * alpha2) * (1 - alpha3) + layer3 * alpha3

                        If Do1 Then
                            c1 = Marshal.ReadInt32(scl1, stl1 * ly + 4 * lx)
                            c1a = Marshal.ReadByte(sca1, sta1 * ly + 4 * lx + 3) / 255

                            c1r = c1 >> 16 And 255
                            c1g = c1 >> 8 And 255
                            c1b = c1 And 255

                            c0r = c0r * (1 - c1a) + c1r * c1a
                            c0g = c0g * (1 - c1a) + c1g * c1a
                            c0b = c0b * (1 - c1a) + c1b * c1a

                        End If

                        If Do2 Then
                            c2 = Marshal.ReadInt32(scl2, stl2 * ly + 4 * lx)
                            c2a = Marshal.ReadByte(sca2, sta2 * ly + 4 * lx + 3) / 255

                            c2r = c2 >> 16 And 255
                            c2g = c2 >> 8 And 255
                            c2b = c2 And 255

                            c0r = c0r * (1 - c2a) + c2r * c2a
                            c0g = c0g * (1 - c2a) + c2g * c2a
                            c0b = c0b * (1 - c2a) + c2b * c2a

                        End If

                        If Do3 Then
                            c3 = Marshal.ReadInt32(scl3, stl3 * ly + 4 * lx)
                            c3a = Marshal.ReadByte(sca3, sta3 * ly + 4 * lx + 3) / 255

                            c3r = c3 >> 16 And 255
                            c3g = c3 >> 8 And 255
                            c3b = c3 And 255

                            c0r = c0r * (1 - c3a) + c3r * c3a
                            c0g = c0g * (1 - c3a) + c3g * c3a
                            c0b = c0b * (1 - c3a) + c3b * c3a

                        End If
                    Else
                        'color = layer0 * (1 - alpha1 - alpha2 - alpha3) + layer1 * alpha1 + layer2 * alpha2 + layer3 * alpha3

                        c1a = 0
                        c2a = 0
                        c3a = 0

                        If Do1 Then
                            c1 = Marshal.ReadInt32(scl1, stl1 * ly + 4 * lx)
                            c1a = Marshal.ReadByte(sca1, sta1 * ly + 4 * lx + 3) / 255
                            c1r = c1 >> 16 And 255
                            c1g = c1 >> 8 And 255
                            c1b = c1 And 255
                        End If

                        If Do2 Then
                            c2 = Marshal.ReadInt32(scl2, stl2 * ly + 4 * lx)
                            c2a = Marshal.ReadByte(sca2, sta2 * ly + 4 * lx + 3) / 255
                            c2r = c2 >> 16 And 255
                            c2g = c2 >> 8 And 255
                            c2b = c2 And 255
                        End If

                        If Do3 Then
                            c3 = Marshal.ReadInt32(scl3, stl3 * ly + 4 * lx)
                            c3a = Marshal.ReadByte(sca3, sta3 * ly + 4 * lx + 3) / 255
                            c3r = c3 >> 16 And 255
                            c3g = c3 >> 8 And 255
                            c3b = c3 And 255
                        End If

                        c0a = (1 - c1a - c2a - c3a)
                        c0r = c0r * c0a + c1r * c1a + c2r * c2a + c3r * c3a
                        c0g = c0g * c0a + c1g * c1a + c2g * c2a + c3g * c3a
                        c0b = c0b * c0a + c1b * c1a + c2b * c2a + c3b * c3a

                    End If

                    c0 = &HFF000000 + (c0r << 16) + (c0g << 8) + c0b

                    Marshal.WriteInt32(scco, stco * ly + 4 * lx, c0)
                Next
            Next

            Combined.UnlockBits(CombinedData)
            If Do0 Then
                Layer0.UnlockBits(Layer0Data)
                Layer0.Dispose()
            End If
            If Do1 Then
                Layer1.UnlockBits(Layer1Data)
                Layer1.Dispose()
                Alpha1.UnlockBits(Alpha1Data)
                Alpha1.Dispose()
            End If
            If Do2 Then
                Layer2.UnlockBits(Layer2Data)
                Layer2.Dispose()
                Alpha2.UnlockBits(Alpha2Data)
                Alpha2.Dispose()
            End If
            If Do3 Then
                Layer3.UnlockBits(Layer3Data)
                Layer3.Dispose()
                Alpha3.UnlockBits(Alpha3Data)
                Alpha3.Dispose()
            End If

            Return Combined
        End With
    End Function

End Class