// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Util {
    /// <summary>
    /// Can be used to highlight the WIM by changing its color.
    /// </summary>
   [DisallowMultipleComponent]
    public class WIMColorHighlighter : MonoBehaviour, IHighlighter {
        public Color hightlightColor = Color.cyan;

        private static readonly int tint = Shader.PropertyToID("_Tint");
        private Material material;
        private Color defaultColor;
        private bool highlightEnabled;


        /// <summary>
        /// Whether the highlight effect is active.
        /// Changing the value will enable or disable the hightlight effect.
        /// </summary>
        public bool HighlightEnabled {
            get => highlightEnabled;
            set {
                highlightEnabled = value;
                if(material) {
                    var newColor = value ? hightlightColor : defaultColor;
                    material.SetColor(tint, newColor);
                }
            }
        }

        private void Awake() {
            material = Resources.Load<Material>("WIM Material");
            defaultColor = material.GetColor(tint);
        }
    }
}