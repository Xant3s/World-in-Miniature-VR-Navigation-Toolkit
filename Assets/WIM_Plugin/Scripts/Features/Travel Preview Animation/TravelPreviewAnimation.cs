using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

namespace WIM_Plugin {
    public class TravelPreviewAnimation : MonoBehaviour {
        public TravelPreviewConfiguration Config;
        [HideInInspector] public TravelPreviewData Data;


        private void Awake() {
            Data = ScriptableObject.CreateInstance<TravelPreviewData>();
        }

        private void OnEnable() {
            MiniatureModel.OnNewDestinationSelected += createController;
        }

        private void OnDisable() {
            MiniatureModel.OnNewDestinationSelected -= createController;
        }

        private void createController(WIMConfiguration config, WIMData data) {
            if(!this.Config.TravelPreviewAnimation) return;
            Assert.IsNotNull(this.Config, "Travel preview configuration is missing.");
            var travelPreview = GameObject.Find("WIM").GetComponent<TravelPreviewAnimation>();
            if (travelPreview.Data.TravelPreviewAnimationObj) travelPreview.Data.TravelPreviewAnimationObj.transform.parent = null;
            Destroy(travelPreview.Data.TravelPreviewAnimationObj);
            travelPreview.Data.TravelPreviewAnimationObj = new GameObject("Travel Preview Animation");
            var travelPreviewController = travelPreview.Data.TravelPreviewAnimationObj.AddComponent<TravelPreviewAnimationController>();
            travelPreviewController.DestinationInWIM = data.DestinationIndicatorInWIM;
            travelPreviewController.PlayerRepresentationInWIM = data.PlayerRepresentationTransform;
            travelPreviewController.DestinationIndicator = config.DestinationIndicator;
            travelPreviewController.AnimationSpeed = this.Config.TravelPreviewAnimationSpeed;
            travelPreviewController.WIMLevelTransform = data.WIMLevelTransform;
            travelPreviewController.Converter = GameObject.Find("WIM").GetComponent<MiniatureModel>().Converter;
        }
    }
}