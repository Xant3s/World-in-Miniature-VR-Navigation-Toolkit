// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WIMVR.Editor.Util {
    public static class AssetUtils {
        /// <summary>
        /// Loads an asset specified by a path relative to another asset using the AssetDatabase.
        /// </summary>
        /// <param name="filePath">Filename with extension.</param>
        /// <param name="relativeTo">The assets defining the relative path.</param>
        /// <typeparam name="T">The type of the asset to load.</typeparam>
        /// <returns>The loaded asset.</returns>
        public static T LoadAtRelativePath<T>(string filePath, ScriptableObject relativeTo) where T : Object {
            return AssetDatabase.LoadAssetAtPath<T>(GetPathRelativeTo(filePath, relativeTo));
        }

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