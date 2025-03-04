﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using WIMVR.Editor.Util;
using WIMVR.Features.Occlusion_Handling;
using WIMVR.Util;


namespace WIMVR.Editor.Features {
    /// <summary>
    /// Custom inspector.
    /// </summary>
    [CustomEditor(typeof(OcclusionHandling))]
    public class OcclusionHandlingEditor : UnityEditor.Editor {
        private MiniatureModel WIM;

        public override void OnInspectorGUI() {
            if(Application.isPlaying) return;
            var occlusionHandling = (OcclusionHandling) target;
            occlusionHandling.UpdateCutoutViewMask(WIM);
            occlusionHandling.UpdateCylinderMask(WIM);
            DrawDefaultInspector();
            EditorGUI.BeginChangeCheck();
            occlusionHandling.Config = (OcclusionHandlingConfiguration) EditorGUILayout.ObjectField("Config",
                occlusionHandling.Config, typeof(OcclusionHandlingConfiguration), false);
            if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
            ref var config = ref ((OcclusionHandling) target).Config;
            if(!config)
                EditorGUILayout.HelpBox(
                    "Occlusion handling configuration missing. " +
                    "Create an occlusion handling configuration asset and add it to the OcclusionHandling component, " +
                    "or re-add the provided default configuration. To create a new configuration asset, " +
                    "click 'Assets -> Create -> WIM -> Feature Configuration -> Occlusion Handling'.",
                    MessageType.Error);
        }

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 0, "Occlusion Handling");
            WIM = ((OcclusionHandling) target).GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw, "Occlusion Handling");
        }

        private void Draw(WIMConfiguration WIMConfig, VisualElement container) {
            if(!target) return;
            var occlusionHandling = (OcclusionHandling) target;
            ref var config = ref occlusionHandling.Config;
            var visualTree = AssetUtils.LoadAtRelativePath<VisualTreeAsset>("OcclusionHandlingEditor.uxml", this);
            var root = new VisualElement();
            if(visualTree) visualTree.CloneTree(root);

            var configField = root.Q<ObjectField>("configuration");
            configField.objectType = typeof(OcclusionHandlingConfiguration);
            configField.RegisterValueChangedCallback(e => WIMGenerator.ConfigureWIM(WIM));

            root.Q<HelpBox>("config-error").SetDisplay(!config);
            configField.SetDisplay(!config);
            var occlusionHandlingMethod = root.Q<EnumField>("occlusion-handling-method");
            occlusionHandlingMethod.SetDisplay(config);
            root.Q<VisualElement>("melt-settings")
                .SetDisplay(config && config.OcclusionHandlingMethod == OcclusionHandlingMethod.MeltWalls);
            root.Q<VisualElement>("cutout-view-settings")
                .SetDisplay(config && config.OcclusionHandlingMethod == OcclusionHandlingMethod.CutoutView);

            if(config) {
                occlusionHandlingMethod.RegisterValueChangedCallback(e
                    => root.schedule.Execute(() => WIMGenerator.ConfigureWIM(WIM)));
                var cutoutLightColor = root.Q<ColorField>("cutout-light-color");
                cutoutLightColor.SetDisplay(config.ShowCutoutLight);
                root.Q<Toggle>("show-cutout-light")
                    .RegisterValueChangedCallback(e => cutoutLightColor.SetDisplay(e.newValue));
                root.Bind(new SerializedObject(config));
            }

            container.Add(root);
            root.Bind(new SerializedObject(occlusionHandling));
        }
    }
}