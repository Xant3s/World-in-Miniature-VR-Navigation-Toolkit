// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine.Events;

namespace WIMVR.Util {
    public enum OcclusionHandlingMethod { None, MeltWalls, CutoutView }
    public enum Hand { None, LeftHand, RightHand }
    public enum DestinationSelection { Pickup, Touch }

    public class HandEvent : UnityEvent<Hand>{}
    
    
    public static class TypeUtils {
        public static Hand GetOppositeHand(Hand hand) {
            return (Hand) ((int) hand * 2 % 3);
        }
    }
}

