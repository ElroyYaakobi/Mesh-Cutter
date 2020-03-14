using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshManipulation.MeshCutting;

namespace MeshManipulation
{
    /// <summary>
    /// Acts as a cutting plane that moves with the mouse.
    /// </summary>
    public class CutController : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer cutPlane = null;

        [SerializeField]
        [Range(1f, 100f)]
        private float minCutForce = 10f;

        [SerializeField]
        [Range(1f, 100f)]
        private float maxCutForce = 30f;

        [SerializeField]
        private bool isKinematicMovement;

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) ||
                !Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                                out var hit, 50f)) return;

            var hitFilter = hit.transform.GetComponent<MeshFilter>();
            if (!hitFilter) return;

            var hitFilterRigidbody = hit.transform.GetComponent<Rigidbody>();
            Destroy(hitFilter.GetComponent<Collider>()); // destroy collider!

            cutPlane.transform.position = hit.point;

            var cutFilters = MeshCutter.CutMeshFilter(hitFilter, cutPlane);

            float multiplier = -1;
            foreach (var cutFilter in cutFilters)
            {
                cutFilter.gameObject.AddComponent<BoxCollider>().isTrigger = true;

                if (isKinematicMovement)
                {
                    cutFilter.transform.position += Vector3.left * .5f * multiplier;
                    multiplier *= -1; // now change to positive

                    continue;
                }

                var rigidBody = cutFilter.gameObject.AddComponent<Rigidbody>();

                rigidBody.velocity = hitFilterRigidbody.velocity;
                rigidBody.AddForce(Vector3.left * Random.Range(minCutForce, maxCutForce) * multiplier);

                multiplier *= -1; // now change to positive
            }
        }
    }
}