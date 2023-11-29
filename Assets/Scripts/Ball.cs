using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace KT {
    // TASK 3.1
    public class Ball : MonoBehaviour {
        public Vector3 velocity { get;  set; }
        public float mass { get; private set; }
        [field: SerializeField] public float gravityScale { get; private set; } = 1f;

        [FormerlySerializedAs("_collider")] [SerializeField] public TriangleSurface TriangleSurface;

        [SerializeField] private float ballradius = 0.1f;

        [Tooltip("0,99 means almost no loss with happen, 0.01 means almost all velocity will be lost")]
        [Range(0.01f, 0.99f)] [SerializeField] private float falloff = 0.6f;
        
        [ContextMenu("ResetVelocity")]
        public void ResetVelocity() {
            velocity = Vector3.zero;
        }
        
        // Events
        public UnityEvent onCollision = new();
        
        // Functions
        public void AddVelocity(Vector3 addVelocity) {
            velocity += addVelocity;
        }

        public void SetVelocity(Vector3 setVelocity) {
            velocity = setVelocity;
        }

        virtual protected void Update() {
            Vector3 velBefore = velocity;
            velocity += Time.deltaTime * Vector3.down * 9.81f * gravityScale;
            transform.position += velocity * Time.deltaTime;

            // Handle collision
            // ------------------------------------------------------------------
            
            // Find current triangles we are in
            CollisionTriangle? nullTriangle = TriangleSurface.GetTriangleFromPosition(transform.position);
            // Bail if not in triangle
            if (nullTriangle == null ) {
                return;
            }
            CollisionTriangle triangle = (CollisionTriangle)nullTriangle;

            // Mostly for redundancy, we alleready know we are in a triangle from TriangleSurface.GetTriangleFromPosition
            if (!triangle.InTriangle(transform.position)) {
                return;
            }

            if (IsCollidingWithPlane(ref triangle, out TriangleCollisionHit triangleHit, transform.position, ballradius)) {
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
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, ballradius);
        }

        private bool IsCollidingWithPlane(ref CollisionTriangle tri, out TriangleCollisionHit hit, Vector3 ballPosition,
            float ballRadius) {
            float distanceFromTrianglePlane = tri.DistanceFromTrianglePlane(ballPosition);
            // Commented out this as disabling it makes the ball bounce off the plane event if it went through on its iteration.
            // This is becouse if the Rigidbody is under, it will still count as a collision.
            // distanceFromTrianglePlane = Mathf.Abs(distanceFromTrianglePlane); 
            
            bool onPlane = distanceFromTrianglePlane < ballRadius;
            if (!onPlane) {
                hit = new TriangleCollisionHit();
                
                return false;
            }

            hit = new TriangleCollisionHit();
            hit.normal = tri.Normal;
            hit.distanceFromPlane = distanceFromTrianglePlane;
            return true;
        }
        
        private bool CheckCollisionWithTriangle(ref CollisionTriangle tri,out TriangleCollisionHit hit, Vector3 ballPosition, float ballRadius) {
            bool inTriangle = tri.InTriangle(ballPosition);
            
            float distanceFromTrianglePlane = tri.DistanceFromTrianglePlane(ballPosition);
            // Commented out this as disabling it makes the ball bounce off the plane event if it went through on its iteration.
            // This is becouse if the Rigidbody is under, it will still count as a collision.
            // distanceFromTrianglePlane = Mathf.Abs(distanceFromTrianglePlane); 
            
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