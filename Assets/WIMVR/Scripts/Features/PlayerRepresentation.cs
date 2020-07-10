// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Features.Pickup_Destination;
using WIMVR.Util;

namespace WIMVR.Features {
   /// <summary>
   /// Displays a player representation in the miniature model. Used to indicate the player's position and orientation in the virtual environment.
   /// </summary>
    [DisallowMultipleComponent]
   public class PlayerRepresentation : MonoBehaviour {
       public static event MiniatureModel.WIMAction OnUpdatePlayerRepresentationInWIM;

       private MiniatureModel WIM;
       private WIMSpaceConverter converter;
       private Transform playerTransform;
       private Transform mainCameraTransform;


        private void Start() {
            mainCameraTransform = Camera.main.transform;
            playerTransform = GameObject.FindWithTag("Player").transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
            if (!WIM.Configuration) return;
            converter = WIM.Converter;
            Assert.IsNotNull(playerTransform);
            Assert.IsNotNull(converter);
        }

        private void OnEnable() {
            MiniatureModel.OnInitHand += setup;
            MiniatureModel.OnUpdate += updatePlayerRepresentationInWIM;
        }

        private void OnDisable() {
            MiniatureModel.OnInitHand -= setup;
            MiniatureModel.OnUpdate -= updatePlayerRepresentationInWIM;
        }

        private void setup(WIMConfiguration config, WIMData data) {
            var tmp = Instantiate(config.PlayerRepresentation).transform;
            var playerRepresentation = tmp.GetChild(0);
            playerRepresentation.parent = data.WIMLevelTransform;
            Destroy(tmp.gameObject);
            data.PlayerRepresentationTransform = playerRepresentation;
            data.PlayerRepresentationTransform.transform.localScale = new Vector3(0.6155667f, 0.6155667f, 0.6155667f);
            if(config.DestinationSelectionMethod == DestinationSelection.Pickup) {
                Assert.IsNotNull(data.PlayerRepresentationTransform);
                Assert.IsNotNull(config);
                Assert.IsNotNull(data);
                data.PlayerRepresentationTransform.gameObject.AddComponent<DetectPickupGesture>();
                data.PlayerRepresentationTransform.gameObject.AddComponent<PickupDestinationSelection>()
                    .DoubleTapInterval = config.DoubleTapInterval;
            }
        }

        private void updatePlayerRepresentationInWIM(WIMConfiguration config, WIMData data) {
            if(!data.PlayerRepresentationTransform) return;

            // Position.
            Debug.Assert(Camera.main != null, "Camera.main != null");
            data.PlayerRepresentationTransform.position = converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(mainCameraTransform.position));
            var playerRepresentationHeight = config.PlayerRepresentation.transform.GetChild(0).localScale.y;
            data.PlayerRepresentationTransform.position += data.WIMLevelTransform.up * playerRepresentationHeight * config.ScaleFactor;

            // Rotation
            var rotationInLevel = data.WIMLevelTransform.rotation * playerTransform.rotation;
            data.PlayerRepresentationTransform.rotation = rotationInLevel;

            OnUpdatePlayerRepresentationInWIM?.Invoke(config, data);
        }
    }
}