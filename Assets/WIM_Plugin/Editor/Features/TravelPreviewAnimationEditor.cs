﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    [CustomEditor(typeof(TravelPreviewAnimation))]
    public class TravelPreviewAnimationEditor : Editor {
        private void OnEnable() {
            //MiniatureModelEditor.OnDraw.AddCallback(Draw, 0);
            //MiniatureModelEditor.OnDraw.AddCallback(draw, 0);
        }

        private void OnDisable() {
            //MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
            //MiniatureModelEditor.OnDraw.RemoveCallback(draw);
        }

        private void Draw(WIMConfiguration WIMConfig) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            ref var config = ref ((TravelPreviewAnimation)target).Config;
            if(!config) {
                EditorGUILayout.HelpBox("Travel preview animation configuration missing. Create a travel preview animation configuration asset and add it to the TravelPreviewAnimation script.", MessageType.Error);
                config = (TravelPreviewConfiguration) EditorGUILayout.ObjectField("Configuration", ((TravelPreviewAnimation)target).Config, typeof(TravelPreviewConfiguration), false);
                return;
            }
            config.TravelPreviewAnimation = EditorGUILayout.Toggle("Travel Preview Animation", config.TravelPreviewAnimation);
            if(config.TravelPreviewAnimation) {
                config.TravelPreviewAnimationSpeed = EditorGUILayout.Slider("Travel Preview Animation Speed",
                    config.TravelPreviewAnimationSpeed, 0, 1);
            }
            EditorUtility.SetDirty(config);
        }

        private void draw(WIMConfiguration WIMConfig, VisualElement container) {
            var root = new VisualElement();

            root.Add(new IMGUIContainer(() => {
                Draw(WIMConfig);
            }));





            container.Add(root);
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((TravelPreviewAnimation)target).Config;
            if(config) return;
            EditorGUILayout.HelpBox("Travel preview animation configuration missing. Create a travel preview animation configuration asset and add it to the TravelPreviewAnimation script.", MessageType.Error);
        }
    }
}