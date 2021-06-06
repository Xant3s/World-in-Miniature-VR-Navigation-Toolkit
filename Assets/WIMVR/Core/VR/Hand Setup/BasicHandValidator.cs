// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Util;
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
            results.LeftIndexFingerTip = PrefabLoader.LeftIndexFingerTipPrefab.GetComponentInChildren<LeftIndexFingerTip>();
            results.LeftThumbFingerTip = PrefabLoader.LeftThumbFingerTipPrefab.GetComponentInChildren<LeftThumbFingerTip>();
        }

        public void CheckRight(GameObject rightHand) {
            results.RightIndexFingerTip = PrefabLoader.RightIndexFingerTipPrefab.GetComponentInChildren<RightIndexFingerTip>();
            results.RightThumbFingerTip = PrefabLoader.RightThumbFingerTipPrefab.GetComponentInChildren<RightThumbFingerTip>();
        }

        public ValidationResults GetResults() => results;
    }
}