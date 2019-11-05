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

        private void Draw(WIMConfiguration WIMConfig) {
            MiniatureModelEditor.Separator("Scaling");
            if(!target) return;
            ref var config = ref ((Scaling) target).ScalingConfig;
            if(!config) {
                EditorGUILayout.HelpBox("Scaling configuration missing. Create a scaling configuration asset and add it to the scaling script.", MessageType.Error);
                config = (ScalingConfiguration) EditorGUILayout.ObjectField("Configuration", config, typeof(ScalingConfiguration), false);
                return;
            }

            config.AllowWIMScaling = EditorGUILayout.Toggle("Allow WIM Scaling", config.AllowWIMScaling);
            if(!config.AllowWIMScaling) return;
            config.MinScaleFactor = EditorGUILayout.FloatField("Min Scale Factor", config.MinScaleFactor);
            config.MaxScaleFactor = EditorGUILayout.FloatField("Max Scale Factor", config.MaxScaleFactor);
            WIMConfig.GrabButtonL = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button L", WIMConfig.GrabButtonL);
            WIMConfig.GrabButtonR = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button R", WIMConfig.GrabButtonR);
            config.ScaleStep = EditorGUILayout.FloatField("Scale Step", config.ScaleStep);
            config.InterHandDistanceDeltaThreshold = EditorGUILayout.FloatField(
                new GUIContent("Inter Hand Distance Delta Threshold",
                    "Ignore inter hand distance deltas below this threshold for scaling."),
                config.InterHandDistanceDeltaThreshold);
        }
    }
}