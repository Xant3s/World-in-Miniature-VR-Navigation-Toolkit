// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIM_Plugin {
    /// <summary>
    /// Detect the player's arm length at the start of the application.
    /// Therefore, the player has to fully extend the dominant arm and press a button.
    /// The arm length is used to determine at what distance the miniature model has to be
    /// spawned in front of the player.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class DetectArmLength : MonoBehaviour {
        private static readonly string actionName = "Confirm Arm Length Button";
        private static readonly string actionTooltip = "Used to confirm the player's arm length.";
        private WIMConfiguration config;
        private WIMData data;
        private bool armLengthDetected;

        private void OnEnable() {
            MiniatureModel.OnLateInit += Init;
            InputManager.RegisterAction(actionName, Detect, tooltip: actionTooltip);
        }

        private void OnDisable() {
            MiniatureModel.OnLateInit -= Init;
            InputManager.UnregisterAction(actionName);
        }

        private void Init(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
        }

        private void Detect() {
            if (!config.AutoDetectArmLength || armLengthDetected) return;
            armLengthDetected = true;
            var rightHand = GameObject.FindWithTag("HandR");
            var mainCamera = Camera.main;
            if(!rightHand || !mainCamera) return;
            var controllerPos = rightHand.transform.position;
            var headPos = mainCamera.transform.position;
            config.SpawnDistance = (controllerPos - headPos).magnitude;
        }
    }
}