﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

namespace WIM_Plugin {
    public static class DestinationIndicators {
        public static Transform SpawnDestinationIndicatorInLevel(WIMConfiguration config, WIMData data) {
            var converter = new WIMSpaceConverterImpl(config, data);
            var levelPosition = converter.ConvertToLevelSpace(data.DestinationIndicatorInWIM.position);
            data.DestinationIndicatorInLevel =
                Object.Instantiate(config.DestinationIndicator, data.LevelTransform).transform;
            data.DestinationIndicatorInLevel.position = levelPosition;

            // Remove frustum.
            Object.Destroy(data.DestinationIndicatorInLevel.GetChild(1).GetChild(0).gameObject);

            // Optional: Set to ground level to prevent the player from being moved to a location in mid-air.
            if(config.DestinationAlwaysOnTheGround) {
                data.DestinationIndicatorInLevel.position = MathUtils.GetGroundPosition(levelPosition) +
                                                            new Vector3(0,
                                                                config.DestinationIndicator.transform.localScale.y, 0);
                data.DestinationIndicatorInWIM.position =
                    converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(levelPosition))
                    + config.ScaleFactor * config.DestinationIndicator.transform.localScale.y *
                    data.WIMLevelTransform.up;
            }

            // Fix orientation.
            if(config.DestinationSelectionMethod == DestinationSelection.Pickup &&
               config.DestinationAlwaysOnTheGround) {
                data.DestinationIndicatorInLevel.rotation = Quaternion.Inverse(data.WIMLevelTransform.rotation) *
                                                            data.DestinationIndicatorInWIM.rotation;
                var forwardPos = data.DestinationIndicatorInLevel.position + data.DestinationIndicatorInLevel.forward;
                forwardPos.y = data.DestinationIndicatorInLevel.position.y;
                var forwardInLevel = forwardPos - data.DestinationIndicatorInLevel.position;
                data.DestinationIndicatorInLevel.rotation = Quaternion.LookRotation(forwardInLevel, Vector3.up);
                data.DestinationIndicatorInWIM.rotation =
                    data.WIMLevelTransform.rotation * data.DestinationIndicatorInLevel.rotation;
            }

            return data.DestinationIndicatorInLevel;
        }

        public static Transform SpawnDestinationIndicatorInWIM(WIMConfiguration config, WIMData data) {
            Assert.IsNotNull(data.WIMLevelTransform);
            Assert.IsNotNull(config.DestinationIndicator);
            data.DestinationIndicatorInWIM =
                Object.Instantiate(config.DestinationIndicator, data.WIMLevelTransform).transform;
            data.DestinationIndicatorInWIM.position = data.FingertipIndexR.position;
            // TODO: Decouple Preview screen config
            var p = GameObject.Find("WIM").GetComponent<PreviewScreen>();
            if(p && p.PreviewScreenConfig.PreviewScreen && !p.PreviewScreenConfig.AutoPositionPreviewScreen)
                data.DestinationIndicatorInWIM.GetChild(1).GetChild(0).gameObject.AddComponent<PickupPreviewScreen>();
            return data.DestinationIndicatorInWIM;
        }

        public static void RemoveDestinationIndicators(MiniatureModel WIM) {
            var data = WIM.Data;
            if(!data.DestinationIndicatorInWIM) return;
            WIM.transform.GetComponent<PreviewScreen>().RemovePreviewScreen();
            // Destroy uses another thread, so make sure they are not copied by removing from parent.
            if(data.TravelPreviewAnimationObj) {
                data.TravelPreviewAnimationObj.transform.parent = null;
                Object.Destroy(data.TravelPreviewAnimationObj);
            }

            data.DestinationIndicatorInWIM.parent = null;
            Object.Destroy(data.DestinationIndicatorInWIM.gameObject);
            if(!data.DestinationIndicatorInLevel) return;
            data.DestinationIndicatorInLevel.parent = null;
            Object.Destroy(data.DestinationIndicatorInLevel.gameObject);
        }
    }
}