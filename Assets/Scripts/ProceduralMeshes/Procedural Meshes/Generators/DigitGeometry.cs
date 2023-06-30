using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators
{
    public struct DigitGeometry : IMeshGenerator
    {
        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1f, 1f, 0f));

        public int VertexCount => 4 * Resolution;

        public int IndexCount => 6 * Resolution;

        public int JobLength => Resolution;

        public int Resolution { get; set; }

        public void Execute<S>(int index, S streams) where S : struct, IMeshStreams
        {

            int vertexIndex = index * 4;
            int triangleIndex = index * 2;

            var xCoordinates = float2(index, index + 1f) / Resolution - 0.5f;
            var yCoordinates = float2(0, 0 + 1f) - 0.5f;

            var vertex = new Vertex();
            half h0 = half(0f), h1 = half(1f);

            // Set index of quad
            vertex.digitIndex = index;

            // Set 4 vertex of quad
            vertex.position.x = xCoordinates.x;
            vertex.position.y = yCoordinates.x;
            vertex.texCoord0 = h0;
            streams.SetVertex(vertexIndex + 0, vertex);

            vertex.position.x = xCoordinates.y;
            vertex.texCoord0 = half2(h1, h0);
            streams.SetVertex(vertexIndex + 1, vertex);

            vertex.position.x = xCoordinates.x;
            vertex.position.y = yCoordinates.y;
            vertex.texCoord0 = half2(h0, h1);
            streams.SetVertex(vertexIndex + 2, vertex);

            vertex.position.x = xCoordinates.y;
            vertex.texCoord0 = h1;
            streams.SetVertex(vertexIndex + 3, vertex);

            // Set 2 triangle of quad
            streams.SetTriangle(triangleIndex + 0, vertexIndex + int3(0, 2, 1));
            streams.SetTriangle(triangleIndex + 1, vertexIndex + int3(1, 2, 3));
        }
    }
}