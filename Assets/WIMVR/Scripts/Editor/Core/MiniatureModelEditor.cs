// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
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
        public static ISeparatorManager Separators => separators;

        private static ISeparatorManager separators;
        private static GUIStyle headerStyle;
        private static VisualElement root;
        private MiniatureModel WIM;
        private VisualTreeAsset visualTree;


        public void OnEnable() {
            WIM = (MiniatureModel) target;
        }

        public override VisualElement CreateInspectorGUI() {
            root = new VisualElement();
            separators = new SeparatorManager(root);
            LoadHierarchy();
            LoadStyle();
            DisplayHelpBoxIfConfigUnassigned();
            if (!WIM.Configuration) return root;
            DisplayHelpBoxIfUnassigned("player-representation", "player-representation-missing", WIM.Configuration.PlayerRepresentation);
            DisplayHelpBoxIfUnassigned("destination-indicator", "destination-indicator-missing", WIM.Configuration.DestinationIndicator);
            AddGenerateWIMButtonBehavior();
            root.Q<ObjectField>("player-representation").objectType = typeof(GameObject);   // Hotfix until 2020.1
            root.Q<ObjectField>("destination-indicator").objectType = typeof(GameObject);   // Hotfix until 2020.1
            AddChangeScaleFactorBehavior();
            AddExpandCollidersVectorFields();
            InvokeCallbacks(root.Q<VisualElement>("basic-container"), "Basic");
            ShowSettingsDependingOnDestinationSelectionMethod();
            ShowTransparencySettings();
            AddChangeTransparencyBehavior();
            InvokeCallbacks(root.Q<VisualElement>("occlusion-handling-container"), "Occlusion Handling");
            ShowDetectArmLengthSettings();
            InvokeCallbacks(root.Q<VisualElement>("usability-container"), "Usability");
            InvokeCallbacks(root);
            Bindings();
            return root;
        }

        private void LoadHierarchy() {
            visualTree = Resources.Load<VisualTreeAsset>("MiniatureModelEditor");
            if(visualTree) visualTree.CloneTree(root);
        }

        private void LoadStyle() {
            var styleSheet = Resources.Load<StyleSheet>("MiniatureModelEditor");
            root.styleSheets.Add(styleSheet);
        }

        private void DisplayHelpBoxIfConfigUnassigned() {
            var configField = root.Q<ObjectField>("configuration");
            configField.objectType = typeof(WIMConfiguration);
            configField.RegisterCallback<ChangeEvent<Object>>(e => {
                root.Q<HelpBox>(name: "config-missing").SetDisplay(!e.newValue);
                root.Q<VisualElement>("master-container").SetDisplay(e.newValue);
            });
            root.Q<HelpBox>(name: "config-missing").SetDisplay(!WIM.Configuration);
            root.Q<VisualElement>("master-container").SetDisplay(WIM.Configuration);
        }

        private void DisplayHelpBoxIfUnassigned(string objectFieldName, string helpBoxName, GameObject obj) {
            var objectField = root.Q<ObjectField>(name: objectFieldName);
            var helpBox = root.Q<HelpBox>(name: helpBoxName);
            helpBox.SetDisplay(!obj);
            objectField.RegisterCallback<ChangeEvent<Object>>(e => helpBox.SetDisplay(!e.newValue));
        }

        private void AddGenerateWIMButtonBehavior() {
            root.Q<Button>("GenerateWIMButton").RegisterCallback<MouseUpEvent>(e => WIMGenerator.GenerateNewWIM(WIM));
        }

        private void AddChangeScaleFactorBehavior() {
            root.Q<FloatSlider>("scale-factor").RegisterCallback<FocusOutEvent>(e => WIMGenerator.GenerateNewWIM(WIM));
        }

        private void AddExpandCollidersVectorFields() {
            root.Q<VisualElement>("expand-colliders-container").Add(new IMGUIContainer(() => {
                if(!WIM.Configuration) return;
                Undo.RecordObject(WIM.Configuration, "Set expand colliders");
                WIM.Configuration.ExpandCollidersX =
                    WIMEditorUtility.NamedVectorField("Expand Colliders X", WIM.Configuration.ExpandCollidersX, "Left",
                        "Right");
                WIM.Configuration.ExpandCollidersY =
                    WIMEditorUtility.NamedVectorField("Expand Colliders Y", WIM.Configuration.ExpandCollidersY, "Up", "Down");
                WIM.Configuration.ExpandCollidersZ =
                    WIMEditorUtility.NamedVectorField("Expand Colliders Z", WIM.Configuration.ExpandCollidersZ, "Front",
                        "Back");
            }));
        }

        private void InvokeCallbacks(VisualElement container, string key = "") {
            OnDraw.InvokeCallbacks(WIM, container, key);
        }

        private void ShowSettingsDependingOnDestinationSelectionMethod() {
            var destinationSelectionTouchAvailable = WIM.GetComponent<DestinationSelectionTouch>() != null;
            var destinationSelectionMethodEnumField = root.Q<EnumField>("destination-selection-method");
            destinationSelectionMethodEnumField.SetEnabled(destinationSelectionTouchAvailable);
            destinationSelectionMethodEnumField.RegisterValueChangedCallback(e
                => root.Q<FloatField>("double-tap-interval")
                    .SetDisplay((DestinationSelection) e.newValue == DestinationSelection.Pickup));
            root.Q<HelpBox>("destination-selection-method-info").SetDisplay(!destinationSelectionTouchAvailable);
            root.Q<FloatField>("double-tap-interval")
                .SetDisplay(WIM.Configuration.DestinationSelectionMethod == DestinationSelection.Pickup);
        }

        private void ShowTransparencySettings() {
            // root.Q<Toggle>("semi-transparent").RegisterValueChangedCallback(e
            // => root.schedule.Execute(() => WIMGenerator.ConfigureWIM(WIM))); // Delay so that newValue is set on execution.
        }

        private void AddChangeTransparencyBehavior() {
            var transparencySliderRoot = root.Q<FloatSlider>("transparency");
            var transparencySlider = transparencySliderRoot.Q<Slider>();
            transparencySlider.RegisterCallback<FocusOutEvent>(e => {
                Undo.RecordObject(WIM.GetComponentInChildren<Renderer>().sharedMaterial, "Set transparency");
                WIMGenerator.ConfigureWIM(WIM);
            });
            transparencySliderRoot.SetDisplay(WIM.Configuration.SemiTransparent);
        }

        private void ShowDetectArmLengthSettings() {
            var spawnDistance = root.Q<FloatField>("spawn-distance");
            spawnDistance.SetDisplay(!WIM.Configuration.AutoDetectArmLength);

            var autoDetectArmLengthAvailable = WIM.GetComponent<DetectArmLength>() != null;
            var detectArmLengthToggle = root.Q<Toggle>("detect-arm-length");
            detectArmLengthToggle.SetEnabled(autoDetectArmLengthAvailable);
            detectArmLengthToggle.RegisterValueChangedCallback(e => spawnDistance.SetDisplay(!e.newValue));
            root.Q<HelpBox>("detect-arm-length-info").SetDisplay(!autoDetectArmLengthAvailable);
            if(!autoDetectArmLengthAvailable) WIM.Configuration.AutoDetectArmLength = false;
        }

        private void Bindings() {
            root.Bind(new SerializedObject(WIM));
            if(WIM.Configuration)
                root.Bind(new SerializedObject(WIM.Configuration));
        }
    }
}