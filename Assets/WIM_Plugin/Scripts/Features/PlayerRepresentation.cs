using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    // Show a player representation in the WIM. Used to indicate the player's position and orientation in the virtual environment.
    public class PlayerRepresentation : MonoBehaviour {
        private MiniatureModel WIM;
        private WIMSpaceConverter converter;
        private Transform playerTransform;


        private void Start() {
            playerTransform = GameObject.Find("OVRCameraRig").transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
            if(!WIM.Configuration) return;
            converter = WIM.Converter;
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
            data.PlayerRepresentationTransform.transform.localScale = new Vector3(0.6155667f, 0.6155667f, 0.6155667f);
            if(config.DestinationSelectionMethod == DestinationSelection.Pickup)
                data.PlayerRepresentationTransform.gameObject.AddComponent<PickupDestinationSelection>().DoubleTapInterval = config.DoubleTapInterval;
        }

        private void updatePlayerRepresentationInWIM(WIMConfiguration config, WIMData data) {
            if(!data.PlayerRepresentationTransform) return;

            // Position.
            Debug.Assert(Camera.main != null, "Camera.main != null");
            data.PlayerRepresentationTransform.position = converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(Camera.main.transform.position));
            data.PlayerRepresentationTransform.position += data.WIMLevelTransform.up * config.PlayerRepresentation.transform.localScale.y * config.ScaleFactor;
            var scrolling = WIM.GetComponent<Scrolling>();
            if(scrolling && scrolling.ScrollingConfig && scrolling.ScrollingConfig.AllowWIMScrolling) {
                // Get closest point on active area bounds. Won't have any effect if already inside active area.
                data.PlayerRepresentationTransform.position =
                    WIM.GetComponent<Collider>().ClosestPoint(data.PlayerRepresentationTransform.position);
            }

            // Rotation
            var rotationInLevel = data.WIMLevelTransform.rotation * playerTransform.rotation;
            data.PlayerRepresentationTransform.rotation = rotationInLevel;
        }
    }
}