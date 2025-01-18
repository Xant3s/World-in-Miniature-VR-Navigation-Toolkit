// Author: Samuel Truman (contact@samueltruman.com)

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
        private VisualElement root;

        [MenuItem("Window/WIMVR/Welcome Window")]
        public static void ShowWindow() {
            var window = GetWindow(typeof(WelcomeWindow), true, "wimVR Welcome");
            window.minSize = window.maxSize = new Vector2(600, 520);
        }

        public void OnEnable() {
            root = rootVisualElement;
            LoadHierarchy();
            LoadStyle();
            LoadButtonIcons();
            AddStyleToIconButtons();
            RegisterOpenExampleSceneEvent();
            RegisterOpenManualEvent();
            RegisterSupportMailEvent();
            ShowOnStartupToggleBehavior();
        }

        private void LoadHierarchy() {
            var visualTree = AssetUtils.LoadAtRelativePath<VisualTreeAsset>("WelcomeWindow.uxml", this);
            visualTree.CloneTree(root);
        }

        private void LoadStyle() {
            var mode = EditorGUIUtility.isProSkin ? "DarkMode" : "LightMode";
            var styleSheet = AssetUtils.LoadAtRelativePath<StyleSheet>("WelcomeWindow.uss", this);
            var modeSpecificStyleSheet = AssetUtils.LoadAtRelativePath<StyleSheet>($"{mode}.uss", this);
            root.styleSheets.Add(styleSheet);
            root.styleSheets.Add(modeSpecificStyleSheet);
        }

        private void LoadButtonIcons() {
            root.Q<Image>(name: "SceneIcon").image = Resources.Load<Texture>("RTF/Icons/RTF_icon_app_dark");
            root.Q<Image>(name: "ManualIcon").image = Resources.Load<Texture>("RTF/Icons/RTF_icon_manual_dark");
            root.Q<Image>(name: "EmailIcon").image = Resources.Load<Texture>("RTF/Icons/RTF_icon_mail_dark");
        }

        private void AddStyleToIconButtons() {
            root.Query(className: "round-button").ForEach(roundButton => {
                var lightGray = new Color(192 / 255f, 192 / 255f, 192 / 255f);
                roundButton.RegisterCallback<MouseOverEvent>(e => roundButton.style.backgroundColor = lightGray);
                roundButton.RegisterCallback<MouseOutEvent>(e => roundButton.style.backgroundColor = Color.white);
            });
        }

        private void RegisterOpenExampleSceneEvent() {
            root.Q<Button>("ExampleSceneBtn").RegisterCallback<MouseUpEvent>(e => {
                var scenePath =
                    AssetUtils.GetPathRelativeTo("../../../Examples/SimpleExample/SimpleExample.unity", this);
                EditorSceneManager.OpenScene(scenePath);
            });
        }

        private void RegisterOpenManualEvent() {
            root.Q<Button>("ManualBtn").RegisterCallback<MouseUpEvent>(e => {
                var manualPath = AssetUtils.GetPathRelativeTo("../../../Manual.pdf", this);
                manualPath = $"{Application.dataPath}{manualPath.Remove(0, 6)}";
                Application.OpenURL(manualPath);
            });
        }

        private void RegisterSupportMailEvent() {
            root.Q<Button>("SupportEmailBtn").RegisterCallback<MouseUpEvent>(e => {
                Application.OpenURL("mailto:contact@samueltruman.com");
            });
        }

        private void ShowOnStartupToggleBehavior() {
            var showOnStartupToggle = root.Q<Toggle>("showOnStartup");
            showOnStartupToggle.value = EditorPrefs.GetBool("WIM_Plugin_ShowWelcomeWindowOnStartup", true);
            showOnStartupToggle.RegisterCallback<ChangeEvent<bool>>(e
                => EditorPrefs.SetBool("WIM_Plugin_ShowWelcomeWindowOnStartup", e.newValue));
        }
    }
}