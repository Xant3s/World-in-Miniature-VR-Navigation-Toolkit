﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using WIMVR.Editor.Util;
using WIMVR.Features.Scrolling;

namespace WIMVR.Editor.Features {
    /// <summary>
    /// Custom inspector.
    /// </summary>
    [CustomEditor(typeof(Scrolling))]
    public class ScrollingEditor : UnityEditor.Editor {
        private MiniatureModel WIM;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var scrolling = (Scrolling) target;
            EditorGUI.BeginChangeCheck();
            scrolling.ScrollingConfig = (ScrollingConfiguration) EditorGUILayout.ObjectField("Config", scrolling.ScrollingConfig, typeof(ScrollingConfiguration), false);
            if(EditorGUI.EndChangeCheck()) {
                WIMGenerator.ConfigureWIM(WIM);
                //scrolling.Remove();
                if(scrolling.ScrollingConfig) {
                    //scrolling.Setup();
                }
            }
            if(!scrolling.ScrollingConfig)
                EditorGUILayout.HelpBox("Scrolling configuration missing. " +
                                        "Create a scrolling configuration asset and add it to the scrolling component, " +
                                        "or re-add the provided default configuration. " +
                                        "To create a new configuration asset, " +
                                        "click 'Assets -> Create -> WIM -> Feature Configuration -> Scrolling'.", MessageType.Error);
        }

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 50);
            WIM = ((Scrolling) target).GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
        }

        private void Draw(WIMConfiguration WIMConfig, VisualElement container) {
            var visualTree = AssetUtils.LoadAtRelativePath<VisualTreeAsset>("ScrollingEditor.uxml", this);
            var root = new VisualElement();
            if(visualTree) visualTree.CloneTree(root);
            var scrolling = (Scrolling) target;
            ref var config = ref scrolling.ScrollingConfig;

            WIMEditorUtility.DisplaySettingsIfConfigNotNull(root, config, typeof(ScrollingConfiguration));

            var scrollingSettings2 = root.Q<VisualElement>("scrolling-settings2");
            var allowVerticalScroll = root.Q<Toggle>("allow-vertical-scroll");

            root.Q<ObjectField>("configuration").RegisterValueChangedCallback(e =>
                allowVerticalScroll.SetDisplay(e.newValue && !((ScrollingConfiguration)e.newValue).AllowVerticalScrolling));

            scrollingSettings2.SetDisplay(config && config.AllowWIMScrolling);
            root.Q<Toggle>("allow-scrolling").RegisterValueChangedCallback(e => {
                scrollingSettings2.SetDisplay(e.newValue);
                root.schedule.Execute(() => WIMGenerator.ConfigureWIM(WIM));
            });

            //root.Q<Vector3Field>("active-area-bounds").RegisterValueChangedCallback(e => scrolling.UpdateScrollingMask(WIM));

            var scrollSpeed = root.Q<FloatField>("scroll-speed");
            scrollSpeed.SetDisplay(config && config.AutoScroll);
            allowVerticalScroll.SetDisplay(config && config.AllowVerticalScrolling);
            root.Q<Toggle>("auto-scroll").RegisterValueChangedCallback(e => {
                scrollSpeed.SetDisplay(e.newValue);
                allowVerticalScroll.SetDisplay(!e.newValue);
                if(!e.newValue && scrolling.ScrollingConfig) scrolling.ScrollingConfig.AllowVerticalScrolling = false;
            });

            container.Add(root);
            if(config) root.Bind(new SerializedObject(config));
            root.Bind(new SerializedObject(scrolling));
        }
    }
}