using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;


namespace WIM_Plugin {
    public class PickupDestinationSelection : MonoBehaviour {
        public float DoubleTapInterval { get; set; } = 2;

        private bool pickupMode = false;
        private Transform thumb;
        private Transform index;
        private bool thumbIsTouching;
        private bool indexIsTouching;
        private bool isGrabbing;
        private bool stoppedGrabbing;
        private bool pickupIndexButtonPressed;
        private bool pickupThumbButtonPressed;


        private void Awake() {
            thumb = GameObject.FindWithTag("ThumbR").transform;
            index = GameObject.FindWithTag("IndexR").transform;
            Assert.IsNotNull(thumb);
            Assert.IsNotNull(index);
        }

        private void OnEnable() {
            MiniatureModel.OnPickupIndexButtonDown += pickupIndexButtonDown;
            MiniatureModel.OnPickupIndexButtonUp += pickupIndexButtonUp;
            MiniatureModel.OnPickupThumbButtonDown += pickupThumbButtonDown;
            MiniatureModel.OnPickupThumbButtonUp += pickupThumbButtonUp;
        }

        private void OnDisable() {
            MiniatureModel.OnPickupIndexButtonDown -= pickupIndexButtonDown;
            MiniatureModel.OnPickupIndexButtonUp -= pickupIndexButtonUp;
            MiniatureModel.OnPickupThumbButtonDown -= pickupThumbButtonDown;
            MiniatureModel.OnPickupThumbButtonUp -= pickupThumbButtonUp;
        }

        private void Update() {
            var capLowerCenter = transform.position - transform.up * transform.localScale.y;
            var capUpperCenter = transform.position + transform.up * transform.localScale.y;
            var radius = GameObject.Find("WIM").GetComponent<MiniatureModel>().Configuration.ScaleFactor * 1.0f;
            var colliders = Physics.OverlapCapsule(capLowerCenter, capUpperCenter, radius, LayerMask.GetMask("Hands"));
            thumbIsTouching = colliders.Contains(thumb.GetComponent<Collider>());
            indexIsTouching = colliders.Contains(index.GetComponent<Collider>());
            var thumbAndIndexTouching = thumbIsTouching && indexIsTouching;
            var thumbAndIndexPressed = pickupThumbButtonPressed && pickupIndexButtonPressed;

            if (!isGrabbing && thumbAndIndexTouching && thumbAndIndexPressed) {
                isGrabbing = true;
                startGrabbing();
            }
            else if (isGrabbing && !thumbAndIndexPressed) {
                isGrabbing = false;
                stopGrabbing();
            }
        }

        private void pickupIndexButtonDown(WIMConfiguration config, WIMData data) {
            pickupIndexButtonPressed = true;
        }

        private void pickupIndexButtonUp(WIMConfiguration config, WIMData data) {
            pickupIndexButtonPressed = false;
        }

        private void pickupThumbButtonDown(WIMConfiguration config, WIMData data) {
            pickupThumbButtonPressed = true;
        }

        private void pickupThumbButtonUp(WIMConfiguration config, WIMData data) {
            pickupThumbButtonPressed = false;
        }

        private void startGrabbing() {
            var WIMTransform = transform.root;
            var WIM = WIMTransform.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);

            // Remove existing destination indicator.
            DestinationIndicators.RemoveDestinationIndicators(WIM);

            // Spawn new destination indicator.
            DestinationIndicators.SpawnDestinationIndicatorInWIM(WIM.Configuration, WIM.Data);

            // Actually pick up the new destination indicator.
            WIM.Data.DestinationIndicatorInWIM.parent = index;
            var midPos = thumb.position + (index.position - thumb.position) / 2.0f;
            WIM.Data.DestinationIndicatorInWIM.position = midPos;
            WIM.Data.DestinationIndicatorInWIM.rotation = WIM.Data.PlayerRepresentationTransform.rotation;
        }

        private void stopGrabbing() {
            var WIMTransform = transform.root;
            var WIM = WIMTransform.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);

            // Let go.
            if (!WIM.Data.DestinationIndicatorInWIM) return;
            WIM.Data.DestinationIndicatorInWIM.parent = WIMTransform.GetChild(0);

            // Make destination indicator in WIM grabbable, so it can be changed without creating a new one.
            Invoke(nameof(allowUpdates), 1);

            // Create destination indicator in level. Includes snap to ground.
            if (!WIM.Data.DestinationIndicatorInLevel)
                DestinationIndicators.SpawnDestinationIndicatorInLevel(WIM.Configuration, WIM.Data);

            // New destination
            WIM.NewDestination();
        }

        private void allowUpdates() {
            var WIMTransform = transform.root;
            var WIM = WIMTransform.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
            Assert.IsNotNull(WIM.Data.DestinationIndicatorInWIM);
            WIM.Data.DestinationIndicatorInWIM.gameObject.AddComponent<PickupDestinationUpdate>().DoubleTapInterval =
                DoubleTapInterval;
        }
    }
}