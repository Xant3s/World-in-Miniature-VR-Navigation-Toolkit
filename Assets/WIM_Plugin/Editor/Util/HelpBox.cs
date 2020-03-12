using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: UxmlNamespacePrefix("WIM_Plugin", "WIM")]


namespace WIM_Plugin {
    public class HelpBox : VisualElement {
        public enum MessageType {
            Info, Warning, Error
        }

        public new class UxmlFactory : UxmlFactory<HelpBox, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits {
            UxmlStringAttributeDescription m_mytext = new UxmlStringAttributeDescription {
                name = "text", defaultValue = ""
            };

            private UxmlEnumAttributeDescription<MessageType> m_messageType = new UxmlEnumAttributeDescription<MessageType> {
                name = "messageType", defaultValue = MessageType.Info
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get { yield break; }    // No children.
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var ate = ve as HelpBox;
                ate.Clear();
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Editor/Util/HelpBox.uxml");
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIM_Plugin/Editor/Util/HelpBox.uss");
                ate.styleSheets.Add(styleSheet);
                ate.text = m_mytext.GetValueFromBag(bag, cc);
                visualTree.CloneTree(ate);
                ate.Q<TextElement>().text = ate.text;
                var img = ate.Q<Image>();
                ate.messageType = m_messageType.GetValueFromBag(bag, cc);
                switch(ate.messageType) {
                    case MessageType.Error:
                        img.image = EditorGUIUtility.FindTexture("d_console.erroricon");
                        break;
                    case MessageType.Warning:
                        img.image = EditorGUIUtility.FindTexture("d_console.warnicon");
                        break;
                    default:
                        img.image = EditorGUIUtility.FindTexture("d_console.infoicon");
                        break;
                }
                img.scaleMode = ScaleMode.ScaleToFit;
            }
        }

        public string text { get; set; }
        public MessageType messageType { get; set; }
    }
}