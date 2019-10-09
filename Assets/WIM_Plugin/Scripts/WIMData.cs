using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    // Data describing the current WIM state. Data only. Modified at runtime.
    public class WIMData : ScriptableObject {
        public Transform WIMLevelTransform;
        public Transform PlayerRepresentationTransform;
        public Transform DestinationIndicatorInLevel;
        public Transform DestinationIndicatorInWIM;
        public Transform LevelTransform;
        public Transform PlayerTransform;
        public GameObject TravelPreviewAnimationObj;
        public bool PreviewScreenEnabled;
        public float WIMHeightRelativeToPlayer;
        public Vector3 WIMLevelLocalPosOnTravel;
        public Transform OVRPlayerController;
        public Vector3 WIMLevelLocalPos;
        public Transform HMDTransform;
        public Transform fingertipIndexR;
    }
}