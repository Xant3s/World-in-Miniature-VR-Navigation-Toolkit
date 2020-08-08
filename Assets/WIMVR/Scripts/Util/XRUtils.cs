// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


namespace WIMVR.Util.XR {
    /// <summary>
    /// A collection of utility functions for the XR interaction toolkit.
    /// </summary>
    public static class XRUtils {
        /// <summary>
        /// Tries to find the InputDevice to the corresponding hand.
        /// Returns null if no InputDevice is found.
        /// If multiple InputDevices are found, the first one is returned.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static InputDevice FindCorrespondingInputDevice(Hand hand) {
            var characteristics = hand == Hand.LeftHand ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right;
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(characteristics, devices);
            if(devices.Count == 0) {
                Debug.LogWarning("Controller not found. Please make sure both controllers are present.");
            }
            return devices.Count > 0 ? devices[0] : new InputDevice();
        }
    }
}