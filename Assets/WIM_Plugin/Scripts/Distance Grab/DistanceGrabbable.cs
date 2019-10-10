using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Rigidbody))]
    public class DistanceGrabbable : MonoBehaviour {
        public Transform Target { get; set; }

        public bool IsBeingGrabbed { get; set; }

        public bool HightlightFX {
            get => hightlightFX;
            set {
                hightlightFX = value;
                if(GetComponent<Renderer>()) {
                    GetComponent<Renderer>().material =
                        hightlightFX ? Resources.Load<Material>("Materials/outline") : defaultMaterial;
                }
                else {
                    // WIM
                    //for(var i = 0; i<transform.GetChild(0).childCount; i++) {
                    //    var child  = transform.GetChild(0).GetChild(i);
                    //    var renderer = child.GetComponent<Renderer>();
                    //    if (!renderer) continue;
                    //    renderer.material = hightlightFX ? Resources.Load<Material>("Materials/outline") : defaultMaterial;
                    //}
                }
            }
        }

        private bool hightlightFX;

        public float SnapSpeed { get; set; } = 1f;
        public float MinDistance { get; set; } = .1f;

        private Material defaultMaterial;
        private Rigidbody rb;

        private void Awake() {
            defaultMaterial = GetComponentInChildren<Renderer>().material;
            rb = GetComponent<Rigidbody>();
        }

        private void Update() {
            HightlightFX = false;
            if(!IsBeingGrabbed || !Target) return;

            if(Vector3.Distance(Target.position, transform.position) < MinDistance) {
                IsBeingGrabbed = false;
                Target = null;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else {
                var step = SnapSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, Target.position, step);
            }
        }
    }
}