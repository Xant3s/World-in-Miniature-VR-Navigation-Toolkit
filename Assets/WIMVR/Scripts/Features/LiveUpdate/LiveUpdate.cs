// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Core;

namespace WIMVR.Features.LiveUpdate {
    /// <summary>
    /// Monitors the full-sized level for any changes.
    /// Automatically apply changes to miniature model.
    /// Works both in editor and at runtime.
    /// Currently, only some changes are detected.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class LiveUpdate : MonoBehaviour {
        internal static void UpdateAutoGenerateWIM(in MiniatureModel WIM) {
            var level = GameObject.FindWithTag("Level");
            if(!level) {
                Debug.LogWarning("Level not found.");
                return;
            }

#if UNITY_EDITOR
            if(WIM.Configuration.AutoGenerateWIM) {
                level.AddComponent<AutoUpdateWIM>().WIM = WIM;  // Add script recursively
            }
            else {
                DestroyImmediate(level.GetComponent<AutoUpdateWIM>());  // Destroy script recursively
            }
#endif
        }

        private void OnEnable() {
            WIMGenerator.OnPreConfigure += RemoveAutoUpdate;
        }

        private void OnDisable() {
            WIMGenerator.OnPreConfigure -= RemoveAutoUpdate;
        }

        private void RemoveAutoUpdate(in MiniatureModel WIM) {
            DestroyImmediate(WIM.GetComponentInChildren<AutoUpdateWIM>());
        }
    }
}