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
            root.Q<ObjectField>("configuration").objectType = typeof(WIMConfiguration);     // Hotfix until 2020.1
            //configField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((e) => {
            //    Debug.Log("update");
            //    //root.Q<HelpBox>(name: "config-missing").style.display = !WIM.Configuration ? DisplayStyle.Flex : DisplayStyle.None;
            //    //root.Q<VisualElement>("master-container").style.display = WIM.Configuration ? DisplayStyle.Flex : DisplayStyle.None;
            //});

            root.Q<HelpBox>(name: "config-missing").style.display = !WIM.Configuration ? DisplayStyle.Flex : DisplayStyle.None;
            root.Q<VisualElement>("master-container").style.display = WIM.Configuration ? DisplayStyle.Flex : DisplayStyle.None;

            root.Q<Button>("GenerateWIMButton").RegisterCallback<MouseUpEvent>(e => {
                WIMGenerator.GenerateNewWIM(WIM);
            });

            root.Q<ObjectField>("player-representation").objectType = typeof(GameObject);   // Hotfix until 2020.1
            root.Q<ObjectField>("destination-indicator").objectType = typeof(GameObject);   // Hotfix until 2020.1

            var scaleFactorInputField = root.Q<FloatField>("scale-factor-input-field");
            var scaleFactor = root.Q<Slider>("scale-factor");
            scaleFactorInputField.value = WIM.Configuration.ScaleFactor;
            scaleFactor.RegisterValueChangedCallback(e => {
                scaleFactorInputField.SetValueWithoutNotify(e.newValue);
            });
            scaleFactorInputField.RegisterValueChangedCallback(e => {
                var newValue = Mathf.Clamp(scaleFactorInputField.value, 0f, 1f);
                scaleFactor.value = newValue;
                scaleFactorInputField.SetValueWithoutNotify(newValue);
            });

            root.Q<VisualElement>("expand-colliders-container").Add(new IMGUIContainer(() => {
                WIM.Configuration.ExpandCollidersX =
                    WIMEditorUtility.NamedVectorField("Expand Colliders X", WIM.Configuration.ExpandCollidersX, "Left", "Right");
                WIM.Configuration.ExpandCollidersY =
                    WIMEditorUtility.NamedVectorField("Expand Colliders Y", WIM.Configuration.ExpandCollidersY, "Up", "Down");
                WIM.Configuration.ExpandCollidersZ =
                    WIMEditorUtility.NamedVectorField("Expand Colliders Z", WIM.Configuration.ExpandCollidersZ, "Front", "Back");
            }));

            root.Q<VisualElement>("basic-container").Add(new IMGUIContainer(() => {
                InvokeCallbacks("Basic");
            }));


            var destinationSelectionTouchAvailable = WIM.GetComponent<DestinationSelectionTouch>() != null;
            root.Q<EnumField>("destination-selection-method").SetEnabled(destinationSelectionTouchAvailable);
            root.Q<HelpBox>("destination-selection-method-info").style.display = !destinationSelectionTouchAvailable ? DisplayStyle.Flex : DisplayStyle.None;


            Action action = () => {
                WIM = (MiniatureModel) target;
                headerStyle = new GUIStyle(GUI.skin.label) {
                    fontStyle = FontStyle.Bold
                };


                if(!WIM.Configuration) {
                    return;
                }

                //Separator("Basic Settings");
                //WIM.Configuration.PlayerRepresentation = (GameObject) EditorGUILayout.ObjectField(
                //    "Player Representation",
                //    WIM.Configuration.PlayerRepresentation, typeof(GameObject), false);
                //WIM.Configuration.DestinationIndicator = (GameObject) EditorGUILayout.ObjectField(
                //    "Destination Indicator",
                //    WIM.Configuration.DestinationIndicator, typeof(GameObject), false);
                //WIM.Configuration.ScaleFactor =
                //    EditorGUILayout.Slider("Scale Factor", WIM.Configuration.ScaleFactor, 0, 1);
                //WIM.Configuration.WIMLevelOffset =
                //    EditorGUILayout.Vector3Field("WIM Level Offset", WIM.Configuration.WIMLevelOffset);
                //WIM.Configuration.ExpandCollidersX =
                //    WIMEditorUtility.NamedVectorField("Expand Colliders X", WIM.Configuration.ExpandCollidersX, "Left",
                //        "Right");
                //WIM.Configuration.ExpandCollidersY =
                //    WIMEditorUtility.NamedVectorField("Expand Colliders Y", WIM.Configuration.ExpandCollidersY, "Up",
                //        "Down");
                //WIM.Configuration.ExpandCollidersZ =
                //    WIMEditorUtility.NamedVectorField("Expand Colliders Z", WIM.Configuration.ExpandCollidersZ, "Front",
                //        "Back");
                //WIM.Configuration.DestinationAlwaysOnTheGround = EditorGUILayout.Toggle(
                //    new GUIContent("Destination Always on the Ground",
                //        "If active, the destination will automatically set to ground level. This protects the player from being teleported to a location in mid-air."),
                //    WIM.Configuration.DestinationAlwaysOnTheGround);
                //InvokeCallbacks("Basic");

                //var destinationSelectionTouchAvailable = WIM.GetComponent<DestinationSelectionTouch>() != null;
                //EditorGUI.BeginDisabledGroup(!destinationSelectionTouchAvailable);
                //WIM.Configuration.DestinationSelectionMethod =
                //    (DestinationSelection) EditorGUILayout.EnumPopup("Destination Selection Method",
                //        WIM.Configuration.DestinationSelectionMethod);
                //EditorGUI.EndDisabledGroup();
                //if(!destinationSelectionTouchAvailable) {
                //    WIM.Configuration.DestinationSelectionMethod = DestinationSelection.Pickup;
                //    EditorGUILayout.HelpBox(
                //        "Add 'DestinationSelectionTouch' script to change destination selection method",
                //        MessageType.Info);
                //}

                if(WIM.Configuration.DestinationSelectionMethod == DestinationSelection.Pickup) {
                    WIM.Configuration.DoubleTapInterval =
                        EditorGUILayout.FloatField("Double Tap Interval", WIM.Configuration.DoubleTapInterval);
                }

                InvokeCallbacks("Input");


                Separator("Occlusion Handling");
                EditorGUI.BeginChangeCheck();
                WIM.Configuration.SemiTransparent =
                    EditorGUILayout.Toggle("Semi-Transparent", WIM.Configuration.SemiTransparent);
                if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
                if(WIM.Configuration.SemiTransparent) {
                    EditorGUI.BeginChangeCheck();
                    WIM.Configuration.Transparency =
                        EditorGUILayout.Slider("Transparency", WIM.Configuration.Transparency, 0, 1);
                    if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
                }

                InvokeCallbacks("Occlusion Handling");


                Separator("Usability");
                WIM.Configuration.WIMSpawnHeight =
                    EditorGUILayout.FloatField("WIM Spawn at Height", WIM.Configuration.WIMSpawnHeight);
                WIM.Configuration.PlayerHeightInCM =
                    EditorGUILayout.FloatField("Player Height (in cm)", WIM.Configuration.PlayerHeightInCM);
                if(!WIM.Configuration.AutoDetectArmLength) {
                    WIM.Configuration.SpawnDistance =
                        EditorGUILayout.FloatField("WIM Spawn Distance", WIM.Configuration.SpawnDistance);
                }

                var autoDetectArmLengthAvailable = WIM.GetComponent<DetectArmLength>() != null;
                EditorGUI.BeginDisabledGroup(!autoDetectArmLengthAvailable);
                WIM.Configuration.AutoDetectArmLength = EditorGUILayout.Toggle(
                    new GUIContent("Auto Detect Arm Length",
                        "At the start of the application, player has to extend the arm and press the confirm teleport button." +
                        "The detected arm length will be used instead of the spawn distance."),
                    WIM.Configuration.AutoDetectArmLength);
                if(!autoDetectArmLengthAvailable) {
                    WIM.Configuration.AutoDetectArmLength = false;
                    EditorGUILayout.HelpBox("Add 'DetectArmLength' script to enable this feature.'", MessageType.Info);
                }

                EditorGUI.EndDisabledGroup();


                WIM.Configuration.AdaptWIMSizeToPlayerHeight = EditorGUILayout.Toggle("Adapt WIM Size to Player Height",
                    WIM.Configuration.AdaptWIMSizeToPlayerHeight);
                InvokeCallbacks("Usability");


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
}