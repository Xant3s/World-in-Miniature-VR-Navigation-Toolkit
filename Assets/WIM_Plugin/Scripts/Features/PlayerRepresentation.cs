using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = System.Diagnostics.Debug;

namespace WIM_Plugin {
    // Show a player representation in the WIM. Used to indicate the player's position and orientation in the virtual environment.
    public class PlayerRepresentation : MonoBehaviour {
        private Transform playerTransform;
        private WIMSpaceConverter converter;

        private void Start() {
            playerTransform = GameObject.Find("OVRCameraRig").transform;
            converter = GameObject.Find("WIM").GetComponent<MiniatureModel>().Converter;
            Assert.IsNotNull(playerTransform);
            Assert.IsNotNull(converter);
        }

        private void OnEnable() {
            MiniatureModel.OnInit += setup;
            MiniatureModel.OnUpdate += updatePlayerRepresentationInWIM;
        }

        private void OnDisable() {
            MiniatureModel.OnInit -= setup;
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
            Debug.Assert(Camera.main != null, "Camera.main != null");
            data.PlayerRepresentationTransform.position = converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(Camera.main.transform.position));
            data.PlayerRepresentationTransform.position += data.WIMLevelTransform.up * config.PlayerRepresentation.transform.localScale.y * config.ScaleFactor;
            // TODO: clamp player representation to visible WIM

            // Rotation
            var rotationInLevel = data.WIMLevelTransform.rotation * playerTransform.rotation;
            data.PlayerRepresentationTransform.rotation = rotationInLevel;
        }
    }
}