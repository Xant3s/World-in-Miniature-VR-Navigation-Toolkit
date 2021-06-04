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
        private readonly OculusValidationResults results = new OculusValidationResults();
        private HandValidator basicValidator = new BasicHandValidator();
        private bool leftPrefabRootValid;
        private bool rightPrefabRootValid;
        
        public void CheckLeft(GameObject leftHand) {
            basicValidator.CheckLeft(leftHand);
            var basicResults = basicValidator.GetResults();
            leftPrefabRootValid = PrefabUtility.GetOutermostPrefabInstanceRoot(leftHand) != null;
            results.PrefabRootsPresent = BothPrefabRootsPresent();
            results.LeftIndexFingerTip = basicResults.LeftIndexFingerTip;
            results.LeftThumbFingerTip = basicResults.LeftThumbFingerTip;
        }

        public void CheckRight(GameObject rightHand) {
            basicValidator.CheckRight(rightHand);
            var basicResults = basicValidator.GetResults();
            rightPrefabRootValid = PrefabUtility.GetOutermostPrefabInstanceRoot(rightHand) != null;
            results.PrefabRootsPresent = BothPrefabRootsPresent();
            results.RightIndexFingerTip = basicResults.RightIndexFingerTip;
            results.RightThumbFingerTip = basicResults.RightThumbFingerTip;
        }

        public ValidationResults GetResults() => results;

        private bool BothPrefabRootsPresent() => leftPrefabRootValid && rightPrefabRootValid;
    }
}