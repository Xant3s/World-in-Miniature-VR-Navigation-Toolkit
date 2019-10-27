using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace WIM_Plugin {
    [CustomEditor(typeof(Scaling))]
    public class ScalingEditor : Editor {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 3);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
        }

        private void Draw(WIMConfiguration config) {
            MiniatureModelEditor.Separator("Scaling");
            config.AllowWIMScaling = EditorGUILayout.Toggle("Allow WIM Scaling", config.AllowWIMScaling);
            if(!config.AllowWIMScaling) return;
            config.MinScaleFactor = EditorGUILayout.FloatField("Min Scale Factor", config.MinScaleFactor);
            config.MaxScaleFactor = EditorGUILayout.FloatField("Max Scale Factor", config.MaxScaleFactor);
            config.GrabButtonL = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button L", config.GrabButtonL);
            config.GrabButtonR = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button R", config.GrabButtonR);
            config.ScaleStep = EditorGUILayout.FloatField("Scale Step", config.ScaleStep);
            config.InterHandDistanceDeltaThreshold = EditorGUILayout.FloatField(
                new GUIContent("Inter Hand Distance Delta Threshold",
                    "Ignore inter hand distance deltas below this threshold for scaling."),
                config.InterHandDistanceDeltaThreshold);
        }
    }
}