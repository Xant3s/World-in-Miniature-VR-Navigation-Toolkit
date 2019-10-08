using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using WIM_Plugin;

// The custom inspector. Displays only relevant settings. Should not contain logic.
[CustomEditor(typeof(MiniatureModel))]
public class MiniatureModelEditor : Editor {
    private MiniatureModel WIM;
    private GUIStyle headerStyle;

    public override void OnInspectorGUI() {
        WIM = (MiniatureModel) target;
        headerStyle = new GUIStyle(GUI.skin.label) {
            fontStyle = FontStyle.Bold
        };

        GUILayout.Label("World-in-Miniature (WIM)"); 
        WIM.Configuration = (WIMConfiguration)
            EditorGUILayout.ObjectField("Configuration", WIM.Configuration, typeof(WIMConfiguration), false);
        if (GUILayout.Button("Generate WIM")) WIM.Generator.GenerateNewWIM(WIM);
        EditorGUI.BeginChangeCheck();
        WIM.Configuration.AutoGenerateWIM = EditorGUILayout.Toggle("Auto Generate WIM", WIM.Configuration.AutoGenerateWIM);
        if (EditorGUI.EndChangeCheck()) WIM.Generator.UpdateAutoGenerateWIM(WIM);

        separator("Basic Settings");
        WIM.Configuration.PlayerRepresentation = (GameObject) EditorGUILayout.ObjectField("Player Representation",
            WIM.Configuration.PlayerRepresentation, typeof(GameObject), false);
        WIM.Configuration.DestinationIndicator = (GameObject) EditorGUILayout.ObjectField("Destination Indicator",
            WIM.Configuration.DestinationIndicator, typeof(GameObject), false);
        WIM.Configuration.ScaleFactor = EditorGUILayout.Slider("Scale Factor", WIM.Configuration.ScaleFactor, 0, 1);
        WIM.Configuration.WIMLevelOffset =
            EditorGUILayout.Vector3Field("WIM Level Offset", WIM.Configuration.WIMLevelOffset);
        WIM.Configuration.ExpandCollidersX =
            NamedVectorField("Expand Colliders X", WIM.Configuration.ExpandCollidersX, "Left", "Right");
        WIM.Configuration.ExpandCollidersY =
            NamedVectorField("Expand Colliders Y", WIM.Configuration.ExpandCollidersY, "Up", "Down");
        WIM.Configuration.ExpandCollidersZ =
            NamedVectorField("Expand Colliders Z", WIM.Configuration.ExpandCollidersZ, "Front", "Back");
        WIM.Configuration.DestinationAlwaysOnTheGround = EditorGUILayout.Toggle(
            new GUIContent("Destination Always on the Ground",
                "If active, the destination will automatically set to ground level. This protects the player from being teleported to a location in mid-air."),
            WIM.Configuration.DestinationAlwaysOnTheGround);


        separator("Input");
        WIM.Configuration.ShowWIMButton = (OVRInput.RawButton) EditorGUILayout.EnumPopup("Show WIM Button", WIM.Configuration.ShowWIMButton);
        WIM.Configuration.DestinationSelectionMethod =
            (DestinationSelection) EditorGUILayout.EnumPopup("Destination Selection Method",
                WIM.Configuration.DestinationSelectionMethod);
        if (WIM.Configuration.DestinationSelectionMethod == DestinationSelection.Selection) {
            WIM.Configuration.DestinationSelectionButton = (OVRInput.RawButton)EditorGUILayout.EnumPopup("Destination Selection Button", WIM.Configuration.DestinationSelectionButton);
            WIM.Configuration.DestinationRotationThumbstick = (OVRInput.RawAxis2D)EditorGUILayout.EnumPopup("Destination Rotation Thumbstick", WIM.Configuration.DestinationRotationThumbstick);
            WIM.Configuration.ConfirmTravelButton = (OVRInput.RawButton)EditorGUILayout.EnumPopup("Confirm Travel Button", WIM.Configuration.ConfirmTravelButton);
        }
        else if (WIM.Configuration.DestinationSelectionMethod == DestinationSelection.Pickup) {
            WIM.Configuration.DoubleTapInterval =
                EditorGUILayout.FloatField("Double Tap Interval", WIM.Configuration.DoubleTapInterval);
        }


        separator("Occlusion Handling");
        WIM.Configuration.SemiTransparent =
            EditorGUILayout.Toggle("Semi-Transparent", WIM.Configuration.SemiTransparent);
        if (WIM.Configuration.SemiTransparent) {
            WIM.Configuration.Transparency =
                EditorGUILayout.Slider("Transparency", WIM.Configuration.Transparency, 0, 1);
        }
        WIM.Configuration.OcclusionHandlingMethod = (OcclusionHandling) EditorGUILayout.EnumPopup(
            new GUIContent("Occlusion Handling Method", "Select occlusion handling strategy. Disable for scrolling."),
            WIM.Configuration.OcclusionHandlingMethod);
        if (WIM.Configuration.OcclusionHandlingMethod == OcclusionHandling.MeltWalls) {
            WIM.Configuration.MeltRadius = EditorGUILayout.FloatField("Melt Radius", WIM.Configuration.MeltRadius);
            WIM.Configuration.MeltHeight = EditorGUILayout.FloatField("Melt Height", WIM.Configuration.MeltHeight);
        }else if (WIM.Configuration.OcclusionHandlingMethod == OcclusionHandling.CutoutView) {
            WIM.Configuration.CutoutRange = EditorGUILayout.FloatField("Cutout Range", WIM.Configuration.CutoutRange);
            WIM.Configuration.CutoutAngle = EditorGUILayout.FloatField("Cutout Angle", WIM.Configuration.CutoutAngle);
            WIM.Configuration.ShowCutoutLight =
                EditorGUILayout.Toggle("Show Cutout Light", WIM.Configuration.ShowCutoutLight);
            if (WIM.Configuration.ShowCutoutLight) {
                WIM.Configuration.CutoutLightColor =
                    EditorGUILayout.ColorField("Cutout Light Color", WIM.Configuration.CutoutLightColor);
            }
        }


        separator("Orientation Aids");
        WIM.Configuration.PreviewScreen =
            EditorGUILayout.Toggle("Show Preview Screen", WIM.Configuration.PreviewScreen);
        if (WIM.Configuration.PreviewScreen) {
            WIM.Configuration.AutoPositionPreviewScreen = EditorGUILayout.Toggle("Auto Position Preview Screen",
                WIM.Configuration.AutoPositionPreviewScreen);
        }
        WIM.Configuration.TravelPreviewAnimation =
            EditorGUILayout.Toggle("Travel Preview Animation", WIM.Configuration.TravelPreviewAnimation);
        if (WIM.Configuration.TravelPreviewAnimation) {
            WIM.Configuration.TravelPreviewAnimationSpeed = EditorGUILayout.Slider("Travel Preview Animation Speed",
                WIM.Configuration.TravelPreviewAnimationSpeed, 0, 1);
        }
        WIM.Configuration.PostTravelPathTrace =
            EditorGUILayout.Toggle("Post Travel Path Trace", WIM.Configuration.PostTravelPathTrace);
        if (WIM.Configuration.PostTravelPathTrace) {
            WIM.Configuration.TraceDuration =
                EditorGUILayout.FloatField("Trace Duration", WIM.Configuration.TraceDuration);
        }


        separator("Usability");
        WIM.Configuration.WIMSpawnOffset =
            EditorGUILayout.Vector3Field("WIM Spawn Offset", WIM.Configuration.WIMSpawnOffset);
        WIM.Configuration.WIMSpawnHeight =
            EditorGUILayout.FloatField("WIM Spawn at Height", WIM.Configuration.WIMSpawnHeight);
        WIM.Configuration.PlayerHeightInCM =
            EditorGUILayout.FloatField("Player Height (in cm)", WIM.Configuration.PlayerHeightInCM);
        WIM.Configuration.PlayerArmLength =
            EditorGUILayout.FloatField("Player Arm Length", WIM.Configuration.PlayerArmLength);
        WIM.Configuration.AutoDetectArmLength = EditorGUILayout.Toggle(
            new GUIContent("Auto Detect Arm Length",
                "At the start of the application, player has to extend the arm and press the confirm teleport button."),
            WIM.Configuration.AutoDetectArmLength);
        if (WIM.Configuration.AutoDetectArmLength) {
            WIM.Configuration.ConfirmArmLengthButton =
                (OVRInput.RawButton) EditorGUILayout.EnumPopup("Confirm Arm Lenght Button",
                    WIM.Configuration.ConfirmArmLengthButton);
        }

        WIM.Configuration.AdaptWIMSizeToPlayerHeight = EditorGUILayout.Toggle("Adapt WIM Size to Player Height",
            WIM.Configuration.AdaptWIMSizeToPlayerHeight);


        separator("Allow Scaling");
        WIM.Configuration.AllowWIMScaling =
            EditorGUILayout.Toggle("Allow WIM Scaling", WIM.Configuration.AllowWIMScaling);
        if (WIM.Configuration.AllowWIMScaling) {
            WIM.Configuration.MinScaleFactor =
                EditorGUILayout.FloatField("Min Scale Factor", WIM.Configuration.MinScaleFactor);
            WIM.Configuration.MaxScaleFactor =
                EditorGUILayout.FloatField("Max Scale Factor", WIM.Configuration.MaxScaleFactor);
            WIM.Configuration.GrabButtonL =
                (OVRInput.RawButton)EditorGUILayout.EnumPopup("Grab Button L",
                    WIM.Configuration.GrabButtonL);
            WIM.Configuration.GrabButtonR =
                (OVRInput.RawButton)EditorGUILayout.EnumPopup("Grab Button R",
                    WIM.Configuration.GrabButtonR);
            WIM.Configuration.ScaleStep = EditorGUILayout.FloatField("Scale Step", WIM.Configuration.ScaleStep);
            WIM.Configuration.InterHandDistanceDeltaThreshold = EditorGUILayout.FloatField(
                new GUIContent("Inter Hand Distance Delta Threshold",
                    "Ignore inter hand distance deltas below this threshold for scaling."),
                    WIM.Configuration.InterHandDistanceDeltaThreshold);
        }


        separator("Allow Scrolling");
        if (WIM.Configuration.OcclusionHandlingMethod != OcclusionHandling.None) {
            EditorGUILayout.LabelField("Disable occlusion handling method to use scrolling.");
        }
        else {
            WIM.Configuration.AllowWIMScrolling =
                EditorGUILayout.Toggle("Allow WIM Scrolling", WIM.Configuration.AllowWIMScrolling);
            if (WIM.Configuration.AllowWIMScrolling) {
                WIM.Configuration.ActiveAreaBounds =
                    EditorGUILayout.Vector3Field("Active Area Bounds", WIM.Configuration.ActiveAreaBounds);
                WIM.Configuration.AutoScroll = EditorGUILayout.Toggle("Auto Scroll", WIM.Configuration.AutoScroll);
                if (WIM.Configuration.AutoScroll) {
                    WIM.Configuration.ScrollSpeed = EditorGUILayout.FloatField("Scroll Speed", WIM.Configuration.ScrollSpeed);
                }
            }
        }



        if (Application.isPlaying) return;
        if (WIM.Generator.ScrollingPropertyChanged(WIM) || WIM.Generator.OcclusionHandlingStrategyChanged(WIM))
            WIM.Generator.ConfigureWIM(WIM);
        WIM.Generator.UpdateCylinderMask(WIM);
        WIM.Generator.UpdateCutoutViewMask(WIM);
        WIM.Generator.UpdateScrollingMask(WIM);
        WIM.Generator.UpdateTransparency(WIM);
        EditorUtility.SetDirty(WIM.Configuration);
    }

    private Vector2 NamedVectorField(string text, Vector2 vector, string xAxisName = "X", string yAxisName = "Y") {
        var namedFloatFieldStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleRight};
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(text);
        var tmpVec = vector;
        GUILayout.Label(xAxisName, namedFloatFieldStyle);
        tmpVec.x = EditorGUILayout.FloatField(tmpVec.x);
        GUILayout.Label(yAxisName, namedFloatFieldStyle);
        tmpVec.y = EditorGUILayout.FloatField(tmpVec.y);
        EditorGUILayout.EndHorizontal();
        return tmpVec;
    }

    private void separator(string text = "", ushort space = 20) {
        GUILayout.Space(space);
        GUILayout.Label(text, headerStyle);
    }
}