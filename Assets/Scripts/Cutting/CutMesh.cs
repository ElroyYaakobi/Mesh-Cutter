using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshManipulation.MeshCutting
{
    /// <summary>
    /// A class for all cut meshes
    /// </summary>
    public class CutMesh
    {
        #region Mesh Data 

        internal List<MeshVertex> Vertices = new List<MeshVertex>();
        internal List<int> Triangles = new List<int>();

        /// <summary>
        /// Old To New Map
        /// This map keeps track of the vertices ids from the original mesh and compares them with the id from
        /// this mesh. This is used to accurately determine what is a shared and not a shared vertex.
        /// </summary>
        private Dictionary<int, int> OTNMap = new Dictionary<int, int>();

        #endregion

        #region Methods

        /// <summary>
        /// Add a new vertex into the mesh and returns the vertex id
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private int AddNewVertex(MeshVertex vertex)
        {
            Vertices.Add(vertex);
            return Vertices.Count - 1;
        }

        /// <summary>
        /// Add a index of a triangle. Takes into account shared vertices.
        /// </summary>
        /// <param name="vertex"></param>
        private void AddNewTriangleVertex(MeshVertex vertex)
        {
            if(!OTNMap.TryGetValue(vertex.OriginalIndex, out int newVIndex))
            {
                newVIndex = AddNewVertex(vertex);

                // make sure we are messing with a valid share-able point and not a newly instanced one.
                if (vertex.OriginalIndex >= 0)
                {
                    OTNMap.Add(vertex.OriginalIndex, newVIndex);
                }
            }

            Triangles.Add(newVIndex);
        }

        /// <summary>
        /// Add a completely new trianlge to the mesh
        /// </summary>
        /// <param name="triangle"></param>
        internal void AddTriangle(MeshTriangle triangle)
        {
            var vertices = triangle.Vertices;

            if (vertices.Length != 3) throw new System.Exception("Triangle has invalid amount of vertices " + triangle.Vertices.Length);

            AddTriangle(vertices[0], vertices[1], vertices[2]);
        }

        /// <summary>
        /// Add a completely new trianlge to the mesh
        /// </summary>
        /// <param name="triangle"></param>
        internal void AddTriangle(MeshVertex v1, MeshVertex v2, MeshVertex v3)
        {
            AddNewTriangleVertex(v1);
            AddNewTriangleVertex(v2);
            AddNewTriangleVertex(v3);
        }

        #endregion
    }
}
