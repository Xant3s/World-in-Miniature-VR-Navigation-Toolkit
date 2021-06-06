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
    }
    
    public class BasicHandValidator : HandValidator {
        private readonly BasicValidationResults results = new BasicValidationResults();
        
        public void CheckLeft(GameObject leftHand) {
            results.LeftIndexFingerTip = leftHand.GetComponentInChildren<LeftIndexFingerTip>();
            results.LeftThumbFingerTip = leftHand.GetComponentInChildren<LeftThumbFingerTip>();
        }

        public void CheckRight(GameObject rightHand) {
            results.RightIndexFingerTip = rightHand.GetComponentInChildren<RightIndexFingerTip>();
            results.RightThumbFingerTip = rightHand.GetComponentInChildren<RightThumbFingerTip>();
        }

        public ValidationResults GetResults() => results;
    }
}