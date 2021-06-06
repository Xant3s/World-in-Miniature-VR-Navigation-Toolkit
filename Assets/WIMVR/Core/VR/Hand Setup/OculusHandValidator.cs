// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEngine;
using WIMVR.Util.Extensions;

namespace WIMVR.VR.HandSetup {
    public class OculusValidationResults : ValidationResults {
        public bool LeftIndexFingerTip { get; set; }
        public bool LeftThumbFingerTip { get; set; }
        public bool RightIndexFingerTip { get; set; }
        public bool RightThumbFingerTip { get; set; }
        public bool PrefabRootsPresent { get; set; }
        public bool OculusCustomHandsNoMissingScripts { get; set; }
        public bool OculusMaterialsConvertedToURP { get; set; }

        public bool OculusCustomHandsImported {
            get => PrefabRootsPresent;
            set => PrefabRootsPresent = value;
        }
    }

    public class OculusHandsValidator : HandValidator {
        private readonly OculusValidationResults results = new OculusValidationResults();
        private readonly HandValidator basicValidator = new BasicHandValidator();
        private bool leftPrefabRootValid;
        private bool rightPrefabRootValid;
        private bool leftPrefabRootHasMissingScripts;
        private bool rightPrefabRootHasMissingScripts;
        
        
        public void CheckLeft(GameObject leftHand) {
            basicValidator.CheckLeft(leftHand);
            var basicResults = basicValidator.GetResults();
            results.LeftIndexFingerTip = basicResults.LeftIndexFingerTip;
            results.LeftThumbFingerTip = basicResults.LeftThumbFingerTip;
            
            var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(leftHand);
            leftPrefabRootValid = prefabRoot != null;
            results.PrefabRootsPresent = BothPrefabRootsPresent;

            leftPrefabRootHasMissingScripts = prefabRoot.HasMissingScripts();
            results.OculusCustomHandsNoMissingScripts = !HandsHaveMissingScripts;
        }

        public void CheckRight(GameObject rightHand) {
            basicValidator.CheckRight(rightHand);
            var basicResults = basicValidator.GetResults();
            results.RightIndexFingerTip = basicResults.RightIndexFingerTip;
            results.RightThumbFingerTip = basicResults.RightThumbFingerTip;

            var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(rightHand);
            rightPrefabRootValid = prefabRoot != null;
            results.PrefabRootsPresent = BothPrefabRootsPresent;

            rightPrefabRootHasMissingScripts = prefabRoot.HasMissingScripts();
            results.OculusCustomHandsNoMissingScripts = !HandsHaveMissingScripts;
        }

        public ValidationResults GetResults() => results;

        private bool BothPrefabRootsPresent => leftPrefabRootValid && rightPrefabRootValid;
        private bool HandsHaveMissingScripts => leftPrefabRootHasMissingScripts || rightPrefabRootHasMissingScripts;
    }
}