// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Util {
    /// <summary>
    /// Can be used to highlight the gameobject by changing its color.
    /// </summary>
    [DisallowMultipleComponent]
    public class HighlightFX : MonoBehaviour {
        public Color hightlightColor = Color.cyan;

        private Material material;
        private Color defaultColor;
        private bool highlightEnabled;
        private bool hasRenderer;


        /// <summary>
        /// Whether the highlight effect is active.
        /// Changing the value will enable or disable the hightlight effect.
        /// </summary>
        public bool HighlightEnabled {
            get => highlightEnabled;
            set {
                highlightEnabled = value;
                if (hasRenderer) material.color = value ? hightlightColor : defaultColor;
            }
        }

        private void Awake() {
            var renderer = GetComponent<Renderer>();
            hasRenderer = renderer != null;
            material = renderer?.material;
            defaultColor = material.color;
        }
    }
}