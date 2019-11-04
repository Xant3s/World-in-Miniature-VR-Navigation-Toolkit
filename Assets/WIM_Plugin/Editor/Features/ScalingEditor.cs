using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;


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
            var scalingConfig = ((Scaling)target).ScalingConfig;
            if(!scalingConfig) 
                EditorGUILayout.HelpBox("Scaling configuration missing. Create a scaling configuration asset and add it to the scaling script.", MessageType.Error);
            scalingConfig.AllowWIMScaling = EditorGUILayout.Toggle("Allow WIM Scaling", scalingConfig.AllowWIMScaling);
            if(!scalingConfig.AllowWIMScaling) return;
            scalingConfig.MinScaleFactor = EditorGUILayout.FloatField("Min Scale Factor", scalingConfig.MinScaleFactor);
            scalingConfig.MaxScaleFactor = EditorGUILayout.FloatField("Max Scale Factor", scalingConfig.MaxScaleFactor);
            config.GrabButtonL = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button L", config.GrabButtonL);
            config.GrabButtonR = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button R", config.GrabButtonR);
            scalingConfig.ScaleStep = EditorGUILayout.FloatField("Scale Step", scalingConfig.ScaleStep);
            scalingConfig.InterHandDistanceDeltaThreshold = EditorGUILayout.FloatField(
                new GUIContent("Inter Hand Distance Delta Threshold",
                    "Ignore inter hand distance deltas below this threshold for scaling."),
                scalingConfig.InterHandDistanceDeltaThreshold);
        }
    }
}