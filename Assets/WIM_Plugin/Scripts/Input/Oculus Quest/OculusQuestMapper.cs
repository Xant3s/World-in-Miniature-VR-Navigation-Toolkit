using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WIM_Plugin {
    [ExecuteAlways]
    public class OculusQuestMapper : MonoBehaviour {
        internal class InputActionMapping {
            public string Name { get; }
            public string PlayerPrefsKey { get; }
            public InputManager.InputAction Action { get; }
            public OVRInput.RawButton Mapping { get; set; } = OVRInput.RawButton.None;

            public InputActionMapping(string name, InputManager.InputAction action, OculusQuestMapper mapper = null) {
                this.Name = name;
                this.Action = action;
                PlayerPrefsKey = "WIMInput_OculusQuestMapper_" + name;
                //if (PlayerPrefs.HasKey(PlayerPrefsKey)) {
                //    this.Mapping = (OVRInput.RawButton)PlayerPrefs.GetInt(PlayerPrefsKey);
                //    if (Application.isPlaying)
                //    {
                //        GameObject.Find("VisualDebug").GetComponent<VisualDebug>().Test();
                //    }
                //} 
                //if (mapper && mapper.InputMappings.HasKey(PlayerPrefsKey)) {
                //    this.Mapping = (OVRInput.RawButton)PlayerPrefs.GetInt(PlayerPrefsKey);
                //    if (Application.isPlaying)
                //    {
                //        GameObject.Find("VisualDebug").GetComponent<VisualDebug>().Test();
                //    }
                //}
                if (mapper && mapper.InputMappings.HasKey(PlayerPrefsKey)) { 
                    this.Mapping = (OVRInput.RawButton) mapper.InputMappings.Get(PlayerPrefsKey);
                    if (Application.isPlaying)
                    {
                        GameObject.Find("VisualDebug").GetComponent<VisualDebug>().Test();
                    }
                }
            }
        }

        // TODO: what about axis etc?

        internal List<InputActionMapping> actionMappings = new List<InputActionMapping>();
        public InputMapping InputMappings;


        private void OnEnable() {
            InputManager.OnUpdateActions -= UpdateActions;
            UpdateActions();
        }

        private void OnDisable() {
            InputManager.OnUpdateActions += UpdateActions;
        }

        public void UpdateActions() {
            actionMappings.Clear();
            foreach(var m in InputManager.Actions) {
                actionMappings.Add(new InputActionMapping(m.Key, m.Value, this));
            }
        }

        private void Start() {
            UpdateActions();
        }

        private void Update() {
            if (actionMappings.Count == 0) return;

            foreach(var actionMapping in actionMappings.Where(m => OVRInput.GetUp(m.Mapping))) {
                actionMapping.Action();
            }

            //if(Input.GetKeyUp(KeyCode.K))
            //    actionMappings[0].Action();

            //if (OVRInput.GetUp(OVRInput.RawButton.X)) {
            //        actionMappings[0].Action();
           
            //}

            //if (actionMappings[0].Mapping != OVRInput.RawButton.None) {
            //    if (Application.isPlaying)
            //    {
            //        GameObject.Find("VisualDebug").GetComponent<VisualDebug>().Test();
            //    }
            //}

            //if (OVRInput.GetUp(actionMappings[0].Mapping)) {
            //    actionMappings[0].Action();
            //}
        }
    }
}