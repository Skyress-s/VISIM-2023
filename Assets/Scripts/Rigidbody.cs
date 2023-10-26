using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KT {
    public class Rigidbody : MonoBehaviour {
        public Vector3 velocity { get; private set; }
        public float mass { get; private set; }
        [field: SerializeField] public float gravityScale { get; private set; } = 1f;

        [SerializeField] private TriangleSurface _collider;

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
            Debug.LogWarning("NewFrame");
            velocity += Time.deltaTime * Vector3.down * 9.81f * gravityScale;
            transform.position += velocity * Time.deltaTime;

            float radius = 0.01f;
            
            // Handle collision
            List<CollisionTriangle> list = _collider.GetTriangles();
            for (int i = 0; i < list.Count; i++) {
                CollisionTriangle triangle = list[i];
                if (CheckCollisionWithTriangle(ref triangle, out TriangleCollisionHit hit, transform.position, radius)) {
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

                    Vector3 positionAfter = transform.position + hit.normal * (radius - hit.distanceFromPlane);
                    velocity = velocityAfter * 1f;
                    transform.position = positionAfter;
                }
            }
            Debug.LogWarning($"Velocity : {velocity} | Length : {velocity.magnitude}");
            Debug.LogWarning($"Position : {transform.position} | Length : {velocity.magnitude}");
            
            Vector3 acceleration = (velocity - velBefore) / Time.deltaTime;
            Debug.LogWarning($"Acceleration : {acceleration} | Length : {acceleration.magnitude}");
            Debug.LogWarning($"Time : {Time.time}");
            
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