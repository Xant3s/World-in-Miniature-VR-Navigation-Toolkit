using UnityEngine;

namespace WIM_Plugin {
    public class DistanceGrabber : MonoBehaviour {
        [SerializeField] private Hand hand;
        [SerializeField] private Transform start;
        [SerializeField] private float snapSpeed = 10f;
        [SerializeField] private float minDistance = .5f;
        [SerializeField] private bool disableWhileInWIM = true;

        private AimAssist aimAssist;
        private LineRenderer lineRenderer;
        private bool isDisabled;
        private bool grabButtonPressed;

        private void Awake() {
            if (!disableWhileInWIM) return;
            if (!this.enabled) {
                isDisabled = true;
                return;
            }

            aimAssist = gameObject.GetComponentInChildren<AimAssist>();
            lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
        }

        private void OnEnable() {
            if (hand == Hand.LeftHand) {
                MiniatureModel.OnLeftGrabButtonDown += grabButtonDown;
                MiniatureModel.OnLeftGrabButtonUp += grabButtonUp;
            }
            else if (hand == Hand.RightHand) {
                MiniatureModel.OnRightGrabButtonDown += grabButtonDown;
                MiniatureModel.OnRightGrabButtonUp += grabButtonUp;
            }
        }

        private void OnDisable() {
            if (hand == Hand.LeftHand) {
                MiniatureModel.OnLeftGrabButtonDown -= grabButtonDown;
                MiniatureModel.OnLeftGrabButtonUp -= grabButtonUp;
            }
            else if (hand == Hand.RightHand) {
                MiniatureModel.OnRightGrabButtonDown -= grabButtonDown;
                MiniatureModel.OnRightGrabButtonUp -= grabButtonUp;
            }
        }

        private void LateUpdate() {
            var allLayersButHands = ~((1 << LayerMask.NameToLayer("Hands")) | (1 << Physics.IgnoreRaycastLayer));
            if (Physics.Raycast(transform.position, start.forward, out var hit, Mathf.Infinity, allLayersButHands)) {
                var grabbable = hit.transform.gameObject.GetComponent<DistanceGrabbable>();
                if(!grabbable) return;
                if(hit.transform.GetComponent<OVRGrabbable>().isGrabbed) return;
                grabbable.HighlightFX = true;
                if(grabButtonPressed) {
                    grabbable.MinDistance = minDistance;
                    grabbable.SnapSpeed = snapSpeed;
                    grabbable.Target = transform;
                }
                grabbable.IsBeingGrabbed = grabButtonPressed;
            }
        }

        private void grabButtonDown(WIMConfiguration config, WIMData data) {
            grabButtonPressed = true;
        }

        private void grabButtonUp(WIMConfiguration config, WIMData data) {
            grabButtonPressed = false;
        }

        private void OnTriggerEnter(Collider other) {
            if (!disableWhileInWIM || isDisabled || !other.CompareTag("WIM")) return;
            setEnable(false);
        }

        private void OnTriggerExit(Collider other) {
            if (!disableWhileInWIM || isDisabled || !other.CompareTag("WIM")) return;
            setEnable(true);
        }

        private void setEnable(bool value) {
            aimAssist.enabled = value;
            lineRenderer.enabled = value;
            this.enabled = value;
        }
    }
}