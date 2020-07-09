// Author: Samuel Truman (contact@samueltruman.com)

using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Input;

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
        private Collider thumbCol;
        private Collider indexCol;
        private HighlightFX highlightFX;
        private bool thumbIsTouching;
        private bool indexIsTouching;
        private bool isGrabbing;
        private bool stoppedGrabbing = true;

        /// <summary>
        /// Tapping the destination indicator twice will be considered a double-tap iff
        /// time between tap does not exceed this time.
        /// </summary>
        public float DoubleTapInterval { get; set; } = 2;

        public static event WIMAction OnRemoveDestinationIndicatorExceptWIM;


        private void OnEnable() {
            MiniatureModel.OnPickupIndexButton += PickupIndexButton;
            MiniatureModel.OnPickupThumbTouchUp += PickupThumbTouchUp;
        }

        private void OnDisable() {
            MiniatureModel.OnPickupIndexButton -= PickupIndexButton;
            MiniatureModel.OnPickupThumbTouchUp -= PickupThumbTouchUp;
        }

        private void Awake() {
            highlightFX = GetComponent<HighlightFX>();
            thumb = GameObject.FindWithTag("ThumbR")?.transform;
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(thumb);
            Assert.IsNotNull(index);
            Assert.IsNotNull(WIM);
            thumbCol = thumb.GetComponent<Collider>();
            indexCol = index.GetComponent<Collider>();
            Assert.IsNotNull(thumbCol);
            Assert.IsNotNull(indexCol);
        }

        private void Start() {
            var collider = gameObject.AddComponent<CapsuleCollider>();
            collider.height = 2.2f;
            collider.radius = .7f;
            collider.isTrigger = true;
        }

        private void Update() {
            var height = transform.localScale.y * WIM.Configuration.ScaleFactor;
            var capLowerCenter = transform.position - transform.up * height / 2.0f;
            var capUpperCenter = transform.position + transform.up * height / 2.0f;
            var radius = WIM.Configuration.ScaleFactor * transform.localScale.x / 2.0f;
            var colliders = Physics.OverlapCapsule(capLowerCenter, capUpperCenter, radius, LayerMask.GetMask("Hands"));
            var prevThumbIsTouching = thumbIsTouching;
            var prevIndexIsTouching = indexIsTouching;
            thumbIsTouching = colliders.Contains(thumbCol);
            indexIsTouching = colliders.Contains(indexCol);
            if (!prevIndexIsTouching && indexIsTouching ||
                !prevThumbIsTouching && thumbIsTouching) Vibrate();
            var thumbAndIndexTouching = thumbIsTouching && indexIsTouching;
            highlightFX.HighlightEnabled = thumbIsTouching || indexIsTouching;

            if (!isGrabbing) {
                // Handle double tap
                switch (tapState) {
                    case TapState.None when indexIsTouching && !thumbIsTouching:
                        // First tap
                        tapState = TapState.FirstTap;
                        Invoke(nameof(ResetDoubleTap), DoubleTapInterval);
                        break;
                    case TapState.FirstTap when !indexIsTouching:
                        // Tapped once, now outside
                        tapState = TapState.WaitingForSecondTap;
                        break;
                    case TapState.WaitingForSecondTap when indexIsTouching && !thumbIsTouching:
                        // 2nd tap
                        tapState = TapState.SecondTap;
                        WIM.ConfirmTravel();
                        break;
                }
            }

            if (!isGrabbing && thumbAndIndexTouching) {
                isGrabbing = true;
                ResetDoubleTap();
                StartGrabbing();
            }
            else if (isGrabbing && !thumbAndIndexTouching) {
                isGrabbing = false;
            }
        }

        private void PickupIndexButton(WIMConfiguration config, WIMData data, float axis) {
            if (axis != 1 && !stoppedGrabbing) StopGrabbing();
        }

        private void PickupThumbTouchUp(WIMConfiguration config, WIMData data) {
            StopGrabbing();
        }

        private void StartGrabbing() {
            // Remove existing destination indicator (except "this").
            RemoveDestinationIndicatorsExceptWIM(WIM);

            // Actually pick up the new destination indicator.
            WIM.Data.DestinationIndicatorInWIM.parent = index;
            var midPos = thumb.position + (index.position - thumb.position) / 2.0f;
            WIM.Data.DestinationIndicatorInWIM.position = midPos;
            stoppedGrabbing = false;
        }

        private void RemoveDestinationIndicatorsExceptWIM(MiniatureModel WIM) {
            if (!WIM.Data.DestinationIndicatorInWIM) return;
            OnRemoveDestinationIndicatorExceptWIM?.Invoke(WIM);
            if (WIM.Data.DestinationIndicatorInLevel) DestroyImmediate(WIM.Data.DestinationIndicatorInLevel.gameObject);
        }

        private void StopGrabbing() {
            if (stoppedGrabbing) return;
            stoppedGrabbing = true;

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

        private void Vibrate() {
            InputManager.SetVibration(frequency: .5f, amplitude: .1f, Hand.RightHand);
            Invoke(nameof(StopVibration), time: .1f);
        }

        private void StopVibration() {
            InputManager.SetVibration(frequency: 0, amplitude: 0, Hand.RightHand);
        }

        private enum TapState {
            None,
            FirstTap,
            WaitingForSecondTap,
            SecondTap
        }
    }
}