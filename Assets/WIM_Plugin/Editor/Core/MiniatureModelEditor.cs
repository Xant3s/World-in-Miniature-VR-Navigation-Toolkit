using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace WIM_Plugin {
// The custom inspector. Displays only relevant settings.
    [CustomEditor(typeof(MiniatureModel))]
    public class MiniatureModelEditor : Editor {
        public static DrawCallbackManager OnDraw = new DrawCallbackManager();

        public delegate VisualElement Test(WIMConfiguration config);
        public static event Test OnBasicDraw;

        private MiniatureModel WIM;
        private static GUIStyle headerStyle;

        private static IList<string> separators = new List<string>();

        private VisualElement root;
        private VisualTreeAsset visualTree;


        public void OnEnable() {
            WIM = (MiniatureModel) target;
            root = new VisualElement();
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Editor/Core/MiniatureModelEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIM_Plugin/Editor/Core/MiniatureModelEditor.uss");
            root.styleSheets.Add(styleSheet);

            if(visualTree) visualTree.CloneTree(root);
        }

        public override VisualElement CreateInspectorGUI() {
            if(!visualTree) return new VisualElement();
            root.Q<ObjectField>("configuration").objectType = typeof(WIMConfiguration);     // Hotfix until 2020.1
            var configField = root.Q<ObjectField>("configuration");
            configField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => {
                root.Q<HelpBox>(name: "config-missing").SetDisplay(!e.newValue);
                root.Q<VisualElement>("master-container").SetDisplay(e.newValue);
            });
            root.Q<HelpBox>(name: "config-missing").SetDisplay(!WIM.Configuration);
            root.Q<VisualElement>("master-container").SetDisplay(WIM.Configuration);
            if (!WIM.Configuration) return root;

            root.Q<Button>("GenerateWIMButton").RegisterCallback<MouseUpEvent>(e => {
                WIMGenerator.GenerateNewWIM(WIM);
            });

            root.Q<ObjectField>("player-representation").objectType = typeof(GameObject);   // Hotfix until 2020.1
            root.Q<ObjectField>("destination-indicator").objectType = typeof(GameObject);   // Hotfix until 2020.1

            root.Q<VisualElement>("expand-colliders-container").Add(new IMGUIContainer(() => {
                if(!WIM.Configuration) return;
                WIM.Configuration.ExpandCollidersX =
                    WIMEditorUtility.NamedVectorField("Expand Colliders X", WIM.Configuration.ExpandCollidersX, "Left", "Right");
                WIM.Configuration.ExpandCollidersY =
                    WIMEditorUtility.NamedVectorField("Expand Colliders Y", WIM.Configuration.ExpandCollidersY, "Up", "Down");
                WIM.Configuration.ExpandCollidersZ =
                    WIMEditorUtility.NamedVectorField("Expand Colliders Z", WIM.Configuration.ExpandCollidersZ, "Front", "Back");
            }));

            //root.Q<VisualElement>("basic-container").Add(new IMGUIContainer(() => {
            //    if(!WIM.Configuration) return;
            //    InvokeCallbacks("Basic");

            //}));
            root.Q<VisualElement>("basic-container").Add(OnBasicDraw?.Invoke(WIM.Configuration));

            var destinationSelectionTouchAvailable = WIM.GetComponent<DestinationSelectionTouch>() != null;
            var destinationSelectionMethodEnumField = root.Q<EnumField>("destination-selection-method");
            destinationSelectionMethodEnumField.SetEnabled(destinationSelectionTouchAvailable);
            destinationSelectionMethodEnumField.RegisterValueChangedCallback(e
                => root.Q<FloatField>("double-tap-interval").SetDisplay((DestinationSelection) e.newValue == DestinationSelection.Pickup));
            root.Q<HelpBox>("destination-selection-method-info").SetDisplay(!destinationSelectionTouchAvailable);
            root.Q<FloatField>("double-tap-interval").SetDisplay(WIM.Configuration.DestinationSelectionMethod == DestinationSelection.Pickup);

            //root.Q<VisualElement>("input-container").Add(new IMGUIContainer(() => {
            //    InvokeCallbacks("Input");
            //}));

            root.Q<Toggle>("semi-transparent").RegisterValueChangedCallback(e => WIMGenerator.ConfigureWIM(WIM));

            var transparencySliderRoot = root.Q<FloatSlider>("transparency");
            var transparencySlider = transparencySliderRoot.Q<Slider>();
            //transparencySlider.RegisterValueChangedCallback(e => WIMGenerator.ConfigureWIM(WIM));
            transparencySlider.RegisterCallback<FocusOutEvent>(e => WIMGenerator.ConfigureWIM(WIM));
            transparencySliderRoot.SetDisplay(WIM.Configuration.SemiTransparent);

            root.Q<VisualElement>("occlusion-handling-container").Add(new IMGUIContainer(() => {
                InvokeCallbacks("Occlusion Handling");
            }));

            var spawnDistance = root.Q<FloatField>("spawn-distance");
            spawnDistance.SetDisplay(!WIM.Configuration.AutoDetectArmLength);

            var autoDetectArmLengthAvailable = WIM.GetComponent<DetectArmLength>() != null;
            var detectArmLengthToggle = root.Q<Toggle>("detect-arm-length");
            detectArmLengthToggle.SetEnabled(autoDetectArmLengthAvailable);
            detectArmLengthToggle.RegisterValueChangedCallback(e => spawnDistance.SetDisplay(!e.newValue));
            root.Q<HelpBox>("detect-arm-length-info").SetDisplay(!autoDetectArmLengthAvailable);
            if(!autoDetectArmLengthAvailable)WIM.Configuration.AutoDetectArmLength = false;

            root.Q<VisualElement>("usability-container").Add(new IMGUIContainer(() => {
                InvokeCallbacks("Usability");
            }));


            Action action = () => {
                WIM = (MiniatureModel) target;
                headerStyle = new GUIStyle(GUI.skin.label) {
                    fontStyle = FontStyle.Bold
                };

                if(!WIM.Configuration) {
                    return;
                }

                separators.Clear();
                EditorUtility.SetDirty(WIM.Configuration);
                InvokeCallbacks();
            };

            root.Add(new IMGUIContainer(action));
            bindings();
            return root;
        }

        private void bindings() {
            root.Bind(new SerializedObject(WIM));
            if(WIM.Configuration)
                root.Bind(new SerializedObject(WIM.Configuration));
        }

        private void InvokeCallbacks(string key = "") {
            OnDraw.InvokeCallbacks(WIM, key);
        }

        public static void Separator(string text = "", ushort space = 20) {
            GUILayout.Space(space);
            GUILayout.Label(text, headerStyle);
        }

        public static void UniqueSeparator(string text = "", ushort space = 20) {
            if(separators.Contains(text)) return;
            Separator(text, space);
            separators.Add(text);
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