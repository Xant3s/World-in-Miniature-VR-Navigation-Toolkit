// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR {
    /// <summary>
    /// Can be used to pull distance grabbable objects that are not in range for regular grabbing.
    /// </summary>
    public class DistanceGrabber : MonoBehaviour {
        [Header("Distance Grabber (Experimental)")]
        [Tooltip("Hand this script is attached to.")]
        [SerializeField] private Hand hand;

        [Tooltip("Start point of the laser pointer.")]
        [SerializeField] private Transform start;

        [Tooltip("Distance grabbing will be disabled if within specified distance to miniature model")]
        [SerializeField] private float requieredDistanceToWIM = .5f;

        [Tooltip("Specifies how fast objects are pulled towards hand.")]
        [SerializeField] private float snapSpeed = 10f;

        [Tooltip("Stop pulling objects that are within specified distance to hand.")]
        [SerializeField] private float minDistance = .5f;

        [Tooltip("Automatically disable while this hand is touching the miniature model.")]
        [SerializeField] private bool disableWhileInWIM = true;

        private AimAssist aimAssist;
        private LineRenderer lineRenderer;
        private Transform WIM;
        private bool grabButtonPressed;
        private bool grabStartedThisFrame;
        private bool isDisabled;
        private bool isInsideWIM;


        private void Awake() {
            if (!disableWhileInWIM || !this.enabled) return;
            aimAssist = gameObject.GetComponentInChildren<AimAssist>();
            lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
            WIM = GameObject.FindWithTag("WIM").transform;
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

        private void LateUpdate() {
            var distanceToWIM = Vector3.Distance(WIM.position, transform.position);
            SetEnable(!(distanceToWIM < requieredDistanceToWIM) && !isInsideWIM);
            if(isDisabled) return;


            var allLayersButHands = ~((1 << LayerMask.NameToLayer("Hands")) | (1 << Physics.IgnoreRaycastLayer));
            if (Physics.Raycast(transform.position, start.forward, out var hit, Mathf.Infinity, allLayersButHands)) {
                var grabbable = hit.transform.GetComponent<DistanceGrabbable>();
                if(!grabbable || hit.transform.GetComponent<OVRGrabbable>().isGrabbed) {
                    grabStartedThisFrame = false;
                    return;
                }
                if(!grabStartedThisFrame && grabButtonPressed) return;
                grabbable.HighlightFX = true;
                grabbable.IsBeingGrabbed = grabButtonPressed;
                if(!grabStartedThisFrame) return;
                grabStartedThisFrame = false;
                if(grabButtonPressed) {
                    grabbable.MinDistance = minDistance;
                    grabbable.SnapSpeed = snapSpeed;
                    grabbable.Target = transform;
                }
            }
        }

        private void GrabButtonDown(WIMConfiguration config, WIMData data) {
            grabButtonPressed = true;
            grabStartedThisFrame = true;
        }

        private void GrabButtonUp(WIMConfiguration config, WIMData data) {
            grabButtonPressed = false;
        }

        private void OnTriggerEnter(Collider other) {
            if (!disableWhileInWIM || !this.enabled || !other.CompareTag("WIM")) return;
            isInsideWIM = true;
        }

        private void OnTriggerExit(Collider other) {
            if (!disableWhileInWIM || !this.enabled || !other.CompareTag("WIM")) return;
            isInsideWIM = false;
        }

        private void SetEnable(bool value) {
            aimAssist.enabled = value;
            lineRenderer.enabled = value;
            isDisabled = !value;
        }
    }
}