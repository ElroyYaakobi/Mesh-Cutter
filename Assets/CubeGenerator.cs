using UnityEngine;

namespace MeshManipulation
{
    /// <summary>
    /// A simple script that creates a cube with shared vertices from start to end manually.
    /// </summary>
    public class CubeGenerator : MonoBehaviour
    {
        private void Awake()
        {
            GenerateCube(transform.position, transform.rotation, transform.localScale);
        }

        private void GenerateCube(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var mesh = new Mesh();

            mesh.vertices = new[]
            {
                new Vector3(-1, -1, -1), // bottom backwards left
                new Vector3(1, -1, -1), // bottom backwards right
                new Vector3(1, -1, 1), // bottom forward right
                new Vector3(-1, -1, 1), // bottom forward left
                new Vector3(-1, 1, -1), // top backwards left
                new Vector3(1, 1, -1), // top backwards right
                new Vector3(1, 1, 1), // top forward right
                new Vector3(-1, 1, 1)
            }; // top forward left

            // triangles approach
            var indices = new[]
            {
                // bottom triangles
                0, 1, 2,
                2, 3, 0,

                // top triangles
                6, 5, 4,
                4, 7, 6,

                // back triangles
                5, 1, 0,
                0, 4, 5,

                // front triangles
                3, 2, 6,
                6, 7, 3,

                // right triangles
                6, 2, 1,
                1, 5, 6,

                // left triangles
                0, 3, 7,
                7, 4, 0

            };
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);

            /* Plane approach
            var indices = new[] {
                0, 1, 2, 3, // bottom plane
                0, 4, 5, 1, // back plane,
                1, 5, 6, 2, // right plane
                2, 6, 7, 3, // front plane
                3, 7, 4, 0, // left plane
                7, 6, 5, 4, // top plane
                };
    
            mesh.SetIndices(indices, MeshTopology.Quads, 0);
            */

            mesh.uv = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
            };

            var objInstance = new GameObject("mesh!");
            objInstance.transform.SetParent(transform, false);

            var mFilterInstance = objInstance.AddComponent<MeshFilter>();
            mFilterInstance.sharedMesh = mesh;

            objInstance.AddComponent<MeshRenderer>();
        }
    }
}