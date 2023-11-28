using UnityEngine;

namespace KT {
    // verts  z          | areas V   U
    //      X   y        |         W
    //        3
    //      1   2
    public struct CollisionTriangle {
        
        public float Area { get; private set; }
        public Vector3 Normal { get; private set; }
        
        public Vector3 Center => (x + y + z) / 3f;
        
        public void DebugDraw(Color color) {
            Debug.DrawLine(x,y, color);
            Debug.DrawLine(z,y, color);
            Debug.DrawLine(x,z, color);
            
            Debug.DrawRay((x + y + z)/3f, Normal, color);
        }
        
        public CollisionTriangle(Vector3 x, Vector3 y, Vector3 z) {
            this.x = x;
            this.y = y;
            this.z = z;
            
            Vector3 yx = x - y;
            Vector3 yz = z - y;

            // Cross off yx yz
            float cross2d = yx.x * yz.z - yx.z * yz.x; 
            Vector3 cross = Vector3.Cross(yx, yz);
            // Area = cross.magnitude / 2f;
            Area = cross2d / 2f;
            Normal = -cross.normalized;
        }
        public Vector3 x{get; private set;}
        public Vector3 y{get; private set;}
        public Vector3 z{get; private set;}

        public float DistanceFromTrianglePlane(Vector3 p) {
            Vector3 px = p - x;
            return Vector3.Dot(px, Normal);
        }
        
        public bool InTriangle(Vector3 p) {
            Vector3 baryc = GetBarycCoordinates(p);
            // Debug.Log(baryc);
            if (baryc.x <0 || baryc.y < 0 || baryc.z < 0) {
                return false;
            }

            return true;
        }
        public Vector3 GetBarycCoordinates(Vector3 p) {
            Vector3 X = x - p;
            Vector3 Y = y - p;
            Vector3 Z = z - p;

            X = X.RemoveY();
            Y = Y.RemoveY();
            Z = Z.RemoveY();
            
            // Debug.DrawRay(p, X, Color.green, 1f);
            // Debug.DrawRay(p, Y, Color.green, 1f);
            // Debug.DrawRay(p, Z, Color.green, 1f);

            float U = Vector3.Cross(Y, Z).y / Area;
            float W = Vector3.Cross(X, Y).y / Area;
            float V = Vector3.Cross(Z, X).y / Area;
            return new Vector3(U, W, V);
            
            Vector3 xp = p - x;
            Vector3 xy = y - x;
            Vector3 xz = z - x;

            float v = xz.x * xp.z - xz.z * xp.x;
            v /= Area;
            float w = xp.x * xy.z - xp.z * xy.x;
            w /= Area;
            float u = 1f - v - w;
            
            return new Vector3(u, v, w);
        }
    }
}