﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using WIMVR.Editor.Util;
using WIMVR.Features.Travel_Preview_Animation;


namespace WIMVR.Editor.Features {
    /// <summary>
    /// Custom inspector.
    /// </summary>
    [CustomEditor(typeof(TravelPreviewAnimation))]
    public class TravelPreviewAnimationEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((TravelPreviewAnimation)target).Config;
            if(config) return;
            EditorGUILayout.HelpBox("Travel preview animation configuration missing. " +
                                    "Create a travel preview animation configuration asset and add it to the TravelPreviewAnimation component, " +
                                    "or re-add the provided default configuration. " +
                                    "To create a new configuration asset, " +
                                    "click 'Assets -> Create -> WIM -> Feature Configuration -> Travel Preview Animation'.", MessageType.Error);
        }

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 10);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
            MiniatureModelEditor.Separators?.UnregisterUnique("Orientation Aids");
        }

        private void Draw(WIMConfiguration WIMConfig, VisualElement container) {
            MiniatureModelEditor.Separators.RegisterUnique("Orientation Aids");
            var visualTree = AssetUtils.LoadAtRelativePath<VisualTreeAsset>("TravelPreviewAnimationEditor.uxml", this);
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
    }
}