using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace WIM_Plugin {
    [ExecuteAlways]
    public class LiveUpdate : MonoBehaviour {
        private void OnEnable() {
            WIMGenerator.OnPreConfigure += RemoveAutoUpdate;
        }

        private void OnDisable() {
            WIMGenerator.OnPreConfigure -= RemoveAutoUpdate;
        }

        private void RemoveAutoUpdate(in MiniatureModel WIM) {
            DestroyImmediate(WIM.GetComponentInChildren<AutoUpdateWIM>());
        }

        internal static void UpdateAutoGenerateWIM(in MiniatureModel WIM) {
            var level = GameObject.FindWithTag("Level");
            if(!level) {
                Debug.LogWarning("Level not found.");
                return;
            }

            if(WIM.Configuration.AutoGenerateWIM) {
#if UNITY_EDITOR
                var c = Undo.AddComponent(level, typeof(AutoUpdateWIM));    // Add script recursively
                ((AutoUpdateWIM) c).WIM = WIM;
#endif
            }
            else {
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(level.GetComponent<AutoUpdateWIM>());   // Destroy script recursively
#endif
            }
        }
    }
}