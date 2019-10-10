using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

namespace WIM_Plugin {
    public class PathTrace : MonoBehaviour {
        private PostTravelPathTraceController controller;

        private void OnEnable() {
            MiniatureModel.OnPreTravel += createPostTravelPathTrace;
            MiniatureModel.OnPostTravel += initPostTravelPathTrace;
        }

        private void OnDisable() {
            MiniatureModel.OnPreTravel -= createPostTravelPathTrace;
            MiniatureModel.OnPostTravel -= initPostTravelPathTrace;
        }

        private void createPostTravelPathTrace(WIMConfiguration config, WIMData data) {
            var emptyGO = new GameObject();
            var postTravelPathTraceObj = new GameObject("Post Travel Path Trace");
            controller = postTravelPathTraceObj.AddComponent<PostTravelPathTraceController>();
            controller.Converter = GameObject.Find("WIM").GetComponent<MiniatureModel>().Converter;
            controller.TraceDurationInSeconds = config.TraceDuration;
            controller.OldPositionInWIM = Instantiate(emptyGO, data.WIMLevelTransform).transform;
            controller.OldPositionInWIM.position = data.PlayerRepresentationTransform.position;
            controller.OldPositionInWIM.name = "PathTraceOldPosition";
            controller.NewPositionInWIM = Instantiate(emptyGO, data.WIMLevelTransform).transform;
            controller.NewPositionInWIM.position = data.DestinationIndicatorInWIM.position;
            controller.NewPositionInWIM.name = "PathTraceNewPosition";
            Destroy(emptyGO);
        }

        private void initPostTravelPathTrace(WIMConfiguration config, WIMData data) {
            controller.WIMLevelTransform = transform.GetChild(0);
            controller.Init();
        }
    }


    [RequireComponent(typeof(LineRenderer))]
    public class PostTravelPathTraceController : MonoBehaviour {
        public WIMSpaceConverter Converter { get; set; }
        public Transform NewPositionInWIM { get; set; }
        public Transform OldPositionInWIM { get; set; }
        public Transform WIMLevelTransform { get; set; }
        public float TraceDurationInSeconds { get; set; }

        private LineRenderer lr;
        private float animationProgress;
        private float endTime;


        private void Awake() {
            lr = GetComponent<LineRenderer>();
        }

        public void Init() {
            Assert.IsNotNull(WIMLevelTransform);
            OldPositionInWIM = WIMLevelTransform.Find("PathTraceOldPosition").transform;
            NewPositionInWIM = WIMLevelTransform.Find("PathTraceNewPosition").transform;
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

        private void Update() {
            if(!WIMLevelTransform) Destroy(gameObject);

            if(Time.time >= endTime) {
                Destroy(gameObject);
            }

            var timeLeft = (endTime - Time.time);
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