using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshManipulation.Utilities
{
    /// <summary>
    /// Some widely used mesh utilities!
    /// </summary>
    public static class MeshUtilities
    {
        private static Vector3 MaxVector = Vector3.one * Mathf.Infinity;
        private static Vector3 MinVector = Vector3.one * -Mathf.Infinity;

        /// <summary>
        /// Create bounds using specified vertices
        /// </summary>
        /// <param name="ltwMatrix">Local To World Matrix used to calculate world coordinates</param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Bounds CalculateWorldBounds(Matrix4x4 ltwMatrix, List<Vector3> vertices)
        {
            if (vertices.Count == 0) return new Bounds();

            // first element
            var firstVector = ltwMatrix.MultiplyPoint(vertices[0]);

            // start with largest possible values
            var minVector = firstVector;
            var maxVector = firstVector;

            for (var i = 1; i < vertices.Count; i++)
            {
                var vertex = ltwMatrix.MultiplyPoint(vertices[i]);

                if (vertex.x > maxVector.x) maxVector.x = vertex.x;
                if (vertex.y > maxVector.y) maxVector.y = vertex.y;
                if (vertex.z > maxVector.z) maxVector.z = vertex.z;

                if (vertex.x < minVector.x) minVector.x = vertex.x;
                if (vertex.y < minVector.y) minVector.y = vertex.y;
                if (vertex.z < minVector.z) minVector.z = vertex.z;
            }

            var bounds = new Bounds((maxVector + minVector) / 2, maxVector - minVector);

            return bounds;
        }
    }
}