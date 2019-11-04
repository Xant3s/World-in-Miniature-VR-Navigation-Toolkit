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
            var s = (Scaling) target;
            if(!s.ScalingConfig) {
                EditorGUILayout.HelpBox("Scaling configuration missing. Create a scaling configuration asset and add it to the scaling script.", MessageType.Error);
                s.ScalingConfig = (ScalingConfiguration) EditorGUILayout.ObjectField("Configuration", ((Scaling)target).ScalingConfig, typeof(ScalingConfiguration), false);
                return;
            }
            s.ScalingConfig.AllowWIMScaling = EditorGUILayout.Toggle("Allow WIM Scaling", s.ScalingConfig.AllowWIMScaling);
            if(!s.ScalingConfig.AllowWIMScaling) return;
            s.ScalingConfig.MinScaleFactor = EditorGUILayout.FloatField("Min Scale Factor", s.ScalingConfig.MinScaleFactor);
            s.ScalingConfig.MaxScaleFactor = EditorGUILayout.FloatField("Max Scale Factor", s.ScalingConfig.MaxScaleFactor);
            config.GrabButtonL = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button L", config.GrabButtonL);
            config.GrabButtonR = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button R", config.GrabButtonR);
            s.ScalingConfig.ScaleStep = EditorGUILayout.FloatField("Scale Step", s.ScalingConfig.ScaleStep);
            s.ScalingConfig.InterHandDistanceDeltaThreshold = EditorGUILayout.FloatField(
                new GUIContent("Inter Hand Distance Delta Threshold",
                    "Ignore inter hand distance deltas below this threshold for scaling."),
                s.ScalingConfig.InterHandDistanceDeltaThreshold);
        }
    }
}