using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    [DisallowMultipleComponent]
    public class Pickup : MonoBehaviour {
        public delegate void Action();

        public event Action OnStartGrabbing;
        public event Action OnStopGrabbing;
        public event Action OnStartTouch;
        public event Action OnStopTouch;

        private Transform thumb;
        private Transform index;
        private Transform WIMTransform;
        private bool thumbIsGrabbing;
        private bool indexIsGrabbing;
        private bool isGrabbing;
        private bool stoppedGrabbing;
        private bool indexIsPressed;


        private void Awake() {
            thumb = GameObject.FindWithTag("ThumbR")?.transform;
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIMTransform = GameObject.FindWithTag("WIM")?.transform;
            Assert.IsNotNull(WIMTransform);
            Assert.IsNotNull(thumb);
            Assert.IsNotNull(index);
        }

        private void OnEnable() {
            MiniatureModel.OnPickupIndexButtonDown += PickupIndexButtonDown;
            MiniatureModel.OnPickupIndexButtonUp += PickupIndexButtonUp;
            MiniatureModel.OnPickupThumbTouchUp += PickupThumbTouchUp;
        }

        private void OnDisable() {
            MiniatureModel.OnPickupIndexButtonDown -= PickupIndexButtonDown;
            MiniatureModel.OnPickupIndexButtonUp -= PickupIndexButtonUp;
            MiniatureModel.OnPickupThumbTouchUp -= PickupThumbTouchUp;
        }

        private void PickupIndexButtonDown(WIMConfiguration config, WIMData data) {
            indexIsPressed = true;
        }

        private void PickupIndexButtonUp(WIMConfiguration config, WIMData data) {
            indexIsPressed = false;
            StopGrabbing();
        }

        private void PickupThumbTouchUp(WIMConfiguration config, WIMData data) {
            StopGrabbing();
        }

        private void Update() {
            var rightHandPinch = thumbIsGrabbing && indexIsGrabbing && indexIsPressed;
            if (rightHandPinch && !isGrabbing) {
                isGrabbing = true;
                stoppedGrabbing = false;
                StartGrabbing();
            }
            else if (isGrabbing && !rightHandPinch) {
                isGrabbing = false;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.transform == thumb) {
                thumbIsGrabbing = true;
                OnStartTouch?.Invoke();
            }
            else if (other.transform == index) {
                indexIsGrabbing = true;
                OnStartTouch?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.transform == thumb) {
                thumbIsGrabbing = false;
                OnStopTouch?.Invoke();
            }
            else if (other.transform == index) {
                indexIsGrabbing = false;
                OnStopTouch?.Invoke();
            }
        }

        private void StartGrabbing() {
            OnStartGrabbing?.Invoke();
        }

        private void StopGrabbing() {
            if (stoppedGrabbing) return;
            OnStopGrabbing?.Invoke();
            stoppedGrabbing = true;
        }
    }
}