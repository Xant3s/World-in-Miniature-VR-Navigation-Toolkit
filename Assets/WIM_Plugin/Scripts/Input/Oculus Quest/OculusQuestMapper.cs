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
            public List<Trigger> ButtonTriggers { get; } = new List<Trigger>();
            public OVRInput.RawButton Mapping { get; set; } = OVRInput.RawButton.None;


            public InputButtonActionMapping(string name, InputManager.InputButtonAction buttonAction,
                ICollection<Trigger> buttonTriggers,
                OculusQuestMapper mapper = null) {
                this.Name = name;
                this.ButtonAction = buttonAction;
                this.ButtonTriggers.AddRange(buttonTriggers);
                MappingKey = "WIMInput_OculusQuestMapper_" + name;
                if (mapper && mapper.InputMappings.HasKey(MappingKey)) {
                    this.Mapping = (OVRInput.RawButton) mapper.InputMappings.Get(MappingKey);
                }
            }

            public bool IsTriggered() {
                return ButtonTriggers.Any(trigger => trigger(Mapping, OVRInput.Controller.Active));
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
            foreach (var actionMapping in actionButtonMappings.Where(m => m.IsTriggered())) {
                actionMapping.ButtonAction();
            }

            foreach (var actionMapping in actionAxisMappings) {
                actionMapping.AxisAction(OVRInput.Get(actionMapping.Mapping));
                if (actionMapping.Mapping != OVRInput.RawAxis2D.None) return;
            }
        }

        private ICollection<InputButtonActionMapping.Trigger> getTrigger(InputManager.ButtonTrigger trigger) {
            switch (trigger) {
                case InputManager.ButtonTrigger.ButtonDown:
                    return new List<InputButtonActionMapping.Trigger>() {OVRInput.GetDown};
                case InputManager.ButtonTrigger.ButtonUpAndDown:
                    return new List<InputButtonActionMapping.Trigger>() {OVRInput.GetDown, OVRInput.GetUp};
                default:
                    return new List<InputButtonActionMapping.Trigger>() {OVRInput.GetUp};
            }
        }
    }
}