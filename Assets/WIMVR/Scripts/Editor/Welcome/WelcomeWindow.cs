// Author: Samuel Truman (contact@samueltruman.com)

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using WIMVR.Editor.Util;

namespace WIMVR.Editor.Welcome {
    /// <summary>
    /// The welcome window visible at the first start.
    /// </summary>
    public class WelcomeWindow : EditorWindow {
        private static readonly string pluginVersion = "0.9.0";


        [MenuItem("Window/WIMVR/Welcome Window")]
        public static void ShowWindow() {
            var window = GetWindow(typeof(WelcomeWindow), true, "WIMVR Welcome");
            window.minSize = window.maxSize = new Vector2(566, 384);
        }

        public void OnEnable() {
            var root = rootVisualElement;
            var visualTree = AssetUtils.LoadAtRelativePath<VisualTreeAsset>("WelcomeWindow.uxml", this);
            var styleSheet = AssetUtils.LoadAtRelativePath<StyleSheet>("WelcomeWindow.uss", this);
            VisualElement uxmlContents = visualTree.CloneTree();
            uxmlContents.styleSheets.Add(styleSheet);
            root.Add(uxmlContents);

            root.Q<Image>(name: "SceneIcon").image = EditorGUIUtility.IconContent("SceneAsset Icon").image;
            // root.Q<Image>(name: "YoutubeIcon").image = Resources.Load<Texture>("Sprites/youtube");
            root.Q<Image>(name: "ManualIcon").image = Resources.Load<Texture>("Sprites/help");
            root.Q<Image>(name: "EmailIcon").image = Resources.Load<Texture>("Sprites/email");

            root.Q<Label>(name: "VersionNumber").text = pluginVersion;

            root.Q<Button>("ExampleSceneBtn").RegisterCallback<MouseUpEvent>((e) => {
                var scenePath = AssetUtils.GetPathRelativeTo("../../../Examples/SimpleExample/SimpleExample.unity", this);
                EditorSceneManager.OpenScene(scenePath);
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

            var isLiteVersion = !File.Exists(AssetUtils.GetPathRelativeTo("../Features/Scrolling/ScrollingEditor.cs", this));
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