﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshManipulation.MeshCutting
{
    /// <summary>
    /// A mesh triangle, has some data
    /// </summary>
    public class MeshTriangle
    {
        internal MeshVertex[] Vertices;

        public MeshTriangle(int v1Index, int v2Index, int v3Index, Vector3[] vertices, Vector3[] normals, Vector2[] uv)
        {
            Vertices = new[]
            {
                new MeshVertex(v1Index, vertices[v1Index], normals[v1Index], uv[v1Index]),
                new MeshVertex(v2Index, vertices[v2Index], normals[v2Index], uv[v2Index]),
                new MeshVertex(v3Index, vertices[v3Index], normals[v3Index], uv[v3Index])
            };
        }
    }
}