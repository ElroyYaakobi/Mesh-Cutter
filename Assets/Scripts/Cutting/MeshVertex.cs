using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshManipulation.MeshCutting
{
    public struct MeshVertex
    {
        public int OriginalIndex;
        public Vector3 Point;
        public Vector3 Normal;
        public Vector2 Uv;

        public MeshVertex(int originalIndex, Vector3 point, Vector3 normal, Vector2 uv)
        {
            OriginalIndex = originalIndex;
            Point = point;
            Normal = normal;
            Uv = uv;
        }

        /// <summary>
        /// Calculate an interpolated intersection vertex on a plane using two edge points
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static MeshVertex InterpolateVertexOnPlane(MeshVertex v1, MeshVertex v2, Plane plane)
        {
            var v1Point = v1.Point;
            var v2Point = v2.Point;

            var dir = (v2Point - v1Point).normalized;
            var ray = new Ray(v1Point, dir);

            if (!plane.Raycast(ray, out float intersectionDistance))
                throw new System.Exception($"Can't get point between two edge points {v1Point} {v2Point}");

            var point = ray.GetPoint(intersectionDistance);
            var normal = v2.Normal;

            // calculate interpolated uv
            var lerpThreshold = intersectionDistance / Vector3.Distance(v1Point, v2Point);
            var uv = Vector2.Lerp(v1.Uv, v2.Uv, lerpThreshold);

            return new MeshVertex(-1, point, normal, uv);
        }
    }

}