// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.VR.HandSetup.Tags;

namespace WIMVR.VR.HandSetup {
    public class BasicValidationResults : ValidationResults {
        public bool LeftIndexFingerTip { get; set; }
        public bool LeftThumbFingerTip { get; set; }
        public bool RightIndexFingerTip { get; set; }
        public bool RightThumbFingerTip { get; set; }
        public bool PrefabRootsPresent { get; set; }
        public bool HandTags { get; set; }
    }
    
    public class BasicHandValidator : HandValidator {
        private readonly BasicValidationResults results = new BasicValidationResults();
        private bool leftHandTagPresent;
        private bool rightHandTagPresent;
        
        public void CheckLeft(GameObject leftHand) {
            results.LeftIndexFingerTip = leftHand.GetComponentInChildren<LeftIndexFingerTip>();
            results.LeftThumbFingerTip = leftHand.GetComponentInChildren<LeftThumbFingerTip>();
            leftHandTagPresent = leftHand.GetComponent<LeftHand>();
            results.HandTags = BothHandTagsPresent;
        }

        public void CheckRight(GameObject rightHand) {
            results.RightIndexFingerTip = rightHand.GetComponentInChildren<RightIndexFingerTip>();
            results.RightThumbFingerTip = rightHand.GetComponentInChildren<RightThumbFingerTip>();
            rightHandTagPresent = rightHand.GetComponent<RightHand>();
            results.HandTags = BothHandTagsPresent;
        }

        public ValidationResults GetResults() => results;

        private bool BothHandTagsPresent => leftHandTagPresent && rightHandTagPresent;
    }
}