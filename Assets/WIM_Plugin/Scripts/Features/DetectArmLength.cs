using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace WIM_Plugin {
    public class DetectArmLength : MonoBehaviour {
        private bool armLengthDetected;

        private void OnEnable() {
            MiniatureModel.OnUpdate += detectArmLength;
        }

        private void OnDisable() {
            MiniatureModel.OnUpdate -= detectArmLength;
        }

        private void detectArmLength(WIMConfiguration config, WIMData data) {
            if (!config.AutoDetectArmLength || armLengthDetected) return;
            if(!OVRInput.GetDown(config.ConfirmArmLengthButton)) return;
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