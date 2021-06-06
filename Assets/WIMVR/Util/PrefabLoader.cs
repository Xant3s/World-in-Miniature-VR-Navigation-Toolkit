// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Util {
    public static class PrefabLoader {
        public static GameObject LeftOculusHandPrefab => Resources.Load<GameObject>("CustomHandLeftDeviceBased");
        public static GameObject RightOculusHandPrefab => Resources.Load<GameObject>("CustomHandRightDeviceBased");
        public static GameObject LeftIndexFingerTipPrefab => Resources.Load<GameObject>("Fingers/Left Index Finger Tip");
        public static GameObject LeftThumbFingerTipPrefab => Resources.Load<GameObject>("Fingers/Left Thumb Tip");
        public static GameObject RightIndexFingerTipPrefab => Resources.Load<GameObject>("Fingers/Right Index Finger Tip");
        public static GameObject RightThumbFingerTipPrefab => Resources.Load<GameObject>("Fingers/Right Thumb Tip");
    }
}