// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;

namespace WIMVR.Features {
    /// <summary>
    /// Manages the destination indicator in the miniature model.
    /// </summary>
    public static class DestinationIndicators {
        public static event MiniatureModel.WIMAction OnSpawnDestinationIndicatorInWIM;
        public static event MiniatureModel.WIMAction OnRemoveDestinationIndicators;


        /// <summary>
        /// Spawns the destination indicator in the full-sized level.
        /// The position and orientation will match its counterpart in the miniature model.
        /// </summary>
        /// <returns>The transform of the newly create destination indicator in the full-sized level.</returns>
        public static Transform SpawnDestinationIndicatorInLevel(WIMConfiguration config, WIMData data) {
            var converter = new WIMSpaceConverterImpl(config, data);
            var levelPosition = converter.ConvertToLevelSpace(data.DestinationIndicatorInWIM.position);
            data.DestinationIndicatorInLevel = Object.Instantiate(config.DestinationIndicator, data.LevelTransform).transform;
            data.DestinationIndicatorInLevel.position = levelPosition;

            // Remove frustum.
            Object.Destroy(data.DestinationIndicatorInLevel.GetChild(0).GetChild(1).GetChild(0).gameObject);

            // Optional: Set to ground level to prevent the player from being moved to a location in mid-air.
            if(config.DestinationAlwaysOnTheGround) {
                var destinationIndicatorHeight = config.DestinationIndicator.transform.GetChild(0).localScale.y;
                data.DestinationIndicatorInLevel.position = MathUtils.GetGroundPosition(levelPosition) +
                                                            new Vector3(0, destinationIndicatorHeight, 0);
                data.DestinationIndicatorInWIM.position =
                    converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(levelPosition))
                    + config.ScaleFactor * destinationIndicatorHeight * data.WIMLevelTransform.up;
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

        /// <summary>
        /// Spawns the destination indicator in the miniature model.
        /// </summary>
        /// <returns>The transform of the newly create destination indicator in the miniature model.</returns>
        public static Transform SpawnDestinationIndicatorInWIM(WIMConfiguration config, WIMData data) {
            Assert.IsNotNull(data.WIMLevelTransform);
            Assert.IsNotNull(config.DestinationIndicator);
            data.DestinationIndicatorInWIM = Object.Instantiate(config.DestinationIndicator, data.WIMLevelTransform).transform;

            data.DestinationIndicatorInWIM.position = data.FingertipIndexR.position;
            OnSpawnDestinationIndicatorInWIM?.Invoke(config, data);
            return data.DestinationIndicatorInWIM;
        }

        /// <summary>
        /// Destroys both the destination indicator in the miniature model and in the full-sized level.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="data"></param>
        public static void RemoveDestinationIndicators(WIMConfiguration config, WIMData data) {
            if(!data.DestinationIndicatorInWIM) return;

            OnRemoveDestinationIndicators?.Invoke(config, data);
            data.DestinationIndicatorInWIM.parent = null;
            Object.Destroy(data.DestinationIndicatorInWIM.gameObject);
            if(!data.DestinationIndicatorInLevel) return;
            data.DestinationIndicatorInLevel.parent = null;
            Object.Destroy(data.DestinationIndicatorInLevel.gameObject);
        }
    }
}