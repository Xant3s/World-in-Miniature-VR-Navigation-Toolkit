using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    [CustomEditor(typeof(PreviewScreen))]
    public class PreviewScreenEditor : Editor {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(draw, 1);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(draw);
            MiniatureModelEditor.UnregisterUniqueSeparator("Orientation Aids");
        }

        private void draw(WIMConfiguration WIMConfig, VisualElement container) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Editor/Features/PreviewScreenEditor.uxml");
            var root = new VisualElement();
            visualTree.CloneTree(root);
            var previewScreen = (PreviewScreen) target;
            ref var config = ref previewScreen.Config;

            WIMEditorUtility.DisplaySettingsIfConfigNotNull(root, config, typeof(PreviewScreenConfiguration));

            var autoPosition = root.Q<Toggle>("auto-position");
            autoPosition.SetDisplay(config && config.PreviewScreen);
            root.Q<Toggle>("enabled").RegisterValueChangedCallback(e => autoPosition.SetDisplay(e.newValue));

            container.Add(root);
            if(config) root.Bind(new SerializedObject(config));
            root.Bind(new SerializedObject(previewScreen));
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((PreviewScreen) target).Config;
            if(config) return;
            EditorGUILayout.HelpBox("Preview screen configuration missing. Create a preview screen configuration asset and add it to the PreviewScreen script.", MessageType.Error);
        }
    }
}