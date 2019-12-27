using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    [ExecuteAlways]
    public class DetectArmLength : MonoBehaviour {
        private static readonly string actionName = "Confirm Arm Length Button";
        private WIMConfiguration config;
        private WIMData data;
        private bool armLengthDetected;

        private void OnEnable() {
            MiniatureModel.OnLateInit += init;
            InputManager.RegisterAction(actionName, detectArmLength);
        }

        private void OnDisable() {
            MiniatureModel.OnLateInit -= init;
            InputManager.UnregisterAction(actionName);
        }

        private void init(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
        }

        private void detectArmLength() {
            if (!config.AutoDetectArmLength || armLengthDetected) return;
            armLengthDetected = true;
            var rightHand = GameObject.Find("CustomHandRight");
            var mainCamera = Camera.main;
            if(!rightHand || !mainCamera) return;
            var controllerPos = rightHand.transform.position;
            var headPos = mainCamera.transform.position;
            config.SpawnDistance = (controllerPos - headPos).magnitude;
        }
    }
}