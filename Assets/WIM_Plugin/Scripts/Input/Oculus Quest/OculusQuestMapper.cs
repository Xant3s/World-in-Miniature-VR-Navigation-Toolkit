﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WIM_Plugin {
    [ExecuteAlways]
    [DisallowMultipleComponent]
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

        internal class InputButtonTouchActionMapping {
            public delegate bool Trigger(OVRInput.RawTouch btn, OVRInput.Controller controllerMask);

            public string Name { get; }

            public string MappingKey { get; }
            public Dictionary<Trigger, InputManager.InputButtonTouchAction> ButtonActions { get; }
            public OVRInput.RawTouch Mapping { get; set; } = OVRInput.RawTouch.None;


            public InputButtonTouchActionMapping(string name,
                Dictionary<Trigger, InputManager.InputButtonTouchAction> buttonActions,
                OculusQuestMapper mapper = null) {
                this.Name = name;
                this.ButtonActions = buttonActions;
                MappingKey = "WIMInput_OculusQuestMapper_" + name;
                if (mapper && mapper.InputMappings.HasKey(MappingKey)) {
                    this.Mapping = (OVRInput.RawTouch) mapper.InputMappings.Get(MappingKey);
                }
            }
        }

        internal class InputAxisActionMapping {
            public string Name { get; }
            public string MappingKey { get; }
            public bool Axis1D { get; }
            public InputManager.InputAxis3DAction AxisAction { get; }
            public InputManager.InputAxis1DAction Axis1DAction { get; }
            public OVRInput.RawAxis2D Mapping { get; set; } = OVRInput.RawAxis2D.None;
            public OVRInput.RawAxis1D Mapping1D { get; set; } = OVRInput.RawAxis1D.None;

            public InputAxisActionMapping(string name, InputManager.InputAxis3DAction axisAction,
                OculusQuestMapper mapper = null) {
                this.Name = name;
                this.Axis1D = false;
                this.AxisAction = axisAction;
                MappingKey = "WIMInput_OculusQuestMapper_" + name;
                if (mapper && mapper.InputMappings.HasKey(MappingKey)) {
                    this.Mapping = (OVRInput.RawAxis2D)mapper.InputMappings.Get(MappingKey);
                }
            }

            public InputAxisActionMapping(string name, InputManager.InputAxis1DAction axisAction,
                OculusQuestMapper mapper = null) {
                this.Name = name;
                this.Axis1D = true;
                this.Axis1DAction = axisAction;
                MappingKey = "WIMInput_OculusQuestMapper_" + name;
                if (mapper && mapper.InputMappings.HasKey(MappingKey)) {
                    this.Mapping1D = (OVRInput.RawAxis1D) mapper.InputMappings.Get(MappingKey);
                }
                int newMapping = 0;
                switch ((int) this.Mapping1D) {
                    case (int) OVRInput.RawButton.RIndexTrigger:
                        newMapping = (int) OVRInput.RawAxis1D.RIndexTrigger;
                        break;
                    case (int) OVRInput.RawButton.RHandTrigger:
                        newMapping = (int) OVRInput.RawAxis1D.RHandTrigger;
                        break;
                    case (int) OVRInput.RawButton.LIndexTrigger:
                        newMapping = (int) OVRInput.RawAxis1D.LIndexTrigger;
                        break;
                    case (int) OVRInput.RawButton.LHandTrigger:
                        newMapping = (int) OVRInput.RawAxis1D.LHandTrigger;
                        break;
                }

                this.Mapping1D = (OVRInput.RawAxis1D) newMapping;
            }
        }

        internal List<InputButtonActionMapping> actionButtonMappings = new List<InputButtonActionMapping>();
        internal List<InputButtonTouchActionMapping> actionButtonTouchMappings = new List<InputButtonTouchActionMapping>();
        internal List<InputAxisActionMapping> actionAxisMappings = new List<InputAxisActionMapping>();
        public InputMapping InputMappings;


        private void OnEnable() {
            InputManager.OnUpdateActions += UpdateActions;
            InputManager.OnSetVibration += SetVibration;
            UpdateActions();
        }

        private void OnDisable() {
            InputManager.OnUpdateActions -= UpdateActions;
            InputManager.OnSetVibration -= SetVibration;
        }

        public void UpdateActions() {
            actionButtonMappings.Clear();
            actionAxisMappings.Clear();
            foreach (var m in InputManager.ButtonActions) {
                actionButtonMappings.Add(new InputButtonActionMapping(m.Key, convertTriggers(m.Value), this));
            }

            foreach (var m in InputManager.ButtonTouchActions) {
                actionButtonTouchMappings.Add(new InputButtonTouchActionMapping(m.Key, convertTriggers(m.Value), this));
            }

            foreach (var m in InputManager.AxisActions) {
                actionAxisMappings.Add(new InputAxisActionMapping(m.Key, m.Value, this));
            }

            foreach (var m in InputManager.Axis1DActions) {
                actionAxisMappings.Add(new InputAxisActionMapping(m.Key, m.Value, this));
            }
        }

        public void SetVibration(float frequency, float amplitude, Hand hand) {
            OVRInput.Controller controller;
            switch (hand) {
                case Hand.LeftHand:
                    controller = OVRInput.Controller.LTouch;
                    break;
                case Hand.RightHand:
                    controller = OVRInput.Controller.RTouch;
                    break;
                default:
                    controller = OVRInput.Controller.None;
                    break;
            }

            OVRInput.SetControllerVibration(frequency, amplitude, controller);
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

            foreach (var actionMapping in actionButtonTouchMappings) {
                foreach (var buttonAction in actionMapping.ButtonActions) {
                    if (buttonAction.Key(actionMapping.Mapping, OVRInput.Controller.Active))
                        buttonAction.Value();
                }
            }

            foreach (var actionMapping in actionAxisMappings) {
                if (actionMapping.Axis1D) {
                    actionMapping.Axis1DAction(OVRInput.Get(actionMapping.Mapping1D));
                }
                else {
                    actionMapping.AxisAction(OVRInput.Get(actionMapping.Mapping));
                }
            }
        }

        private InputButtonActionMapping.Trigger GetTrigger(InputManager.ButtonTrigger trigger) {
            switch (trigger) {
                case InputManager.ButtonTrigger.ButtonDown:
                    return OVRInput.GetDown;
                case InputManager.ButtonTrigger.ButtonGet:
                    return OVRInput.Get;
                default:
                    return OVRInput.GetUp;
            }
        }

        private InputButtonTouchActionMapping.Trigger GetTouchTrigger(InputManager.ButtonTrigger trigger) {
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
            return dict.ToDictionary(entry => GetTrigger(entry.Key), entry => entry.Value);
        }

        private Dictionary<InputButtonTouchActionMapping.Trigger, InputManager.InputButtonTouchAction>
            convertTriggers(Dictionary<InputManager.ButtonTrigger, InputManager.InputButtonTouchAction> dict) {
            return dict.ToDictionary(entry => GetTouchTrigger(entry.Key), entry => entry.Value);
        }
    }
}