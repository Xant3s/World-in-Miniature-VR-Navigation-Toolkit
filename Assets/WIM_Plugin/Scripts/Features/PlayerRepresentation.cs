using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    // Show a player representation in the WIM. Used to indicate the player's position and orientation in the virtual environment.
    public class PlayerRepresentation : MonoBehaviour {
        private Transform playerTransform;
        private WIMSpaceConverter converter;

        void Start() {
            playerTransform = GameObject.Find("OVRCameraRig").transform;
            converter = GameObject.Find("WIM").GetComponent<MiniatureModel>().Converter;
            Assert.IsNotNull(playerTransform);
            Assert.IsNotNull(converter);
        }

        void OnEnable() {
            MiniatureModel.OnStart += setup;
            MiniatureModel.OnUpdate += updatePlayerRepresentationInWIM;
        }

        void OnDisable() {
            MiniatureModel.OnStart -= setup;
            MiniatureModel.OnUpdate -= updatePlayerRepresentationInWIM;
        }

        private void setup(WIMConfiguration config, WIMData data) {
            data.PlayerRepresentationTransform = Instantiate(config.PlayerRepresentation, data.WIMLevelTransform).transform;
            if(config.DestinationSelectionMethod == DestinationSelection.Pickup)
                data.PlayerRepresentationTransform.gameObject.AddComponent<PickupDestinationSelection>().DoubleTapInterval = config.DoubleTapInterval;
        }

        private void updatePlayerRepresentationInWIM(WIMConfiguration config, WIMData data) {
            if(!data.PlayerRepresentationTransform) return;

            // Position.
            data.PlayerRepresentationTransform.position = converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(Camera.main.transform.position));
            data.PlayerRepresentationTransform.position += data.WIMLevelTransform.up * config.PlayerRepresentation.transform.localScale.y * config.ScaleFactor;

            // Rotation
            var rotationInLevel = data.WIMLevelTransform.rotation * playerTransform.rotation;
            data.PlayerRepresentationTransform.rotation = rotationInLevel;
        }
    }
}