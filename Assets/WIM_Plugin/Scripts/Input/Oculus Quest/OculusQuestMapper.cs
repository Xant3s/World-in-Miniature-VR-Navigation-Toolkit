using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WIM_Plugin {
    [ExecuteAlways]
    public class OculusQuestMapper : MonoBehaviour {
        internal class InputButtonActionMapping {
            public delegate bool Trigger(OVRInput.RawButton btn, OVRInput.Controller controllerMask);

            public string Name { get; }
            public string MappingKey { get; }
            public InputManager.InputButtonAction ButtonAction { get; }
            public Trigger ButtonTrigger { get; }
            public OVRInput.RawButton Mapping { get; set; } = OVRInput.RawButton.None;


            public InputButtonActionMapping(string name, InputManager.InputButtonAction buttonAction,
                Trigger buttonTrigger,
                OculusQuestMapper mapper = null) {
                this.Name = name;
                this.ButtonAction = buttonAction;
                this.ButtonTrigger = buttonTrigger;
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
                var trigger = getTrigger(InputManager.ButtonTriggers[m.Key]);
                actionButtonMappings.Add(new InputButtonActionMapping(m.Key, m.Value, trigger, this));
            }

            foreach (var m in InputManager.AxisActions) {
                actionAxisMappings.Add(new InputAxisActionMapping(m.Key, m.Value, this));
            }
        }

        private void Start() {
            UpdateActions();
        }

        private void Update() {
            foreach (var actionMapping in actionButtonMappings.Where(m => m.ButtonTrigger(m.Mapping, OVRInput.Controller.Active))) {
                actionMapping.ButtonAction();
            }

            foreach (var actionMapping in actionAxisMappings) {
                actionMapping.AxisAction(OVRInput.Get(actionMapping.Mapping));
                if (actionMapping.Mapping != OVRInput.RawAxis2D.None) return;
            }
        }

        private InputButtonActionMapping.Trigger getTrigger(InputManager.ButtonTrigger trigger) {
            switch (trigger) {
                case InputManager.ButtonTrigger.ButtonDown:
                    return OVRInput.GetUp;
                default:
                    return OVRInput.GetUp;
            }
        }
    }
}