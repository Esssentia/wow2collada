Imports Microsoft.DirectX

Namespace wow2collada.FileReaders

    Public Structure sTriangle
        Public VertexIndex1 As UInt16
        Public VertexIndex2 As UInt16
        Public VertexIndex3 As UInt16
    End Structure

    Public Structure sVertex
        Public Position As Vector3
        Public BoneWeights As Byte()
        Public BoneIndices As Byte()
        Public Normal As Vector3
        Public TextureCoords As Vector2
    End Structure

End Namespace
