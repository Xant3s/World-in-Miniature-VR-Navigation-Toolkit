// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Core {
    /// <summary>
    /// Data describing the current miniature model state. Modified at runtime.
    /// </summary>
    public class WIMData : ScriptableObject {
        public Transform WIMLevelTransform;
        public Transform PlayerRepresentationTransform;
        public Transform DestinationIndicatorInLevel;
        public Transform DestinationIndicatorInWIM;
        public Transform LevelTransform;
        public float WIMHeightRelativeToPlayer;
        public Transform PlayerController;
        public Transform HMDTransform;
    }
}