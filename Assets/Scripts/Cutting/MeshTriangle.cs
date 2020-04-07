using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MeshManipulation.MeshCutting
{
    /// <summary>
    /// A mesh triangle, has some data
    /// </summary>
    public class MeshTriangle
    {
        internal MeshVertex[] Vertices;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MeshTriangle(int v1Index, int v2Index, int v3Index,
                                List<Vector3> vertices, List<Vector3> normals, List<Vector4> tangents, List<Vector2> uv)
        {
            Vertices = new[]
            {
                new MeshVertex(v1Index, vertices[v1Index], normals[v1Index], tangents[v1Index], uv[v1Index]),
                new MeshVertex(v2Index, vertices[v2Index], normals[v2Index], tangents[v2Index], uv[v2Index]),
                new MeshVertex(v3Index, vertices[v3Index], normals[v3Index], tangents[v3Index], uv[v3Index])
            };
        }
    }
}