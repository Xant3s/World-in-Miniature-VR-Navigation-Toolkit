using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WIM_Plugin;

namespace WIM_Plugin {
    public interface TravelStrategy {
        void Travel(MiniatureModel WIM);
    }

    public class InstantTravel : TravelStrategy {
        public void Travel(MiniatureModel WIM) {
            WIM.transform.parent = WIM.Data.OVRPlayerController; // Maintain transform relative to player.
            WIM.Data.WIMHeightRelativeToPlayer = WIM.transform.position.y - WIM.Data.OVRPlayerController.position.y; // Maintain height relative to player.
            var playerHeight = WIM.Data.OVRPlayerController.position.y - MathUtils.GetGroundPosition(WIM.Data.OVRPlayerController.position).y;
            WIM.Data.OVRPlayerController.position = WIM.Configuration.DestinationAlwaysOnTheGround
                ? MathUtils.GetGroundPosition(WIM.Data.DestinationIndicatorInLevel.position) + Vector3.up * playerHeight
                : WIM.Data.DestinationIndicatorInLevel.position;
            WIM.Data.OVRPlayerController.rotation = WIM.Data.DestinationIndicatorInLevel.rotation;
            WIM.transform.parent = null;
            WIM.GetComponent<Respawn>().respawnWIM(true); // Assist player to orientate at new location.
        }
    }
}