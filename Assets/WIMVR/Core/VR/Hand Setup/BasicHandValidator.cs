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
            results.LeftIndexFingerTip = LeftIndexFingerTipPrefab.GetComponentInChildren<LeftIndexFingerTip>();
            results.LeftThumbFingerTip = LeftThumbFingerTipPrefab.GetComponentInChildren<LeftThumbFingerTip>();
        }

        public void CheckRight(GameObject rightHand) {
            results.RightIndexFingerTip = RightIndexFingerTipPrefab.GetComponentInChildren<RightIndexFingerTip>();
            results.RightThumbFingerTip = RightThumbFingerTipPrefab.GetComponentInChildren<RightThumbFingerTip>();
        }

        public ValidationResults GetResults() => results;

        private static GameObject LeftIndexFingerTipPrefab => Resources.Load<GameObject>("Fingers/Left Index Finger Tip");
        private static GameObject LeftThumbFingerTipPrefab => Resources.Load<GameObject>("Fingers/Left Thumb Tip");
        private static GameObject RightIndexFingerTipPrefab => Resources.Load<GameObject>("Fingers/Right Index Finger Tip");
        private static GameObject RightThumbFingerTipPrefab => Resources.Load<GameObject>("Fingers/Right Thumb Tip");
    }
}