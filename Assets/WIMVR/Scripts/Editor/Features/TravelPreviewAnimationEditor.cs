// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using WIMVR.Features.Travel_Preview_Animation;


namespace WIMVR.Editor.Features {
    /// <summary>
    /// Custom inspector.
    /// </summary>
    [CustomEditor(typeof(TravelPreviewAnimation))]
    public class TravelPreviewAnimationEditor : UnityEditor.Editor {
        private static bool initialized;

        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            ref var config = ref ((TravelPreviewAnimation)target).Config;
            if(config) return;
            EditorGUILayout.HelpBox("Travel preview animation configuration missing. Create a travel preview animation configuration asset and add it to the TravelPreviewAnimation script.", MessageType.Error);
        }

        private void OnEnable() {
            if(initialized) return;
            initialized = true;
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 0);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
            MiniatureModelEditor.UnregisterUniqueSeparator("Orientation Aids");
            initialized = false;
        }

        private void Draw(WIMConfiguration WIMConfig, VisualElement container) {
            MiniatureModelEditor.UniqueSeparator("Orientation Aids");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIMVR/Scripts/Editor/Features/TravelPreviewAnimationEditor.uxml");
            var root = new VisualElement();
            visualTree.CloneTree(root);
            var travelPreviewAnimation = (TravelPreviewAnimation) target;
            ref var config = ref travelPreviewAnimation.Config;

            WIMEditorUtility.DisplaySettingsIfConfigNotNull(root, config, typeof(TravelPreviewConfiguration));

            var animationSpeed = root.Q<FloatSlider>("animation-speed");
            animationSpeed.SetDisplay(config && config.TravelPreviewAnimation);
            root.Q<Toggle>("enabled").RegisterValueChangedCallback(e => animationSpeed.SetDisplay(e.newValue));

            container.Add(root);
            if(config) root.Bind(new SerializedObject(config));
            root.Bind(new SerializedObject(travelPreviewAnimation));
        }
    }
}