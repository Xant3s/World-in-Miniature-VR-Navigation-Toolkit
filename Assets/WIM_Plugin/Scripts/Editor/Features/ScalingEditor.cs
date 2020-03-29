// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    [CustomEditor(typeof(Scaling))]
    public class ScalingEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((Scaling) target).ScalingConfig;
            if(config) return;
            EditorGUILayout.HelpBox("Scaling configuration missing. Create a scaling configuration asset and add it to the scaling script.", MessageType.Error);
        }

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
        }

        private void Draw(WIMConfiguration WIMconfig, VisualElement container) {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Scripts/Editor/Features/ScalingEditor.uxml");
            if(!visualTree) return;
            var root = new VisualElement();
            if(visualTree) visualTree.CloneTree(root);
            var scaling = (Scaling) target;
            ref var config = ref scaling.ScalingConfig;

            WIMEditorUtility.DisplaySettingsIfConfigNotNull(root, config, typeof(ScalingConfiguration));

            var scalingSettings2 = root.Q<VisualElement>("scaling-settings2");
            scalingSettings2.SetDisplay(config && config.AllowWIMScaling);
            root.Q<Toggle>("allow-scaling").RegisterValueChangedCallback(e => scalingSettings2.SetDisplay(e.newValue));

            container.Add(root);
            if(config) root.Bind(new SerializedObject(config));
            root.Bind(new SerializedObject(scaling));
        }
    }
}