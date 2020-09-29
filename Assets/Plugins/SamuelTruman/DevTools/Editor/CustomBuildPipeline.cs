// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace ST.DevTools {
    public sealed class CustomBuildPipeline {
        private static readonly string androidBuildPathPref = "ViaVRDemonstrator2_AndroidBuildPath";


        [MenuItem("Build/Build Oculus Quest")]
        private static void BuildQuest() {
            // ReSharper disable once IntroduceOptionalParameters.Local
            BuildQuest(BuildOptions.CompressWithLz4);
        }

        [MenuItem("Build/Build and Run Oculus Quest")]
        private static void BuildRunQuest() {
            BuildRunQuest(string.Empty);
        }

        [MenuItem("Build/Patch and Run Oculus Quest (Scripts Only Build)")]
        private static void PatchRunQuest() {
            var buildOptions = BuildOptions.CompressWithLz4 | BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.PatchPackage;
            BuildQuest(buildOptions);
        }

        [MenuItem("Build/Reset Build Paths")]
        private static void ResetBuildPaths() {
            EditorPrefs.SetString(androidBuildPathPref, string.Empty);
            Debug.Log("Build path preferences have been reset. You will be prompted next time you build.");
        }

        private static void BuildQuest(BuildOptions buildOptions, string path = "") {
            if(string.IsNullOrEmpty(path)) {
                path = EditorPrefs.GetString(androidBuildPathPref);
                if(string.IsNullOrEmpty(path)) {
                    path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
                }
            }
            if(string.IsNullOrEmpty(path)) return;
            var scenes = EditorBuildSettings.scenes;
            var fileName = Application.productName;
            var filePath = $"{path}/{fileName}.apk";

            // Save path to editor prefs.
            EditorPrefs.SetString(androidBuildPathPref, path);

            // Build player.
            BuildPipeline.BuildPlayer(scenes, filePath, BuildTarget.Android, buildOptions);
        }

        private static void BuildRunQuest(string path) {
            var buildOptions = BuildOptions.CompressWithLz4 | BuildOptions.AutoRunPlayer;
            BuildQuest(buildOptions, path);
        }
    }
}