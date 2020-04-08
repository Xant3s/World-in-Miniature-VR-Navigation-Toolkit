using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace ST.Utils {
    public sealed class CustomBuildPipeline {
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

        private static void BuildQuest(BuildOptions buildOptions, string path = "") {
            if(string.IsNullOrEmpty(path)) {
                path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
            }
            if(string.IsNullOrEmpty(path)) return;
            var scenes = EditorBuildSettings.scenes;
            var fileName = Application.productName;
            var filePath = $"{path}/{fileName}.apk";

            // Build player.
            BuildPipeline.BuildPlayer(scenes, filePath, BuildTarget.Android, buildOptions);
        }

        private static void BuildRunQuest(string path) {
            var buildOptions = BuildOptions.CompressWithLz4 | BuildOptions.AutoRunPlayer;
            BuildQuest(buildOptions, path);
        }
    }
}