using System;
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

        private void Draw(WIMConfiguration config) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            var pathTrace = (PathTrace) target;
            if(!pathTrace.PathTraceConfig) {
                EditorGUILayout.HelpBox("Path trace configuration missing. Create a path trace configuration asset and add it to the PathTrace script.", MessageType.Error);
                pathTrace.PathTraceConfig = (PathTraceConfiguration) EditorGUILayout.ObjectField("Configuration", ((PathTrace)target).PathTraceConfig, typeof(PathTraceConfiguration), false);
                return;
            }
            pathTrace.PathTraceConfig.PostTravelPathTrace = EditorGUILayout.Toggle("Post Travel Path Trace", pathTrace.PathTraceConfig.PostTravelPathTrace);
            if(pathTrace.PathTraceConfig.PostTravelPathTrace) {
                pathTrace.PathTraceConfig.TraceDuration = EditorGUILayout.FloatField("Trace Duration", pathTrace.PathTraceConfig.TraceDuration);
            }
        }
    }
}