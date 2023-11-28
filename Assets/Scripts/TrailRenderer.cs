using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRenderer : MonoBehaviour
{
    [SerializeField] private float tickPeriod = 0.1f;
    
    Queue<Vector3> trailQueue = new Queue<Vector3>();

    // Backend DeBoor
    [SerializeField] private List<Vector3> c = new();
    [SerializeField] private List<float> t = new();
    [SerializeField] private int n = 0;
    
    
    private float tickTimer = 0f;
    private void Update() {
        tickTimer += Time.deltaTime;
        if (tickTimer > tickPeriod) {
            tickTimer -= tickPeriod;
            Tick();
        }


    }

    private void OnDrawGizmosSelected() {
        const int segments = 10;
        List<Vector3> points = new();
        for (int i = 0; i < segments; i++) {
            float t = (float)i / ((float)segments - 1);
            points.Add(EvaluateBSpline(t));
        }
        
        Vector3 startPos = transform.position;
        for (int i = 0; i < points.Count - 1; i++) {
            Debug.DrawLine(startPos + points[i], startPos + points[i+1], Color.blue);
        }
    }

    private void Tick() {
        trailQueue.Enqueue(transform.position);    
    }

    private int d = 2;
    /// <summary>
    /// Using De Boor's algorithm
    /// See Forelesningsnotater 7.7 Listing 7.5
    /// </summary>
    /// <returns></returns>
    Vector3 EvaluateBSpline(float x) {
        int my = FindKnowInterval(x);
        List<Vector3> a = new();
        for (int i = 0; i <= d * 4; i++) {
            a.Add(Vector3.zero);
        }

        for (int j = 0; j <= d; j++) {
            Debug.Log($"c Size : {c.Count}, Index: {my - j}");
            a[d - j] = c[my - j];
            // var f = a[d - j];
            // var g = c[my - j]; // Error here

        }

        for (int k = d; k > 0; k--) {
            int j = my - k;
            for (int i = 0; i < k; i++) {
                j++;
                float w = (x - t[j]) / (t[j + k] - t[j]);
                a[i] = a[i] * (1f - w) + a[i + 1] * w;
                // float alpha = (x - j) / (k + 1);
                // a[i] = (1 - alpha) * a[i] + alpha * a[i + 1];
            }
        }

        return a[0];

    }

    private int FindKnowInterval(float x) {
        int my = n - 1;
        while (x < t[my] && my > d) {
            my--;
        }

        return my;
    }
}
