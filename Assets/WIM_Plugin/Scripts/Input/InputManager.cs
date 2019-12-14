using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    public static class InputManager {

        public delegate void InputMappers();

        public delegate void InputAction();

        public static event InputMappers OnUpdateActions;

        public static Dictionary<string, InputAction> Actions = new Dictionary<string, InputAction>();

        public static void RegisterAction(string name, InputAction action) {
            Actions[name] = action;
            OnUpdateActions?.Invoke();
        }

        public static void UnregisterAction(string name) {
            Actions.Remove(name);
            OnUpdateActions?.Invoke();
        }
    }
}