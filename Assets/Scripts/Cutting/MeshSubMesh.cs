using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MeshManipulation.MeshCutting
{
    /// <summary>
    /// A submesh class that includes references to the submesh indices.
    /// </summary>
    public class MeshSubMesh
    {
        public List<int> TrianglesIndices { get; } = new List<int>();

        public void AddIndices(int vIndex)
        {
            TrianglesIndices.Add(vIndex);
        }
    }
}
