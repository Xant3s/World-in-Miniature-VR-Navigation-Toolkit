// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Features;
using WIMVR.Util;

namespace WIMVR.Core {
    /// <summary>
    /// Defines how the player is moved to the selected destination.
    /// </summary>
    public interface TravelStrategy {
        /// <summary>
        /// Starts the travel phase.
        /// </summary>
        /// <param name="WIM">The miniature model.</param>
        void Travel(MiniatureModel WIM);
    }


    /// <summary>
    /// A travel strategy that instantly teleports the player to the selected destination.
    /// </summary>
    public class InstantTravel : TravelStrategy {
        /// <summary>
        /// Instantly teleports the player to the selected destination.
        /// </summary>
        /// <param name="WIM">The miniature model.</param>
        public void Travel(MiniatureModel WIM) {
            WIM.transform.parent = WIM.Data.PlayerController; // Maintain transform relative to player.
            WIM.Data.WIMHeightRelativeToPlayer = WIM.transform.position.y - WIM.Data.PlayerController.position.y; // Maintain height relative to player.
            var playerHeight = WIM.Data.PlayerController.position.y - MathUtils.GetGroundPosition(WIM.Data.PlayerController.position).y;
            WIM.Data.PlayerController.position = WIM.Configuration.DestinationAlwaysOnTheGround
                ? MathUtils.GetGroundPosition(WIM.Data.DestinationIndicatorInLevel.position) + Vector3.up * playerHeight
                : WIM.Data.DestinationIndicatorInLevel.position;
            WIM.Data.PlayerController.rotation = WIM.Data.DestinationIndicatorInLevel.rotation;
            WIM.transform.parent = null;
            WIM.GetComponent<Respawn>().RespawnWIM(true); // Assist player to orientate at new location.
        }
    }
}