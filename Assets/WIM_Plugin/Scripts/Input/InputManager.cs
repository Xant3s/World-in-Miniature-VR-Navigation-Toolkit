using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    public static class InputManager {
        public enum ButtonTrigger { ButtonUp, ButtonDown, ButtonGet}

        public delegate void VibrationAction(float frequency, float amplitude, Hand hand);

        public delegate void InputMappers();

        public delegate void InputButtonAction();

        public delegate void InputButtonTouchAction();

        public delegate void InputAxis3DAction(Vector3 axis);

        public delegate void InputAxis1DAction(float axis);

        public static event InputMappers OnUpdateActions;

        public static event VibrationAction OnSetVibration;

        public static Dictionary<string, Dictionary<ButtonTrigger, InputButtonAction>> ButtonActions =
            new Dictionary<string, Dictionary<ButtonTrigger, InputButtonAction>>();

        public static Dictionary<string, Dictionary<ButtonTrigger, InputButtonTouchAction>> ButtonTouchActions =
            new Dictionary<string, Dictionary<ButtonTrigger, InputButtonTouchAction>>();

        public static Dictionary<string, InputAxis3DAction> AxisActions = new Dictionary<string, InputAxis3DAction>();

        public static Dictionary<string, InputAxis1DAction> Axis1DActions = new Dictionary<string, InputAxis1DAction>();

        public static void RegisterAction(string name, InputButtonAction buttonAction, ButtonTrigger trigger = ButtonTrigger.ButtonUp) {
            if (!ButtonActions.ContainsKey(name)) {
                ButtonActions[name] = new Dictionary<ButtonTrigger, InputButtonAction>();
            }
            ButtonActions[name].Add(trigger, buttonAction);
            OnUpdateActions?.Invoke();
        }

        public static void RegisterTouchAction(string name, InputButtonTouchAction buttonAction, ButtonTrigger trigger = ButtonTrigger.ButtonUp) {
            if (!ButtonTouchActions.ContainsKey(name)) {
                ButtonTouchActions[name] = new Dictionary<ButtonTrigger, InputButtonTouchAction>();
            }
            ButtonTouchActions[name].Add(trigger, buttonAction);
            OnUpdateActions?.Invoke();
        }

        public static void RegisterAction(string name, InputAxis3DAction action) {
            AxisActions[name] = action;
            OnUpdateActions?.Invoke();
        }

        public static void RegisterAction(string name, InputAxis1DAction action) {
            Axis1DActions[name] = action;
            OnUpdateActions?.Invoke();
        }

        public static void UnregisterAction(string name) {
            ButtonActions.Remove(name);
            ButtonTouchActions.Remove(name);
            AxisActions.Remove(name);
            Axis1DActions.Remove(name);
            OnUpdateActions?.Invoke();
        }

        public static void SetVibration(float frequency, float amplitude, Hand hand) {
            OnSetVibration?.Invoke(frequency, amplitude, hand);
        }
    }
}