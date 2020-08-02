// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine.Events;

namespace WIMVR.Util {
    public enum OcclusionHandlingMethod { None, MeltWalls, CutoutView }
    public enum Hand { None, LeftHand, RightHand }
    public enum DestinationSelection { Pickup, Touch }

    public class HandEvent : UnityEvent<Hand>{}
}

