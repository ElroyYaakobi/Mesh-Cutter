using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshManipulation
{
    public class MeshSpawner : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private GameObject[] spawnableObjects = new GameObject[0];

        [SerializeField]
        [Range(.1f, 5)]
        private float spawnFrequencyMin = 1.5f;
        [SerializeField]
        [Range(.1f, 5)]
        private float spawnFrequencyMax = 3f;

        private float nextSpawnTime;

        #endregion

        private void Update()
        {
            if(nextSpawnTime == 0)
            {
                nextSpawnTime = Time.time + Random.Range(spawnFrequencyMin, spawnFrequencyMax);
            }

            if (Time.time < nextSpawnTime) return;

            Spawn();

            nextSpawnTime = 0;
        }

        private void Spawn()
        {
            var spawnPosition = GetRandomSpawnPoint();

            var rndItemIndex = Random.Range(0, spawnableObjects.Length);
            var rndItem = spawnableObjects[rndItemIndex];

            if (!rndItem) throw new System.NullReferenceException($"Result random item is null! index:{rndItemIndex}");

            var go = Instantiate(rndItem);
            go.transform.position = spawnPosition;
            go.transform.eulerAngles = new Vector3(Random.Range(0, 360), 0, 0);
            go.transform.localScale *= Random.Range(0.8f, 1f);
            go.AddComponent<Rigidbody>();

            Destroy(go, 5f);
        }

        private Vector3 GetRandomSpawnPoint()
        {
            var sizeX = transform.localScale.x;

            var minX = transform.position.x - sizeX / 2f;
            var maxX = transform.position.x + sizeX / 2f;

            var position = transform.position;
            position.x = Random.Range(minX, maxX);

            return position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawCube(transform.position, transform.localScale);

            Gizmos.color = Color.white;
        }
    }
}