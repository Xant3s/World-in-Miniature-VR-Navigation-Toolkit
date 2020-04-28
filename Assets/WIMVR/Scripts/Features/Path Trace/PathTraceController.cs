// Author: Samuel Truman (contact@samueltruman.com)

using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Util;

namespace WIMVR.Features.Path_Trace {
    /// <summary>
    /// Manages the path trace effect.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [DisallowMultipleComponent]
    public class PathTraceController : MonoBehaviour {
        private LineRenderer lr;
        private float animationProgress;
        private float endTime;

        /// <summary>
        /// The converter used to convert between world space and WIM space.
        /// </summary>
        public WIMSpaceConverter Converter { get; set; }

        /// <summary>
        /// The position in the WIM after the travel phase.
        /// </summary>
        public Transform NewPositionInWIM { get; set; }

        /// <summary>
        /// The position in the WIM before the travel phase.
        /// </summary>
        public Transform OldPositionInWIM { get; set; }

        /// <summary>
        /// The miniature model level.
        /// </summary>
        public Transform WIMLevelTransform { get; set; }

        /// <summary>
        ///  The duration of the path trace animation.
        /// </summary>
        public float TraceDurationInSeconds { get; set; }


        /// <summary>
        /// Initialize.
        /// </summary>
        public void Init() {
            Assert.IsNotNull(WIMLevelTransform);

            Transform FindLastChild(Transform p, string name) {
                return p.Cast<Transform>().LastOrDefault(c => c.name.Equals(name));
            }
            OldPositionInWIM = FindLastChild(WIMLevelTransform, "PathTraceOldPosition");
            NewPositionInWIM = FindLastChild(WIMLevelTransform, "PathTraceNewPosition");
            Assert.IsNotNull(OldPositionInWIM);
            Assert.IsNotNull(NewPositionInWIM);
            lr.widthMultiplier = .001f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            var gradient = new Gradient();
            gradient.SetKeys(
                new[] {new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1.0f)},
                new[] {new GradientAlphaKey(0, 0), new GradientAlphaKey(1, 1)}
            );
            lr.colorGradient = gradient;
            lr.material = Resources.Load<Material>("SemiTransparent");
            endTime = Time.time + TraceDurationInSeconds;
        }


        private void Awake() {
            lr = GetComponent<LineRenderer>();
        }

        private void Update() {
            var timeIsUp = Time.time >= endTime;
            if(!WIMLevelTransform || timeIsUp) {
                Destroy(gameObject);
                return;
            }

            var timeLeft = endTime - Time.time;
            var timePast = TraceDurationInSeconds - timeLeft;
            var progress = timePast / TraceDurationInSeconds;
            progress = Mathf.Clamp(progress, 0, 1);

            var dir = NewPositionInWIM.position - OldPositionInWIM.position;
            var currPos = OldPositionInWIM.position + dir * progress;
            lr.SetPosition(0, currPos);
            lr.SetPosition(1, NewPositionInWIM.position);
        }

        private void OnDestroy() {
            if(OldPositionInWIM) Destroy(OldPositionInWIM.gameObject);
            if(NewPositionInWIM) Destroy(NewPositionInWIM.gameObject);
        }
    }
}