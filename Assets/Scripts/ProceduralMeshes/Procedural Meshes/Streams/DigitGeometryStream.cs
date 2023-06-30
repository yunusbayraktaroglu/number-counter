using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralMeshes.Streams {

    public struct DigitGeometryStream : IMeshStreams {

		[StructLayout(LayoutKind.Sequential)]
		struct Stream0 {
			public float3 position;
			public half2 texCoord0;
			public float digitIndex;
		}

		[NativeDisableContainerSafetyRestriction]
		NativeArray<Stream0> stream0;

		[NativeDisableContainerSafetyRestriction]
		NativeArray<TriangleUInt16> triangles;

		public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
		{
			var descriptor = new NativeArray<VertexAttributeDescriptor>(
				3, Allocator.Temp, NativeArrayOptions.UninitializedMemory
			);

			descriptor[0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3);
			descriptor[1] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, dimension: 2);
			descriptor[2] = new VertexAttributeDescriptor(VertexAttribute.TexCoord1, dimension: 1);

			meshData.SetVertexBufferParams(vertexCount, descriptor);
			descriptor.Dispose();

			meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt16);

			meshData.subMeshCount = 1;
			meshData.SetSubMesh(
				0, 
				new SubMeshDescriptor(0, indexCount) {
					bounds = bounds,
					vertexCount = vertexCount
				},
				MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices
			);

			stream0 = meshData.GetVertexData<Stream0>();
			triangles = meshData.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetVertex(int index, Vertex vertex)
		{
            stream0[index] = new Stream0
            {
                position = vertex.position,
                texCoord0 = vertex.texCoord0,
                digitIndex = vertex.digitIndex
            };
        }

		public void SetTriangle(int index, int3 triangle) => triangles[index] = triangle;
	}
}