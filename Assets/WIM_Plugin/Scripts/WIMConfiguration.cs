﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    // The WIM configuration. Data only. Modified via GUI.
    [CreateAssetMenu(menuName = "WIM/Configuration")]
    public class WIMConfiguration : ScriptableObject {
        public bool AutoGenerateWIM;
        public GameObject PlayerRepresentation;
        public GameObject DestinationIndicator;
        public float ScaleFactor = 0.1f;
        public Vector3 WIMLevelOffset;
        public Vector2 ExpandCollidersX;
        public Vector2 ExpandCollidersY;
        public Vector2 ExpandCollidersZ;
        public bool DestinationAlwaysOnTheGround = true;
        public DestinationSelection DestinationSelectionMethod = DestinationSelection.Pickup;
        public OVRInput.RawButton DestinationSelectionButton = OVRInput.RawButton.A;
        public OVRInput.RawAxis2D DestinationRotationThumbstick = OVRInput.RawAxis2D.RThumbstick;
        public OVRInput.RawButton ConfirmTravelButton = OVRInput.RawButton.B;
        public float DoubleTapInterval = 2;
        public bool SemiTransparent = true;
        public float Transparency = 0.33f;
        public float WIMSpawnHeight = 150;
        public float PlayerHeightInCM = 170;
        public float SpawnDistance;
        public bool AutoDetectArmLength;
        public OVRInput.RawButton ConfirmArmLengthButton = OVRInput.RawButton.A;
        public bool AdaptWIMSizeToPlayerHeight;
        public OVRInput.RawButton GrabButtonL = OVRInput.RawButton.LHandTrigger;
        public OVRInput.RawButton GrabButtonR = OVRInput.RawButton.RHandTrigger;
        public Vector3 ActiveAreaBounds = new Vector3(10, 10, 10);
        public float MaxWIMScaleFactorDelta = 0.005f;   // The maximum value scale factor can be changed by (positive or negative) when adapting to player height.
    }
}