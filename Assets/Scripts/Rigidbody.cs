using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace KT {
    public class Rigidbody : MonoBehaviour {
        public Vector3 velocity { get; private set; }
        public float mass { get; private set; }
        [field: SerializeField] public float gravityScale { get; private set; } = 1f;

        [SerializeField] private TriangleSurface _collider;

        [SerializeField] private float ballradius = 0.1f;

        [Tooltip("0,99 means almost no loss with happen, 0.01 means almost all velocity will be lost")]
        [Range(0.01f, 0.99f)] [SerializeField] private float falloff = 0.6f;
        
        
        // Events
        public UnityAction onCollision = delegate { };
        
        // Functions
        public void AddVelocity(Vector3 addVelocity) {
            velocity += addVelocity;
        }

        public void SetVelocity(Vector3 setVelocity) {
            velocity = setVelocity;
        }

        private void Start() {
            
        }

        private void Update() {
            Vector3 velBefore = velocity;
            // Debug.LogWarning("NewFrame");
            velocity += Time.deltaTime * Vector3.down * 9.81f * gravityScale;
            transform.position += velocity * Time.deltaTime;

            
            // Handle collision
            
            // FInd current triangles we are in
            
            // _collider.GetTriangleFromPositon(TODO)
            
            List<CollisionTriangle> list = _collider.GetTriangles();
            for (int i = 0; i < list.Count; i++) {
                CollisionTriangle triangle = list[i];
                if (triangle.InTriangle(transform.position)) {
                    
                    Debug.DrawLine(triangle.z, triangle.x, Color.blue, 1f);
                    Debug.DrawLine(triangle.x, triangle.y, Color.blue, 1f);
                    Debug.DrawLine(triangle.y, triangle.z, Color.blue, 1f);

                    if (CheckCollisionWithTriangle(ref triangle, out TriangleCollisionHit triangleHit, transform.position, ballradius)) {
                        Vector3 velocityAfter = velocity - (2f - 0.0f) * (Vector3.Dot(velocity, triangleHit.normal) * triangleHit.normal);
                    
                        // keeping "falloff" of the velocity away from the plane
                        var velNormal = Vector3.Dot(triangleHit.normal, velocityAfter);
                        velNormal *= ( 1f -falloff);
                        velocityAfter += -velNormal * triangleHit.normal;

                        Vector3 positionAfter = transform.position + triangleHit.normal * (ballradius - triangleHit.distanceFromPlane);
                        velocity = velocityAfter * 1f;
                        transform.position = positionAfter;
                        
                        // Invoking event. ? is null check operator -> Short for if (onCollision != null) { onCollision.Invoke(); }
                        onCollision?.Invoke();
                    }
                    
                    
                    break;
                }
                continue;
                if (CheckCollisionWithTriangle(ref triangle, out TriangleCollisionHit hit, transform.position, ballradius)) {
                    /*
                    if (i == 1) {
                        EditorApplication.isPaused = true;
                    }
                    */
                    // handle respons
                    
                    Vector3 velocityAfter = velocity - (2f - 0.0f) * (Vector3.Dot(velocity, hit.normal) * hit.normal);
                    
                    // keeping 60% of the velocity away from the plane
                    var velNormal = Vector3.Dot(hit.normal, velocityAfter);
                    velNormal *= ( 1f -0.6f);
                    velocityAfter += -velNormal * hit.normal;

                    Vector3 positionAfter = transform.position + hit.normal * (ballradius - hit.distanceFromPlane);
                    velocity = velocityAfter * 1f;
                    transform.position = positionAfter;
                }
            }
            // Debug.LogWarning($"Velocity : {velocity} | Length : {velocity.magnitude}");
            // Debug.LogWarning($"Position : {transform.position} | Length : {velocity.magnitude}");
            
            // Vector3 acceleration = (velocity - velBefore) / Time.deltaTime;
            // Debug.LogWarning($"Acceleration : {acceleration} | Length : {acceleration.magnitude}");
            // Debug.LogWarning($"Time : {Time.time}");
            
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, ballradius);
        }

        private bool CheckCollisionWithTriangle(ref CollisionTriangle tri,out TriangleCollisionHit hit, Vector3 ballPosition, float ballRadius) {
            bool inTriangle = tri.InTriangle(ballPosition);
            
            float distanceFromTrianglePlane = tri.DistanceFromTrianglePlane(ballPosition);
            distanceFromTrianglePlane = Mathf.Abs(distanceFromTrianglePlane);
            
            bool onPlane = distanceFromTrianglePlane < ballRadius;
            if (!onPlane || !inTriangle) {
                hit = new TriangleCollisionHit();
                
                return false;
            }

            hit = new TriangleCollisionHit();
            hit.normal = tri.Normal;
            hit.distanceFromPlane = distanceFromTrianglePlane;
            return true;
        }
        
    }
}