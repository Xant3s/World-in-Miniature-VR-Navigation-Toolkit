// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine.XR;


namespace WIMVR.Util.Haptics {
    public static class Haptics {
        public static void Vibrate(InputDevice device, float amplitude, float duration) {
            if(device.TryGetHapticCapabilities(out var capabilities)) {
                if(!capabilities.supportsImpulse) return;
                uint channel = 0;
                device.SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }
}