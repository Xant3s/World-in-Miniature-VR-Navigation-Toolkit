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
                    //PlayerPrefs.SetInt(mapper.actionMappings[i].PlayerPrefsKey, (int) mapper.actionMappings[i].Mapping);
                    //PlayerPrefs.Save();
                    mapper.InputMappings.Set(mapper.actionMappings[i].PlayerPrefsKey, (int)mapper.actionMappings[i].Mapping);
                }
            }
        }
    }
}