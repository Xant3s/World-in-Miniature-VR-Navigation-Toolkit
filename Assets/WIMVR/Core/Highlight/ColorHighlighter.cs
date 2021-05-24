// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Util {
    /// <summary>
    /// Can be used to highlight the gameobject by changing its color.
    /// </summary>
    [DisallowMultipleComponent]
    public class ColorHighlighter : MonoBehaviour, IHighlighter {
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
                if(hasRenderer) {
                    material.color = value ? hightlightColor : defaultColor;
                }
            }
        }

        private void Awake() {
            var myRenderer = GetComponent<Renderer>();
            hasRenderer = myRenderer != null;
            if(hasRenderer) {
                material = myRenderer.material;
                defaultColor = material.color;
            }
        }
    }
}