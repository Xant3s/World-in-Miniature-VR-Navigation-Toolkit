using UnityEngine;

namespace WIM_Plugin {
    public class DistanceGrabber : MonoBehaviour {
        [SerializeField] private Hand hand;
        [SerializeField] private Transform start;
        [SerializeField] private float requieredDistanceToWIM = .5f;
        [SerializeField] private float snapSpeed = 10f;
        [SerializeField] private float minDistance = .5f;
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

        private void grabButtonDown(WIMConfiguration config, WIMData data) {
            grabButtonPressed = true;
            grabStartedThisFrame = true;
        }

        private void grabButtonUp(WIMConfiguration config, WIMData data) {
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