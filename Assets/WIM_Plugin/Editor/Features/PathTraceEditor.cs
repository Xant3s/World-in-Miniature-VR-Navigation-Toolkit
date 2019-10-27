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
            config.PostTravelPathTrace = EditorGUILayout.Toggle("Post Travel Path Trace", config.PostTravelPathTrace);
            if(config.PostTravelPathTrace) {
                config.TraceDuration = EditorGUILayout.FloatField("Trace Duration", config.TraceDuration);
            }
        }
    }
}