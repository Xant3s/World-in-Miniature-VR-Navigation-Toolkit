// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.VR.HandSetup {
    public interface ValidationResults {
        bool LeftIndexFingerTip { get; }
        bool LeftThumbFingerTip { get; }
        bool RightIndexFingerTip { get; }
        bool RightThumbFingerTip { get; }
        bool PrefabRootsPresent { get; }
        public bool HandTags { get; set; }
    }

    public interface HandValidator {
        void CheckLeft(GameObject leftHand);
        void CheckRight(GameObject rightHand);
        ValidationResults GetResults();
    }
}