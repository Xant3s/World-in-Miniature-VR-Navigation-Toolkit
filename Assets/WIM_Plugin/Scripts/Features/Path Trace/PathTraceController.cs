// Author: Samuel Truman (contact@samueltruman.com)

using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    [RequireComponent(typeof(LineRenderer))]
    [DisallowMultipleComponent]
    public class PathTraceController : MonoBehaviour {
        private LineRenderer lr;
        private float animationProgress;
        private float endTime;
        public WIMSpaceConverter Converter { get; set; }
        public Transform NewPositionInWIM { get; set; }
        public Transform OldPositionInWIM { get; set; }
        public Transform WIMLevelTransform { get; set; }
        public float TraceDurationInSeconds { get; set; }

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
            lr.material = Resources.Load<Material>("Materials/SemiTransparent");
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