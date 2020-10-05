// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using WIMVR.Editor.Util;
using WIMVR.Features.Path_Trace;

namespace WIMVR.Editor.Features {
    /// <summary>
    /// Custom inspector.
    /// </summary>
    [CustomEditor(typeof(PathTrace))]
    public class PathTraceEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((PathTrace) target).PathTraceConfig;
            if(config) return;
            EditorGUILayout.HelpBox("Path trace configuration missing. Create a path trace configuration asset and add it to the PathTrace script.", MessageType.Error);
        }

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 30);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
            MiniatureModelEditor.Separators?.UnregisterUnique("Orientation Aids");
        }

        private void Draw(WIMConfiguration WIMConfig, VisualElement container) {
            MiniatureModelEditor.Separators.RegisterUnique("Orientation Aids");
            var visualTree = AssetUtils.LoadAtRelativePath<VisualTreeAsset>("PathTraceEditor.uxml", this);
            var root = new VisualElement();
            visualTree.CloneTree(root);
            var pathTrace = (PathTrace) target;
            ref var config = ref pathTrace.PathTraceConfig;

            WIMEditorUtility.DisplaySettingsIfConfigNotNull(root, config, typeof(PathTraceConfiguration));

            var traceDuration = root.Q<FloatField>("trace-duration");
            traceDuration.SetDisplay(config && config.PostTravelPathTrace);
            root.Q<Toggle>("enabled").RegisterValueChangedCallback(e => traceDuration.SetDisplay(e.newValue));
            
            container.Add(root);
            if(config) root.Bind(new SerializedObject(config));
            root.Bind(new SerializedObject(pathTrace));
        }
    }
}