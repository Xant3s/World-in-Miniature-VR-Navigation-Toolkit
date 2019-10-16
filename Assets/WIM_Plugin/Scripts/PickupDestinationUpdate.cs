using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;
using Debug = System.Diagnostics.Debug;

namespace WIM_Plugin {
    public class PickupDestinationUpdate : MonoBehaviour {
        public float DoubleTapInterval { get; set; } = 2;

        private enum TapState {
            None,
            FirstTap,
            WaitingForSecondTap,
            SecondTap
        }

        private TapState tapState;
        private bool pickupMode = false;
        private Transform thumb;
        private Transform index;
        private bool thumbIsTouching;
        private bool indexIsTouching;
        private bool isGrabbing;


        private void Awake() {
            thumb = GameObject.FindWithTag("ThumbR").transform;
            index = GameObject.FindWithTag("IndexR").transform;
            Assert.IsNotNull(thumb);
            Assert.IsNotNull(index);
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
            var radius = GameObject.Find("WIM").GetComponent<MiniatureModel>().Configuration.ScaleFactor * 1.0f;
            var colliders = Physics.OverlapCapsule(capLowerCenter, capUpperCenter, radius, LayerMask.GetMask("Hands"));
            thumbIsTouching = colliders.Contains(thumb.GetComponent<Collider>());
            indexIsTouching = colliders.Contains(index.GetComponent<Collider>());
            var thumbAndIndexTouching = thumbIsTouching && indexIsTouching;
            var thumbAndIndexPressed =
                OVRInput.Get(OVRInput.RawButton.A) && OVRInput.Get(OVRInput.RawButton.RIndexTrigger);

            if(!isGrabbing) {
                // Handle double tap
                switch(tapState) {
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
                        var WIM = GameObject.Find("WIM")?.GetComponent<MiniatureModel>();
                        Debug.Assert(WIM != null, nameof(WIM) + " != null");
                        WIM.ConfirmTravel();
                        break;
                }
            }

            if(!isGrabbing && thumbAndIndexTouching && thumbAndIndexPressed) {
                isGrabbing = true;
                resetDoubleTap();
                startGrabbing();
            }
            else if(isGrabbing && !thumbAndIndexPressed) {
                isGrabbing = false;
                stopGrabbing();
            }
        }

        private void startGrabbing() {
            var WIMTransform = GameObject.Find("WIM").transform;
            var WIM = WIMTransform.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);

            // Remove existing destination indicator (except "this").
            RemoveDestinationIndicatorsExceptWIM(WIM);

            // Actually pick up the new destination indicator.
            WIM.Data.DestinationIndicatorInWIM.parent = index;
            var midPos = thumb.position + (index.position - thumb.position) / 2.0f;
            WIM.Data.DestinationIndicatorInWIM.position = midPos;
        }

        private void RemoveDestinationIndicatorsExceptWIM(MiniatureModel WIM) {
            if(!WIM.Data.DestinationIndicatorInWIM) return;
            if(WIM.Configuration.PreviewScreen) GetComponent<PreviewScreen>().RemovePreviewScreen();
            // Using DestroyImmediate because the WIM is about to being copied and we don't want to copy these objects too.
            DestroyImmediate(WIM.Data.TravelPreviewAnimationObj);
            if(WIM.Data.DestinationIndicatorInLevel) DestroyImmediate(WIM.Data.DestinationIndicatorInLevel.gameObject);
        }

        private void stopGrabbing() {
            var WIMTransform = GameObject.Find("WIM").transform;
            var WIM = WIMTransform.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);

            // Let go. 
            WIM.Data.DestinationIndicatorInWIM.parent = WIMTransform.GetChild(0);

            // Create destination indicator in level. Includes snap to ground.
            if(!WIM.Data.DestinationIndicatorInLevel)
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