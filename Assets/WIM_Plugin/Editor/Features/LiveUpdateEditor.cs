using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    [CustomEditor(typeof(LiveUpdate))]
    public class LiveUpdateEditor : Editor {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 0, "Basic");
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw, "Basic");
        }

        private void Draw(WIMConfiguration config, VisualElement container) {
            var liveUpdate = (LiveUpdate) target;
            if(!liveUpdate) return;
            var WIM = liveUpdate.GetComponent<MiniatureModel>();
            var autoGenerateWIM = new Toggle {
                label="Live Update WIM",
                bindingPath = "AutoGenerateWIM"
            };
            autoGenerateWIM.RegisterValueChangedCallback(e =>
                autoGenerateWIM.schedule.Execute(() => LiveUpdate.UpdateAutoGenerateWIM(WIM))); // Delay so that newValue is set on execution.
            container.Add(autoGenerateWIM);
        }
    }
}


