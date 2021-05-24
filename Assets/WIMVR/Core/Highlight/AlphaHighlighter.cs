// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Util {
    /// <summary>
    /// Can be used to highlight the gameobject by changing its transparency.
    /// </summary>
    [DisallowMultipleComponent]
    public class AlphaHighlighter : MonoBehaviour, IHighlighter {
        public float hightlightAlpha = 1.0f;

        private Material material;
        private string alphaPropertyName = "_Alpha";
        private bool hasRenderer;
        private bool highlightEnabled;
        private float defaultAlpha;


        public bool HighlightEnabled {
            get => highlightEnabled;
            set {
                highlightEnabled = value;
                var alpha = value ? hightlightAlpha : defaultAlpha;
                material.SetFloat(alphaPropertyName, alpha);
            }
        }

        public string AlphaPropertyName {
            get => alphaPropertyName;
            set {
                alphaPropertyName = value;
                GetDefaultAlpha();
            }
        }

        private void Awake() {
            InitMaterial();
            GetDefaultAlpha();
        }

        private void InitMaterial() {
            var myRenderer = GetComponent<Renderer>();
            hasRenderer = myRenderer != null;
            if(hasRenderer) {
                material = myRenderer.material;
            }
        }

        private void GetDefaultAlpha() {
            if(hasRenderer) defaultAlpha = material.GetFloat(alphaPropertyName);
        }
    }
}