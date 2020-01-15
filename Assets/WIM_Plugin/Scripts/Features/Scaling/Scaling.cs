using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    // Allow scaling the WIM at runtime.
    [ExecuteAlways]
    public class Scaling : MonoBehaviour {
        public ScalingConfiguration ScalingConfig;
        private WIMConfiguration config;
        private WIMData data;
        private OVRGrabbable grabbable;
        private Transform handL;
        private Transform handR;
        private Transform WIMTransform;
        private Hand scalingHand = Hand.None;
        private float prevInterHandDistance;
        private bool leftScaleButtonPressed;
        private bool rightScaleButtonPressed;


        private void OnEnable() {
            if(!ScalingConfig) return;
            MiniatureModel.OnUpdate += ScaleWIM;
            MiniatureModel.OnLeftGrabButtonDown += leftScalingButtonDown;
            MiniatureModel.OnLeftGrabButtonUp += leftScalingButtonUp;
            MiniatureModel.OnRightGrabButtonDown += rightScalingButtonDown;
            MiniatureModel.OnRightGrabButtonUp += rightScalingButtonUp;
        }

        private void OnDisable() {
            if(!ScalingConfig) return;
            MiniatureModel.OnUpdate -= ScaleWIM;
            MiniatureModel.OnLeftGrabButtonDown -= leftScalingButtonDown;
            MiniatureModel.OnLeftGrabButtonUp -= leftScalingButtonUp;
            MiniatureModel.OnRightGrabButtonDown -= rightScalingButtonDown;
            MiniatureModel.OnRightGrabButtonUp -= rightScalingButtonUp;
        }

        private void Awake() {
            if(!ScalingConfig) return;
            WIMTransform = GameObject.FindWithTag("WIM").transform;
            grabbable = WIMTransform.GetComponent<OVRGrabbable>();
            handL = GameObject.FindWithTag("HandL").transform;
            handR = GameObject.FindWithTag("HandR").transform;
            Assert.IsNotNull(grabbable);
            Assert.IsNotNull(handL);
            Assert.IsNotNull(handR);
        }

        private void leftScalingButtonDown(WIMConfiguration config, WIMData data) {
            leftScaleButtonPressed = true;
            updateScalingHand(rightScaleButtonPressed, Hand.LeftHand);
        }

        private void leftScalingButtonUp(WIMConfiguration config, WIMData data) {
            leftScaleButtonPressed = false;
            updateScalingHand(rightScaleButtonPressed, Hand.LeftHand);
        }

        private void rightScalingButtonDown(WIMConfiguration config, WIMData data) {
            rightScaleButtonPressed = true;
            updateScalingHand(rightScaleButtonPressed, Hand.RightHand);
        }

        private void rightScalingButtonUp(WIMConfiguration config, WIMData data) {
            rightScaleButtonPressed = false;
            updateScalingHand(rightScaleButtonPressed, Hand.RightHand);
        }

        private void updateScalingHand(bool buttonPressed, Hand hand) {
            // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
            if (!ScalingConfig.AllowWIMScaling || !grabbable.isGrabbed) return;

            var grabbingHand = getGrabbingHand();
            var oppositeHand = getOppositeHand(grabbingHand); // This is the potential scaling hand.
            if (oppositeHand != hand) return;

            // Start scaling if the potential scaling hand (the hand currently not grabbing the WIM) is inside the WIM and starts grabbing.
            // Stop scaling if either hand lets go.
            if (getHandIsInside(oppositeHand) && buttonPressed) {
                // Start scaling.
                scalingHand = oppositeHand;
            }
            else if (!buttonPressed) {
                // Stop scaling.
                scalingHand = Hand.None;
            }
        }

        private void ScaleWIM(WIMConfiguration configuration, WIMData data) {
            this.config = configuration;
            this.data = data;
            Assert.IsNotNull(ScalingConfig, "Scaling configuration is missing.");

            // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
            if (!ScalingConfig.AllowWIMScaling || !grabbable.isGrabbed) return;

            // Check if currently scaling. Abort if not.
            if (scalingHand == Hand.None) return;

            // Scale using inter hand distance delta.
            var currInterHandDistance = Vector3.Distance(handL.position, handR.position);
            var distanceDelta = currInterHandDistance - prevInterHandDistance;
            var deltaBeyondThreshold = Mathf.Abs(distanceDelta) >= ScalingConfig.InterHandDistanceDeltaThreshold;
            if (distanceDelta > 0 && deltaBeyondThreshold) {
                config.ScaleFactor += ScalingConfig.ScaleStep;
            }
            else if (distanceDelta < 0 && deltaBeyondThreshold) {
                config.ScaleFactor -= ScalingConfig.ScaleStep;
            }

            // Apply scale factor.
            WIMTransform.localScale = new Vector3(config.ScaleFactor, config.ScaleFactor, config.ScaleFactor);

            prevInterHandDistance = currInterHandDistance;
        }

        private Hand getGrabbingHand() {
            return grabbable.grabbedBy.CompareTag("HandL") ? Hand.LeftHand : Hand.RightHand;
        }

        private Hand getOppositeHand(Hand hand) {
            if (hand == Hand.None) return Hand.None;
            return (hand == Hand.LeftHand) ? Hand.RightHand : Hand.LeftHand;
        }

        private bool getHandIsInside(Hand hand) {
            if (hand == Hand.None) return false;
            var handTag = hand == Hand.LeftHand ? "HandL" : "HandR";
            var hitColliders = Physics.OverlapBox(transform.position, config.ActiveAreaBounds,
                data.WIMLevelTransform.rotation, LayerMask.GetMask("Hands"));
            return hitColliders.Any(col => col.transform.root.CompareTag(handTag));
        }
    }
}