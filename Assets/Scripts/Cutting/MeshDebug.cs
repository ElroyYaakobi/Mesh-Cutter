using System.Collections.Generic;
using UnityEngine;
using MeshUtilities = MeshManipulation.Utilities.MeshUtilities;

namespace MeshManipulation.MeshCutting
{
    /// <summary>
    /// A cool utility that does some debug on meshes!
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class MeshDebug : MonoBehaviour
    {
        [System.Flags]
        public enum DebugOptions
        {
            Vertices = 1,
            Edges = 2,
            Triangles = 4,
            Bounds = 8,
            Normals = 16
        }

        #region Variables
        private MeshFilter MFilter { get; set; }
        private MeshRenderer MRenderer { get; set; }

        [SerializeField]
        [BitMask(typeof(DebugOptions))]
        private DebugOptions debugOptions = 0;

        #endregion

        private void Awake()
        {
            MFilter = GetComponent<MeshFilter>();
            MRenderer = GetComponent<MeshRenderer>();
        }

        public void OnDrawGizmos()
        {
            if (debugOptions == 0) return;

            if ((debugOptions & DebugOptions.Vertices) != 0)
                DrawVerticesGizmos(MFilter);
            if ((debugOptions & DebugOptions.Triangles) != 0)
                DrawTrianglesGizmos(MFilter);
            if ((debugOptions & DebugOptions.Bounds) != 0)
                DrawBoundsGizmos(MFilter);
            if ((debugOptions & DebugOptions.Normals) != 0)
                DrawNormalsGizmos(MFilter);

            Gizmos.color = Color.white;
        }

        #region Statics

        private static void DrawVerticesGizmos(MeshFilter mFilter)
        {
            if (!mFilter) throw new System.NullReferenceException("Mesh Filter is null");

            var mesh = mFilter.sharedMesh;
            if (!mesh) throw new System.NullReferenceException("Mesh is null");

            var vertices = mesh.vertices;

            Gizmos.color = Color.white;

            foreach (var vertex in vertices)
            {
                var vWorldPosition = mFilter.transform.TransformPoint(vertex);

                Gizmos.DrawSphere(vWorldPosition, .1f);
            }
        }

        private static void DrawTrianglesGizmos(MeshFilter mFilter)
        {
            if (!mFilter) throw new System.NullReferenceException("Mesh Filter is null");

            var mesh = mFilter.sharedMesh;
            if (!mesh) throw new System.NullReferenceException("Mesh is null");

            var vertices = mesh.vertices;
            var indices = mesh.GetIndices(0);
            var ltwMatrix = mFilter.transform.localToWorldMatrix;

            Gizmos.color = Color.yellow;

            for (var i = 0; i < indices.Length - 2; i += 3)
            {
                var v1 = ltwMatrix.MultiplyPoint3x4(vertices[indices[i]]);
                var v2 = ltwMatrix.MultiplyPoint3x4(vertices[indices[i + 1]]);
                var v3 = ltwMatrix.MultiplyPoint3x4(vertices[indices[i + 2]]);

                var edge1 = v2 - v1;
                var edge2 = v3 - v1;
                var edge3 = v3 - v2;

                Gizmos.DrawRay(v1, edge1);
                Gizmos.DrawRay(v1, edge2);
                Gizmos.DrawRay(v2, edge3);
            }

            Gizmos.color = Color.white;
        }

        private static void DrawBoundsGizmos(MeshFilter mFilter)
        {
            if (!mFilter) throw new System.NullReferenceException("Mesh Filter is null");

            var mesh = mFilter.sharedMesh;
            if (!mesh) throw new System.NullReferenceException("Mesh is null");

            var vertices = new List<Vector3>();
            mesh.GetVertices(vertices);

            var bounds = MeshUtilities.CalculateWorldBounds(mFilter.transform.localToWorldMatrix, vertices);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            Gizmos.color = Color.white;
        }

        private static void DrawNormalsGizmos(MeshFilter mFilter)
        {
            if (!mFilter) throw new System.NullReferenceException("Mesh Filter is null");

            var mesh = mFilter.sharedMesh;
            if (!mesh) throw new System.NullReferenceException("Mesh is null");

            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();

            mesh.GetVertices(vertices);
            mesh.GetNormals(normals);

            var indices = mesh.GetIndices(0);
            var ltwMatrix = mFilter.transform.localToWorldMatrix;

            Gizmos.color = Color.green;

            for (var i = 0; i < indices.Length - 2; i += 3)
            {
                var v1 = ltwMatrix.MultiplyPoint3x4(vertices[indices[i]]);
                var v2 = ltwMatrix.MultiplyPoint3x4(vertices[indices[i + 1]]);
                var v3 = ltwMatrix.MultiplyPoint3x4(vertices[indices[i + 2]]);

                var normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

                Gizmos.DrawRay(v1, normal);
                Gizmos.DrawRay(v1, normal);
                Gizmos.DrawRay(v2, normal);
            }

            Gizmos.color = Color.white;
        }

        #endregion

    }
}