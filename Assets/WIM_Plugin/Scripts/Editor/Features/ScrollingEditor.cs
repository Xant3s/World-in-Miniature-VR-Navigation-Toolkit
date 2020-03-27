using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    [CustomEditor(typeof(Scrolling))]
    public class ScrollingEditor : Editor {

        private MiniatureModel WIM;

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 4);
            WIM = ((Scrolling) target).GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);
        }

        private void Draw(WIMConfiguration WIMConfig, VisualElement container) {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Scripts/Editor/Features/ScrollingEditor.uxml");
            var root = new VisualElement();
            if(visualTree) visualTree.CloneTree(root);
            var scrolling = (Scrolling) target;
            ref var config = ref scrolling.ScrollingConfig;

            WIMEditorUtility.DisplaySettingsIfConfigNotNull(root, config, typeof(ScrollingConfiguration));

            var scrollingSettings2 = root.Q<VisualElement>("scrolling-settings2");
            var allowVerticalScroll = root.Q<Toggle>("allow-vertical-scroll");

            root.Q<ObjectField>("configuration").RegisterValueChangedCallback(e =>
                allowVerticalScroll.SetDisplay(e.newValue && !((ScrollingConfiguration)e.newValue).AllowVerticalScrolling));

            scrollingSettings2.SetDisplay(config && config.AllowWIMScrolling);
            root.Q<Toggle>("allow-scrolling").RegisterValueChangedCallback(e => {
                scrollingSettings2.SetDisplay(e.newValue);
                root.schedule.Execute(() => WIMGenerator.ConfigureWIM(WIM));
            });

            root.Q<Vector3Field>("active-area-bounds").RegisterValueChangedCallback(e => scrolling.UpdateScrollingMask(WIM));

            var scrollSpeed = root.Q<FloatField>("scroll-speed");
            scrollSpeed.SetDisplay(config && config.AutoScroll);
            allowVerticalScroll.SetDisplay(config && !config.AllowVerticalScrolling);
            root.Q<Toggle>("auto-scroll").RegisterValueChangedCallback(e => {
                scrollSpeed.SetDisplay(e.newValue);
                allowVerticalScroll.SetDisplay(!e.newValue);
                if(!e.newValue && scrolling.ScrollingConfig) scrolling.ScrollingConfig.AllowVerticalScrolling = false;
            });

            container.Add(root);
            if(config) root.Bind(new SerializedObject(config));
            root.Bind(new SerializedObject(scrolling));
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var scrolling = (Scrolling) target;
            EditorGUI.BeginChangeCheck();
            scrolling.ScrollingConfig = (ScrollingConfiguration) EditorGUILayout.ObjectField("Config", scrolling.ScrollingConfig, typeof(ScrollingConfiguration), false);
            if(EditorGUI.EndChangeCheck()) {
                WIMGenerator.ConfigureWIM(WIM);
                scrolling.Remove();
                if(scrolling.ScrollingConfig) {
                    scrolling.Setup();
                }
            }
            if(!scrolling.ScrollingConfig)
                EditorGUILayout.HelpBox("Scrolling configuration missing. Create a scrolling configuration asset and add it to the scrolling script.", MessageType.Error);
        }
    }
}