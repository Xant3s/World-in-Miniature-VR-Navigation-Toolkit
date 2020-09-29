// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Features.Scaling {
    /// <summary>
    /// Scaling configuration. Modified via GUI.
    /// </summary>
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Scaling")]
    public class ScalingConfiguration : ScriptableObject {
        public bool AllowWIMScaling;
        public float MinScaleFactor;
        public float MaxScaleFactor = .5f;
        public float ScaleStep = .0001f;
        public float InterHandDistanceDeltaThreshold = .1f;
    }
}