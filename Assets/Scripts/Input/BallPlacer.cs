using System;
using System.Collections;
using System.Collections.Generic;
using KT;
using UnityEngine;

public class BallPlacer : MonoBehaviour {
    [SerializeField] private TriangleSurface _surface;

    [SerializeField] private GameObject ballPrefab;
    
    public Camera Camera;
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            HandlePlaceBall();
        }
    }

    private void HandlePlaceBall() {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        // This is not very efficient, but this does not run every frame, so other placed like Rigidbody.cs can be optimized instead
        List<CollisionTriangle> triangles = _surface.GetTriangles();
        foreach (var collisionTriangle in triangles) {
            if (Intersect(collisionTriangle.x, collisionTriangle.y, collisionTriangle.z, ray)) {
                // Place a ball at location
                Instantiate(ballPrefab, collisionTriangle.Center + collisionTriangle.Normal, Quaternion.identity).SetActive(true);
            }
        }
    }

    // Reference for ray triangle intersection
    //https://discussions.unity.com/t/a-fast-triangle-triangle-intersection-algorithm-for-unity/126010
    
    /// <summary>
    /// Checks if the specified ray hits the triagnlge descibed by p1, p2 and p3.
    /// Möller–Trumbore ray-triangle intersection algorithm implementation.
    /// </summary>
    /// <param name="p1">Vertex 1 of the triangle.</param>
    /// <param name="p2">Vertex 2 of the triangle.</param>
    /// <param name="p3">Vertex 3 of the triangle.</param>
    /// <param name="ray">The ray to test hit for.</param>
    /// <returns><c>true</c> when the ray hits the triangle, otherwise <c>false</c></returns> 
    public static bool Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray) {
        float Epsilon = Mathf.Epsilon;
        
        // Vectors from p1 to p2/p3 (edges)
        Vector3 e1, e2;  
    
        Vector3 p, q, t;
        float det, invDet, u, v;
    
    
        //Find vectors for two edges sharing vertex/point p1
        e1 = p2 - p1;
        e2 = p3 - p1;
    
        // calculating determinant 
        p = Vector3.Cross(ray.direction, e2);
    
        //Calculate determinat
        det = Vector3.Dot(e1, p);
    
        //if determinant is near zero, ray lies in plane of triangle otherwise not
        if (det > -Epsilon && det < Epsilon) { return false; }
        invDet = 1.0f / det;
    
        //calculate distance from p1 to ray origin
        t = ray.origin - p1;
    
        //Calculate u parameter
        u = Vector3.Dot(t, p) * invDet;
    
        //Check for ray hit
        if (u < 0 || u > 1) { return false; }
    
        //Prepare to test v parameter
        q = Vector3.Cross(t, e1);
    
        //Calculate v parameter
        v = Vector3.Dot(ray.direction, q) * invDet;
    
        //Check for ray hit
        if (v < 0 || u + v > 1) { return false; }
    
        if ((Vector3.Dot(e2, q) * invDet) > Epsilon)
        { 
            //ray does intersect
            return true;
        }
    
        // No hit at all
        return false;
    }
}