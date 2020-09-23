using System;

namespace WIMVR.Core.Input {
    internal class ButtonListener : IButtonListener {
        public event Action OnButtonDown;
        public event Action OnButtonUp;

        private readonly InputHelpers.Button button;
        private UnityEngine.XR.InputDevice device;
        private bool buttonWasPressedLastUpdate;


        public ButtonListener(InputHelpers.Button button, UnityEngine.XR.InputDevice device) {
            this.button = button;
            this.device = device;
        }

        public void Update() {
            if(!device.isValid) return;
            device.IsPressed(button, out var isPressed);
            if(!buttonWasPressedLastUpdate && isPressed) OnButtonDown?.Invoke();
            else if(buttonWasPressedLastUpdate && !isPressed) OnButtonUp?.Invoke();
            buttonWasPressedLastUpdate = isPressed;
        }
    }
}