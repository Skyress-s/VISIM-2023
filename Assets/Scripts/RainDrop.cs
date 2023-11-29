using System;
using UnityEngine;

namespace KT {
    [RequireComponent(typeof(RainTrailRenderer))]
    public class RainDrop : Ball {
        public RainTrailRenderer RainTrailRenderer { get; private set; }
        private void Awake() {
            RainTrailRenderer = GetComponent<RainTrailRenderer>();
            
        }

        protected override void Update() {
            base.Update();

            if (transform.position.y < -100f) {
                Destroy(gameObject);
            }
        }
    }
}