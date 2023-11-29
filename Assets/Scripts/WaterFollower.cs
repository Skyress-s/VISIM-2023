using System;
using System.Collections;
using System.Linq;
using KT.Utility;
using UnityEngine;

namespace KT {
    [RequireComponent(typeof(Ball))]
    public class WaterFollower : MonoBehaviour {
        private Ball _ball;


        private void Awake() {
            _ball = GetComponent<Ball>();
        }
        float k_dragRadius = 4f * 4f;
        private void Update() {
            // Get closes tings in area
            var trailRenderers = FindObjectsOfType<RainDrop>();
            
            var validRainDrops = trailRenderers./*.Select(x => x.transform).*/Where((x) => {
                float d1 = VectorExtensions.SqrDistanceXZ(x.RainTrailRenderer.StartPoint, transform.position);
                float d2 = VectorExtensions.SqrDistanceXZ(x.RainTrailRenderer.MiddlePoint, transform.position);
                float d3 = VectorExtensions.SqrDistanceXZ(x.RainTrailRenderer.EndPoint, transform.position);
                
                Debug.DrawRay(x.RainTrailRenderer.StartPoint, Vector3.up, Color.red);
                Debug.DrawRay(x.RainTrailRenderer.MiddlePoint, Vector3.up, Color.red);
                Debug.DrawRay(x.RainTrailRenderer.EndPoint, Vector3.up, Color.red);
                
                const float k_detectionDragRadius = 12f * 12f;
                if (d1 < k_detectionDragRadius || d2 < k_detectionDragRadius || d3 < k_detectionDragRadius) {
                    return true;
                }
                return false;
            });
            //Vector3 closestPoint = ball.GetComponent<LineRenderer>().GetPositions().OrderBy(x => Vector3.Distance(x, transform.position)).First();
            var allBalls = validRainDrops.ToArray();
            foreach (var ball in allBalls) {
                // ball.GetComponent<MeshRenderer>().material.color = Color.red;
                LineRenderer lr = ball.GetComponent<LineRenderer>();
                
                Vector3[] pos = new Vector3[lr.positionCount];
                lr.GetPositions(pos);
                
                
                // StartCoroutine(DisableColor(lr, 0.1f));
                

                for (int i = 0; i < pos.Length / 2; i++) {
                    Vector3 from = transform.position;
                    Vector3 to = pos[i*2];
                    float sqrDistance = Vector2.SqrMagnitude(new Vector2(from.x, from.z) - new Vector2(to.x, to.z));
                    if (sqrDistance < 4f * 4f) {
                        Vector3 dir = (pos[i*2+1] - pos[i*2]) / ball.GetComponent<RainTrailRenderer>().tickPeriod;
                        // Vector3 c1 = Vector3.Cross(dir, _ball.velocity);
                        // Vector3 c2 = Vector3.Cross(dir, c1);
                        //     
                        // _ball.velocity += c2.normalized * dir.magnitude * Time.deltaTime;
                        
                        if (_ball.velocity.sqrMagnitude < dir.sqrMagnitude * 2 || Vector3.Dot(_ball.velocity.normalized, dir.normalized) < 0f) {

                            _ball.velocity += dir * Time.deltaTime * 30f;
                            lr.startColor = Color.red;
                            lr.endColor = Color.red;
                        }
                        break; // only one effect run per raindrop
                    }
                }
                
            }

        }
        // private IEnumerator DisableColor(LineRenderer lineRenderer, float waitDuration) {
        //     yield return new Wait();
        //     lineRenderer.startColor = Color.white;
        //     lineRenderer.endColor = Color.white;
        // }
    }
}