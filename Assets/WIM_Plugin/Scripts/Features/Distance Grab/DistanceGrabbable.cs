using UnityEngine;

namespace WIM_Plugin {
    [DisallowMultipleComponent]
    public class DistanceGrabbable : MonoBehaviour {
        public Transform Target { get; set; }

        public bool IsBeingGrabbed { get; set; }

        public bool HighlightFX {
            get => highlightFX;
            set {
                highlightFX = value;
                if(isWIM) {
                    var color = highlightFX ? highlightColor : defaultColor;
                    material.SetColor(colorProperty, color);
                }
                else {
                    GetComponent<Renderer>().material = highlightFX ? blueMaterial : material;
                }
            }
        }

        private bool highlightFX;

        public float SnapSpeed { get; set; } = 1f;
        public float MinDistance { get; set; } = .1f;

        private static readonly int tintID = Shader.PropertyToID("_Tint");
        private static readonly int colorID = Shader.PropertyToID("_BaseColor");
        private int colorProperty;
        private Material material;
        private Material blueMaterial;
        private Color defaultColor = new Color(80f/255f, 80f/255f, 80f/255f);   // Gray
        private readonly Color highlightColor = new Color(37f/255f, 188/255f, 1);  // Cyan
        private Rigidbody rb;
        private bool isWIM;

        private void Awake() {
            blueMaterial = Resources.Load<Material>("Materials/Blue");
            material = GetComponentInChildren<Renderer>().sharedMaterial;
            colorProperty = material.HasProperty(tintID) ? tintID : colorID;    
            defaultColor = material.GetColor(colorProperty);
            isWIM = gameObject.CompareTag("WIM");
        }

        private void Update() {
            HighlightFX = false;
            if (!IsBeingGrabbed || !Target) return;

            if (Vector3.Distance(Target.position, transform.position) < MinDistance) {
                IsBeingGrabbed = false;
                Target = null;
                rb = GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else {
                var step = SnapSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, Target.position, step);
            }
        }
    }
}