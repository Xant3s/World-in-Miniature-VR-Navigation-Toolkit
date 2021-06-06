// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using WIMVR.Util;
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
                HideResults();
            });
            root.Add(dropdown);
        }

        private void HideResults() {
            root.Q<VisualElement>("integrity-check-results").Hide();
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
                HideResults();
            });        
        
            root.Q<ObjectField>("right-hand-prefab").RegisterValueChangedCallback(e => {
                rightHandPrefabPresent = e.newValue != null;
                root.Q("prefabs-missing-help").SetVisible(!bothHandsPresent());
                root.Q("integrity-check-container").SetVisible(bothHandsPresent());
                HideResults();
            });
        }

        private void SetupValidateButton() {
            root.Q<Button>("btn-check-integrity").RegisterCallback<ClickEvent>(e => Validate());
        }

        private void SetupFixButtons() {
            root.Q<Button>("btn-fix-oculus-integration")
                .RegisterCallback<ClickEvent>(e =>Debug.Log("Import Oculus Integration custom hands."));
            
            root.Q<Button>("btn-fix-left-index-finger-tip")
                .RegisterCallback<ClickEvent>(e 
                    => DisplayFingerTipPrefabMissingNotification(PrefabLoader.LeftIndexFingerTipPrefab));
            
            root.Q<Button>("btn-fix-left-thumb-tip")
                .RegisterCallback<ClickEvent>(e 
                    => DisplayFingerTipPrefabMissingNotification(PrefabLoader.LeftThumbFingerTipPrefab));
            
            root.Q<Button>("btn-fix-right-index-finger-tip")
                .RegisterCallback<ClickEvent>(e 
                    => DisplayFingerTipPrefabMissingNotification(PrefabLoader.RightIndexFingerTipPrefab));
            
            root.Q<Button>("btn-fix-right-thumb-tip")
                .RegisterCallback<ClickEvent>(e 
                    => DisplayFingerTipPrefabMissingNotification(PrefabLoader.RightThumbFingerTipPrefab));
            
        }

        private static void DisplayFingerTipPrefabMissingNotification(GameObject prefab) {
            var prefabName = prefab.name.ToLower();
            HighlightPrefab(prefab);
            EditorUtility.DisplayDialog("wimVR Hand Setup",
                $"Please attach the {prefabName} prefab to the {prefabName} of your hand model.", "Ok");
        }

        private static void HighlightPrefab(Object prefab) {
            Selection.objects = new[] {prefab};
            EditorGUIUtility.PingObject(prefab);
        }

        private void SetupBindings() {
            root.Bind(new SerializedObject(this));
        }

        private void Validate() {
            HandValidator validator = OculusHandsSelected switch {
                true => new OculusHandsValidator(),
                _ => new BasicHandValidator()
            };
            validator.CheckLeft(OculusHandsSelected ? LeftOculusHandPrefab : LeftHandPrefab);
            validator.CheckRight(OculusHandsSelected ? RightOculusHandPrefab : RightHandPrefab);
            var results = validator.GetResults();
            DisplayValidationResults(results);
        }

        private void DisplayValidationResults(ValidationResults results) {
            root.Q<VisualElement>("integrity-check-results").Show();
            root.Q<VisualElement>("oculus-integration-present").SetVisible(OculusHandsSelected);
            
            root.Q<Image>("oculus-integration-present-icon").image = results.PrefabRootsPresent ? validIcon : invalidIcon;
            root.Q<Button>("btn-fix-oculus-integration").SetVisible(!results.PrefabRootsPresent);
            
            root.Q<Image>("left-index-finger-tip-present-icon").image = results.LeftIndexFingerTip ? validIcon : invalidIcon;
            root.Q<Button>("btn-fix-left-index-finger-tip").SetVisible(!results.LeftIndexFingerTip);
            
            root.Q<Image>("left-thumb-tip-present-icon").image = results.LeftThumbFingerTip ? validIcon : invalidIcon;
            root.Q<Button>("btn-fix-left-thumb-tip").SetVisible(!results.LeftThumbFingerTip);
            
            root.Q<Image>("right-index-finger-tip-present-icon").image = results.RightIndexFingerTip ? validIcon : invalidIcon;
            root.Q<Button>("btn-fix-right-index-finger-tip").SetVisible(!results.RightIndexFingerTip);
            
            root.Q<Image>("right-thumb-tip-present-icon").image = results.RightThumbFingerTip ? validIcon : invalidIcon;
            root.Q<Button>("btn-fix-right-thumb-tip").SetVisible(!results.RightThumbFingerTip);
        }

        private GameObject LeftHandPrefab => root.Q<ObjectField>("left-hand-prefab").value as GameObject;

        private GameObject RightHandPrefab => root.Q<ObjectField>("right-hand-prefab").value as GameObject;

        private static GameObject LeftOculusHandPrefab => Resources.Load<GameObject>("CustomHandLeftDeviceBased");

        private static GameObject RightOculusHandPrefab => Resources.Load<GameObject>("CustomHandRightDeviceBased");

        private bool OculusHandsSelected => root.Q<PopupField<string>>("hand-models-type")
                                                .value
                                                .Equals("Oculus custom hands (recommended)");
    }
}