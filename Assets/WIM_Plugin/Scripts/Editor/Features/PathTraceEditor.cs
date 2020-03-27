using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    [CustomEditor(typeof(PathTrace))]
    public class PathTraceEditor : Editor {
        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 2);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
            MiniatureModelEditor.UnregisterUniqueSeparator("Orientation Aids");
        }

        private void Draw(WIMConfiguration WIMConfig, VisualElement container) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Scripts/Editor/Features/PathTraceEditor.uxml");
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

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((PathTrace) target).PathTraceConfig;
            if(config) return;
            EditorGUILayout.HelpBox("Path trace configuration missing. Create a path trace configuration asset and add it to the PathTrace script.", MessageType.Error);
        }
    }
}