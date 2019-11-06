using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace WIM_Plugin {
// The custom inspector. Displays only relevant settings. Should not contain logic.
    [CustomEditor(typeof(MiniatureModel))]
    public class MiniatureModelEditor : Editor {
        public static DrawCallbackManager OnDraw = new DrawCallbackManager();

        private MiniatureModel WIM;
        private static GUIStyle headerStyle;

        private static IList<string> separators = new List<string>();


        public override void OnInspectorGUI() {
            WIM = (MiniatureModel) target;
            headerStyle = new GUIStyle(GUI.skin.label) {
                fontStyle = FontStyle.Bold
            };

            GUILayout.Label("World-in-Miniature (WIM)");
            WIM.Configuration = (WIMConfiguration)
                EditorGUILayout.ObjectField("Configuration", WIM.Configuration, typeof(WIMConfiguration), false);
            if(GUILayout.Button("Generate WIM")) WIMGenerator.GenerateNewWIM(WIM);
            EditorGUI.BeginChangeCheck();
            WIM.Configuration.AutoGenerateWIM =
                EditorGUILayout.Toggle("Auto Generate WIM", WIM.Configuration.AutoGenerateWIM);
            if(EditorGUI.EndChangeCheck()) WIMGenerator.UpdateAutoGenerateWIM(WIM);

            Separator("Basic Settings");
            WIM.Configuration.PlayerRepresentation = (GameObject) EditorGUILayout.ObjectField("Player Representation",
                WIM.Configuration.PlayerRepresentation, typeof(GameObject), false);
            WIM.Configuration.DestinationIndicator = (GameObject) EditorGUILayout.ObjectField("Destination Indicator",
                WIM.Configuration.DestinationIndicator, typeof(GameObject), false);
            WIM.Configuration.ScaleFactor = EditorGUILayout.Slider("Scale Factor", WIM.Configuration.ScaleFactor, 0, 1);
            WIM.Configuration.WIMLevelOffset =
                EditorGUILayout.Vector3Field("WIM Level Offset", WIM.Configuration.WIMLevelOffset);
            WIM.Configuration.ExpandCollidersX =
                WIMEditorUtility.NamedVectorField("Expand Colliders X", WIM.Configuration.ExpandCollidersX, "Left", "Right");
            WIM.Configuration.ExpandCollidersY =
                WIMEditorUtility.NamedVectorField("Expand Colliders Y", WIM.Configuration.ExpandCollidersY, "Up", "Down");
            WIM.Configuration.ExpandCollidersZ =
                WIMEditorUtility.NamedVectorField("Expand Colliders Z", WIM.Configuration.ExpandCollidersZ, "Front", "Back");
            WIM.Configuration.DestinationAlwaysOnTheGround = EditorGUILayout.Toggle(
                new GUIContent("Destination Always on the Ground",
                    "If active, the destination will automatically set to ground level. This protects the player from being teleported to a location in mid-air."),
                WIM.Configuration.DestinationAlwaysOnTheGround);


            Separator("Input");
            WIM.Configuration.ShowWIMButton =
                (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Show WIM Button", WIM.Configuration.ShowWIMButton);
            WIM.Configuration.DestinationSelectionMethod =
                (DestinationSelection) EditorGUILayout.EnumPopup("Destination Selection Method",
                    WIM.Configuration.DestinationSelectionMethod);
            if(WIM.Configuration.DestinationSelectionMethod == DestinationSelection.Selection) {
                WIM.Configuration.DestinationSelectionButton =
                    (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Destination Selection Button",
                        WIM.Configuration.DestinationSelectionButton);
                WIM.Configuration.DestinationRotationThumbstick =
                    (OVRInput.RawAxis2D) EditorGUILayout.EnumFlagsField("Destination Rotation Thumbstick",
                        WIM.Configuration.DestinationRotationThumbstick);
                WIM.Configuration.ConfirmTravelButton =
                    (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Confirm Travel Button",
                        WIM.Configuration.ConfirmTravelButton);
            }
            else if(WIM.Configuration.DestinationSelectionMethod == DestinationSelection.Pickup) {
                WIM.Configuration.DoubleTapInterval =
                    EditorGUILayout.FloatField("Double Tap Interval", WIM.Configuration.DoubleTapInterval);
            }
            InvokeCallbacks("Input");


            Separator("Occlusion Handling");
            EditorGUI.BeginChangeCheck();
            WIM.Configuration.SemiTransparent =
                EditorGUILayout.Toggle("Semi-Transparent", WIM.Configuration.SemiTransparent);
            if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
            if(WIM.Configuration.SemiTransparent) {
                EditorGUI.BeginChangeCheck();
                WIM.Configuration.Transparency =
                    EditorGUILayout.Slider("Transparency", WIM.Configuration.Transparency, 0, 1);
                if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
            }
            InvokeCallbacks("Occlusion Handling");


            Separator("Usability");
            WIM.Configuration.WIMSpawnHeight =
                EditorGUILayout.FloatField("WIM Spawn at Height", WIM.Configuration.WIMSpawnHeight);
            WIM.Configuration.PlayerHeightInCM =
                EditorGUILayout.FloatField("Player Height (in cm)", WIM.Configuration.PlayerHeightInCM);
            if(!WIM.Configuration.AutoDetectArmLength) {
                WIM.Configuration.SpawnDistance =
                    EditorGUILayout.FloatField("WIM Spawn Distance", WIM.Configuration.SpawnDistance);
            }

            WIM.Configuration.AutoDetectArmLength = EditorGUILayout.Toggle(
                new GUIContent("Auto Detect Arm Length",
                    "At the start of the application, player has to extend the arm and press the confirm teleport button." +
                    "The detected arm length will be used instead of the spawn distance."),
                WIM.Configuration.AutoDetectArmLength);
            if(WIM.Configuration.AutoDetectArmLength) {
                WIM.Configuration.ConfirmArmLengthButton =
                    (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Confirm Arm Length Button",
                        WIM.Configuration.ConfirmArmLengthButton);
            }

            WIM.Configuration.AdaptWIMSizeToPlayerHeight = EditorGUILayout.Toggle("Adapt WIM Size to Player Height",
                WIM.Configuration.AdaptWIMSizeToPlayerHeight);
            InvokeCallbacks("Usability");


            separators.Clear();
            EditorUtility.SetDirty(WIM.Configuration);
            InvokeCallbacks();
        }

        private void InvokeCallbacks(string key = "") {
            OnDraw.InvokeCallbacks(WIM, key);
        }

        public static void Separator(string text = "", ushort space = 20) {
            GUILayout.Space(space);
            GUILayout.Label(text, headerStyle);
        }

        public static void UniqueSeparator(string text = "", ushort space = 20) {
            if(separators.Contains(text)) return;
            Separator(text, space);
            separators.Add(text);
        }
    }
}