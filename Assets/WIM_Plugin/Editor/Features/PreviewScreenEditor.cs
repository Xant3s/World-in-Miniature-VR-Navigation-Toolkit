using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace WIM_Plugin {
    [CustomEditor(typeof(PreviewScreen))]
    public class PreviewScreenEditor : Editor {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 1);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
        }

        private void Draw(WIMConfiguration WIMConfig) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            ref var config = ref ((PreviewScreen) target).Config;
            if(!config) {
                EditorGUILayout.HelpBox("Preview screen configuration missing. Create a preview screen configuration asset and add it to the PreviewScreen script.", MessageType.Error);
                config = (PreviewScreenConfiguration)EditorGUILayout.ObjectField("Configuration", config, typeof(PreviewScreenConfiguration), false);
                return;
            }
            config.PreviewScreen = EditorGUILayout.Toggle("Show Preview Screen", config.PreviewScreen);
            if(config.PreviewScreen) {
                config.AutoPositionPreviewScreen = EditorGUILayout.Toggle("Auto Position Preview Screen",
                    config.AutoPositionPreviewScreen);
            }
        }
    }
}