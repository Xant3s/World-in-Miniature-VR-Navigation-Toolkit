// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: UxmlNamespacePrefix("WIMVR", "WIM")]


namespace WIMVR.Editor.Util {
    /// <summary>
    /// Custom UIElement. A float slider similar to the IMGUI element.
    /// </summary>
    public class FloatSlider : VisualElement {
        private readonly Slider slider;

        public FloatSlider() {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIMVR/Scripts/Editor/Util/FloatSlider.uxml");
            var floatSlider = new VisualElement();
            visualTree.CloneTree(floatSlider);
            hierarchy.Add(floatSlider);
            slider = floatSlider.Q<Slider>();
            var floatField = floatSlider.Q<FloatField>();

            slider.schedule.Execute(() => floatField.SetValueWithoutNotify(slider.value));
            slider.RegisterValueChangedCallback(e => floatField.SetValueWithoutNotify(e.newValue));
            floatField.RegisterValueChangedCallback(e => {
                var newValue = Mathf.Clamp(floatField.value, slider.lowValue, slider.highValue);
                slider.value = newValue;
                floatField.SetValueWithoutNotify(newValue);
            });
        }

        public FloatSlider(float lowValue, float hightValue, string label = "", string bindingPath = "") : this() {
            Init(lowValue, hightValue, label, bindingPath);
        }

        public string Label {
            get => slider.label;
            set => slider.label = value;
        }

        public string BindingPath {
            get => slider.bindingPath;
            set => slider.bindingPath = value;
        }

        public float LowValue {
            get => slider.lowValue;
            set => slider.lowValue = value;
        }

        public float HightValue {
            get => slider.highValue;
            set => slider.highValue = value;
        }

        public void Init(float lowValue, float hightValue, string label = "", string bindingPath = "") {
            this.Label = label;
            this.BindingPath = bindingPath;
            this.LowValue = lowValue;
            this.HightValue = hightValue;
        }

        public new class UxmlFactory : UxmlFactory<FloatSlider, UxmlTraits> {}

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
                var label = m_label.GetValueFromBag(bag, cc);
                var lowValue = m_lowValue.GetValueFromBag(bag, cc);
                var highValue = m_highValue.GetValueFromBag(bag, cc);
                var bindingPath = m_bindingPath.GetValueFromBag(bag, cc);
                ((FloatSlider) ve).Init(lowValue, highValue, label, bindingPath);
            }
        }
    }
}