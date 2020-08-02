// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Util;
using WIMVR.Input;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// Manages interactions with preview screen i.e. vibration on touch and pickup.
    /// </summary>
    [DisallowMultipleComponent]
    public class PreviewScreenController : MonoBehaviour {
        private Transform index;
        private Transform WIMTransform;

        private bool isVibrating;

        private void Awake() {
            WIMTransform = GameObject.FindWithTag("WIM")?.transform;
            Assert.IsNotNull(WIMTransform);
            index = GameObject.FindWithTag("IndexR")?.transform;
            Assert.IsNotNull(index);
        }

        private void Start() {
            var pickup = gameObject.AddComponent<DetectPickupGesture>();
            pickup.OnStartGrabbing.AddListener(Pickup_OnStartGrabbing);
            pickup.OnStopGrabbing.AddListener(Pickup_OnStopGrabbing);
        }

        private void Pickup_OnStopGrabbing() {
            transform.parent = WIMTransform.GetChild(0);
        }

        private void Pickup_OnStartGrabbing() {
            transform.parent = index;
            transform.localPosition = Vector3.zero;
        }

        private void OnDestroy() {
            CancelInvoke();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.transform != index) return;
            if (transform.root.CompareTag("HandR")) return;
            Vibrate();
        }

        private void Vibrate() {
            if (isVibrating) return;
            isVibrating = true;
            InputManager.SetVibration(frequency: .5f, amplitude: .1f, Hand.RightHand);
            Invoke(nameof(StopVibration), time: .1f);
        }

        private void StopVibration() {
            isVibrating = false;
            InputManager.SetVibration(frequency: 0, amplitude: 0, Hand.RightHand);
        }
    }
}