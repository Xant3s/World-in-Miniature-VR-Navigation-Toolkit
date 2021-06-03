// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using WIMVR.Util.Extensions;
using WIMVR.VR.HandSetup;

namespace WIMVR.Core.VR.HandSetup.Editor {
    public class HandSetupWindow : EditorWindow {
        private VisualElement root;
        private Texture validIcon;
        private Texture invalidIcon;
    
    
        [MenuItem("Window/wimVR/Hand Setup")]
        public static void ShowExample() {
            var window = GetWindow<HandSetupWindow>();
            window.titleContent = new GUIContent("wimVR Hand Setup");
        }

        public void OnEnable() {
            root = rootVisualElement;
            LoadHierarchy();
            LoadStyle();
            LoadIcons();
            RegisterPrefabValueChangeCallbacks();
            SetupValidateButton();
            SetupFixButtons();
            SetupBindings();
        }

        private void LoadHierarchy() {
            var visualTree = Resources.Load<VisualTreeAsset>("HandSetupWindow");
            visualTree.CloneTree(root);
        }

        private void LoadStyle() {
            var style = Resources.Load<StyleSheet>("HandSetupWindowStyle");
            root.styleSheets.Add(style);
        }

        private void LoadIcons() {
            validIcon = EditorGUIUtility.IconContent("Valid@2x").image;
            invalidIcon = EditorGUIUtility.IconContent("d_Invalid@2x").image;
        }

        private void RegisterPrefabValueChangeCallbacks() {
            var leftHandPrefabPresent = false;
            var rightHandPrefabPresent = false;

            bool bothHandsPresent() => leftHandPrefabPresent && rightHandPrefabPresent;

            root.Q<ObjectField>("left-hand-prefab").RegisterValueChangedCallback(e => {
                leftHandPrefabPresent = e.newValue != null;
                root.Q("prefabs-missing-help").SetVisible(!bothHandsPresent());
                root.Q("integrity-check-container").SetVisible(bothHandsPresent());
            });        
        
            root.Q<ObjectField>("right-hand-prefab").RegisterValueChangedCallback(e => {
                rightHandPrefabPresent = e.newValue != null;
                root.Q("prefabs-missing-help").SetVisible(!bothHandsPresent());
                root.Q("integrity-check-container").SetVisible(bothHandsPresent());
            });
        }

        private void SetupValidateButton() {
            root.Q<Button>("btn-check-integrity").RegisterCallback<ClickEvent>(e => Validate());
        }

        private void SetupFixButtons() {
            root.Q<Button>("btn-fix-oculus-integration")
                .RegisterCallback<ClickEvent>(e =>Debug.Log("Import Oculus Integration custom hands."));
        }

        private void SetupBindings() {
            root.Bind(new SerializedObject(this));
        }

        private void Validate() {
            HandValidator validator = new OculusHandsValidator();
            validator.CheckLeft(LeftHandPrefab);
            validator.CheckRight(RightHandPrefab);
            var results = validator.GetResults();
            DisplayValidationResults(results);
        }

        private void DisplayValidationResults(ValidationResults results) {
            root.Q<VisualElement>("integrity-check-results").Show();
            root.Q<Image>("oculus-integration-present-icon").image = results.PrefabRootsPresent ? validIcon : invalidIcon;
            root.Q<Button>("btn-fix-oculus-integration").SetVisible(!results.PrefabRootsPresent);
        }

        private GameObject LeftHandPrefab => root.Q<ObjectField>("left-hand-prefab").value as GameObject;
        private GameObject RightHandPrefab => root.Q<ObjectField>("right-hand-prefab").value as GameObject;
    }
}