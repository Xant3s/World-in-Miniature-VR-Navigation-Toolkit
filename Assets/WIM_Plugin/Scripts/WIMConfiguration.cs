using System.Collections;
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
        public OVRInput.RawButton ShowWIMButton = OVRInput.RawButton.X;
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
        public OVRInput.RawAxis2D VerticalScrollingAxis = OVRInput.RawAxis2D.LThumbstick;
        public float MaxWIMScaleFactorDelta = 0.005f;   // The maximum value scale factor can be changed by (positive or negative) when adapting to player height.


        // Occlusion Handling: Melt Walls
        public OcclusionHandling OcclusionHandlingMethod;
        public float MeltRadius = 1.0f;
        public float MeltHeight = 2.0f;

        // Occlusion Handling: Cutout View
        public float CutoutRange = 10;
        public float CutoutAngle = 30;
        public bool ShowCutoutLight;
        public Color CutoutLightColor = Color.white;

        // Preview Screen
        public bool PreviewScreen;
        public bool AutoPositionPreviewScreen;

        // Travel Preview Animation
        public bool TravelPreviewAnimation;
        public float TravelPreviewAnimationSpeed = 1.0f;

        // Path Trace
        public bool PostTravelPathTrace;
        public float TraceDuration = 1.0f;

        // Scaling
        public bool AllowWIMScaling;
        public float MinScaleFactor;
        public float MaxScaleFactor = .5f;
        public float ScaleStep = .0001f;
        public float InterHandDistanceDeltaThreshold = .1f;

        // Scrolling
        public bool AllowWIMScrolling;
        public bool AutoScroll;
        public bool AllowVerticalScrolling = true;
        public float ScrollSpeed = 1;

    }
}