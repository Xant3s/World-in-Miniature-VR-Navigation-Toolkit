using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WIM_Plugin {
    [CustomEditor(typeof(OculusQuestMapper))]
    public class OculusQuestMapperEditor : Editor {
        public override void OnInspectorGUI() {
            var mapper = (OculusQuestMapper) target;
            base.OnInspectorGUI();

            if(InputManager.ButtonActions.Count != mapper.actionButtonMappings.Count
               || InputManager.AxisActions.Count != mapper.actionAxisMappings.Count) 
                mapper.UpdateActions();

            for(var i = 0; i < InputManager.ButtonActions.Count; i++) {
                EditorGUI.BeginChangeCheck();
                mapper.actionButtonMappings[i].Mapping =
                    (OVRInput.RawButton) EditorGUILayout.EnumFlagsField(mapper.actionButtonMappings[i].Name,
                        mapper.actionButtonMappings[i].Mapping);
                if(EditorGUI.EndChangeCheck()) {
                    mapper.InputMappings.Set(mapper.actionButtonMappings[i].MappingKey, (int)mapper.actionButtonMappings[i].Mapping);
                }
            }

            for (var i = 0; i < InputManager.AxisActions.Count; i++) {
                EditorGUI.BeginChangeCheck();
                mapper.actionAxisMappings[i].Mapping =
                    (OVRInput.RawAxis2D) EditorGUILayout.EnumFlagsField(mapper.actionAxisMappings[i].Name,
                        mapper.actionAxisMappings[i].Mapping);
                if (EditorGUI.EndChangeCheck()) {
                    mapper.InputMappings.Set(mapper.actionAxisMappings[i].MappingKey, (int)mapper.actionAxisMappings[i].Mapping);
                }
            }
            EditorUtility.SetDirty(mapper.InputMappings);
        }
    }
}