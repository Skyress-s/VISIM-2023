using System;
using System.Collections.Generic;
using UnityEngine;

namespace KT {
    /// <summary>
    /// BSpline implementation based on forelesningsnotater 7.7.
    /// </summary>
    
    [Serializable]
    public class BSpline {

        [SerializeField] private List<Vector3> c;
        [SerializeField] private List<float> t;
        [field:SerializeField] public int n { get; private set; }
        [SerializeField] private int d;

        private int minTValuesRequired => 2 * d + 2;
        public int MinPointsRequired => minTValuesRequired - d - 1; 
        
        
        
        public BSpline(int degree, List<Vector3> controlPoints) {
            d = degree;
            c = new( controlPoints);

            n = controlPoints.Count;
            
            // Set T Values
            t = CalculateTValues(d,n);
        }

        public void Initialize() {
            
        }

        private static List<float> CalculateTValues(int d, int n) {
            List<float >t = new List<float>();
            int totalTValues = d + n + 1;

            // Start Values
            for (int i = 0; i < d + 1; i++) {
                t.Add(0);
            }

            // Middle Values
            int numMiddlePoints = totalTValues - 2 * d - 2;
            for (int i = 0; i < numMiddlePoints; i++) {
                t.Add(i + 1);
            }

            // End Values
            for (int i = 0; i < d + 1; i++) {
                t.Add(numMiddlePoints + 1);
            }

            // Reparamatrizing t parameters : 0 -> totalTValues to 0 -> 1 as its easier to use.
            for (int i = 0; i < t.Count; i++) {
                t[i] = t[i] / (float)(numMiddlePoints + 1);
            }

            return t;
        }

        /// <summary>
        /// Using De Boor's algorithm
        /// See Forelesningsnotater 7.7 Listing 7.5
        /// </summary>
        /// <returns></returns>
        public Vector3 EvaluateBSpline(float x) {
            int my = FindKnowInterval(x);
            List<Vector3> a = new();
            for (int i = 0; i <= d * 4; i++) 
                a.Add(Vector3.zero);
            

            for (int j = 0; j <= d; j++) {
                a[d - j] = c[my - j];
            }

            for (int k = d; k > 0; k--) {
                int j = my - k;
                for (int i = 0; i < k; i++) {
                    j++;
                    float w = (x - t[j]) / (t[j + k] - t[j]);
                    a[i] = a[i] * (1f - w) + a[i + 1] * w;
                }
            }
            return a[0];
        }
        /// <summary>
        /// See Forelesningsnotater 7.7 Listing 7.4
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private int FindKnowInterval(float x) {
            int my = n - 1;
            while (x < t[my] && my > d) {
                my--;
            }

            return my;
        }

        /// <summary>
        /// Helper function to evaluate the spline at a number of points.
        /// </summary>
        /// <param name="numberOfSamples"></param>
        /// <returns></returns>
        public Vector3[] EvalueatePoints(int numberOfSamples) {
            
            Vector3[] points = new Vector3[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++) {
                points[i] = EvaluateBSpline((float)i / ((float)numberOfSamples - 1));
            }

            return points;
        }
        
        
    }
}