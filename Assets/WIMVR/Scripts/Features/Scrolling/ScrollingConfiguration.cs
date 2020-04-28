// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Features.Scrolling {
    /// <summary>
    /// The scrolling configuration. Modified via GUI.
    /// </summary>
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Scrolling")]
    public class ScrollingConfiguration : ScriptableObject {
        public bool AllowWIMScrolling;
        public bool AutoScroll;
        public bool AllowVerticalScrolling = true;
        public float ScrollSpeed = 1;
    }
}