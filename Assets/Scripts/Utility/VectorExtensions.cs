using UnityEngine;

namespace KT.Utility {
    public static class VectorExtensions {
        public static float SqrDistanceXZ(Vector3 v1, Vector3 v2) {
            return Vector2.SqrMagnitude(new Vector2(v1.x, v1.z) - new Vector2(v2.x, v2.z));
        }
    }
}