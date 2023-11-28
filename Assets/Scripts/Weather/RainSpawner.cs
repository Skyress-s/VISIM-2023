using System;
using UnityEngine;

namespace KT {
    public class RainSpawner : MonoBehaviour {
        [SerializeField] private GameObject raindropPrefab;
        
        [SerializeField]  private Vector3 spawnVolumeSize = Vector3.one * 4f;

        [SerializeField] private float spawnFrequency = 1f;
        private float SpawnPeriod => 1f / spawnFrequency;

        private float spawnTimer = 0f;
        
        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position,spawnVolumeSize);
        }


        private void Update() {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > SpawnPeriod) {
                spawnTimer -= SpawnPeriod;
                SpawnRaindrop();
                
            }
        }

        private void SpawnRaindrop() {
            Vector3 spawnPos = transform.position + new Vector3(
                UnityEngine.Random.Range(-spawnVolumeSize.x, spawnVolumeSize.x),
                UnityEngine.Random.Range(-spawnVolumeSize.y, spawnVolumeSize.y),
                UnityEngine.Random.Range(-spawnVolumeSize.z, spawnVolumeSize.z)
            ) / 2f;
            Instantiate(raindropPrefab, spawnPos, Quaternion.identity);
        }
    }
}