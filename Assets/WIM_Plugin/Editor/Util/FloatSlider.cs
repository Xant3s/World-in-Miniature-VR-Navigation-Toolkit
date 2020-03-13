using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[assembly: UxmlNamespacePrefix("WIM_Plugin", "WIM")]


namespace WIM_Plugin {
    public class FloatSlider : VisualElement {
        public new class UxmlFactory : UxmlFactory<FloatSlider, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits {
            UxmlStringAttributeDescription m_label = new UxmlStringAttributeDescription {
                name = "label", defaultValue = ""
            };        
            UxmlStringAttributeDescription m_bindingPath = new UxmlStringAttributeDescription {
                name = "binding-path", defaultValue = ""
            };

            private UxmlFloatAttributeDescription m_lowValue = new UxmlFloatAttributeDescription {
                name = "low-value", defaultValue = 0
            };    

            private UxmlFloatAttributeDescription m_highValue = new UxmlFloatAttributeDescription {
                name = "high-value", defaultValue = 1
            };


            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get { yield break; } // No children.
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var fs = ve as FloatSlider;
                fs.Clear();
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Editor/Util/FloatSlider.uxml");
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIM_Plugin/Editor/Util/FloatSlider.uss");
                fs.styleSheets.Add(styleSheet);
                visualTree.CloneTree(fs);
                var slider = fs.Q<Slider>();
                var floatField = fs.Q<FloatField>();
                slider.label = fs.label = m_label.GetValueFromBag(bag, cc);
                slider.lowValue = fs.lowValue = m_lowValue.GetValueFromBag(bag, cc);
                slider.highValue = fs.hightValue = m_highValue.GetValueFromBag(bag, cc);
                slider.bindingPath = fs.bindingPath = m_bindingPath.GetValueFromBag(bag, cc);
                slider.schedule.Execute(() => floatField.SetValueWithoutNotify(slider.value));
                slider.RegisterValueChangedCallback(e => floatField.SetValueWithoutNotify(e.newValue));
                floatField.RegisterValueChangedCallback(e => {
                    var newValue = Mathf.Clamp(floatField.value, fs.lowValue, fs.hightValue);
                    slider.value = newValue;
                    floatField.SetValueWithoutNotify(newValue);
                });
            }
        }



        public string label { get; set; }
        public string bindingPath { get; set; }
        public float lowValue { get; set; }
        public float hightValue { get; set; }
    }
}