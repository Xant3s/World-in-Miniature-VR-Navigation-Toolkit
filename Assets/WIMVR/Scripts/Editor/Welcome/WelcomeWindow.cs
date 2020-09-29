// Author: Samuel Truman (contact@samueltruman.com)

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace WIMVR.Editor.Welcome {
    /// <summary>
    /// The welcome window visible at the first start.
    /// </summary>
    public class WelcomeWindow : EditorWindow {
        private static readonly string pluginVersion = "0.9.0";
        private static readonly string imagePath = "Assets/WIMVR/Sprites/";


        [MenuItem("Window/WIMVR/Welcome Window")]
        public static void ShowWindow() {
            var window = GetWindow<WelcomeWindow>();
            window.titleContent = new GUIContent("WIMVR Welcome");
            window.minSize = new Vector2(566, 384);
        }

        public void OnEnable() {
            var root = rootVisualElement;
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIMVR/Scripts/Editor/Welcome/WelcomeWindow.uss");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIMVR/Scripts/Editor/Welcome/WelcomeWindow.uxml");
            VisualElement uxmlContents = visualTree.CloneTree();
            uxmlContents.styleSheets.Add(styleSheet);
            root.Add(uxmlContents);

            root.Q<Image>(name: "SceneIcon").image = EditorGUIUtility.IconContent("SceneAsset Icon").image;
            // root.Q<Image>(name: "YoutubeIcon").image = AssetDatabase.LoadAssetAtPath<Texture>(imagePath + "youtube.png");
            root.Q<Image>(name: "ManualIcon").image = AssetDatabase.LoadAssetAtPath<Texture>(imagePath + "help.png");
            root.Q<Image>(name: "EmailIcon").image = AssetDatabase.LoadAssetAtPath<Texture>(imagePath + "email.png");

            root.Q<Label>(name: "VersionNumber").text = pluginVersion;

            root.Q<Button>("ExampleSceneBtn").RegisterCallback<MouseUpEvent>((e) => {
                EditorSceneManager.OpenScene("Assets/WIMVR/Examples/SimpleExample/SimpleExample.unity");
            });

            // root.Q<Button>("VideoBtn").SetEnabled(false); // TODO remove as soon as tutorial is available
            // root.Q<Button>("VideoBtn").RegisterCallback<MouseUpEvent>((e) => {
            //     // TODO
            //     Application.OpenURL("");
            // });

            root.Q<Button>("ManualBtn").RegisterCallback<MouseUpEvent>((e) => {
                Application.OpenURL(Application.dataPath + "/WIMVR/Manual.pdf");
            });

            root.Q<Button>("SupportEmailBtn").RegisterCallback<MouseUpEvent>((e) => {
                Application.OpenURL("mailto:contact@samueltruman.com");
            });

            var isLiteVersion = !File.Exists(Application.dataPath + "/WIMVR/Scripts/Features/Scrolling/Scrolling.cs");
            root.Q<VisualElement>(name: "FullFeatureLabel").visible = isLiteVersion;
            root.Q<Label>(name: "FullFeatureURL").RegisterCallback<MouseUpEvent>((e) => {
                Application.OpenURL("https://assetstore.unity.com/");   // TODO: Link specific product page of the full-feature version
            });

            root.Q<Toggle>("showOnStartup").value = EditorPrefs.GetBool("WIM_Plugin_ShowWelcomeWindowOnStartup", true);
            var showOnStartupToggle = root.Q<Toggle>("showOnStartup");
            showOnStartupToggle.RegisterCallback<ChangeEvent<bool>>((e) => {
                EditorPrefs.SetBool("WIM_Plugin_ShowWelcomeWindowOnStartup", e.newValue);
            });
        }
    }

}