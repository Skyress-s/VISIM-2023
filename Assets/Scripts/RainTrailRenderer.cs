using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KT;
using UnityEngine;

// TASK 3.3 TASK 3.4
[RequireComponent(typeof(LineRenderer))]
public class RainTrailRenderer : MonoBehaviour {
    private LineRenderer _lr;
    [SerializeField] public float tickPeriod = 1f;
    
    Queue<Vector3> trailQueue = new Queue<Vector3>();
    [SerializeField] private int goalPoints = 100;

    public Vector3 StartPoint => transform.position;
    public Vector3 MiddlePoint => trailQueue.Count >= 1 ? trailQueue.ElementAt( trailQueue.Count / 2) : transform.position;
    
    public Vector3 EndPoint => trailQueue.Count >= 1 ? trailQueue.Last() : transform.position;
    
    private float tickTimer = 0f;
    
    [SerializeField] private BSpline spline;

    private void Start() {
        trailQueue = new();
        _lr = GetComponent<LineRenderer>();
    }

    private void OnDrawGizmosSelected() {
        // // spline =  new BSpline(2, c);
        // if (spline.n < spline.MinPointsRequired) {
        //     return;
        // }
        //
        // Vector3[] positions = spline.EvalueatePoints(100);
        //
        // Vector3 startPos = transform.position;
        // startPos = Vector3.zero;
        // for (int i = 0; i < positions.Length - 1; i++) {
        //     Debug.DrawLine(startPos + positions[i], startPos + positions[i+1], Color.blue);
        // }
    }
    private void Update() {
        tickTimer += Time.deltaTime;
        if (tickTimer > tickPeriod) {
            tickTimer -= tickPeriod;
            Tick();

            UpdateSpline();
        }

        Color color = _lr.startColor;
        Color targetColor = Color.white;
        targetColor.r = targetColor.g = targetColor.b = 0.5f;
        color = Color.Lerp(color, targetColor, Time.deltaTime * 10f);
        _lr.startColor = _lr.endColor = color;

    }

    [ContextMenu("Tick")]
    private void Tick() {
        trailQueue.Enqueue(transform.position);
        if (trailQueue.Count > goalPoints) {
            trailQueue.Dequeue();
        }

        var list = trailQueue.ToList();
        // list.Add(transform.position);
        spline = new BSpline(2, list);
    }
    
    private void UpdateSpline() {
        if (spline.n < spline.MinPointsRequired) 
            return;
        
        // TASK 3.3 Lift spline (y) coordiante up to triangle surface
        Vector3[] positions = spline.EvalueatePoints(100);
        TriangleSurface triangleSurface = TriangleSurface.Instance;
        // Flatten points, and read y value From triangle surface.
        for (int i = 0; i < positions.Length; i++) {
            // positions[i].y = 0; // Flatten

            CollisionTriangle? triangle =  triangleSurface.GetTriangleFromPosition(positions[i]);
            if (triangle == null) {
                continue;
            }

            CollisionTriangle tri = (CollisionTriangle)triangle;
            Vector3 baryc = tri.GetBarycCoordinates(positions[i]);
            positions[i] = (tri.x * baryc.x + tri.y * baryc.y + tri.z * baryc.z) + tri.Normal * 0.3f;
        }
        _lr.positionCount = positions.Length;
        _lr.SetPositions(positions);
    }



}