using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    public class PickupDestinationUpdate : MonoBehaviour {
        public float DoubleTapInterval { get; set; } = 2;

        public bool HightlightFX
        {
            get => hightlightFX;
            set
            {
                hightlightFX = value;
                if (GetComponent<Renderer>())
                {
                    material.color = value ? hightlightColor : defaultColor;
                }
            }
        }

        private enum TapState {
            None,
            FirstTap,
            WaitingForSecondTap,
            SecondTap
        }

        private MiniatureModel WIM;
        private TapState tapState;
        private Material material;
        private Transform thumb;
        private Transform index;
        private Color defaultColor;
        private Color hightlightColor = Color.cyan;
        private bool hightlightFX;
        private bool pickupMode = false;
        private bool thumbIsTouching;
        private bool indexIsTouching;
        private bool isGrabbing;
        private bool pickupIndexButtonPressed;
        private bool pickupThumbButtonPressed;


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

        private void Awake() {
            ColorUtility.TryParseHtmlString("#25DFFF", out hightlightColor);
            thumb = GameObject.FindWithTag("ThumbR")?.transform;
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            material = GetComponentInChildren<Renderer>()?.material;
            defaultColor = material.color;
            Assert.IsNotNull(thumb);
            Assert.IsNotNull(index);
            Assert.IsNotNull(WIM);
            Assert.IsNotNull(material);
        }


        private void Start() {
            var collider = gameObject.AddComponent<CapsuleCollider>();
            collider.height = 2.2f;
            collider.radius = .7f;
            collider.isTrigger = true;
        }

        private void Update() {
            var capLowerCenter = transform.position - transform.up * transform.localScale.y;
            var capUpperCenter = transform.position + transform.up * transform.localScale.y;
            var radius = WIM.Configuration.ScaleFactor * 1.0f;
            var colliders = Physics.OverlapCapsule(capLowerCenter, capUpperCenter, radius, LayerMask.GetMask("Hands"));
            thumbIsTouching = colliders.Contains(thumb.GetComponent<Collider>());
            indexIsTouching = colliders.Contains(index.GetComponent<Collider>());
            var thumbAndIndexTouching = thumbIsTouching && indexIsTouching;
            var thumbAndIndexPressed = pickupThumbButtonPressed && pickupIndexButtonPressed;
            HightlightFX = thumbIsTouching || indexIsTouching;

            if (!isGrabbing) {
                // Handle double tap
                switch (tapState) {
                    case TapState.None when indexIsTouching && !thumbIsTouching:
                        // First tap
                        tapState = TapState.FirstTap;
                        Invoke(nameof(resetDoubleTap), DoubleTapInterval);
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

            if (!isGrabbing && thumbAndIndexTouching && thumbAndIndexPressed) {
                isGrabbing = true;
                resetDoubleTap();
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
            // Remove existing destination indicator (except "this").
            RemoveDestinationIndicatorsExceptWIM(WIM);

            // Actually pick up the new destination indicator.
            WIM.Data.DestinationIndicatorInWIM.parent = index;
            var midPos = thumb.position + (index.position - thumb.position) / 2.0f;
            WIM.Data.DestinationIndicatorInWIM.position = midPos;
        }

        private void RemoveDestinationIndicatorsExceptWIM(MiniatureModel WIM) {
            if (!WIM.Data.DestinationIndicatorInWIM) return;
            WIM.GetComponent<PreviewScreen>()?.RemovePreviewScreen();
            // Using DestroyImmediate because the WIM is about to being copied and we don't want to copy these objects too.
            DestroyImmediate(WIM.GetComponent<TravelPreviewAnimation>().Data.TravelPreviewAnimationObj);
            if (WIM.Data.DestinationIndicatorInLevel) DestroyImmediate(WIM.Data.DestinationIndicatorInLevel.gameObject);
        }

        private void stopGrabbing() {
            // Let go. 
            WIM.Data.DestinationIndicatorInWIM.parent = WIM.transform.GetChild(0);

            // Create destination indicator in level. Includes snap to ground.
            if (!WIM.Data.DestinationIndicatorInLevel)
                DestinationIndicators.SpawnDestinationIndicatorInLevel(WIM.Configuration, WIM.Data);

            // New destination
            WIM.NewDestination();
        }

        private void resetDoubleTap() {
            //firstTap = false;
            tapState = TapState.None;
            CancelInvoke(nameof(resetDoubleTap));
        }
    }
}