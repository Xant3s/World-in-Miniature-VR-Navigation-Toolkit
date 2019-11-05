using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace WIM_Plugin {
    [CustomEditor(typeof(TravelPreviewAnimation))]
    public class TravelPreviewAnimationEditor : Editor {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 0);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
        }

        private void Draw(WIMConfiguration config) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            var travelPreview = (TravelPreviewAnimation) target;
            if(!travelPreview.TravelPreviewConfig) {
                EditorGUILayout.HelpBox("Travel preview animation configuration missing. Create a travel preview animation configuration asset and add it to the TravelPreviewAnimation script.", MessageType.Error);
                travelPreview.TravelPreviewConfig = (TravelPreviewConfiguration) EditorGUILayout.ObjectField("Configuration", ((TravelPreviewAnimation)target).TravelPreviewConfig, typeof(TravelPreviewConfiguration), false);
                return;
            }
            travelPreview.TravelPreviewConfig.TravelPreviewAnimation = EditorGUILayout.Toggle("Travel Preview Animation", travelPreview.TravelPreviewConfig.TravelPreviewAnimation);
            if(travelPreview.TravelPreviewConfig.TravelPreviewAnimation) {
                travelPreview.TravelPreviewConfig.TravelPreviewAnimationSpeed = EditorGUILayout.Slider("Travel Preview Animation Speed",
                    travelPreview.TravelPreviewConfig.TravelPreviewAnimationSpeed, 0, 1);
            }
        }
    }
}