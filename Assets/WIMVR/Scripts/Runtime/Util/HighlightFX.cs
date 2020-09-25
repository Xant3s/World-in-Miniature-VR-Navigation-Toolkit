// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Util {
    /// <summary>
    /// Can be used to highlight the gameobject by changing its color.
    /// </summary>
    [DisallowMultipleComponent]
    public class HighlightFX : MonoBehaviour {
        public Color HightlightColor = Color.cyan;
        public float HightlightAlpha = 1.0f;

        private Material material;
        private Color defaultColor;
        private float defaultAlpha;
        private string alphaPropertyName = "_Alpha";
        private bool highlightEnabled;
        private bool hasRenderer;
        private bool useAlpha;


        /// <summary>
        /// Whether the highlight effect is active.
        /// Changing the value will enable or disable the hightlight effect.
        /// </summary>
        public bool HighlightEnabled {
            get => highlightEnabled;
            set {
                highlightEnabled = value;
                if(hasRenderer) {
                    material.color = value ? HightlightColor : defaultColor;
                    if(useAlpha) {
                        var alpha = value ? HightlightAlpha : defaultAlpha;
                        material.SetFloat(alphaPropertyName, alpha);
                    }
                }
            }
        }

        public void SetUseAlpha(bool value, string alphaName = "_Alpha") {
            useAlpha = value;
            alphaPropertyName = alphaName;
            if(useAlpha) defaultAlpha = material.GetFloat(alphaPropertyName);
        }

        private void Awake() {
            var renderer = GetComponent<Renderer>();
            hasRenderer = renderer != null;
            if(hasRenderer) {
                material = renderer.material;
                defaultColor = material.color;
            }
        }
    }
}