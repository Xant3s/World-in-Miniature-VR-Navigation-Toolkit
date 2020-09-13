// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Core;
using WIMVR.Util;

namespace WIMVR.Features.Distance_Grab {
    [DisallowMultipleComponent]
    public class DistanceGrabbing : HandInitializer {
        
        // TODO add distance grabbing to settings (editor)
        // TODO update hands on enable/disable distance grabbing (in editor? -> runtime)
        // TODO forward input to corresponding hand
        
        // TODO setup when enabled (public bool member)
        
        public bool distanceGrabbingEnabled;
        

        private void Start() {
            if(!distanceGrabbingEnabled) return;
            StartWaitForHands();
        }
        
        protected override void LeftHandInitialized(GameObject leftHand) {
            Debug.Log("left hand initialized");
            SetupHand(Hand.LeftHand, leftHand);
        }
        
        protected override void RightHandInitialized(GameObject rightHand) {
            Debug.Log("right hand initialized");
            SetupHand(Hand.RightHand, rightHand);
        }

        private void SetupHand(Hand hand, GameObject obj) {
            // var distanceGrabber = obj.AddComponent<DistanceGrabber>();
            
        }

        public void OnDistanceGrabRightHand() {
            // if(!rightHandInitialized) return;
            Debug.Log("Distance grab right");
        }

        public void OnDistanceGrabLeftHand() {
            // if(!leftHandInitialized) return;
            Debug.Log("Distance grab left");
        }
    }
}