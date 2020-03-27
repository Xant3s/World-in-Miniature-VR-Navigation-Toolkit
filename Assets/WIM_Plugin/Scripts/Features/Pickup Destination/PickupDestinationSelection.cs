using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    [DisallowMultipleComponent]
    public class PickupDestinationSelection : MonoBehaviour {
        public float DoubleTapInterval { get; set; } = 2;

        public bool HightlightFX {
            get => hightlightFX;
            set {
                hightlightFX = value;
                if (GetComponent<Renderer>()) {
                    material.color = value ? hightlightColor : defaultColor;
                }
            }
        }

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


        private void Awake() {
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
            MiniatureModel.OnPickpuIndexButton += pickupIndexButton;
            MiniatureModel.OnPickupThumbTouchUp += pickupThumbTouchUp;
        }

        private void OnDisable() {
            MiniatureModel.OnPickpuIndexButton -= pickupIndexButton;
            MiniatureModel.OnPickupThumbTouchUp -= pickupThumbTouchUp;
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
            if(!prevIndexIsTouching && indexIsTouching || 
               !prevThumbIsTouching && thumbIsTouching) vibrate();
            var thumbAndIndexTouching = thumbIsTouching && indexIsTouching;
            HightlightFX = thumbIsTouching || indexIsTouching;

            if (!isGrabbing && thumbAndIndexTouching) {
                isGrabbing = true;
                startGrabbing();
            }
            else if (isGrabbing && !thumbAndIndexTouching) {
                isGrabbing = false;
            }
        }

        private void pickupIndexButton(WIMConfiguration config, WIMData data, float axis) {
            if (!isGrabbing && axis != 1 && !stoppedGrabbing) stopGrabbing();
        }

        private void pickupThumbTouchUp(WIMConfiguration config, WIMData data) {
            if(!isGrabbing) stopGrabbing();
        }

        private void vibrate() {
            InputManager.SetVibration(frequency:.5f, amplitude:.1f, Hand.RightHand);
            Invoke(nameof(stopVibration), time:.1f);
        }

        private void stopVibration() {
            InputManager.SetVibration(frequency: 0, amplitude: 0, Hand.RightHand);
        }

        private void startGrabbing() {
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

        private void stopGrabbing() {
            if (stoppedGrabbing) return;
            stoppedGrabbing = true;

            // Let go.
            if (!WIM.Data.DestinationIndicatorInWIM) return;
            WIM.Data.DestinationIndicatorInWIM.parent = WIM.transform.GetChild(0);

            // Make destination indicator in WIM grabbable, so it can be changed without creating a new one.
            Invoke(nameof(allowUpdates), 1);

            // Create destination indicator in level. Includes snap to ground.
            if (!WIM.Data.DestinationIndicatorInLevel)
                DestinationIndicators.SpawnDestinationIndicatorInLevel(WIM.Configuration, WIM.Data);

            // New destination
            WIM.NewDestination();
        }

        private void allowUpdates() {
            Assert.IsNotNull(WIM.Data.DestinationIndicatorInWIM);
            WIM.Data.DestinationIndicatorInWIM.gameObject.AddComponent<PickupDestinationUpdate>().DoubleTapInterval =
                DoubleTapInterval;
        }
    }
}