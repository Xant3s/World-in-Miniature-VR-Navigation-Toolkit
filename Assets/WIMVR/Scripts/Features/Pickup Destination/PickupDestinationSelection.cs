// Author: Samuel Truman (contact@samueltruman.com)

using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Input;

namespace WIMVR.Features.Pickup_Destination {
    /// <summary>
    /// Can be used to select a destination by picking up the player
    /// representation and placing is at the desired location in the miniature model.
    /// To confirm the destination the destination indicator hast to be double-tapped.
    /// </summary>
    [DisallowMultipleComponent]
    public class PickupDestinationSelection : MonoBehaviour {
        private MiniatureModel WIM;
        private Material material;
        private Transform thumb;
        private Transform index;
        private Collider thumbCol;
        private Collider indexCol;
        private Color defaultColor;
        private Color hightlightColor = Color.red;
        private bool hightlightFX;
        private bool thumbIsTouching;
        private bool indexIsTouching;
        private bool isGrabbing;
        private bool stoppedGrabbing = true;

        /// <summary>
        /// Tapping the destination indicator twice will be considered a double-tap iff
        /// time between tap does not exceed this time.
        /// </summary>
        public float DoubleTapInterval { get; set; } = 2;

        /// <summary>
        /// The highlight effect displayed when the player touches the player represenation.
        /// </summary>
        public bool HightlightFX {
            get => hightlightFX;
            set {
                hightlightFX = value;
                if (GetComponent<Renderer>()) {
                    material.color = value ? hightlightColor : defaultColor;
                }
            }
        }

        private void Start() {
            thumb = GameObject.FindWithTag("ThumbR")?.transform;
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            material = GetComponentInChildren<Renderer>()?.material;
            defaultColor = material.color;
            Assert.IsNotNull(thumb);
            Assert.IsNotNull(index);
            Assert.IsNotNull(WIM);
            Assert.IsNotNull(material);
            thumbCol = thumb.GetComponent<Collider>();
            indexCol = index.GetComponent<Collider>();
            Assert.IsNotNull(thumbCol);
            Assert.IsNotNull(indexCol);
        }

        private void OnEnable() {
            MiniatureModel.OnPickupIndexButton += PickupIndexButton;
            MiniatureModel.OnPickupThumbTouchUp += PickupThumbTouchUp;
        }

        private void OnDisable() {
            MiniatureModel.OnPickupIndexButton -= PickupIndexButton;
            MiniatureModel.OnPickupThumbTouchUp -= PickupThumbTouchUp;
        }

        private void Update() {
            Assert.IsNotNull(WIM);
            Assert.IsNotNull(WIM.Configuration);
            var height = transform.localScale.y * WIM.Configuration.ScaleFactor;
            var capLowerCenter = transform.position - transform.up * height / 2.0f;
            var capUpperCenter = transform.position + transform.up * height / 2.0f;
            var radius = WIM.Configuration.ScaleFactor * transform.localScale.x / 2.0f;
            var colliders = Physics.OverlapCapsule(capLowerCenter, capUpperCenter, radius, LayerMask.GetMask("Hands"));
            var prevThumbIsTouching = thumbIsTouching;
            var prevIndexIsTouching = indexIsTouching;
            thumbIsTouching = colliders.Contains(thumbCol);
            indexIsTouching = colliders.Contains(indexCol);
            if(!prevIndexIsTouching && indexIsTouching ||
               !prevThumbIsTouching && thumbIsTouching) Vibrate();
            var thumbAndIndexTouching = thumbIsTouching && indexIsTouching;
            HightlightFX = thumbIsTouching || indexIsTouching;

            if (!isGrabbing && thumbAndIndexTouching) {
                isGrabbing = true;
                StartGrabbing();
            }
            else if (isGrabbing && !thumbAndIndexTouching) {
                isGrabbing = false;
            }
        }

        private void PickupIndexButton(WIMConfiguration config, WIMData data, float axis) {
            if (!isGrabbing && axis != 1 && !stoppedGrabbing) StopGrabbing();
        }

        private void PickupThumbTouchUp(WIMConfiguration config, WIMData data) {
            if(!isGrabbing) StopGrabbing();
        }

        private void Vibrate() {
            InputManager.SetVibration(frequency:.5f, amplitude:.1f, Hand.RightHand);
            Invoke(nameof(StopVibration), time:.1f);
        }

        private void StopVibration() {
            InputManager.SetVibration(frequency: 0, amplitude: 0, Hand.RightHand);
        }

        private void StartGrabbing() {
            // Remove existing destination indicator.
            DestinationIndicators.RemoveDestinationIndicators(WIM);

            // Spawn new destination indicator.
            DestinationIndicators.SpawnDestinationIndicatorInWIM(WIM.Configuration, WIM.Data);

            // Actually pick up the new destination indicator.
            WIM.Data.DestinationIndicatorInWIM.parent = index;
            var midPos = thumb.position + (index.position - thumb.position) / 2.0f;
            WIM.Data.DestinationIndicatorInWIM.position = midPos;
            WIM.Data.DestinationIndicatorInWIM.rotation = WIM.Data.PlayerRepresentationTransform.rotation;
            stoppedGrabbing = false;
        }

        private void StopGrabbing() {
            if (stoppedGrabbing) return;
            stoppedGrabbing = true;

            // Let go.
            if (!WIM.Data.DestinationIndicatorInWIM) return;
            WIM.Data.DestinationIndicatorInWIM.parent = WIM.transform.GetChild(0);

            // Make destination indicator in WIM grabbable, so it can be changed without creating a new one.
            Invoke(nameof(AllowUpdates), 1);

            // Create destination indicator in level. Includes snap to ground.
            if (!WIM.Data.DestinationIndicatorInLevel)
                DestinationIndicators.SpawnDestinationIndicatorInLevel(WIM.Configuration, WIM.Data);

            // New destination
            WIM.NewDestination();
        }

        private void AllowUpdates() {
            Assert.IsNotNull(WIM.Data.DestinationIndicatorInWIM);
            WIM.Data.DestinationIndicatorInWIM.gameObject.AddComponent<PickupDestinationUpdate>().DoubleTapInterval =
                DoubleTapInterval;
        }
    }
}