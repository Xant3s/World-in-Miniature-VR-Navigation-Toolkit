// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using UnityEngine;

namespace WIMVR {
    /// <summary>
    /// A platform independent input manager.
    /// For each supported platform, a platform specific input mapper has to be provided.
    /// Will be eventually replaced by the new Unity input manager.
    /// </summary>
    public static class InputManager {
        public delegate void InputAxis1DAction(float axis);

        public delegate void InputAxis3DAction(Vector3 axis);

        public delegate void InputButtonAction();

        public delegate void InputButtonTouchAction();

        public delegate void InputMappers();

        public delegate void VibrationAction(float frequency, float amplitude, Hand hand);

        public enum ButtonTrigger { ButtonUp, ButtonDown, ButtonGet}

        public static Dictionary<string, Dictionary<ButtonTrigger, InputButtonAction>> ButtonActions =
            new Dictionary<string, Dictionary<ButtonTrigger, InputButtonAction>>();

        public static Dictionary<string, Dictionary<ButtonTrigger, InputButtonTouchAction>> ButtonTouchActions =
            new Dictionary<string, Dictionary<ButtonTrigger, InputButtonTouchAction>>();

        public static Dictionary<string, InputAxis3DAction> AxisActions = new Dictionary<string, InputAxis3DAction>();

        public static Dictionary<string, InputAxis1DAction> Axis1DActions = new Dictionary<string, InputAxis1DAction>();

        public static Dictionary<string ,string> Tooltips = new Dictionary<string, string>();

        public static event InputMappers OnUpdateActions;

        public static event VibrationAction OnSetVibration;

        /// <summary>
        /// Registers a new button action.
        /// </summary>
        /// <param name="name">The unique action identifier. This will also be the name displayed in UI.</param>
        /// <param name="buttonAction">The action to execute when triggered.</param>
        /// <param name="trigger">Specifies what triggers the action.</param>
        /// <param name="tooltip">Tooltip to display in UI.</param>
        public static void RegisterAction(string name, InputButtonAction buttonAction,
            ButtonTrigger trigger = ButtonTrigger.ButtonUp,
            string tooltip = "") {
            if (!ButtonActions.ContainsKey(name)) {
                ButtonActions[name] = new Dictionary<ButtonTrigger, InputButtonAction>();
            }
            ButtonActions[name].Add(trigger, buttonAction);
            if(!Tooltips.ContainsKey(name)) Tooltips.Add(name, tooltip);
            OnUpdateActions?.Invoke();
        }

        /// <summary>
        /// Registers a new touch action.
        /// </summary>
        /// <param name="name">The unique action identifier. This will also be the name displayed in UI.</param>
        /// <param name="buttonAction">The action to execute when triggered.</param>
        /// <param name="trigger">Specifies what triggers the action.</param>
        /// <param name="tooltip">Tooltip to display in UI.</param>
        public static void RegisterTouchAction(string name, InputButtonTouchAction buttonAction,
            ButtonTrigger trigger = ButtonTrigger.ButtonUp, string tooltip = "") {
            if (!ButtonTouchActions.ContainsKey(name)) {
                ButtonTouchActions[name] = new Dictionary<ButtonTrigger, InputButtonTouchAction>();
            }
            ButtonTouchActions[name].Add(trigger, buttonAction);
            if(!Tooltips.ContainsKey(name)) Tooltips.Add(name, tooltip);
            OnUpdateActions?.Invoke();
        }

        /// <summary>
        /// Registers a new axis3D action.
        /// </summary>
        /// <param name="name">The unique action identifier. This will also be the name displayed in UI.</param>
        /// <param name="action">The action to execute when triggered.</param>
        /// <param name="tooltip">Tooltip to display in UI.</param>
        public static void RegisterAction(string name, InputAxis3DAction action, string tooltip = "") {
            AxisActions[name] = action;
            if(!Tooltips.ContainsKey(name)) Tooltips.Add(name, tooltip);
            OnUpdateActions?.Invoke();
        }

        /// <summary>
        /// Registers a new axis1D action.
        /// </summary>
        /// <param name="name">The unique action identifier. This will also be the name displayed in UI.</param>
        /// <param name="action">The action to execute when triggered.</param>
        /// <param name="tooltip">Tooltip to display in UI.</param>
        public static void RegisterAction(string name, InputAxis1DAction action, string tooltip = "") {
            Axis1DActions[name] = action;
            if(!Tooltips.ContainsKey(name)) Tooltips.Add(name, tooltip);
            OnUpdateActions?.Invoke();
        }

        /// <summary>
        /// Unregisters an input by name.
        /// Type of input (button, touch, axis, etc) does not matter.
        /// </summary>
        /// <param name="name">The name of the input to unregister.</param>
        public static void UnregisterAction(string name) {
            ButtonActions.Remove(name);
            ButtonTouchActions.Remove(name);
            AxisActions.Remove(name);
            Axis1DActions.Remove(name);
            Tooltips.Remove(name);
            OnUpdateActions?.Invoke();
        }

        /// <summary>
        /// Set the vibration state of the specified controller.
        /// Set frequency and amplitude to 0 to stop vibration.
        /// </summary>
        /// <param name="frequency">The new frequency.</param>
        /// <param name="amplitude">The new amplitude.</param>
        /// <param name="hand">The controller to apply this changes to.</param>
        public static void SetVibration(float frequency, float amplitude, Hand hand) {
            OnSetVibration?.Invoke(frequency, amplitude, hand);
        }
    }
}