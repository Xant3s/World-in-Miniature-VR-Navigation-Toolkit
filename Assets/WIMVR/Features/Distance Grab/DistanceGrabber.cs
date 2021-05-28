// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Core;
using WIMVR.VR;

namespace WIMVR.Features.Distance_Grab {
    /// <summary>
    /// Can be used to pull distance grabbable objects that are not in range for regular grabbing.
    /// </summary>
    public class DistanceGrabber : MonoBehaviour {
        [Header("Distance Grabber (Experimental)")]

        [Tooltip("Start point of the laser pointer.")]
        [SerializeField] private Transform start = null;

        [Tooltip("Distance grabbing will be disabled if within specified distance to miniature model")]
        [SerializeField] private float requiredDistanceToWIM = .5f;

        [Tooltip("Specifies how fast objects are pulled towards the hand.")]
        [SerializeField] private float snapSpeed = 10f;

        [Tooltip("Stop pulling objects that are within specified distance to the hand.")]
        [SerializeField] private float minDistance = .5f;

        [Tooltip("Automatically disable while this hand is touching the miniature model.")]
        [SerializeField] private bool disableWhileInWIM = true;

        private AimAssist aimAssist;
        private LineRenderer lineRenderer;
        private Transform WIM;
        private DistanceGrabbable lastGrabbable;
        private bool grabButtonPressed;
        private bool grabStartedThisFrame;
        private bool isDisabled;
        private bool isInsideWIM;


        private void Awake() {
            if (!disableWhileInWIM || !this.enabled) return;
            aimAssist = gameObject.GetComponentInChildren<AimAssist>();
            lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
            WIM = GameObject.FindWithTag("WIM").transform;
            if(!start) start = aimAssist.transform;
        }

        private void LateUpdate() {
            var distanceToWIM = Vector3.Distance(WIM.position, transform.position);
            SetEnable(!(distanceToWIM < requiredDistanceToWIM) && !isInsideWIM);
            if(isDisabled) return;
        
            var allLayersButHands = ~((1 << LayerMask.NameToLayer("Hands")) | (1 << Physics.IgnoreRaycastLayer));
            if (Physics.Raycast(transform.position, start.forward, out var hit, Mathf.Infinity, allLayersButHands)) {
                var currentGrabbable = hit.transform.GetComponent<DistanceGrabbable>();

                if(!currentGrabbable || hit.transform.GetComponent<OffsetGrabInteractable>().IsGrabbed) {
                    grabStartedThisFrame = false;
                    
                    if(lastGrabbable) {
                        lastGrabbable.HighlightFX = false;
                        lastGrabbable = null;
                    }
                    return;
                }

                lastGrabbable = currentGrabbable;
                if(!grabStartedThisFrame && grabButtonPressed) return;
                currentGrabbable.HighlightFX = true;
                currentGrabbable.IsBeingGrabbed = grabButtonPressed;
                if(!grabStartedThisFrame) return;
                grabStartedThisFrame = false;
                if(grabButtonPressed) {
                    currentGrabbable.MinDistance = minDistance;
                    currentGrabbable.SnapSpeed = snapSpeed;
                    currentGrabbable.Target = transform;
                }
            }
        }

        public void StartDistanceGrab() {
            grabButtonPressed = true;
            grabStartedThisFrame = true;
        }
        
        public void StopDistanceGrab() => grabButtonPressed = false;

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