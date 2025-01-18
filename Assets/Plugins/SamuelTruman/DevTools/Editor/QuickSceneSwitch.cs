// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ST.DevTools {
    public sealed class QuickSceneSwitch {
        private static readonly string simpleExampleScene = "Packages/com.samueltruman.wimvr/Examples/SimpleExample/SimpleExample.unity";
        private static readonly string cityExampleScene = "Assets/ExampleProjects/CityExample/CityExample.unity";


        [MenuItem("Scenes/Simple Example")]
        private static void LoadSceneLauncher() {
            LoadScene(simpleExampleScene);
        }

        [MenuItem("Scenes/City Example")]
        private static void LoadSceneVR() {
            LoadScene(cityExampleScene);
        }

        private static void LoadScene(string scene) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if(Application.isPlaying) SceneManager.LoadScene(scene);
            else EditorSceneManager.OpenScene(scene);
        }
    }
}