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
            if(!previewScreen.PreviewScreenConfig) {
                EditorGUILayout.HelpBox("Preview screen configuration missing. Create a preview screen configuration asset and add it to the PreviewScreen script.", MessageType.Error);
                previewScreen.PreviewScreenConfig = (PreviewScreenConfiguration) EditorGUILayout.ObjectField("Configuration", previewScreen.PreviewScreenConfig, typeof(PreviewScreenConfiguration), false);
                return;
            }
            previewScreen.PreviewScreenConfig.PreviewScreen = EditorGUILayout.Toggle("Show Preview Screen", previewScreen.PreviewScreenConfig.PreviewScreen);
            if(previewScreen.PreviewScreenConfig.PreviewScreen) {
                previewScreen.PreviewScreenConfig.AutoPositionPreviewScreen = EditorGUILayout.Toggle("Auto Position Preview Screen",
                    previewScreen.PreviewScreenConfig.AutoPositionPreviewScreen);
            }
        }
    }
}