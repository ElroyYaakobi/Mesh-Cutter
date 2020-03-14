using System.Collections.Generic;

namespace MeshManipulation.MeshCutting
{
    /// <summary>
    /// A class for all cut meshes
    /// </summary>
    public class CutMesh
    {
        #region Mesh Data

        internal List<MeshVertex> Vertices { get; } = new List<MeshVertex>();
        internal MeshSubMesh[] SubMeshes { get; }

        /// <summary>
        /// Old To New Map
        /// This map keeps track of the vertices ids from the original mesh and compares them with the id from
        /// this mesh. This is used to accurately determine what is a shared and not a shared vertex.
        /// </summary>
        private Dictionary<int, int> OldToNewMap { get; } = new Dictionary<int, int>();

        #endregion

        #region Constructors

        public CutMesh(int subMeshesCount)
        {
            // populate subMeshes references
            SubMeshes = new MeshSubMesh[subMeshesCount];
            for (var subMesh = 0; subMesh < subMeshesCount; subMesh++)
            {
                SubMeshes[subMesh] = new MeshSubMesh();
            }
        }

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
        ///
        /// NOTE: Assumes you already verified that the subMesh index is in bounds!
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="subMesh">target subMesh of the indices</param>
        private void AddNewTriangleVertex(MeshVertex vertex, int subMesh)
        {
            if (!OldToNewMap.TryGetValue(vertex.OriginalIndex, out var newVIndex))
            {
                newVIndex = AddNewVertex(vertex);

                // make sure we are messing with a valid share-able point and not a newly instanced one.
                if (vertex.OriginalIndex >= 0)
                {
                    OldToNewMap.Add(vertex.OriginalIndex, newVIndex);
                }
            }

            // Assumes you already verified that the subMesh index is in bounds! 
            SubMeshes[subMesh].AddIndices(newVIndex);
        }

        /// <summary>
        /// Add a completely new trianlge to the mesh
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="subMesh">target subMesh</param>
        internal void AddTriangle(MeshTriangle triangle, int subMesh)
        {
            var vertices = triangle.Vertices;

            if (vertices.Length != 3) throw new System.Exception("Triangle has invalid amount of vertices " + triangle.Vertices.Length);

            AddTriangle(vertices[0], vertices[1], vertices[2], subMesh);
        }

        /// <summary>
        /// Add a completely new triangle to the mesh
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="subMesh">target subMesh</param>
        internal void AddTriangle(MeshVertex v1, MeshVertex v2, MeshVertex v3, int subMesh)
        {
            if(subMesh >= SubMeshes.Length)
                throw new System.IndexOutOfRangeException($"SubMesh index is out of bounds! total: {SubMeshes.Length} attempt: {subMesh}");

            AddNewTriangleVertex(v1, subMesh);
            AddNewTriangleVertex(v2, subMesh);
            AddNewTriangleVertex(v3, subMesh);
        }

        #endregion

    }
}
