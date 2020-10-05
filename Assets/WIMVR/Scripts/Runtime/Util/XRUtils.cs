// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


namespace WIMVR.Util.XR {
    /// <summary>
    /// A collection of utility functions for the XR interaction toolkit.
    /// </summary>
    public static class XRUtils {
        /// <summary>
        /// Tries to find the InputDevice to the corresponding hand.
        /// If multiple InputDevices are found, the first one is returned.
        /// Returns invalid input device if no input devices are found.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static InputDevice TryFindCorrespondingInputDevice(Hand hand) {
            var characteristics = hand == Hand.LeftHand ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right;
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(characteristics, devices);
            return devices.Count > 0 ? devices[0] : new InputDevice();
        }

        public static InputHelpers.Button DetectGrabButton(Hand hand) {
            var controllers = Object.FindObjectsOfType(typeof(XRController)) as XRController[];
            if(controllers == null || controllers.Length == 0) return InputHelpers.Button.None;
            var node = hand == Hand.RightHand ? XRNode.RightHand : XRNode.LeftHand;
            var controller = controllers.First(c => c.controllerNode == node);
            return controller.selectUsage;
        }
    }
}