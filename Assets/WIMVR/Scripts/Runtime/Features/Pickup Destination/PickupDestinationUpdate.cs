// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Util.Haptics;
using WIMVR.Util.XR;

namespace WIMVR.Features.Pickup_Destination {
    /// <summary>
    /// Can be used to pickup the destination indicator again to choose a new destination.
    /// </summary>
    [DisallowMultipleComponent]
    public class PickupDestinationUpdate : MonoBehaviour {
        public delegate void WIMAction(in MiniatureModel WIM);

        private MiniatureModel WIM;
        private TapState tapState;
        private Transform thumb;
        private Transform index;
        private IHighlighter colorHighlighter;
        private DetectPickupGesture detectPickupGesture;

        /// <summary>
        /// Tapping the destination indicator twice will be considered a double-tap iff
        /// time between tap does not exceed this time.
        /// </summary>
        public float DoubleTapInterval { get; set; } = 2;

        public static event WIMAction OnRemoveDestinationIndicatorExceptWIM;


        private void Awake() {
            colorHighlighter = GetComponentInChildren<IHighlighter>();
            thumb = GameObject.FindWithTag("ThumbR")?.transform;
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(thumb);
            Assert.IsNotNull(index);
            Assert.IsNotNull(WIM);
            detectPickupGesture = GetComponentInChildren<DetectPickupGesture>();
            detectPickupGesture.OnStartTouch.AddListener(OnStartTouch);
            detectPickupGesture.OnStopTouch.AddListener(OnStopTouch);
            detectPickupGesture.OnStartGrabbing.AddListener(StartGrabbing);
            detectPickupGesture.OnStartGrabbing.AddListener(ResetDoubleTap);
            detectPickupGesture.OnStopGrabbing.AddListener(StopGrabbing);
        }

        private void OnStartTouch(Hand hand) {
            // Haptic feedback.
            var inputDevice = XRUtils.TryFindCorrespondingInputDevice(hand);
            Haptics.Vibrate(inputDevice, .1f, .1f);

            // Visual feedback.
            colorHighlighter.HighlightEnabled = true;

            // Handle double tap
            switch (tapState) {
                case TapState.None:
                    // First tap
                    tapState = TapState.FirstTap;
                    Invoke(nameof(ResetDoubleTap), DoubleTapInterval);
                    break;
                case TapState.FirstTap:
                    // Tapped once, now outside
                    tapState = TapState.WaitingForSecondTap;
                    break;
                case TapState.WaitingForSecondTap:
                    // 2nd tap
                    tapState = TapState.SecondTap;
                    WIM.ConfirmTravel();
                    break;
            }
        }

        private void OnStopTouch() {
            colorHighlighter.HighlightEnabled = false;
        }

        private void StartGrabbing() {
            // Remove existing destination indicator (except "this").
            RemoveDestinationIndicatorsExceptWIM(WIM);

            // Actually pick up the new destination indicator.
            WIM.Data.DestinationIndicatorInWIM.parent = index;
            var midPos = thumb.position + (index.position - thumb.position) / 2.0f;
            WIM.Data.DestinationIndicatorInWIM.position = midPos;
        }

        private void RemoveDestinationIndicatorsExceptWIM(MiniatureModel WIM) {
            if (!WIM.Data.DestinationIndicatorInWIM) return;
            OnRemoveDestinationIndicatorExceptWIM?.Invoke(WIM);
            if (WIM.Data.DestinationIndicatorInLevel) Destroy(WIM.Data.DestinationIndicatorInLevel.gameObject);
        }

        private void StopGrabbing() {
            // Let go.
            WIM.Data.DestinationIndicatorInWIM.parent = WIM.transform.GetChild(0);

            // Create destination indicator in level. Includes snap to ground.
            if (!WIM.Data.DestinationIndicatorInLevel)
                DestinationIndicators.SpawnDestinationIndicatorInLevel(WIM.Configuration, WIM.Data);

            // New destination
            WIM.NewDestination();
        }

        private void ResetDoubleTap() {
            tapState = TapState.None;
            CancelInvoke(nameof(ResetDoubleTap));
        }

        private enum TapState {
            None,
            FirstTap,
            WaitingForSecondTap,
            SecondTap
        }
    }
}