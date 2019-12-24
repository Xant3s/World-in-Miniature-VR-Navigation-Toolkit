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
            public string MappingKey { get; }
            public InputManager.InputAction Action { get; }
            public OVRInput.RawButton Mapping { get; set; } = OVRInput.RawButton.None;

            public InputActionMapping(string name, InputManager.InputAction action, OculusQuestMapper mapper = null) {
                this.Name = name;
                this.Action = action;
                MappingKey = "WIMInput_OculusQuestMapper_" + name;
                if (mapper && mapper.InputMappings.HasKey(MappingKey)) { 
                    this.Mapping = (OVRInput.RawButton) mapper.InputMappings.Get(MappingKey);
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
        }
    }
}