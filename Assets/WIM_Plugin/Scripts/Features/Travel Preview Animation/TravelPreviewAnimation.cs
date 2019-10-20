using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

namespace WIM_Plugin {
    public class TravelPreviewAnimation : MonoBehaviour {
        private void OnEnable() {
            MiniatureModel.OnNewDestinationSelected += createController;
        }

        private void OnDisable() {
            MiniatureModel.OnNewDestinationSelected -= createController;
        }

        private void createController(WIMConfiguration config, WIMData data) {
            if(!config.TravelPreviewAnimation) return;
            if(data.TravelPreviewAnimationObj) data.TravelPreviewAnimationObj.transform.parent = null;
            Destroy(data.TravelPreviewAnimationObj);
            data.TravelPreviewAnimationObj = new GameObject("Travel Preview Animation");
            var travelPreview = data.TravelPreviewAnimationObj.AddComponent<TravelPreviewAnimationController>();
            travelPreview.DestinationInWIM = data.DestinationIndicatorInWIM;
            travelPreview.PlayerRepresentationInWIM = data.PlayerRepresentationTransform;
            travelPreview.DestinationIndicator = config.DestinationIndicator;
            travelPreview.AnimationSpeed = config.TravelPreviewAnimationSpeed;
            travelPreview.WIMLevelTransform = data.WIMLevelTransform;
            travelPreview.Converter = GameObject.Find("WIM").GetComponent<MiniatureModel>().Converter;
        }
    }
}