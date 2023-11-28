using System;
using System.Linq;
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
            
            var validRainDrops = trailRenderers.Select(x => x.transform).Where((x) => {
                if (Vector2.SqrMagnitude(new Vector2 (x.position.x, x.position.z) - new Vector2(transform.position.x, transform.position.z)) < 4f * 4f) {
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
                
                lr.startColor = Color.red;
                lr.endColor = Color.red;

                for (int i = 0; i < pos.Length / 2; i++) {
                    Vector3 from = transform.position;
                    Vector3 to = pos[i*2];
                    float sqrDistance = Vector2.SqrMagnitude(new Vector2(from.x, from.z) - new Vector2(to.x, to.z));
                    if (sqrDistance < 4f * 4f) {
                        Vector3 dir = (pos[i*2+1] - pos[i*2]) / ball.GetComponent<RainTrailRenderer>().tickPeriod;
                        if (_ball.velocity.sqrMagnitude < dir.sqrMagnitude) {
                            
                            _ball.velocity += dir * Time.smoothDeltaTime * 30f;
                        }
                        break; // only one effect run per raindrop
                    }
                }
                
            }
        }
    }
}