using UnityEngine;

namespace WIM_Plugin {
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Scaling")]
    public class ScalingConfiguration : ScriptableObject {
        public bool AllowWIMScaling;
        public float MinScaleFactor;
        public float MaxScaleFactor = .5f;
        public float ScaleStep = .0001f;
        public float InterHandDistanceDeltaThreshold = .1f;
    }
}