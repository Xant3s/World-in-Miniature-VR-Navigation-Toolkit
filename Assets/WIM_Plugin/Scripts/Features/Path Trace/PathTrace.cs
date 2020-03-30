// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    /// <summary>
    /// Can be used to display a path trace after the travel phase.
    /// Used as orientation aid.
    /// </summary>
    [DisallowMultipleComponent]
    public class PathTrace : MonoBehaviour {
        public PathTraceConfiguration PathTraceConfig;

        private PathTraceController controller;
        private MiniatureModel WIM;


        private void Awake() {
            if(!PathTraceConfig) return;
            WIM = GameObject.Find("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

        private void OnEnable() {
            if(!PathTraceConfig) return;
            MiniatureModel.OnPreTravel += CreatePostTravelPathTrace;
            MiniatureModel.OnPostTravel += InitPostTravelPathTrace;
        }

        private void OnDisable() {
            if(!PathTraceConfig) return;
            MiniatureModel.OnPreTravel -= CreatePostTravelPathTrace;
            MiniatureModel.OnPostTravel -= InitPostTravelPathTrace;
        }

        private void CreatePostTravelPathTrace(WIMConfiguration config, WIMData data) {
            Assert.IsNotNull(PathTraceConfig, "Path trace configuration is missing.");

            if(!PathTraceConfig.PostTravelPathTrace) return;
            var emptyGO = new GameObject();
            var postTravelPathTraceObj = new GameObject("Post Travel Path Trace");
            controller = postTravelPathTraceObj.AddComponent<PathTraceController>();
            controller.Converter = WIM.Converter;
            controller.TraceDurationInSeconds = PathTraceConfig.TraceDuration;
            controller.OldPositionInWIM = Instantiate(emptyGO, data.WIMLevelTransform).transform;
            controller.OldPositionInWIM.position = data.PlayerRepresentationTransform.position;
            controller.OldPositionInWIM.name = "PathTraceOldPosition";
            controller.NewPositionInWIM = Instantiate(emptyGO, data.WIMLevelTransform).transform;
            controller.NewPositionInWIM.position = data.DestinationIndicatorInWIM.position;
            controller.NewPositionInWIM.name = "PathTraceNewPosition";
            Destroy(emptyGO);
        }

        private void InitPostTravelPathTrace(WIMConfiguration config, WIMData data) {
            if(!PathTraceConfig.PostTravelPathTrace) return;
            controller.WIMLevelTransform = transform.GetChild(0);
            controller.Init();
        }
    }
}