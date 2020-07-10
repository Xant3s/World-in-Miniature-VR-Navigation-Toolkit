// Author: Samuel Truman (contact@samueltruman.com)

using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Input;
using WIMVR.Util.Haptics;
using WIMVR.Util.XR;

namespace WIMVR.Features.Pickup_Destination {
    /// <summary>
    /// Can be used to select a destination by picking up the player
    /// representation and placing is at the desired location in the miniature model.
    /// To confirm the destination the destination indicator hast to be double-tapped.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DetectPickupGesture))]
    public class PickupDestinationSelection : MonoBehaviour {
        private MiniatureModel WIM;
        private Transform thumbR;
        private Transform indexR;
        private HighlightFX highlightFX;
        private bool thumbIsTouching;
        private bool indexIsTouching;
        private bool isGrabbing;

        /// <summary>
        /// Tapping the destination indicator twice will be considered a double-tap iff
        /// time between tap does not exceed this time.
        /// </summary>
        public float DoubleTapInterval { get; set; } = 2;


        private void Start() {
            highlightFX = GetComponent<HighlightFX>();
            thumbR = GameObject.FindWithTag("ThumbR")?.transform;
            indexR = GameObject.FindWithTag("IndexR")?.transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(thumbR);
            Assert.IsNotNull(indexR);
            Assert.IsNotNull(WIM);
            var detectPickupGesture = GetComponent<DetectPickupGesture>();
            detectPickupGesture.OnStartTouch += OnStartTouch;
            detectPickupGesture.OnStopTouch += OnStopTouch;
            detectPickupGesture.OnStartGrabbing += StartGrabbing;
            detectPickupGesture.OnStopGrabbing += StopGrabbing;
        }

        private void OnStartTouch(Hand hand) {
            // Haptic feedback.
            var inputDevice = XRUtils.FindCorrespondingInputDevice(hand);
            Haptics.Vibrate(inputDevice, .1f, .1f);

            // Visual feedback.
            highlightFX.HighlightEnabled = true;
        }

        private void OnStopTouch() {
            highlightFX.HighlightEnabled = false;
        }

        private void StartGrabbing() {
            // Remove existing destination indicator.
            DestinationIndicators.RemoveDestinationIndicators(WIM);

            // Spawn new destination indicator.
            DestinationIndicators.SpawnDestinationIndicatorInWIM(WIM.Configuration, WIM.Data);

            // Actually pick up the new destination indicator.
            WIM.Data.DestinationIndicatorInWIM.parent = indexR;
            var midPos = thumbR.position + (indexR.position - thumbR.position) / 2.0f;
            WIM.Data.DestinationIndicatorInWIM.position = midPos;
            WIM.Data.DestinationIndicatorInWIM.rotation = WIM.Data.PlayerRepresentationTransform.rotation;
        }

        private void StopGrabbing() {
            // Let go.
            if (!WIM.Data.DestinationIndicatorInWIM) return;
            WIM.Data.DestinationIndicatorInWIM.parent = WIM.transform.GetChild(0);

            // Make destination indicator in WIM grabbable, so it can be changed without creating a new one.
            Invoke(nameof(MakeDestinationIndicatorGrabbable), 1);

            // Create destination indicator in level. Includes snap to ground.
            if (!WIM.Data.DestinationIndicatorInLevel)
                DestinationIndicators.SpawnDestinationIndicatorInLevel(WIM.Configuration, WIM.Data);

            // New destination
            WIM.NewDestination();
        }

        private void MakeDestinationIndicatorGrabbable() {
            Assert.IsNotNull(WIM.Data.DestinationIndicatorInWIM);
            WIM.Data.DestinationIndicatorInWIM.gameObject.AddComponent<PickupDestinationUpdate>().DoubleTapInterval =
                DoubleTapInterval;
        }
    }
}