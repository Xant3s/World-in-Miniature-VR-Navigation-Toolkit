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

        private void Draw(WIMConfiguration config) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            var previewScreen = (PreviewScreen) target;
            if(!previewScreen.Config) {
                EditorGUILayout.HelpBox("Preview screen configuration missing. Create a preview screen configuration asset and add it to the PreviewScreen script.", MessageType.Error);
                previewScreen.Config = (PreviewScreenConfiguration) EditorGUILayout.ObjectField("Configuration", previewScreen.Config, typeof(PreviewScreenConfiguration), false);
                return;
            }
            previewScreen.Config.PreviewScreen = EditorGUILayout.Toggle("Show Preview Screen", previewScreen.Config.PreviewScreen);
            if(previewScreen.Config.PreviewScreen) {
                previewScreen.Config.AutoPositionPreviewScreen = EditorGUILayout.Toggle("Auto Position Preview Screen",
                    previewScreen.Config.AutoPositionPreviewScreen);
            }
        }
    }
}