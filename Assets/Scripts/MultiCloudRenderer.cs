using System;
using System.Collections.Generic;
using UnityEngine;

namespace KT {
    [RequireComponent(typeof(PointCloudRenderer))]
    public class MultiCloudRenderer : MonoBehaviour {
        [SerializeField] TextAsset[] pointCloudData;

        List<PointCloudRenderer> _renderers = new List<PointCloudRenderer>();
        public int start, end;

        [ContextMenu("Update")]
        private void UpdateRenders() {
            for (int i = 0; i < start; i++) {
                _renderers[i].enabled = false;
            }
            
            for (int i = start; i < end; i++) {
                
                _renderers[i].enabled = true;
            }
            for (int i = end ; i < _renderers.Count; i++) {
                
                _renderers[i].enabled = false;
            }
        }
        private void Start() {
            var preset = GetComponent<PointCloudRenderer>();
            
            foreach (var textAsset in pointCloudData) {
                var renderer = gameObject.AddComponent<PointCloudRenderer>();
                _renderers.Add(renderer);
                renderer.pointCloudData = textAsset;
                renderer._mesh = preset._mesh;
                renderer._material = preset._material;
            }
        }
    }
}