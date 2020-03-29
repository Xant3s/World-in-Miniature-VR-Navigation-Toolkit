// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;


namespace WIM_Plugin {
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Path Trace")]
    public class PathTraceConfiguration : ScriptableObject {
        public bool PostTravelPathTrace;
        public float TraceDuration = 1.0f;
    }
}