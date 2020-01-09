using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    public class WelcomeWindow : EditorWindow {
        private static readonly bool isProVersion = false;
        private static readonly string pluginVersion = "0.9.0";
        private static readonly string imagePath = "Assets/WIM_Plugin/Sprites/";


        [MenuItem("Window/WIM Plugin/ Welcome Window")]
        public static void ShowWindow() {
            var window = GetWindow<WelcomeWindow>();
            window.titleContent = new GUIContent("WIM Plugin Welcome");
        }

        public void OnEnable() {
            VisualElement root = rootVisualElement;
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIM_Plugin/Editor/Welcome/WelcomeWindow.uss");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Editor/Welcome/WelcomeWindow.uxml");
            VisualElement uxmlContents = visualTree.CloneTree();
            uxmlContents.styleSheets.Add(styleSheet);
            root.Add(uxmlContents);

            root.Q<Image>(name: "SceneIcon").image = EditorGUIUtility.IconContent("SceneAsset Icon").image;
            root.Q<Image>(name: "YoutubeIcon").image = AssetDatabase.LoadAssetAtPath<Texture>(imagePath + "youtube.png");
            root.Q<Image>(name: "ManualIcon").image = EditorGUIUtility.IconContent("_Help").image;
            root.Q<Image>(name: "EmailIcon").image = AssetDatabase.LoadAssetAtPath<Texture>(imagePath + "email.png");

            root.Q<Label>(name: "VersionNumber").text = pluginVersion;

            root.Q<Button>("ExampleSceneBtn").RegisterCallback<MouseUpEvent>((e) => {
                Debug.Log("Example Scene Button");
            });

            root.Q<Button>("VideoBtn").RegisterCallback<MouseUpEvent>((e) => {
                Debug.Log("Video Button");
            });

            root.Q<Button>("ManualBtn").RegisterCallback<MouseUpEvent>((e) => {
                Debug.Log("Manual Button");
            });

            root.Q<Button>("SupportEmailBtn").RegisterCallback<MouseUpEvent>((e) => {
                Debug.Log("Support Email Button");
            });

            root.Q<VisualElement>(name: "FullFeatureLabel").visible = !isProVersion;
            root.Q<Label>(name: "FullFeatureURL").RegisterCallback<MouseUpEvent>((e) => {
                Debug.Log("URL to full feature version");
            });

            var showOnStartupToggle = root.Q<Toggle>("showOnStartup");
            showOnStartupToggle.RegisterCallback<ChangeEvent<bool>>((e) => {
                Debug.Log("Toggle changed: " + showOnStartupToggle.value);
            });
        }
    }

}