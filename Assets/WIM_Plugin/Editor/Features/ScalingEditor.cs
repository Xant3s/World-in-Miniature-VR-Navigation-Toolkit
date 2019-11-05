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
            var scaling = (Scaling) target;
            if(!scaling.ScalingConfig) {
                EditorGUILayout.HelpBox("Scaling configuration missing. Create a scaling configuration asset and add it to the scaling script.", MessageType.Error);
                scaling.ScalingConfig = (ScalingConfiguration) EditorGUILayout.ObjectField("Configuration", scaling.ScalingConfig, typeof(ScalingConfiguration), false);
                return;
            }

            ref var sconfig = ref scaling.ScalingConfig;
            sconfig.AllowWIMScaling = EditorGUILayout.Toggle("Allow WIM Scaling", sconfig.AllowWIMScaling);
            if(!scaling.ScalingConfig.AllowWIMScaling) return;
            scaling.ScalingConfig.MinScaleFactor = EditorGUILayout.FloatField("Min Scale Factor", scaling.ScalingConfig.MinScaleFactor);
            scaling.ScalingConfig.MaxScaleFactor = EditorGUILayout.FloatField("Max Scale Factor", scaling.ScalingConfig.MaxScaleFactor);
            config.GrabButtonL = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button L", config.GrabButtonL);
            config.GrabButtonR = (OVRInput.RawButton) EditorGUILayout.EnumFlagsField("Grab Button R", config.GrabButtonR);
            scaling.ScalingConfig.ScaleStep = EditorGUILayout.FloatField("Scale Step", scaling.ScalingConfig.ScaleStep);
            scaling.ScalingConfig.InterHandDistanceDeltaThreshold = EditorGUILayout.FloatField(
                new GUIContent("Inter Hand Distance Delta Threshold",
                    "Ignore inter hand distance deltas below this threshold for scaling."),
                scaling.ScalingConfig.InterHandDistanceDeltaThreshold);
        }
    }
}