﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    [CustomEditor(typeof(TravelPreviewAnimation))]
    public class TravelPreviewAnimationEditor : Editor {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(draw, 0);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(draw);
            MiniatureModelEditor.UnregisterUniqueSeparator("Orientation Aids");
        }

        private void draw(WIMConfiguration WIMConfig, VisualElement container) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Editor/Features/TravelPreviewAnimationEditor.uxml");
            var root = new VisualElement();
            visualTree.CloneTree(root);
            var travelPreviewAnimation = (TravelPreviewAnimation) target;
            ref var config = ref travelPreviewAnimation.Config;

            WIMEditorUtility.DisplaySettingsIfConfigNotNull(root, config, typeof(TravelPreviewConfiguration));

            var animationSpeed = root.Q<FloatSlider>("animation-speed");
            animationSpeed.SetDisplay(config && config.TravelPreviewAnimation);
            root.Q<Toggle>("enabled").RegisterValueChangedCallback(e => animationSpeed.SetDisplay(e.newValue));

            container.Add(root);
            if(config) root.Bind(new SerializedObject(config));
            root.Bind(new SerializedObject(travelPreviewAnimation));
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((TravelPreviewAnimation)target).Config;
            if(config) return;
            EditorGUILayout.HelpBox("Travel preview animation configuration missing. Create a travel preview animation configuration asset and add it to the TravelPreviewAnimation script.", MessageType.Error);
        }
    }
}