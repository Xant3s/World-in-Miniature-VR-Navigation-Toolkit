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

            public InputActionMapping(string name, InputManager.InputAction action) {
                this.Name = name;
                this.Action = action;
                PlayerPrefsKey = "WIMInput_OculusQuestMapper_" + name;
                if(PlayerPrefs.HasKey(PlayerPrefsKey))
                    this.Mapping = (OVRInput.RawButton) PlayerPrefs.GetInt(PlayerPrefsKey);
            }
        }

        // TODO: what about axis etc?

        internal List<InputActionMapping> actionMappings = new List<InputActionMapping>();

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
                actionMappings.Add(new InputActionMapping(m.Key, m.Value));
            }
        }

        private void Start() {
            UpdateActions();
        }

        private void Update() {
            foreach(var actionMapping in actionMappings.Where(m => OVRInput.GetUp(m.Mapping))) {
                actionMapping.Action();
            }

            if(Input.GetKeyUp(KeyCode.K))
                actionMappings[0].Action();

            if(OVRInput.GetUp(actionMappings[0].Mapping))
                actionMappings[0].Action();
        }
    }
}