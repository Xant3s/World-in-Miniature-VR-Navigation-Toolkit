﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;
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
            playerTransform = FindObjectOfType<XRRig>()?.transform;
            WIM = FindObjectOfType<MiniatureModel>();
            Assert.IsNotNull(WIM);
            if (!WIM.Configuration) return;
            converter = WIM.Converter;
            Assert.IsNotNull(playerTransform, "XR Rig not found in scene.");
            Assert.IsNotNull(converter);
        }

        private void OnEnable() {
            MiniatureModel.OnInit += Setup;
            MiniatureModel.OnUpdate += UpdatePlayerRepresentationInWIM;
        }

        private void OnDisable() {
            MiniatureModel.OnInit -= Setup;
            MiniatureModel.OnUpdate -= UpdatePlayerRepresentationInWIM;
        }
        
        private static void Setup(WIMConfiguration config, WIMData data) {
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

        private void UpdatePlayerRepresentationInWIM(WIMConfiguration config, WIMData data) {
            if(!data.PlayerRepresentationTransform) return;

            // Position.
            Assert.IsNotNull(Camera.main, "No main camera found.");
            data.PlayerRepresentationTransform.position = converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(mainCameraTransform.position));
            var playerRepresentationHeight = config.PlayerRepresentation.transform.GetChild(0).localScale.y;
            data.PlayerRepresentationTransform.position += data.WIMLevelTransform.up * playerRepresentationHeight * config.ScaleFactor;

            // Rotation
            var playerRotation = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);  ;
            var rotationInLevel = data.WIMLevelTransform.rotation * Quaternion.Euler(playerRotation);
            data.PlayerRepresentationTransform.rotation = rotationInLevel;

            OnUpdatePlayerRepresentationInWIM?.Invoke(config, data);
        }
    }
}