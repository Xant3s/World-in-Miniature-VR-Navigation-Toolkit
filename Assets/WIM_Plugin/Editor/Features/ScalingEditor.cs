using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    [CustomEditor(typeof(Scaling))]
    public class ScalingEditor : Editor {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(draw);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(draw);
        }

        private void draw(WIMConfiguration WIMconfig, VisualElement container) {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Editor/Features/ScalingEditor.uxml");
            if(!visualTree) return;
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIM_Plugin/Editor/Core/MiniatureModelEditor.uss");
            var root = new VisualElement();
            root.styleSheets.Add(styleSheet);
            if(visualTree) visualTree.CloneTree(root);
            var scaling = (Scaling) target;
            ref var config = ref scaling.ScalingConfig;

            root.Q<HelpBox>("config-info").SetDisplay(!config);
            var scalingConfig = root.Q<ObjectField>("configuration");
            var scalingSettings = root.Q<VisualElement>("scaling-settings");
            scalingSettings.SetDisplay(config);
            scalingConfig.SetDisplay(!config);
            scalingConfig.objectType = typeof(ScalingConfiguration);
            scalingConfig.RegisterValueChangedCallback(e => {
                root.Q<HelpBox>("config-info").SetDisplay(!e.newValue);
                scalingConfig.SetDisplay(!e.newValue);
                scalingSettings.SetDisplay(e.newValue);
                if(e.newValue) root.Bind(new SerializedObject(e.newValue));
            });

            var scalingSettings2 = root.Q<VisualElement>("scaling-settings2");
            scalingSettings2.SetDisplay(config && config.AllowWIMScaling);
            root.Q<Toggle>("allow-scaling").RegisterValueChangedCallback(e => scalingSettings2.SetDisplay(e.newValue));

            container.Add(root);
            if(config) root.Bind(new SerializedObject(config));
            root.Bind(new SerializedObject(scaling));
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((Scaling) target).ScalingConfig;
            if(config) return;
            EditorGUILayout.HelpBox("Scaling configuration missing. Create a scaling configuration asset and add it to the scaling script.", MessageType.Error);
        }
    }
}