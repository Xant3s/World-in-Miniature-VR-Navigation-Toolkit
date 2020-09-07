﻿// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Features;
using WIMVR.Util;


namespace WIMVR.Editor.Core {
    /// <summary>
    /// Custom inspector. Displays only relevant settings.
    /// </summary>
    [CustomEditor(typeof(MiniatureModel))]
    public class MiniatureModelEditor : UnityEditor.Editor {
        public static DrawCallbackManager OnDraw = new DrawCallbackManager();
        private static GUIStyle headerStyle;
        private static IDictionary<string, int> separators = new Dictionary<string, int>();
        private static IDictionary<string, Label> separatorLabels = new Dictionary<string, Label>();
        private static VisualElement root;

        private MiniatureModel WIM;
        private VisualTreeAsset visualTree;

        public static void UniqueSeparator(string text = "", ushort space = 20) {
            if(separators.ContainsKey(text)) {
                separators[text]++;
                return;
            }
            separators.Add(text, 1);
            var newSeparator = new Label(text);
            root.Add(newSeparator);
            separatorLabels.Add(text, newSeparator);
            newSeparator.AddToClassList("Separator");
        }

        public static void UnregisterUniqueSeparator(string text = "") {
            if(!separators.ContainsKey(text)) return;
            separators[text]--;
            if(separators[text] > 0) return;
            root.Remove(separatorLabels[text]);
            separatorLabels.Remove(text);
            separators.Remove(text);
        }


        public void OnEnable() {
            WIM = (MiniatureModel) target;
        }

        public override VisualElement CreateInspectorGUI() {
            root = new VisualElement();
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIMVR/Scripts/Editor/Core/MiniatureModelEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIMVR/Scripts/Editor/Core/MiniatureModelEditor.uss");
            root.styleSheets.Add(styleSheet);
            if(visualTree) visualTree.CloneTree(root);

            root.Q<ObjectField>("configuration").objectType = typeof(WIMConfiguration);
            var configField = root.Q<ObjectField>("configuration");
            configField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => {
                root.Q<HelpBox>(name: "config-missing").SetDisplay(!e.newValue);
                root.Q<VisualElement>("master-container").SetDisplay(e.newValue);
                if (e.newValue) root.schedule.Execute(() => WIMGenerator.ConfigureWIM(WIM));    // Redraw UI: new config is not null so wait for the config to be loaded.
                else WIMGenerator.ConfigureWIM(WIM); // Redraw UI: new config is null so do it now.
            });
            root.Q<HelpBox>(name: "config-missing").SetDisplay(!WIM.Configuration);
            root.Q<VisualElement>("master-container").SetDisplay(WIM.Configuration);
            if (!WIM.Configuration) return root;

            root.Q<Button>("GenerateWIMButton").RegisterCallback<MouseUpEvent>(e => WIMGenerator.GenerateNewWIM(WIM));

            root.Q<ObjectField>("player-representation").objectType = typeof(GameObject);   // Hotfix until 2020.1
            root.Q<ObjectField>("destination-indicator").objectType = typeof(GameObject);   // Hotfix until 2020.1
            root.Q<FloatSlider>("scale-factor").RegisterCallback<FocusOutEvent>(e => WIMGenerator.GenerateNewWIM(WIM));

            root.Q<VisualElement>("expand-colliders-container").Add(new IMGUIContainer(() => {
                if(!WIM.Configuration) return;
                Undo.RecordObject(WIM.Configuration, "Set expand colliders");
                WIM.Configuration.ExpandCollidersX =
                    WIMEditorUtility.NamedVectorField("Expand Colliders X", WIM.Configuration.ExpandCollidersX, "Left", "Right");
                WIM.Configuration.ExpandCollidersY =
                    WIMEditorUtility.NamedVectorField("Expand Colliders Y", WIM.Configuration.ExpandCollidersY, "Up", "Down");
                WIM.Configuration.ExpandCollidersZ =
                    WIMEditorUtility.NamedVectorField("Expand Colliders Z", WIM.Configuration.ExpandCollidersZ, "Front", "Back");
            }));

            InvokeCallbacks(root.Q<VisualElement>("basic-container"), "Basic");
            
            var destinationSelectionTouchAvailable = WIM.GetComponent<DestinationSelectionTouch>() != null;
            var destinationSelectionMethodEnumField = root.Q<EnumField>("destination-selection-method");
            destinationSelectionMethodEnumField.SetEnabled(destinationSelectionTouchAvailable);
            destinationSelectionMethodEnumField.RegisterValueChangedCallback(e
                => root.Q<FloatField>("double-tap-interval").SetDisplay((DestinationSelection) e.newValue == DestinationSelection.Pickup));
            root.Q<HelpBox>("destination-selection-method-info").SetDisplay(!destinationSelectionTouchAvailable);
            root.Q<FloatField>("double-tap-interval").SetDisplay(WIM.Configuration.DestinationSelectionMethod == DestinationSelection.Pickup);

            root.Q<Toggle>("semi-transparent").RegisterValueChangedCallback(e
                => root.schedule.Execute(()=>WIMGenerator.ConfigureWIM(WIM)));  // Delay so that newValue is set on execution.

            var transparencySliderRoot = root.Q<FloatSlider>("transparency");
            var transparencySlider = transparencySliderRoot.Q<Slider>();
            transparencySlider.RegisterCallback<FocusOutEvent>(e => {
                    Undo.RecordObject(WIM.GetComponentInChildren<Renderer>().sharedMaterial, "Set transparency");
                    WIMGenerator.ConfigureWIM(WIM);
                });
            transparencySliderRoot.SetDisplay(WIM.Configuration.SemiTransparent);

            InvokeCallbacks(root.Q<VisualElement>("occlusion-handling-container"), "Occlusion Handling");

            var spawnDistance = root.Q<FloatField>("spawn-distance");
            spawnDistance.SetDisplay(!WIM.Configuration.AutoDetectArmLength);

            var autoDetectArmLengthAvailable = WIM.GetComponent<DetectArmLength>() != null;
            var detectArmLengthToggle = root.Q<Toggle>("detect-arm-length");
            detectArmLengthToggle.SetEnabled(autoDetectArmLengthAvailable);
            detectArmLengthToggle.RegisterValueChangedCallback(e => spawnDistance.SetDisplay(!e.newValue));
            root.Q<HelpBox>("detect-arm-length-info").SetDisplay(!autoDetectArmLengthAvailable);
            if(!autoDetectArmLengthAvailable)WIM.Configuration.AutoDetectArmLength = false;

            InvokeCallbacks(root.Q<VisualElement>("usability-container"), "Usability");

            InvokeCallbacks(root);
            Bindings();
            return root;
        }

        private void Bindings() {
            root.Bind(new SerializedObject(WIM));
            if(WIM.Configuration)
                root.Bind(new SerializedObject(WIM.Configuration));
        }

        private void InvokeCallbacks(VisualElement container, string key = "") {
            OnDraw.InvokeCallbacks(WIM, container, key);
        }
    }

    public static class VEExtension {
        /// <summary>
        /// Changes the VisualElement display state. If element is not displayed, it is invisible and doesn't use any space.
        /// </summary>
        /// <param name="element">The element to change.</param>
        /// <param name="value">Whether to display element.</param>
        public static void SetDisplay(this VisualElement element, bool value) {
            element.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}