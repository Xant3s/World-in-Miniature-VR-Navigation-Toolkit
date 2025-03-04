﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using WIMVR.Editor.Util;
using WIMVR.Features.Preview_Screen;

namespace WIMVR.Editor.Features {
    /// <summary>
    /// Custom inspector.
    /// </summary>
    [CustomEditor(typeof(PreviewScreen))]
    public class PreviewScreenEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((PreviewScreen) target).Config;
            if(config) return;
            EditorGUILayout.HelpBox("Preview screen configuration missing. " +
                                    "Create a preview screen configuration asset and add it to the PreviewScreen component, " +
                                    "or re-add the provided default configuration. " +
                                    "To create a new configuration asset, " +
                                    "click 'Assets -> Create -> WIM -> Feature Configuration -> Preview Screen'.", MessageType.Error);
        }

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 20);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
            MiniatureModelEditor.Separators?.UnregisterUnique("Orientation Aids");
        }

        private void Draw(WIMConfiguration WIMConfig, VisualElement container) {
            MiniatureModelEditor.Separators.RegisterUnique("Orientation Aids");
            var visualTree = AssetUtils.LoadAtRelativePath<VisualTreeAsset>("PreviewScreenEditor.uxml", this);
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
    }
}