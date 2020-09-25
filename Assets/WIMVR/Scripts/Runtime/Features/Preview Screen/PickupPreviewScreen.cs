// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Util.Haptics;
using WIMVR.Util.XR;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// Used to open the preview screen when the player grabs the destination indicator's view cone.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DetectPickupGesture))]
    public class PickupPreviewScreen : MonoBehaviour {
        private MiniatureModel WIM;
        private Transform thumb;
        private Transform index;
        private Transform WIMTransform;
        private HighlightFX highlightFX;


        private void Awake() {
            thumb = GameObject.FindWithTag("ThumbR")?.transform;
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIMTransform = GameObject.FindWithTag("WIM")?.transform;
            Assert.IsNotNull(WIMTransform);
            WIM = WIMTransform.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
            Assert.IsNotNull(thumb);
            Assert.IsNotNull(index);
        }

        private void Start() {
            var detectPickupGesture = GetComponent<DetectPickupGesture>();
            highlightFX = gameObject.AddComponent<HighlightFX>();
            highlightFX.SetUseAlpha(true, "_Alpha");
            Assert.IsNotNull(detectPickupGesture);
            detectPickupGesture.OnStartTouch.AddListener(StartTouch);
            detectPickupGesture.OnStopTouch.AddListener(StopTouch);
            detectPickupGesture.OnStartGrabbing.AddListener(StartGrabbing);
            detectPickupGesture.OnStopGrabbing.AddListener(StopGrabbing);
        }

        private void StartTouch(Hand hand) {
            // Haptic feedback.
            var inputDevice = XRUtils.TryFindCorrespondingInputDevice(hand);
            Haptics.Vibrate(inputDevice, .1f, .1f);

            // Visual feedback.
            highlightFX.HighlightEnabled = true;
        }

        private void StopTouch() {
            highlightFX.HighlightEnabled = false;
        }

        private void StartGrabbing() {
            // Spawn new preview screen.
            var previewScreen = WIMTransform.GetComponent<PreviewScreen>();
            previewScreen.ShowPreviewScreenPickup(WIM.Configuration, WIM.Data);
            var previewScreenTransform = previewScreen.Data.PreviewScreenTransform;
            Assert.IsNotNull(previewScreenTransform);

            // Pick up the new preview screen.
            previewScreenTransform.parent = index;
            previewScreenTransform.localPosition = Vector3.zero;
        }

        private void StopGrabbing() {
            var previewScreen = WIM.GetComponent<PreviewScreen>();
            var previewScreenTransform = previewScreen.Data.PreviewScreenTransform;
            if (!previewScreenTransform) return;
            previewScreenTransform.GetChild(0).gameObject.AddComponent<PreviewScreenController>();
            previewScreenTransform.GetChild(0).GetChild(0).gameObject.AddComponent<ClosePreviewScreen>();

            // Let go.
            previewScreenTransform.parent = WIMTransform.GetChild(0);
        }
    }
}