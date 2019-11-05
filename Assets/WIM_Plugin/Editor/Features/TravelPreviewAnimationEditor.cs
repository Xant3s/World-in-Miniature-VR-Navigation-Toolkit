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
            if(!travelPreview.Config) {
                EditorGUILayout.HelpBox("Travel preview animation configuration missing. Create a travel preview animation configuration asset and add it to the TravelPreviewAnimation script.", MessageType.Error);
                travelPreview.Config = (TravelPreviewConfiguration) EditorGUILayout.ObjectField("Configuration", ((TravelPreviewAnimation)target).Config, typeof(TravelPreviewConfiguration), false);
                return;
            }
            travelPreview.Config.TravelPreviewAnimation = EditorGUILayout.Toggle("Travel Preview Animation", travelPreview.Config.TravelPreviewAnimation);
            if(travelPreview.Config.TravelPreviewAnimation) {
                travelPreview.Config.TravelPreviewAnimationSpeed = EditorGUILayout.Slider("Travel Preview Animation Speed",
                    travelPreview.Config.TravelPreviewAnimationSpeed, 0, 1);
            }
        }
    }
}