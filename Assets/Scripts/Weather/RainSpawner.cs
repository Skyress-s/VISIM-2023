using System;
using UnityEngine;

namespace KT {
    public class RainSpawner : MonoBehaviour {
        [SerializeField] private GameObject raindropPrefab;
        
        
        [SerializeField]  private Vector3 spawnVolumeSize = Vector3.one * 4f;


        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position,spawnVolumeSize);
        }
    }
}