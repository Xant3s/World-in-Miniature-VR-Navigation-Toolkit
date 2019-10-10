using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    // Allow scaling the WIM at runtime.
    public class Scaling : MonoBehaviour {
        private WIMConfiguration config;
        private WIMData data;
        private OVRGrabbable grabbable;
        private Transform handL;
        private Transform handR;
        private Hand scalingHand = Hand.NONE;
        private float prevInterHandDistance;


        private void OnEnable() {
            MiniatureModel.OnUpdate += ScaleWIM;
        }

        private void OnDisable() {
            MiniatureModel.OnUpdate -= ScaleWIM;
        }

        private void Awake() {
            grabbable = GameObject.Find("WIM").GetComponent<OVRGrabbable>();
            handL = GameObject.FindWithTag("HandL").transform;
            handR = GameObject.FindWithTag("HandR").transform;
            Assert.IsNotNull(grabbable);
            Assert.IsNotNull(handL);
            Assert.IsNotNull(handR);
        }

        private void ScaleWIM(WIMConfiguration configuration, WIMData data) {
            this.config = configuration;
            this.data = data;
            // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
            if(!config.AllowWIMScaling || !grabbable.isGrabbed) return;

            var grabbingHand = getGrabbingHand();
            var oppositeHand = getOppositeHand(grabbingHand); // This is the potential scaling hand.
            var scaleButton = getGrabButton(oppositeHand);

            // Start scaling if the potential scaling hand (the hand currently not grabbing the WIM) is inside the WIM and starts grabbing.
            // Stop scaling if either hand lets go.
            if(getHandIsInside(oppositeHand) && OVRInput.GetDown(scaleButton)) {
                // Start scaling.
                scalingHand = oppositeHand;
            }
            else if(OVRInput.GetUp(scaleButton)) {
                // Stop scaling.
                scalingHand = Hand.NONE;
            }

            // Check if currently scaling. Abort if not.
            if(scalingHand == Hand.NONE) return;

            // Scale using inter hand distance delta.
            var currInterHandDistance = Vector3.Distance(handL.position, handR.position);
            var distanceDelta = currInterHandDistance - prevInterHandDistance;
            var deltaBeyondThreshold = Mathf.Abs(distanceDelta) >= config.InterHandDistanceDeltaThreshold;
            if(distanceDelta > 0 && deltaBeyondThreshold) {
                config.ScaleFactor += config.ScaleStep;
            }
            else if(distanceDelta < 0 && deltaBeyondThreshold) {
                config.ScaleFactor -= config.ScaleStep;
            }

            prevInterHandDistance = currInterHandDistance;
        }

        private OVRInput.RawButton getGrabButton(Hand hand) {
            if(hand == Hand.NONE) return OVRInput.RawButton.None;
            return hand == Hand.HAND_L ? config.GrabButtonL : config.GrabButtonR;
        }

        private Hand getGrabbingHand() {
            return grabbable.grabbedBy.CompareTag("HandL") ? Hand.HAND_L : Hand.HAND_R;
        }

        private Hand getOppositeHand(Hand hand) {
            if(hand == Hand.NONE) return Hand.NONE;
            return (hand == Hand.HAND_L) ? Hand.HAND_R : Hand.HAND_L;
        }

        private bool getHandIsInside(Hand hand) {
            if(hand == Hand.NONE) return false;
            var handTag = hand == Hand.HAND_L ? "HandL" : "HandR";
            var hitColliders = Physics.OverlapBox(transform.position, config.ActiveAreaBounds,
                data.WIMLevelTransform.rotation, LayerMask.GetMask("Hands"));
            return hitColliders.Any(col => col.transform.root.CompareTag(handTag));
        }
    }
}