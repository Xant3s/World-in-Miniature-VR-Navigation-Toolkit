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
            CopyResults();
            CheckPrefabRoot(leftHand, out leftPrefabRootValid, out var prefabRoot);
            if(!prefabRoot) return;
            CheckForMissingScripts(prefabRoot, out leftPrefabRootHasMissingScripts);
        }

        public void CheckRight(GameObject rightHand) {
            basicValidator.CheckRight(rightHand);
            CopyResults();
            CheckPrefabRoot(rightHand, out rightPrefabRootValid, out var prefabRoot);
            if(!prefabRoot) return;
            CheckForMissingScripts(prefabRoot, out rightPrefabRootHasMissingScripts);
        }

        private void CopyResults() {
            var basicResults = basicValidator.GetResults();
            results.LeftIndexFingerTip |= basicResults.LeftIndexFingerTip;
            results.LeftThumbFingerTip |= basicResults.LeftThumbFingerTip;
            results.RightIndexFingerTip |= basicResults.RightIndexFingerTip;
            results.RightThumbFingerTip |= basicResults.RightThumbFingerTip;
        }

        private void CheckPrefabRoot(GameObject hand, out bool isValid, out GameObject prefabRoot) {
            prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(hand);
            isValid = prefabRoot != null;
            results.PrefabRootsPresent = BothPrefabRootsPresent;
        }

        private void CheckForMissingScripts(GameObject prefabRoot, out bool hasMissingScripts) {
            hasMissingScripts = prefabRoot.HasMissingScripts();
            results.OculusCustomHandsNoMissingScripts = !HandsHaveMissingScripts;
        }

        public ValidationResults GetResults() => results;

        private bool BothPrefabRootsPresent => leftPrefabRootValid && rightPrefabRootValid;
        private bool HandsHaveMissingScripts => leftPrefabRootHasMissingScripts || rightPrefabRootHasMissingScripts;
    }
}