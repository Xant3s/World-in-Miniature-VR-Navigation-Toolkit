// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using WIMVR.Features.LiveUpdate;

namespace WIMVR.Editor.Features {
    /// <summary>
    /// Custom inspector.
    /// </summary>
    [CustomEditor(typeof(LiveUpdate))]
    public class LiveUpdateEditor : UnityEditor.Editor {
        private static bool initialized;
        
        private void OnEnable() {
            if(initialized) return;
            initialized = true;
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 0, "Basic");
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw, "Basic");
            initialized = false;
        }

        private void Draw(WIMConfiguration config, VisualElement container) {
            var liveUpdate = (LiveUpdate) target;
            if(!liveUpdate) return;
            var WIM = liveUpdate.GetComponent<MiniatureModel>();
            var autoGenerateWIM = new Toggle {
                label="Live Update WIM (experimental)",
                bindingPath = "AutoGenerateWIM",
                tooltip = "Automatically update the miniature model when changes are made to the level. Works also in editor. Not all changes are detected (see manual to learn more). Experimental feature."
            };
            autoGenerateWIM.RegisterValueChangedCallback(e =>
                autoGenerateWIM.schedule.Execute(() => LiveUpdate.UpdateAutoGenerateWIM(WIM))); // Delay so that newValue is set on execution.
            container.Add(autoGenerateWIM);
        }
    }
}


