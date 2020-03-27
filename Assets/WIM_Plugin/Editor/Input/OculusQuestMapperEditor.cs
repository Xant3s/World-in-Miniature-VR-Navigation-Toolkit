using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace WIM_Plugin {
    [CustomEditor(typeof(OculusQuestMapper))]
    public class OculusQuestMapperEditor : Editor {
        private OculusQuestMapper mapper;
        private VisualElement root;
        private VisualTreeAsset visualTree;


        public void OnEnable() {
            mapper = (OculusQuestMapper) target;
            root = new VisualElement();
        }

        public override VisualElement CreateInspectorGUI() {
            root.Add(new ObjectField("Input Mapping") {
                allowSceneObjects = false,
                objectType = typeof(InputMapping),
                bindingPath = nameof(mapper.InputMappings)
            });
            root.Bind(new SerializedObject(mapper));

            if(InputManager.ButtonActions.Count != mapper.actionButtonMappings.Count
               || InputManager.AxisActions.Count != mapper.actionAxisMappings.Count)
                mapper.UpdateActions();

            Action action = () => {
                for(var i = 0; i < InputManager.ButtonActions.Count; i++) {
                    EditorGUI.BeginChangeCheck();
                    mapper.actionButtonMappings[i].Mapping =
                        (OVRInput.RawButton) EditorGUILayout.EnumFlagsField(mapper.actionButtonMappings[i].Name,
                            mapper.actionButtonMappings[i].Mapping);
                    if(EditorGUI.EndChangeCheck()) {
                        mapper.InputMappings.Set(mapper.actionButtonMappings[i].MappingKey,
                            (int) mapper.actionButtonMappings[i].Mapping);
                    }
                }

                for(var i = 0; i < InputManager.ButtonTouchActions.Count; i++) {
                    EditorGUI.BeginChangeCheck();
                    mapper.actionButtonTouchMappings[i].Mapping =
                        (OVRInput.RawTouch) EditorGUILayout.EnumFlagsField(mapper.actionButtonTouchMappings[i].Name,
                            mapper.actionButtonTouchMappings[i].Mapping);
                    if(EditorGUI.EndChangeCheck()) {
                        mapper.InputMappings.Set(mapper.actionButtonTouchMappings[i].MappingKey,
                            (int) mapper.actionButtonTouchMappings[i].Mapping);
                    }
                }

                for(var i = 0; i < InputManager.AxisActions.Count; i++) {
                    EditorGUI.BeginChangeCheck();
                    mapper.actionAxisMappings[i].Mapping =
                        (OVRInput.RawAxis2D) EditorGUILayout.EnumFlagsField(mapper.actionAxisMappings[i].Name,
                            mapper.actionAxisMappings[i].Mapping);
                    if(EditorGUI.EndChangeCheck()) {
                        mapper.InputMappings.Set(mapper.actionAxisMappings[i].MappingKey,
                            (int) mapper.actionAxisMappings[i].Mapping);
                    }
                }
            };
            root.Add(new IMGUIContainer(action));

            return root;
        }
    }
}