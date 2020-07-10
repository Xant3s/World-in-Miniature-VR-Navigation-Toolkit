// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
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
            return devices[0];
        }
    }
}