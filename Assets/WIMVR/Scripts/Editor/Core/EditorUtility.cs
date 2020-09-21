// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace WIMVR.Editor.Core {
    /// <summary>
    /// Editor utility functions.
    /// </summary>
    public static class WIMEditorUtility {
        /// <summary>
        /// A Vector2 float field with custom axis names.
        /// </summary>
        /// <param name="text">The label.</param>
        /// <param name="vector">The vector.</param>
        /// <param name="xAxisName">X axis name.</param>
        /// <param name="yAxisName">Y axis name.</param>
        /// <returns></returns>
        public static Vector2 NamedVectorField(string text, Vector2 vector, string xAxisName = "X", string yAxisName = "Y") {
            var namedFloatFieldStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleRight};
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(text);
            var tmpVec = vector;
            GUILayout.Label(xAxisName, namedFloatFieldStyle);
            tmpVec.x = EditorGUILayout.FloatField(tmpVec.x);
            GUILayout.Label(yAxisName, namedFloatFieldStyle);
            tmpVec.y = EditorGUILayout.FloatField(tmpVec.y);
            EditorGUILayout.EndHorizontal();
            return tmpVec;
        }

        // Makes assumptions about UIElements hierarchy.
        internal static void DisplaySettingsIfConfigNotNull(VisualElement root, bool configNotNull, Type configType) {
            root.Q<HelpBox>("config-info").SetDisplay(!configNotNull);
            var config = root.Q<ObjectField>("configuration");
            var settings = root.Q<VisualElement>("settings");
            settings.SetDisplay(configNotNull);
            config.SetDisplay(!configNotNull);
            config.objectType = configType;
            config.RegisterValueChangedCallback(e => {
                root.Q<HelpBox>("config-info").SetDisplay(!e.newValue);
                config.SetDisplay(!e.newValue);
                settings.SetDisplay(e.newValue);
                if(e.newValue) root.Bind(new SerializedObject(e.newValue));
            });
        }
    }
}
