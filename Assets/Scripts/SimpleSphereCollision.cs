using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KT {
    [DefaultExecutionOrder(10000)]
    [RequireComponent(typeof(Ball))]
    public class SimpleSphereCollision : MonoBehaviour {
        private Ball _ball;
        private float _radius;
        private void Awake() {
            _ball = GetComponent<Ball>();
            _radius = _ball.ballradius;
        }

        private void Update() {
            SimpleSphereCollision[] allOtherColliders = GetAllOtherColliders();
            
            for (int i = 0; i < allOtherColliders.Length; i++) {
                SimpleSphereCollision otherColl = allOtherColliders[i];
                float distance = Vector3.Distance(transform.position, otherColl.transform.position);
                if ((distance > _radius + otherColl._radius)) 
                    continue;
                Vector3 fromTo = otherColl.transform.position - transform.position;
                
                float overlapDistance = _radius + otherColl._radius - distance;
                _ball.transform.position -= fromTo.normalized * (overlapDistance/2f +0.1f);
                otherColl.transform.position += fromTo.normalized * (overlapDistance/2f +0.1f);
                
                _ball.velocity = -fromTo.normalized * _ball.velocity.magnitude;
                otherColl._ball.velocity = fromTo.normalized * otherColl._ball.velocity.magnitude;
                // _ball.velocity = Vector3.Reflect(_ball.velocity, fromTo.normalized);
                // otherColl._ball.velocity = Vector3.Reflect(otherColl._ball.velocity, -fromTo.normalized);
            }
        }

        private SimpleSphereCollision[] GetAllOtherColliders() {
            List<SimpleSphereCollision> a = GameObject.FindObjectsOfType<SimpleSphereCollision>().ToList();
            a.Remove(this);
            return a.ToArray();

        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}