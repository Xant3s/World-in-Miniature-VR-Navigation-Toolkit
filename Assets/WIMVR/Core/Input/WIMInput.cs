// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.InputSystem;

namespace WIMVR.Core.Input {
    [DisallowMultipleComponent]
    public class WIMInput : MonoBehaviour {
        public InputActionProperty directMovement;
        public InputActionProperty respawn;
        public InputActionProperty destinationSelectionTouch;
        public InputActionProperty confirmTravel;
        public InputActionProperty destinationRotation;
        public InputActionProperty scrollWIM;
        public InputActionProperty scrollWIMVertically;
        public InputActionProperty detectArmLength;


        private void OnEnable() {
            directMovement.action.Enable();
            respawn.action.Enable();
            destinationSelectionTouch.action.Enable();
            confirmTravel.action.Enable();
            destinationRotation.action.Enable();
            scrollWIM.action.Enable();
            scrollWIMVertically.action.Enable();
            detectArmLength.action.Enable();
        }

        private void OnDisable() {
            directMovement.action.Disable();
            respawn.action.Disable();
            destinationSelectionTouch.action.Disable();
            confirmTravel.action.Disable();
            destinationRotation.action.Disable();
            scrollWIM.action.Disable();
            scrollWIMVertically.action.Disable();
            detectArmLength.action.Disable();
        }
    }
}