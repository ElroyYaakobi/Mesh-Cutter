using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeshManipulation.MeshCutting
{
    /// <summary>
    /// Handles mesh cutting!
    /// This method also takes a mesh renderer and will calculate the cut plane using that.
    /// </summary>
    public static class MeshCutter
    {
        public static CutMesh[] PerformCut(Mesh mesh, Plane cutPlane)
        {
            var meshVertices = mesh.vertices;
            var meshIndices = mesh.GetIndices(0);
            var meshUV = mesh.uv;
            var meshNormals = mesh.normals;

            var leftMesh = new CutMesh();
            var rightMesh = new CutMesh();

            // iterate triangles
            for (var i = 0; i < meshIndices.Length - 2; i += 3)
            {
                var v1Index = meshIndices[i];
                var v2Index = meshIndices[i + 1];
                var v3Index = meshIndices[i + 2];

                var triangle = new MeshTriangle(v1Index, v2Index, v3Index, meshVertices, meshNormals, meshUV);

                var v1Side = cutPlane.GetSide(triangle.Vertices[0].Point);
                var v2Side = cutPlane.GetSide(triangle.Vertices[1].Point);
                var v3Side = cutPlane.GetSide(triangle.Vertices[2].Point);

                // add to proper sides
                if (v1Side && v2Side && v3Side)
                {
                    rightMesh.AddTriangle(triangle);
                    continue;
                }

                if (!v1Side && !v2Side && !v3Side)
                {
                    leftMesh.AddTriangle(triangle);
                    continue;
                }

                if (v1Side == v2Side)
                {
                    if (v1Side)
                    {
                        CreateTrianglesFromTwoVertices(rightMesh, 0, 1, 2, triangle, cutPlane);
                        CreateTrianglesFromOneVertices(leftMesh, 2, 0, 1, triangle, cutPlane);

                        continue;
                    }

                    CreateTrianglesFromTwoVertices(leftMesh, 0, 1, 2, triangle, cutPlane);
                    CreateTrianglesFromOneVertices(rightMesh, 2, 0, 1, triangle, cutPlane);

                    continue;
                }

                if (v2Side == v3Side)
                {
                    if (v2Side)
                    {
                        CreateTrianglesFromTwoVertices(rightMesh, 1, 2, 0, triangle, cutPlane);
                        CreateTrianglesFromOneVertices(leftMesh, 0, 1, 2, triangle, cutPlane);

                        continue;
                    }

                    CreateTrianglesFromTwoVertices(leftMesh, 1, 2, 0, triangle, cutPlane);
                    CreateTrianglesFromOneVertices(rightMesh, 0, 1, 2, triangle, cutPlane);

                    continue;
                }

                // last possibility, v3Side == v1Side

                if (v1Side)
                {
                    CreateTrianglesFromTwoVertices(rightMesh, 2, 0, 1, triangle, cutPlane);
                    CreateTrianglesFromOneVertices(leftMesh, 1, 2, 0, triangle, cutPlane);

                    continue;
                }

                CreateTrianglesFromTwoVertices(leftMesh, 2, 0, 1, triangle, cutPlane);
                CreateTrianglesFromOneVertices(rightMesh, 1, 2, 0, triangle, cutPlane);

            }

            return new[] { leftMesh, rightMesh };
        }
        private static void CreateTrianglesFromTwoVertices(CutMesh mesh, int v1Index, int v2Index, int v3Index, MeshTriangle triangle, Plane plane)
        {
            var v1In = triangle.Vertices[v1Index];
            var v2In = triangle.Vertices[v2Index];
            var v3Out = triangle.Vertices[v3Index];

            var v1ToV3Point = MeshVertex.InterpolateVertexOnPlane(v1In, v3Out, plane);
            var v2ToV3Point = MeshVertex.InterpolateVertexOnPlane(v2In, v3Out, plane);

            // create triangles

            mesh.AddTriangle(v1In, v2In, v1ToV3Point);
            mesh.AddTriangle(v1ToV3Point, v2In, v2ToV3Point);
        }
        private static void CreateTrianglesFromOneVertices(CutMesh mesh, int v1Index, int v2Index, int v3Index, MeshTriangle triangle, Plane plane)
        {
            var v1In = triangle.Vertices[v1Index];
            var v2Out = triangle.Vertices[v2Index];
            var v3Out = triangle.Vertices[v3Index];

            var v1ToV2Point = MeshVertex.InterpolateVertexOnPlane(v1In, v2Out, plane);
            var v1ToV3Point = MeshVertex.InterpolateVertexOnPlane(v1In, v3Out, plane);

            // create triangle

            mesh.AddTriangle(v1In, v1ToV2Point, v1ToV3Point);
        }

        #region Utility

        /// <summary>
        /// Cut a specified mesh filter using a provided plane renderer.
        /// </summary>
        /// <param name="meshFilter">The mesh filter you wish to cut</param>
        /// <param name="cutPlaneRenderer">The mesh renderer of a plane that cuts this mesh filter</param>
        /// <param name="hideMesh">Should the original mesh get hidden after being cut?</param>
        /// <returns></returns>
        public static MeshFilter[] CutMeshFilter(MeshFilter meshFilter, MeshRenderer cutPlaneRenderer, bool hideMesh = true)
        {
            var cutMeshes = PerformCut(meshFilter, cutPlaneRenderer);

            var meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            var material = meshRenderer?.sharedMaterial ?? null;

            var mFilters = new MeshFilter[cutMeshes.Length];
            for (int i = 0; i < cutMeshes.Length; i++)
            {
                var cutMesh = cutMeshes[i];

                var mesh = new Mesh
                {
                    vertices = cutMesh.Vertices.Select(x => x.Point).ToArray(),
                    normals = cutMesh.Vertices.Select(x => x.Normal).ToArray(),
                    uv = cutMesh.Vertices.Select(x => x.Uv).ToArray()
                };

                mesh.SetIndices(cutMesh.Triangles.ToArray(), MeshTopology.Triangles, 0);

                var go = new GameObject("side mesh " + i);
                go.AddComponent<MeshRenderer>().sharedMaterial = material;

                var mFilter = go.AddComponent<MeshFilter>();
                mFilter.sharedMesh = mesh;

                var goTransform = go.transform;
                goTransform.SetParent(meshFilter.transform, false);

                go.AddComponent<MeshDebug>(); // add the custom mesh debug utility!

                mFilters[i] = mFilter;
            }

            if (!hideMesh) return mFilters;

            meshRenderer.enabled = false;

            return mFilters;
        }

        public static CutMesh[] PerformCut(MeshFilter meshFilter, MeshRenderer cutPlaneRenderer)
        {
            if (!cutPlaneRenderer) throw new System.NullReferenceException("cut plane not specified");
            if (!meshFilter) throw new System.NullReferenceException("Mesh filter is null");

            var mesh = meshFilter.mesh;
            if (!mesh) throw new System.NullReferenceException("Mesh is null");

            var cutPlaneBounds = cutPlaneRenderer.bounds;

            var rightSize = cutPlaneRenderer.transform.right;
            rightSize.Scale(cutPlaneBounds.size);

            var p1 = cutPlaneBounds.min;
            var p2 = cutPlaneBounds.max;
            var p3 = p1 + rightSize;

            p1 = meshFilter.transform.InverseTransformPoint(p1);
            p2 = meshFilter.transform.InverseTransformPoint(p2);
            p3 = meshFilter.transform.InverseTransformPoint(p3);

            return PerformCut(mesh, new Plane(p1, p2, p3));
        }

        #endregion
    }
}