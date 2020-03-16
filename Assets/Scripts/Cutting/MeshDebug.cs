using System.Collections;
using System.Collections.Generic;
using MeshManipulation.Utilities;
using UnityEngine;

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
            Bounds = 8
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
            if ((debugOptions & DebugOptions.Bounds) != 0)
                DrawBoundsGizmos(MFilter);

            Gizmos.color = Color.white;
        }

        #region Statics

        private static void DrawVerticesGizmos(MeshFilter mFilter)
        {
            if (!mFilter) throw new System.NullReferenceException("Mesh Filter is null");

            var mesh = mFilter.sharedMesh;
            if (!mesh) throw new System.NullReferenceException("Mesh is null");

            var vertices = mesh.vertices;
            var indices = mesh.GetIndices(0);

            foreach (var vIndex in indices)
            {
                var vertex = vertices[vIndex];
                var vWorldPosition = mFilter.transform.TransformPoint(vertex);

                Gizmos.color = Color.white;
                Gizmos.DrawSphere(vWorldPosition, .1f);
            }
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

        #endregion

    }
}