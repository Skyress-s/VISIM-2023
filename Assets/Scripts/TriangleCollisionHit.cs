using UnityEngine;

namespace KT {
    public struct TriangleCollisionHit {
        public Vector3 point;
        public Vector3 normal;
        public float distanceFromPlane;
    }
}