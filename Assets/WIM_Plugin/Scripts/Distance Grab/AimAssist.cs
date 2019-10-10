using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    [RequireComponent(typeof(LineRenderer))]
    public class AimAssist : MonoBehaviour {
        [SerializeField] private OVRInput.RawButton grabButton;
        [SerializeField] private float length = 10.0f;

        void Start() {
            // Check if enabled.
            var grabber = gameObject.GetComponentInParent<DistanceGrabber>();
            if(grabber == null || !grabber.enabled) {
                gameObject.GetComponent<LineRenderer>().enabled = false;
                this.enabled = false;
            }
        }

        void Update() {
            var lr = GetComponent<LineRenderer>();
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position + transform.forward * length);
            lr.enabled = !OVRInput.Get(grabButton);
        }
    }
}