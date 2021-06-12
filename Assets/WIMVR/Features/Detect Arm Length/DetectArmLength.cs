// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Core;
using WIMVR.Core.Input;
using WIMVR.VR.HandSetup.Tags;

namespace WIMVR.Features {
    /// <summary>
    /// Detect the player's arm length at the start of the application.
    /// Therefore, the player has to fully extend the dominant arm and press a button.
    /// The arm length is used to determine at what distance the miniature model has to be
    /// spawned in front of the player.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(WIMInput))]
    public class DetectArmLength : MonoBehaviour {
        private WIMInput wimInput;
        private WIMConfiguration config;
        private bool armLengthDetected;

        
        private void OnEnable() {
            MiniatureModel.OnLateInit += Init;
        }

        private void OnDisable() {
            MiniatureModel.OnLateInit -= Init;
        }

        private void Awake() {
            wimInput = GetComponent<WIMInput>();
            wimInput.detectArmLength.action.performed += ctx => OnDetectArmLength();
        }

        private void Init(WIMConfiguration config, WIMData data) => this.config = config;

        public void OnDetectArmLength() => Detect();

        private void Detect() {
            if (!config.AutoDetectArmLength || armLengthDetected) return;
            armLengthDetected = true;
            var rightHand = FindObjectOfType<RightHand>().gameObject;
            var mainCamera = Camera.main;
            if(!rightHand || !mainCamera) return;
            var controllerPos = rightHand.transform.position;
            var headPos = mainCamera.transform.position;
            config.SpawnDistance = (controllerPos - headPos).magnitude;
            Debug.Log(config.SpawnDistance);
        }
    }
}