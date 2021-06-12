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
    /// Used to open the preview screen when the player grabs the destination indicator's view cone.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DetectPickupGesture))]
    public class PickupPreviewScreen : MonoBehaviour {
        private MiniatureModel WIM;
        private Transform thumbR;   // TODO: allow both hands
        private Transform indexR;
        private Transform WIMTransform;
        private IHighlighter alphaHighlighter;
        private DetectPickupGesture detectPickupGesture;


        private void Awake() {
            thumbR = FindObjectOfType<RightThumbFingerTip>()?.transform;
            indexR = FindObjectOfType<RightIndexFingerTip>()?.transform;
            WIM = FindObjectOfType<MiniatureModel>();
            WIMTransform = WIM ? WIM.transform : null;
            Assert.IsNotNull(WIM);
            Assert.IsNotNull(WIMTransform);
            Assert.IsNotNull(thumbR);
            Assert.IsNotNull(indexR);
        }

        private void Start() {
            detectPickupGesture = GetComponent<DetectPickupGesture>();
            alphaHighlighter = gameObject.AddComponent<AlphaHighlighter>();
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
            alphaHighlighter.HighlightEnabled = true;
        }

        private void StopTouch() {
            alphaHighlighter.HighlightEnabled = false;
        }

        private void StartGrabbing() {
            // Spawn new preview screen.
            var previewScreen = WIMTransform.GetComponent<PreviewScreen>();
            previewScreen.ShowPreviewScreenPickup(WIM.Configuration, WIM.Data);
            var previewScreenTransform = previewScreen.Data.PreviewScreenTransform;
            Assert.IsNotNull(previewScreenTransform);

            // Pick up the new preview screen.
            previewScreenTransform.parent = indexR;
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