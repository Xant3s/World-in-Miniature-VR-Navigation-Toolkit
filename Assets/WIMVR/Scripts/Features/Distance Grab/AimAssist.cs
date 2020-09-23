// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Util;

namespace WIMVR.Features.Distance_Grab {
    /// <summary>
    /// Adds a laser pointer to hand to make it easier to aim using distance grabbing.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [DisallowMultipleComponent]
    public class AimAssist : MonoBehaviour {
        [Header("Aim Assist (Experimental)")]
        [Tooltip("The hand this script is attached to.")]
        public Hand hand = Hand.None;

        [Tooltip("The length of the laser pointer.")]
        [SerializeField] private float length = 10.0f;

        private LineRenderer lr;


        private void Awake() {
            lr = GetComponent<LineRenderer>();
        }

        private void Start() {
            var grabber = gameObject.GetComponentInParent<DistanceGrabber>();
            if (grabber == null || !grabber.enabled) {
                lr.enabled = false;
                this.enabled = false;
            }
        }

        private void Update() {
            var position = transform.position;
            lr.SetPosition(0, position);
            lr.SetPosition(1, position + transform.forward * length);
        }

        public void Disable() => lr.enabled = false;

        public void Enable() => lr.enabled = true;
    }
}