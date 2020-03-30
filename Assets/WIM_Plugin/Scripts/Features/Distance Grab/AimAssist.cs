// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIM_Plugin {
    /// <summary>
    /// Adds a laser pointer to hand to make it easier to aim using distance grabbing.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [DisallowMultipleComponent]
    public class AimAssist : MonoBehaviour {
        [Header("Aim Assist (Experimental)")]
        [Tooltip("The hand this script is attached to.")]
        [SerializeField] private Hand hand;

        [Tooltip("The length of the laser pointer.")]
        [SerializeField] private float length = 10.0f;

        private LineRenderer lr;


        private void Awake() {
            lr = GetComponent<LineRenderer>();
        }

        private void OnEnable() {
            if (hand == Hand.LeftHand) {
                MiniatureModel.OnLeftGrabButtonDown += GrabButtonDown;
                MiniatureModel.OnLeftGrabButtonUp += GrabButtonUp;
            }
            else if (hand == Hand.RightHand) {
                MiniatureModel.OnRightGrabButtonDown += GrabButtonDown;
                MiniatureModel.OnRightGrabButtonUp += GrabButtonUp;
            }
        }

        private void OnDisable() {
            if (hand == Hand.LeftHand) {
                MiniatureModel.OnLeftGrabButtonDown -= GrabButtonDown;
                MiniatureModel.OnLeftGrabButtonUp -= GrabButtonUp;
            }
            else if (hand == Hand.RightHand) {
                MiniatureModel.OnRightGrabButtonDown -= GrabButtonDown;
                MiniatureModel.OnRightGrabButtonUp -= GrabButtonUp;
            }
        }

        private void GrabButtonDown(WIMConfiguration config, WIMData data) {
            lr.enabled = false;
        }

        private void GrabButtonUp(WIMConfiguration config, WIMData data) {
            lr.enabled = true;
        }

        private void Start() {
            var grabber = gameObject.GetComponentInParent<DistanceGrabber>();
            if (grabber == null || !grabber.enabled) {
                gameObject.GetComponent<LineRenderer>().enabled = false;
                this.enabled = false;
            }
        }

        private void Update() {
            var position = transform.position;
            lr.SetPosition(0, position);
            lr.SetPosition(1, position + transform.forward * length);
        }
    }
}