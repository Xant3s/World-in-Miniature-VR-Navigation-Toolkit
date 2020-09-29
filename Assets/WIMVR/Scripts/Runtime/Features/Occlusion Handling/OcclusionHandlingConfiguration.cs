// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Util;

namespace WIMVR.Features.Occlusion_Handling {
    /// <summary>
    /// The occlusion handling configuration. Modified via GUI.
    /// </summary>
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Occlusion Handling")]
    public class OcclusionHandlingConfiguration : ScriptableObject {
        // Occlusion Handling: Melt Walls
        public OcclusionHandlingMethod OcclusionHandlingMethod;
        public float MeltRadius = 1.0f;
        public float MeltHeight = 2.0f;

        // Occlusion Handling: Cutout View
        public float CutoutRange = 10;
        public float CutoutAngle = 30;
        public bool ShowCutoutLight;
        public Color CutoutLightColor = Color.white;
    }
}