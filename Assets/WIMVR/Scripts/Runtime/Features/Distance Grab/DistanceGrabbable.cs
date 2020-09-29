// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Features.Distance_Grab {
    /// <summary>
    /// Makes object distance grabbable.
    /// </summary>
    [DisallowMultipleComponent]
    public class DistanceGrabbable : MonoBehaviour {
        private static readonly int tintID = Shader.PropertyToID("_Tint");
        private static readonly int colorID = Shader.PropertyToID("_BaseColor");
        private readonly Color highlightColor = new Color(37f/255f, 188/255f, 1);  // Cyan

        private bool highlightFX;
        private int colorProperty;
        private Material material;
        private Material blueMaterial;
        private Color defaultColor = new Color(80f/255f, 80f/255f, 80f/255f);   // Gray
        private Rigidbody rb;
        private bool isWIM;

        /// <summary>
        /// The transform this object is moving towards to while being pulled by distance grabber.
        /// </summary>
        public Transform Target { get; set; }

        /// <summary>
        /// Whether this object is currently being pulled by distance grabber.
        /// </summary>
        public bool IsBeingGrabbed { get; set; }

        /// <summary>
        /// The highlight effect visible while player is aiming at this object to pull it using distance grabber.
        /// </summary>
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

        /// <summary>
        /// Specifies how fast the object is being pulled by distance grabber.
        /// </summary>
        public float SnapSpeed { get; set; } = 1f;

        /// <summary>
        /// Specifies at which distance to the target the object will stop.
        /// </summary>
        public float MinDistance { get; set; } = .1f;

        private void Awake() {
            blueMaterial = Resources.Load<Material>("Blue");
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