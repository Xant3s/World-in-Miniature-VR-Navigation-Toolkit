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
            //MiniatureModelEditor.OnDraw.AddCallback(Draw, 0, "Basic");
            MiniatureModelEditor.OnBasicDraw += Draw;
        }

        private void OnDisable() {
            //MiniatureModelEditor.OnDraw.RemoveCallback(Draw, "Basic");
            MiniatureModelEditor.OnBasicDraw -= Draw;
        }

        //private void Draw(WIMConfiguration config) {
        //    var liveUpdate = (LiveUpdate) target;
        //    if(!liveUpdate) return;
        //    var WIM = liveUpdate.GetComponent<MiniatureModel>();
        //    EditorGUI.BeginChangeCheck();
        //    config.AutoGenerateWIM = EditorGUILayout.Toggle("Live Update WIM", config.AutoGenerateWIM);
        //    if (EditorGUI.EndChangeCheck()) LiveUpdate.UpdateAutoGenerateWIM(WIM);
        //}

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


