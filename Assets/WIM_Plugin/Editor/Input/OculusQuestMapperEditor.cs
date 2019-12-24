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

            if(InputManager.Actions.Count != mapper.actionMappings.Count) 
                mapper.UpdateActions();

            for(var i = 0; i < InputManager.Actions.Count; i++) {
                EditorGUI.BeginChangeCheck();
                mapper.actionMappings[i].Mapping =
                    (OVRInput.RawButton) EditorGUILayout.EnumFlagsField(mapper.actionMappings[i].Name,
                        mapper.actionMappings[i].Mapping);
                if(EditorGUI.EndChangeCheck()) {
                    mapper.InputMappings.Set(mapper.actionMappings[i].MappingKey, (int)mapper.actionMappings[i].Mapping);
                }
            }
        }
    }
}