using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    public static class InputManager {
        public enum ButtonTrigger { ButtonUp, ButtonDown, ButtonGet}

        public delegate void InputMappers();

        public delegate void InputButtonAction();

        public delegate void InputAxis3DAction(Vector3 axis);

        public static event InputMappers OnUpdateActions;

        public static Dictionary<string, Dictionary<ButtonTrigger, InputButtonAction>> ButtonActions =
            new Dictionary<string, Dictionary<ButtonTrigger, InputButtonAction>>();


        public static Dictionary<string, InputAxis3DAction> AxisActions = new Dictionary<string, InputAxis3DAction>();

        public static void RegisterAction(string name, InputButtonAction buttonAction, ButtonTrigger trigger = ButtonTrigger.ButtonUp) {
            if (!ButtonActions.ContainsKey(name)) {
                ButtonActions[name] = new Dictionary<ButtonTrigger, InputButtonAction>();
            }
            ButtonActions[name].Add(trigger, buttonAction);
            OnUpdateActions?.Invoke();
        }

        public static void RegisterAction(string name, InputAxis3DAction action) {
            AxisActions[name] = action;
            OnUpdateActions?.Invoke();
        }

        public static void UnregisterAction(string name) {
            ButtonActions.Remove(name);
            AxisActions.Remove(name);
            OnUpdateActions?.Invoke();
        }
    }
}