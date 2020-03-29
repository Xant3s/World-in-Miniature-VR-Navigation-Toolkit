﻿// Author: Samuel Truman (contact@samueltruman.com)

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
            UxmlStringAttributeDescription m_text = new UxmlStringAttributeDescription {
                name = "text", defaultValue = ""
            };

            private UxmlEnumAttributeDescription<MessageType> m_messageType = new UxmlEnumAttributeDescription<MessageType> {
                name = "message-type", defaultValue = MessageType.Info
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get { yield break; }    // No children.
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var text = m_text.GetValueFromBag(bag, cc);
                var messageType = m_messageType.GetValueFromBag(bag, cc);
                ((HelpBox) ve).Init(text, messageType);
            }
        }

        public HelpBox() {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WIM_Plugin/Scripts/Editor/Util/HelpBox.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WIM_Plugin/Scripts/Editor/Util/HelpBox.uss");
            var helpBox = new VisualElement();
            helpBox.styleSheets.Add(styleSheet);
            visualTree.CloneTree(helpBox);
            hierarchy.Add(helpBox);
            textElement = helpBox.Q<TextElement>();
            image = helpBox.Q<Image>();
            this.Type = MessageType.Info;
        }

        public HelpBox(string text, MessageType messageType = MessageType.Info): this() {
            Init(text, messageType);
        }

        public void Init(string text, MessageType messageType = MessageType.Info) {
            this.Text = text;
            this.Type = messageType;
            image.scaleMode = ScaleMode.ScaleToFit;
        }

        public string Text {
            get => textElement.text;
            set => textElement.text = value;
        }

        public MessageType Type {
            get => type;
            set {
                type = value;
                switch (type) {
                    case MessageType.Error:
                        image.image = EditorGUIUtility.FindTexture("d_console.erroricon");
                        break;
                    case MessageType.Warning:
                        image.image = EditorGUIUtility.FindTexture("d_console.warnicon");
                        break;
                    default:
                        image.image = EditorGUIUtility.FindTexture("d_console.infoicon");
                        break;
                }
            }
        }

        private readonly TextElement textElement;
        private readonly Image image;
        private MessageType type;
    }
}