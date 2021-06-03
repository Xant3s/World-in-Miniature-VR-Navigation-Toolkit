// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEngine;

namespace WIMVR.VR.HandSetup {
    public class OculusValidationResults : ValidationResults {
        public bool LeftIndexFingerTip { get; set; }
        public bool LeftThumbFingerTip { get; set; }
        public bool RightIndexFingerTip { get; set; }
        public bool RightThumbFingerTip { get; set; }
        public bool PrefabRootsPresent { get; set; }
    }

    public class OculusHandsValidator : HandValidator {
        private readonly OculusValidationResults validationResults = new OculusValidationResults();
        private bool leftPrefabRootValid;
        private bool rightPrefabRootValid;
        
        public void CheckLeft(GameObject leftHand) {
            leftPrefabRootValid = PrefabUtility.GetOutermostPrefabInstanceRoot(leftHand) != null;
            validationResults.PrefabRootsPresent = BothPrefabRootsPresent();
            // TODO: check fingertips
        }

        public void CheckRight(GameObject rightHand) {
            rightPrefabRootValid = PrefabUtility.GetOutermostPrefabInstanceRoot(rightHand) != null;
            validationResults.PrefabRootsPresent = BothPrefabRootsPresent();
            // TODO: check fingertips
        }

        public ValidationResults GetResults() => validationResults;

        private bool BothPrefabRootsPresent() => leftPrefabRootValid && rightPrefabRootValid;
    }
}