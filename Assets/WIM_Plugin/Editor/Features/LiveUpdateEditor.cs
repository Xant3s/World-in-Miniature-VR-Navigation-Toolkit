using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace WIM_Plugin {
    [CustomEditor(typeof(LiveUpdate))]
    public class LiveUpdateEditor : Editor
    {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 0, "Basic");
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw, "Basic");
        }

        private void Draw(WIMConfiguration config) {
            var liveUpdate = (LiveUpdate) target;
            if(!liveUpdate) return;
            var WIM = liveUpdate.GetComponent<MiniatureModel>();
            EditorGUI.BeginChangeCheck();
            config.AutoGenerateWIM = EditorGUILayout.Toggle("Live Update WIM", config.AutoGenerateWIM);
            if (EditorGUI.EndChangeCheck()) LiveUpdate.UpdateAutoGenerateWIM(WIM);
        }
    }
}


