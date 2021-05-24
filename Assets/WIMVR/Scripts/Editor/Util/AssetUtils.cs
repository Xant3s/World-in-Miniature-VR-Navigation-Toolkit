// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WIMVR.Editor.Util {
    public static class AssetUtils {
        public static string GetPathRelativeTo(string filePath, ScriptableObject relativeTo) {
            var relativeFilePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(relativeTo));
            var dir = relativeFilePath.Remove(relativeFilePath.LastIndexOf('/')) + "/";
            return RemoveRedundanciesFromPath(dir + filePath);
        }

        public static string RemoveRedundanciesFromPath(string path) {
            var folders = new Stack<string>(path.Split('/'));
            var foldersWithoutRedundancies = new Stack<string>();

            while(folders.Count > 0) {
                PutTopOnSecondStackIfNoRedundancy(folders, foldersWithoutRedundancies);
            }

            return foldersWithoutRedundancies.Aggregate((folder1, folder2) => $"{folder1}/{folder2}");
        }

        private static void PutTopOnSecondStackIfNoRedundancy(Stack<string> folders, Stack<string> foldersWithoutRedundancies) {
            var top = folders.Pop();
            if(top.Equals("..")) {
                if(folders.Peek().Equals("..")) {
                    PutTopOnSecondStackIfNoRedundancy(folders, foldersWithoutRedundancies);
                }
                folders.Pop();
            } else {
                foldersWithoutRedundancies.Push(top);
            }
        }
    }
}