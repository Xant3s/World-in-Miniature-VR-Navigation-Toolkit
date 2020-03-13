using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;


namespace WIM_Plugin {
    [CustomEditor(typeof(Scrolling))]
    public class ScrollingEditor : Editor {

        private MiniatureModel WIM;

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(draw, 4);
            WIM = ((Scrolling) target).GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(draw);
        }

        private void draw(WIMConfiguration WIMConfig, VisualElement container) {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Editor/Features/ScrollingEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIM_Plugin/Editor/Core/MiniatureModelEditor.uss");
            var root = new VisualElement();
            root.styleSheets.Add(styleSheet);
            if(visualTree) visualTree.CloneTree(root);
            var scrolling = (Scrolling) target;
            ref var config = ref scrolling.ScrollingConfig;

            root.Q<HelpBox>("config-info").SetDisplay(!config);
            var scrollingConfig = root.Q<ObjectField>("configuration");
            var scrollingSettings = root.Q<VisualElement>("scrolling-settings");
            var scrollingSettings2 = root.Q<VisualElement>("scrolling-settings2");
            var scrollSpeed = root.Q<FloatField>("scroll-speed");
            var allowVerticalScroll = root.Q<Toggle>("allow-vertical-scroll");
            scrollingSettings.SetDisplay(config);
            scrollingConfig.SetDisplay(!config);
            scrollingConfig.objectType = typeof(ScrollingConfiguration);
            scrollingConfig.RegisterValueChangedCallback(e => {
                root.Q<HelpBox>("config-info").SetDisplay(!e.newValue);
                scrollingConfig.SetDisplay(!e.newValue);
                scrollingSettings.SetDisplay(e.newValue);
                allowVerticalScroll.SetDisplay(e.newValue && !((ScrollingConfiguration)e.newValue).AllowVerticalScrolling);
                if (e.newValue) root.Bind(new SerializedObject(e.newValue));
            });

            scrollingSettings2.SetDisplay(config && config.AllowWIMScrolling);
            root.Q<Toggle>("allow-scrolling").RegisterValueChangedCallback(e => {
                scrollingSettings2.SetDisplay(e.newValue);
                root.schedule.Execute(() => WIMGenerator.ConfigureWIM(WIM));
            });

            root.Q<Vector3Field>("active-area-bounds").RegisterValueChangedCallback(e => scrolling.UpdateScrollingMask(WIM));

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
                scrolling.remove();
                if(scrolling.ScrollingConfig) {
                    scrolling.setup();
                }
            }
            if(!scrolling.ScrollingConfig)
                EditorGUILayout.HelpBox("Scrolling configuration missing. Create a scrolling configuration asset and add it to the scrolling script.", MessageType.Error);
        }
    }
}