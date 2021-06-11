// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Util.Haptics;
using WIMVR.Util.XR;
using WIMVR.VR.HandSetup.Tags;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// Manages interactions with preview screen i.e. vibration on touch and pickup.
    /// </summary>
    [DisallowMultipleComponent]
    public class PreviewScreenController : MonoBehaviour {
        private Transform indexL;
        private Transform indexR;
        private Transform WIMTransform;


        private void Awake() {
            WIMTransform = FindObjectOfType<MiniatureModel>()?.transform;
            Assert.IsNotNull(WIMTransform);
            indexL = FindObjectOfType<LeftIndexFingerTip>()?.transform;
            indexR = FindObjectOfType<RightIndexFingerTip>()?.transform;
            Assert.IsNotNull(indexL);
            Assert.IsNotNull(indexR);
        }

        private void Start() {
            var pickup = gameObject.AddComponent<DetectPickupGesture>();
            pickup.OnStartTouch.AddListener(Vibrate);
            pickup.OnStartGrabbing.AddListener(Pickup_OnStartGrabbing);
            pickup.OnStopGrabbing.AddListener(Pickup_OnStopGrabbing);
        }
        
        private void Pickup_OnStartGrabbing() {
            transform.parent = indexR;  // TODO: allow both hands
            transform.localPosition = Vector3.zero;
        }

        private void Pickup_OnStopGrabbing() {
            transform.parent = WIMTransform.GetChild(0);
        }

        private void OnDestroy() {
            CancelInvoke();
        }

        private static void Vibrate(Hand hand) 
            => Haptics.Vibrate(XRUtils.TryFindCorrespondingInputDevice(hand), .1f, .1f);
    }
}