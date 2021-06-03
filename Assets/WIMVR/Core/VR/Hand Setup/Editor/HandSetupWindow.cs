// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using WIMVR.Util.Extensions;
using WIMVR.VR.HandSetup;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UIElements.Image;

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
            AddHandsTypeDropdown();
            LoadHierarchy();
            LoadStyle();
            LoadIcons();
            RegisterPrefabValueChangeCallbacks();
            SetupValidateButton();
            SetupFixButtons();
            SetupBindings();
        }

        private void AddHandsTypeDropdown() {
            var choices = new List<string> { "Oculus custom hands (recommended)", "I use you own hand models" };
            var dropdown = new PopupField<string>("Hands Models", choices, 0) {
                name = "hand-models-type",
                tooltip = "Choose whether you want to use the Oculus hands or your own.",
                style = {
                    marginTop = 10,
                    marginLeft = 10,
                    marginRight = 10
                }
            };
            dropdown.RegisterValueChangedCallback(e => {
                var oculusHands = e.newValue == "Oculus custom hands (recommended)";
                var customHandPrefabsAssigned = LeftHandPrefab && RightHandPrefab;
                root.Q<VisualElement>("prefab-selection-container").SetVisible(!oculusHands);
                root.Q<VisualElement>("integrity-check-container").SetVisible(oculusHands || customHandPrefabsAssigned);
                root.Q<VisualElement>("integrity-check-results").Hide();
            });
            root.Add(dropdown);
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
            validator.CheckLeft(OculusHandsSelected ? LeftOculusHandPrefab : LeftHandPrefab);
            validator.CheckRight(OculusHandsSelected ? RightOculusHandPrefab : RightHandPrefab);
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

        private GameObject LeftOculusHandPrefab => Resources.Load<GameObject>("CustomHandLeftDeviceBased");
        
        private GameObject RightOculusHandPrefab => Resources.Load<GameObject>("CustomHandRightDeviceBased");

        private bool OculusHandsSelected => root.Q<PopupField<string>>("hand-models-type")
                                                .value
                                                .Equals("Oculus custom hands (recommended)");
    }
}