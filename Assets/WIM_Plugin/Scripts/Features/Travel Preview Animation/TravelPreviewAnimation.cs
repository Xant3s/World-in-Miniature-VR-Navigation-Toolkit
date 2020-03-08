using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

namespace WIM_Plugin {
    public class TravelPreviewAnimation : MonoBehaviour {
        public TravelPreviewConfiguration Config;
        [HideInInspector] public TravelPreviewData Data;


        private void Awake() {
            if(!Config) return;
            Data = ScriptableObject.CreateInstance<TravelPreviewData>();
        }

        private void OnEnable() {
            if(!Config) return;
            MiniatureModel.OnNewDestinationSelected += createController;
            DestinationIndicators.OnRemoveDestinationIndicators += destroyTravelPreviewAnimation;
            PickupDestinationUpdate.OnRemoveDestinationIndicatorExceptWIM += destroyTravelPreviewAnimation;
        }

        private void OnDisable() {
            if(!Config) return;
            MiniatureModel.OnNewDestinationSelected -= createController;
            DestinationIndicators.OnRemoveDestinationIndicators -= destroyTravelPreviewAnimation;
            PickupDestinationUpdate.OnRemoveDestinationIndicatorExceptWIM -= destroyTravelPreviewAnimation;
        }

        private void createController(WIMConfiguration WIMConfig, WIMData WIMData) {
            if(!this.Config.TravelPreviewAnimation) return;
            Assert.IsNotNull(this.Config, "Travel preview configuration is missing.");
            if (Data.TravelPreviewAnimationObj) Data.TravelPreviewAnimationObj.transform.parent = null;
            Destroy(Data.TravelPreviewAnimationObj);
            Data.TravelPreviewAnimationObj = new GameObject("Travel Preview Animation");
            var travelPreviewController = Data.TravelPreviewAnimationObj.AddComponent<TravelPreviewAnimationController>();
            travelPreviewController.DestinationInWIM = WIMData.DestinationIndicatorInWIM;
            travelPreviewController.PlayerRepresentationInWIM = WIMData.PlayerRepresentationTransform;
            travelPreviewController.DestinationIndicator = WIMConfig.DestinationIndicator;
            travelPreviewController.AnimationSpeed = this.Config.TravelPreviewAnimationSpeed;
            travelPreviewController.WIMLevelTransform = WIMData.WIMLevelTransform;
            travelPreviewController.Converter = 
                GameObject.FindWithTag("WIM").GetComponent<MiniatureModel>().Converter;
        }

        private void destroyTravelPreviewAnimation(WIMConfiguration WIMConfig, WIMData WIMData) {
            // Destroy uses another thread, so make sure they are not copied on WIM respawn by removing from parent.
            if(!Data || !Data.TravelPreviewAnimationObj) return;
            Data.TravelPreviewAnimationObj.transform.parent = null;
            Destroy(Data.TravelPreviewAnimationObj);
        }

        private void destroyTravelPreviewAnimation(in MiniatureModel WIM) {
            destroyTravelPreviewAnimation(WIM.Configuration, WIM.Data);
        }
    }
}