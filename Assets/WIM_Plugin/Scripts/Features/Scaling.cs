using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    // Allow scaling the WIM at runtime.
    public class Scaling : MonoBehaviour {
        private Hand scalingHand = Hand.NONE;
        private float prevInterHandDistance;


        void OnEnable() { }

        void OnDisable() { }

        //void ScaleWIM(WIMConfiguration config) {
        //    // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
        //    if(!config.AllowWIMScaling || !grabbable.isGrabbed) return;

        //    var grabbingHand = getGrabbingHand();
        //    var oppositeHand = getOppositeHand(grabbingHand); // This is the potential scaling hand.
        //    var scaleButton = getGrabButton(oppositeHand, config);

        //    // Start scaling if the potential scaling hand (the hand currently not grabbing the WIM) is inside the WIM and starts grabbing.
        //    // Stop scaling if either hand lets go.
        //    if(getHandIsInside(oppositeHand) && OVRInput.GetDown(scaleButton)) {
        //        // Start scaling.
        //        scalingHand = oppositeHand;
        //    }
        //    else if(OVRInput.GetUp(scaleButton)) {
        //        // Stop scaling.
        //        scalingHand = Hand.NONE;
        //    }

        //    // Check if currently scaling. Abort if not.
        //    if(scalingHand == Hand.NONE) return;

        //    // Scale using inter hand distance delta.
        //    var currInterHandDistance = Vector3.Distance(handL.position, handR.position);
        //    var distanceDelta = currInterHandDistance - prevInterHandDistance;
        //    var deltaBeyondThreshold = Mathf.Abs(distanceDelta) >= config.InterHandDistanceDeltaThreshold;
        //    if(distanceDelta > 0 && deltaBeyondThreshold) {
        //        config.ScaleFactor += config.ScaleStep;
        //    }
        //    else if(distanceDelta < 0 && deltaBeyondThreshold) {
        //        config.ScaleFactor -= config.ScaleStep;
        //    }

        //    prevInterHandDistance = currInterHandDistance;
        //}

        private OVRInput.RawButton getGrabButton(Hand hand, WIMConfiguration config) {
            if (hand == Hand.NONE) return OVRInput.RawButton.None;
            return hand == Hand.HAND_L ? config.GrabButtonL : config.GrabButtonR;
        }

        //private Hand getGrabbingHand() {
        //    return (grabbable.grabbedBy.tag == "HandL") ? Hand.HAND_L : Hand.HAND_R;
        //}

        private Hand getOppositeHand(Hand hand) {
            if (hand == Hand.NONE) return Hand.NONE;
            return (hand == Hand.HAND_L) ? Hand.HAND_R : Hand.HAND_L;
        }
    }
}