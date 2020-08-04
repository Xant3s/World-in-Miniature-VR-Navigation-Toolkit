// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Util;
using WIMVR.Util.Haptics;
using WIMVR.Util.XR;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// Manages interactions with preview screen i.e. vibration on touch and pickup.
    /// </summary>
    [DisallowMultipleComponent]
    public class PreviewScreenController : MonoBehaviour {
        private Transform index;
        private Transform WIMTransform;


        private void Awake() {
            WIMTransform = GameObject.FindWithTag("WIM")?.transform;
            Assert.IsNotNull(WIMTransform);
            index = GameObject.FindWithTag("IndexR")?.transform;
            Assert.IsNotNull(index);
        }

        private void Start() {
            var pickup = gameObject.AddComponent<DetectPickupGesture>();
            pickup.OnStartTouch.AddListener(Vibrate);
            pickup.OnStartGrabbing.AddListener(Pickup_OnStartGrabbing);
            pickup.OnStopGrabbing.AddListener(Pickup_OnStopGrabbing);
        }
        
        private void Pickup_OnStartGrabbing() {
            transform.parent = index;
            transform.localPosition = Vector3.zero;
        }

        private void Pickup_OnStopGrabbing() {
            transform.parent = WIMTransform.GetChild(0);
        }

        private void OnDestroy() {
            CancelInvoke();
        }

        private void Vibrate(Hand hand) {
            Haptics.Vibrate(XRUtils.FindCorrespondingInputDevice(hand), .1f, .1f);
        }
    }
}