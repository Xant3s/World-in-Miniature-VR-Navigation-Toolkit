using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    [RequireComponent(typeof(LineRenderer))]
    public class AimAssist : MonoBehaviour {
        [SerializeField] private OVRInput.RawButton grabButton;
        [SerializeField] private float length = 10.0f;

        private LineRenderer lr;


        private void Awake() {
            lr = GetComponent<LineRenderer>();
        }

        private void Start() {
            // Check if enabled.
            var grabber = gameObject.GetComponentInParent<DistanceGrabber>();
            if(grabber == null || !grabber.enabled) {
                gameObject.GetComponent<LineRenderer>().enabled = false;
                this.enabled = false;
            }
        }

        private void Update() {
            var position = transform.position;
            lr.SetPosition(0, position);
            lr.SetPosition(1, position + transform.forward * length);
            lr.enabled = !OVRInput.Get(grabButton);
        }
    }
}