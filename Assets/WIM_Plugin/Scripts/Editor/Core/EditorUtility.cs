// Author: Samuel Truman (contact@samueltruman.com)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace WIM_Plugin {
    public class DrawCallbackManager {
        public delegate void InspectorAction(WIMConfiguration config, VisualElement container);

        private static IDictionary<string, IDictionary<int, InspectorAction>> OnDraw = new Dictionary<string, IDictionary<int, InspectorAction>>();

        public void AddCallback(InspectorAction callback, int priority = 0, string key = "") {
            if(OnDraw.ContainsKey(key)) {
                OnDraw[key].Push(priority, callback);
            }
            else {
                OnDraw.Add(key, new Dictionary<int, InspectorAction>());
                OnDraw[key].Push(priority, callback);
            }
        }

        public void RemoveCallback(InspectorAction callback, string key = "") {
            foreach(var item in OnDraw[key].Where(pair => pair.Value == callback).ToList()) {
                OnDraw[key].Remove(item.Key);
            }
        }

        public int GetNumberOfCallbacks(string key = "") {
            return OnDraw[key].Count;
        }

        public void InvokeCallbacks(MiniatureModel WIM, VisualElement container, string key = "") {
            if(!OnDraw.ContainsKey(key)) return;
            var pairs = OnDraw[key].ToList();
            pairs.Sort((x,y) =>x.Key.CompareTo(y.Key));
            pairs.ForEach(callback => callback.Value(WIM.Configuration, container));
        }
    }


    public static class WIMEditorUtility {
        // Try to add with key priority. If key already exists, try key + 1.
        public static void Push(this IDictionary<int, DrawCallbackManager.InspectorAction> dict, int priority, DrawCallbackManager.InspectorAction callback) {
            if(!dict.ContainsKey(priority)) {
                dict.Add(priority, callback);
            }
            else {
                var i = 0;
                while (dict.ContainsKey(i)) i++;
                dict.Add(i, callback);
            }
        }

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
