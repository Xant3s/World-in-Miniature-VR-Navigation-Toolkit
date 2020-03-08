﻿using System;
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
            if(!WIM.Configuration) {
                EditorGUILayout.HelpBox("WIM configuration missing. Create a WIM configuration asset and add it to the Miniature Model script.", MessageType.Error);
                return;
            }
            if(GUILayout.Button("Generate WIM")) WIMGenerator.GenerateNewWIM(WIM);

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
            InvokeCallbacks("Basic");

            var destinationSelectionTouchAvailable = WIM.GetComponent<DestinationSelectionTouch>() != null;
            EditorGUI.BeginDisabledGroup(!destinationSelectionTouchAvailable);
            WIM.Configuration.DestinationSelectionMethod =
                (DestinationSelection)EditorGUILayout.EnumPopup("Destination Selection Method",
                    WIM.Configuration.DestinationSelectionMethod);
            EditorGUI.EndDisabledGroup();
            if (!destinationSelectionTouchAvailable) {
                WIM.Configuration.DestinationSelectionMethod = DestinationSelection.Pickup;
                EditorGUILayout.HelpBox("Add 'DestinationSelectionTouch' script to change destination selection method", MessageType.Info);
            }
            if (WIM.Configuration.DestinationSelectionMethod == DestinationSelection.Pickup) {
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

            var autoDetectArmLengthAvailable = WIM.GetComponent<DetectArmLength>() != null;
            EditorGUI.BeginDisabledGroup(!autoDetectArmLengthAvailable);
            WIM.Configuration.AutoDetectArmLength = EditorGUILayout.Toggle(
                new GUIContent("Auto Detect Arm Length",
                    "At the start of the application, player has to extend the arm and press the confirm teleport button." +
                    "The detected arm length will be used instead of the spawn distance."),
                WIM.Configuration.AutoDetectArmLength);
            if(!autoDetectArmLengthAvailable) {
                WIM.Configuration.AutoDetectArmLength = false;
                EditorGUILayout.HelpBox("Add 'DetectArmLength' script to enable this feature.'", MessageType.Info);
            }
            EditorGUI.EndDisabledGroup();



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