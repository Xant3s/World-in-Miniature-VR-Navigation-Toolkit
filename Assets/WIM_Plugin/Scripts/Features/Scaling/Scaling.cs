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

        private static readonly string grabLActionName = "Left Grab Button";
        private static readonly string grabRActionName = "Right Grab Button";
        private WIMConfiguration config;
        private WIMData data;
        private OVRGrabbable grabbable;
        private Transform handL;
        private Transform handR;
        private Hand scalingHand = Hand.NONE;
        private float prevInterHandDistance;
        private bool leftScaleButtonPressed;
        private bool rightScaleButtonPressed;


        private void OnEnable() {
            MiniatureModel.OnUpdate += ScaleWIM;
            InputManager.RegisterAction(grabLActionName, leftScalingButtonEvent,
                InputManager.ButtonTrigger.ButtonUpAndDown);
            InputManager.RegisterAction(grabRActionName, rightScalingButtonEvent,
                InputManager.ButtonTrigger.ButtonUpAndDown);
        }

        private void OnDisable() {
            MiniatureModel.OnUpdate -= ScaleWIM;
            InputManager.UnregisterAction(grabLActionName);
            InputManager.UnregisterAction(grabRActionName);
        }

        private void Awake() {
            grabbable = GameObject.Find("WIM").GetComponent<OVRGrabbable>();
            handL = GameObject.FindWithTag("HandL").transform;
            handR = GameObject.FindWithTag("HandR").transform;
            Assert.IsNotNull(grabbable);
            Assert.IsNotNull(handL);
            Assert.IsNotNull(handR);
        }

        private void leftScalingButtonEvent() {
            updateScalingHand(ref leftScaleButtonPressed, Hand.HAND_L);
        }

        private void rightScalingButtonEvent() {
            updateScalingHand(ref rightScaleButtonPressed, Hand.HAND_R);
        }

        private void updateScalingHand(ref bool buttonPressed, Hand hand) {
            // Either GetDown or GetUp event.
            buttonPressed = !buttonPressed;

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
                scalingHand = Hand.NONE;
            }
        }

        private void ScaleWIM(WIMConfiguration configuration, WIMData data) {
            this.config = configuration;
            this.data = data;
            Assert.IsNotNull(ScalingConfig, "Scaling configuration is missing.");

            // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
            if (!ScalingConfig.AllowWIMScaling || !grabbable.isGrabbed) return;

            // Check if currently scaling. Abort if not.
            if (scalingHand == Hand.NONE) return;

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
            GameObject.Find("WIM").transform.localScale =
                new Vector3(config.ScaleFactor, config.ScaleFactor, config.ScaleFactor);

            prevInterHandDistance = currInterHandDistance;
        }

        private Hand getGrabbingHand() {
            return grabbable.grabbedBy.CompareTag("HandL") ? Hand.HAND_L : Hand.HAND_R;
        }

        private Hand getOppositeHand(Hand hand) {
            if (hand == Hand.NONE) return Hand.NONE;
            return (hand == Hand.HAND_L) ? Hand.HAND_R : Hand.HAND_L;
        }

        private bool getHandIsInside(Hand hand) {
            if (hand == Hand.NONE) return false;
            var handTag = hand == Hand.HAND_L ? "HandL" : "HandR";
            var hitColliders = Physics.OverlapBox(transform.position, config.ActiveAreaBounds,
                data.WIMLevelTransform.rotation, LayerMask.GetMask("Hands"));
            return hitColliders.Any(col => col.transform.root.CompareTag(handTag));
        }
    }
}