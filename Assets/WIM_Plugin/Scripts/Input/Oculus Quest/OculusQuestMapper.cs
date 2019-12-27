using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace WIM_Plugin {
    [ExecuteAlways]
    public class OculusQuestMapper : MonoBehaviour {
        internal class InputButtonActionMapping {
            public delegate bool Trigger(OVRInput.RawButton btn, OVRInput.Controller controllerMask);

            public string Name { get; }

            public string MappingKey { get; }
            public Dictionary<Trigger, InputManager.InputButtonAction> ButtonActions { get; }
            public OVRInput.RawButton Mapping { get; set; } = OVRInput.RawButton.None;


            public InputButtonActionMapping(string name,
                Dictionary<Trigger, InputManager.InputButtonAction> buttonActions,
                OculusQuestMapper mapper = null) {
                this.Name = name;
                this.ButtonActions = buttonActions;
                MappingKey = "WIMInput_OculusQuestMapper_" + name;
                if (mapper && mapper.InputMappings.HasKey(MappingKey)) {
                    this.Mapping = (OVRInput.RawButton) mapper.InputMappings.Get(MappingKey);
                }
            }
        }

        internal class InputAxisActionMapping {
            public string Name { get; }
            public string MappingKey { get; }
            public InputManager.InputAxis3DAction AxisAction { get; }
            public OVRInput.RawAxis2D Mapping { get; set; } = OVRInput.RawAxis2D.None;

            public InputAxisActionMapping(string name, InputManager.InputAxis3DAction axisAction,
                OculusQuestMapper mapper = null) {
                this.Name = name;
                this.AxisAction = axisAction;
                MappingKey = "WIMInput_OculusQuestMapper_" + name;
                if (mapper && mapper.InputMappings.HasKey(MappingKey)) {
                    this.Mapping = (OVRInput.RawAxis2D) mapper.InputMappings.Get(MappingKey);
                }
            }
        }

        internal List<InputButtonActionMapping> actionButtonMappings = new List<InputButtonActionMapping>();
        internal List<InputAxisActionMapping> actionAxisMappings = new List<InputAxisActionMapping>();
        public InputMapping InputMappings;


        private void OnEnable() {
            InputManager.OnUpdateActions -= UpdateActions;
            UpdateActions();
        }

        private void OnDisable() {
            InputManager.OnUpdateActions += UpdateActions;
        }

        public void UpdateActions() {
            actionButtonMappings.Clear();
            actionAxisMappings.Clear();
            foreach (var m in InputManager.ButtonActions) {
                actionButtonMappings.Add(new InputButtonActionMapping(m.Key, convertTriggers(m.Value), this));
            }

            foreach (var m in InputManager.AxisActions) {
                actionAxisMappings.Add(new InputAxisActionMapping(m.Key, m.Value, this));
            }
        }

        private void Start() {
            UpdateActions();
        }

        private void Update() {
            foreach (var actionMapping in actionButtonMappings) {
                foreach (var buttonAction in actionMapping.ButtonActions) {
                    if (buttonAction.Key(actionMapping.Mapping, OVRInput.Controller.Active))
                        buttonAction.Value();
                }
            }

            foreach (var actionMapping in actionAxisMappings) {
                actionMapping.AxisAction(OVRInput.Get(actionMapping.Mapping));
            }
        }

        private InputButtonActionMapping.Trigger getTrigger(InputManager.ButtonTrigger trigger) {
            switch (trigger) {
                case InputManager.ButtonTrigger.ButtonDown:
                    return OVRInput.GetDown;
                case InputManager.ButtonTrigger.ButtonGet:
                    return OVRInput.Get;
                default:
                    return OVRInput.GetUp;
            }
        }

        private Dictionary<InputButtonActionMapping.Trigger, InputManager.InputButtonAction>
            convertTriggers(Dictionary<InputManager.ButtonTrigger, InputManager.InputButtonAction> dict) {
            return dict.ToDictionary(entry => getTrigger(entry.Key), entry => entry.Value);
        }
    }
}