using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: UxmlNamespacePrefix("WIM_Plugin", "WIM")]


namespace WIM_Plugin {
    public class HelpBoxTest : VisualElement {
        public new class UxmlFactory : UxmlFactory<HelpBoxTest, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits {
            UxmlStringAttributeDescription m_mytext = new UxmlStringAttributeDescription {
                name = "text", defaultValue = ""
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get { yield break; }    // No children.
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var ate = ve as HelpBoxTest;
                ate.Clear();
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Editor/Util/HelpBox.uxml");
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIM_Plugin/Editor/Util/HelpBox.uss");
                ate.styleSheets.Add(styleSheet);
                ate.text = m_mytext.GetValueFromBag(bag, cc);
                visualTree.CloneTree(ate);
                ate.Q<TextElement>().text = ate.text;
                var img = ate.Q<Image>();
                img.image = EditorGUIUtility.FindTexture("d_console.erroricon");
                img.scaleMode = ScaleMode.ScaleToFit;
            }
        }

        //public HelpBoxTest() { }

        //public HelpBoxTest(string text) {
        //    this.text = text;
        //}


        public string text { get; set; }
    }
}