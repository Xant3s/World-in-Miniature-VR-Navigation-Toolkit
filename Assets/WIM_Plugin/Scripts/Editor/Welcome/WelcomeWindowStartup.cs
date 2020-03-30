// Author: Samuel Truman (contact@samueltruman.com)

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace WIM_Plugin {
    /// <summary>
    /// Opens the welcome window on first start and every consecutive start of the Unity editor if
    /// not configured otherwise.
    /// </summary>
    [InitializeOnLoad]
    public class WelcomeWindowStartup : ScriptableObject {
        private static WelcomeWindowStartup instance;

        static WelcomeWindowStartup() {
            EditorApplication.update += OnInit;
            EditorApplication.quitting += OnQuitting;
        }

        private static void OnInit() {
            EditorApplication.update -= OnInit;
            instance = FindObjectOfType<WelcomeWindowStartup>();
            if(instance) return;
            instance = CreateInstance<WelcomeWindowStartup>();
            if(!EditorPrefs.GetBool("WIM_Plugin_NewEditorSession", true)) return;
            EditorPrefs.SetBool("WIM_Plugin_NewEditorSession", false);    

            if(EditorPrefs.GetBool("WIM_Plugin_ShowWelcomeWindowOnStartup", true)) {
                var window = EditorWindow.GetWindow<WelcomeWindow>();
                window.titleContent = new GUIContent("WIM Plugin Welcome");
                window.minSize = new Vector2(566, 384);
            }
        }

        private static void OnQuitting() {
            EditorPrefs.SetBool("WIM_Plugin_NewEditorSession", true);    
        }
    }
} 
#endif