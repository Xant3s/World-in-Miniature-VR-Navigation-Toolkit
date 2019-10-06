using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    // The WIM configuration. Data only. Modified via GUI.
    // TODO: Replace with scriptable object.
    public interface WIMConfiguration {
        bool AutoGenerateWIM { get; set; }
        GameObject PlayerRepresentation { get; set; }
        GameObject DestinationIndicator { get; set; }
        float ScaleFactor { get; set; }
        Vector3 WIMLevelOffset { get; set; }
        Vector2 ExpandCollidersX { get; set; }
        Vector2 ExpandCollidersY { get; set; }
        Vector2 ExpandCollidersZ { get; set; }
        bool DestinationAlwaysOnTheGround { get; set; }
        OVRInput.RawButton ShowWIMButton { get; set; }
        DestinationSelection DestinationSelectionMethod { get; set; }
        OVRInput.RawButton DestinationSelectionButton { get; set; }
        OVRInput.RawAxis2D DestinationRotationThumbstick { get; set; }
        OVRInput.RawButton ConfirmTravelButton { get; set; }
        float DoubleTapInterval { get; set; }
        bool SemiTransparent { get; set; }
        float Transparency { get; set; }
        OcclusionHandling OcclusionHandlingMethod { get; set; }
        float MeltRadius { get; set; }
        float MeltHeihgt { get; set; }
        float CutoutRange { get; set; }
        float CutoutAngle { get; set; }
        bool ShowCutoutLight { get; set; }
        Color CutoutLightColor { get; set; }
        bool PreviewScreen { get; set; }
        bool AutoPositionPreviewScreen { get; set; }
        bool TravelPreviewAnimaition { get; set; }
        float TravelPreviewAnimationSpeed { get; set; }
        bool PostTravelPathTrace { get; set; }
        float TraceDuration { get; set; }
        Vector3 WIMSpawnOffset { get; set; }
        float WIMSpawnHeight { get; set; }
        float PlayerHeightInCM { get; set; }
        float PlayerArmLength { get; set; }
        bool AutoDetectArmLength { get; set; }
        OVRInput.RawButton ConfirmArmLengthButton { get; set; }
        bool AdaptWIMSizeToPlayerHeight { get; set; }
        bool AllowWIMScaling { get; set; }
        float MinScaleFactor { get; set; }
        float MaxScaleFactor { get; set; }
        OVRInput.RawButton GrabButttonL { get; set; }
        OVRInput.RawButton GrabButttonR { get; set; }
        float ScaleStep { get; set; }
        float InterHandDistanceDeltaThreshold { get; set; }
        bool AllowWIMScrolling { get; set; }
        Vector3 ActiveAreaBounds { get; set; }
        bool AutoScroll { get; set; }
        float ScrollSpeed { get; set; }
        float MaxWIMScaleFactorDelta { get; set; }  // The maximum value scale factor can be changed by (positive or negative) when adapting to player height.

    }

    public class WIMConfigurationImpl : WIMConfiguration {
        public bool AutoGenerateWIM { get; set; }
        public GameObject PlayerRepresentation { get; set; }
        public GameObject DestinationIndicator { get; set; }
        public float ScaleFactor { get; set; } = 0.1f;
        public Vector3 WIMLevelOffset { get; set; }
        public Vector2 ExpandCollidersX { get; set; }
        public Vector2 ExpandCollidersY { get; set; }
        public Vector2 ExpandCollidersZ { get; set; }
        public bool DestinationAlwaysOnTheGround { get; set; } = true;
        public OVRInput.RawButton ShowWIMButton { get; set; } = OVRInput.RawButton.X;
        public DestinationSelection DestinationSelectionMethod { get; set; } = DestinationSelection.Pickup;
        public OVRInput.RawButton DestinationSelectionButton { get; set; } = OVRInput.RawButton.A;
        public OVRInput.RawAxis2D DestinationRotationThumbstick { get; set; } = OVRInput.RawAxis2D.RThumbstick;
        public OVRInput.RawButton ConfirmTravelButton { get; set; } = OVRInput.RawButton.B;
        public float DoubleTapInterval { get; set; } = 2;
        public bool SemiTransparent { get; set; } = true;
        public float Transparency { get; set; } = 0.33f;
        public OcclusionHandling OcclusionHandlingMethod { get; set; }
        public float MeltRadius { get; set; } = 1.0f;
        public float MeltHeihgt { get; set; } = 2.0f;
        public float CutoutRange { get; set; } = 10;
        public float CutoutAngle { get; set; } = 30;
        public bool ShowCutoutLight { get; set; }
        public Color CutoutLightColor { get; set; } = Color.white;
        public bool PreviewScreen { get; set; }
        public bool AutoPositionPreviewScreen { get; set; }
        public bool TravelPreviewAnimaition { get; set; }
        public float TravelPreviewAnimationSpeed { get; set; } = 1.0f;
        public bool PostTravelPathTrace { get; set; }
        public float TraceDuration { get; set; } = 1.0f;
        public Vector3 WIMSpawnOffset { get; set; }
        public float WIMSpawnHeight { get; set; } = 150;
        public float PlayerHeightInCM { get; set; } = 170;
        public float PlayerArmLength { get; set; }
        public bool AutoDetectArmLength { get; set; }
        public OVRInput.RawButton ConfirmArmLengthButton { get; set; } = OVRInput.RawButton.A;
        public bool AdaptWIMSizeToPlayerHeight { get; set; }
        public bool AllowWIMScaling { get; set; }
        public float MinScaleFactor { get; set; } = 0;
        public float MaxScaleFactor { get; set; } = .5f;
        public OVRInput.RawButton GrabButttonL { get; set; } = OVRInput.RawButton.LHandTrigger;
        public OVRInput.RawButton GrabButttonR { get; set; } = OVRInput.RawButton.RHandTrigger;
        public float ScaleStep { get; set; } = .0001f;
        public float InterHandDistanceDeltaThreshold { get; set; } = .1f;
        public bool AllowWIMScrolling { get; set; }
        public Vector3 ActiveAreaBounds { get; set; } = new Vector3(10,10,10);
        public bool AutoScroll { get; set; }
        public float ScrollSpeed { get; set; } = 1;
        public float MaxWIMScaleFactorDelta { get; set; }
    }
}