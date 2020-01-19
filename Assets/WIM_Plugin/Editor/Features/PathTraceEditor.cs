﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace WIM_Plugin {
    [CustomEditor(typeof(PathTrace))]
    public class PathTraceEditor : Editor {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 2);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
        }

        private void Draw(WIMConfiguration WIMConfig) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            if(!target) return;
            ref var config = ref ((PathTrace) target).PathTraceConfig;
            if(!config) {
                EditorGUILayout.HelpBox("Path trace configuration missing. Create a path trace configuration asset and add it to the PathTrace script.", MessageType.Error);
                config = (PathTraceConfiguration) EditorGUILayout.ObjectField("Configuration", ((PathTrace)target).PathTraceConfig, typeof(PathTraceConfiguration), false);
                return;
            }
            config.PostTravelPathTrace = EditorGUILayout.Toggle("Post Travel Path Trace", config.PostTravelPathTrace);
            if(config.PostTravelPathTrace) {
                config.TraceDuration = EditorGUILayout.FloatField("Trace Duration", config.TraceDuration);
            }
            EditorUtility.SetDirty(config);
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((PathTrace) target).PathTraceConfig;
            if(config) return;
            EditorGUILayout.HelpBox("Path trace configuration missing. Create a path trace configuration asset and add it to the PathTrace script.", MessageType.Error);
        }
    }
}