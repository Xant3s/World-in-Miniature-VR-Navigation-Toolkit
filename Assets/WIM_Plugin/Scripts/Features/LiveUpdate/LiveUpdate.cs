using UnityEngine;


namespace WIM_Plugin {
    [ExecuteAlways]
    [DisallowMultipleComponent]
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

#if UNITY_EDITOR
            if(WIM.Configuration.AutoGenerateWIM) {
                level.AddComponent<AutoUpdateWIM>().WIM = WIM;  // Add script recursively
            }
            else {
                DestroyImmediate(level.GetComponent<AutoUpdateWIM>());  // Destroy script recursively
            }
#endif
        }
    }
}