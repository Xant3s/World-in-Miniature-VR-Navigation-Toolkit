using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

namespace WIM_Plugin {
    public class TravelPreviewAnimation : MonoBehaviour {
        public TravelPreviewConfiguration TravelPreviewConfig;


        private void OnEnable() {
            MiniatureModel.OnNewDestinationSelected += createController;
        }

        private void OnDisable() {
            MiniatureModel.OnNewDestinationSelected -= createController;
        }

        private void createController(WIMConfiguration config, WIMData data) {
            if(!TravelPreviewConfig.TravelPreviewAnimation) return;
            Assert.IsNotNull(TravelPreviewConfig, "Travel preview configuration is missing.");
            if(data.TravelPreviewAnimationObj) data.TravelPreviewAnimationObj.transform.parent = null;
            Destroy(data.TravelPreviewAnimationObj);
            data.TravelPreviewAnimationObj = new GameObject("Travel Preview Animation");
            var travelPreview = data.TravelPreviewAnimationObj.AddComponent<TravelPreviewAnimationController>();
            travelPreview.DestinationInWIM = data.DestinationIndicatorInWIM;
            travelPreview.PlayerRepresentationInWIM = data.PlayerRepresentationTransform;
            travelPreview.DestinationIndicator = config.DestinationIndicator;
            travelPreview.AnimationSpeed = TravelPreviewConfig.TravelPreviewAnimationSpeed;
            travelPreview.WIMLevelTransform = data.WIMLevelTransform;
            travelPreview.Converter = GameObject.Find("WIM").GetComponent<MiniatureModel>().Converter;
        }
    }


    [CreateAssetMenu(menuName = "WIM/Features/Travel Preview Animation/Configuration")]
    public class TravelPreviewConfiguration : ScriptableObject {
        public bool TravelPreviewAnimation;
        public float TravelPreviewAnimationSpeed = 1.0f;
    }
}