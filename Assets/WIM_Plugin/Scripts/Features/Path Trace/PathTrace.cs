using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    public class PathTrace : MonoBehaviour {
        public PathTraceConfiguration PathTraceConfig;

        private PathTraceController controller;

        private void OnEnable() {
            MiniatureModel.OnPreTravel += createPostTravelPathTrace;
            MiniatureModel.OnPostTravel += initPostTravelPathTrace;
        }

        private void OnDisable() {
            MiniatureModel.OnPreTravel -= createPostTravelPathTrace;
            MiniatureModel.OnPostTravel -= initPostTravelPathTrace;
        }

        private void createPostTravelPathTrace(WIMConfiguration config, WIMData data) {
            Assert.IsNotNull(PathTraceConfig, "Path trace configuration is missing.");

            if(!PathTraceConfig.PostTravelPathTrace) return;
            var emptyGO = new GameObject();
            var postTravelPathTraceObj = new GameObject("Post Travel Path Trace");
            controller = postTravelPathTraceObj.AddComponent<PathTraceController>();
            controller.Converter = GameObject.Find("WIM").GetComponent<MiniatureModel>().Converter;
            controller.TraceDurationInSeconds = PathTraceConfig.TraceDuration;
            controller.OldPositionInWIM = Instantiate(emptyGO, data.WIMLevelTransform).transform;
            controller.OldPositionInWIM.position = data.PlayerRepresentationTransform.position;
            controller.OldPositionInWIM.name = "PathTraceOldPosition";
            controller.NewPositionInWIM = Instantiate(emptyGO, data.WIMLevelTransform).transform;
            controller.NewPositionInWIM.position = data.DestinationIndicatorInWIM.position;
            controller.NewPositionInWIM.name = "PathTraceNewPosition";
            Destroy(emptyGO);
        }

        private void initPostTravelPathTrace(WIMConfiguration config, WIMData data) {
            if(!PathTraceConfig.PostTravelPathTrace) return;
            controller.WIMLevelTransform = transform.GetChild(0);
            controller.Init();
        }
    }


    [CreateAssetMenu(menuName = "WIM/Features/Path Trace/Configuration")]
    public class PathTraceConfiguration : ScriptableObject {
        public bool PostTravelPathTrace;
        public float TraceDuration = 1.0f;
    }
}