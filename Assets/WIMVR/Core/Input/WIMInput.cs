// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.InputSystem;

namespace WIMVR.Core.Input {
    [DisallowMultipleComponent]
    public class WIMInput : MonoBehaviour {
        public InputActionProperty respawn;
        public InputActionProperty destinationSelectionTouchLeft;
        public InputActionProperty destinationSelectionTouchRight;
        public InputActionProperty confirmTravel;
        public InputActionProperty destinationRotation;
        public InputActionProperty scrollWIM;
        public InputActionProperty scrollWIMVertically;
        public InputActionProperty detectArmLength;


        private void OnEnable() {
            respawn.action.Enable();
            destinationSelectionTouchLeft.action.Enable();
            destinationSelectionTouchRight.action.Enable();
            confirmTravel.action.Enable();
            destinationRotation.action.Enable();
            scrollWIM.action.Enable();
            scrollWIMVertically.action.Enable();
            detectArmLength.action.Enable();
        }

        private void OnDisable() {
            respawn.action.Disable();
            destinationSelectionTouchLeft.action.Disable();
            destinationSelectionTouchRight.action.Disable();
            confirmTravel.action.Disable();
            destinationRotation.action.Disable();
            scrollWIM.action.Disable();
            scrollWIMVertically.action.Disable();
            detectArmLength.action.Disable();
        }
    }
}