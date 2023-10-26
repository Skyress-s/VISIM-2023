using UnityEngine;

namespace KT {
    public static class ExtensionMethods {
        public static float Sum(this Vector3 vec) {
            return vec.x + vec.y + vec.z;
        }

        public static Vector3 RemoveY(this Vector3 vector3) {
            vector3.y = 0f;
            return vector3;
        }

        public static Color RGBmul(this Color color, float multiply) {
            color.r *= multiply;
            color.g *= multiply;
            color.b *= multiply;
            return color;
        }
    }
}