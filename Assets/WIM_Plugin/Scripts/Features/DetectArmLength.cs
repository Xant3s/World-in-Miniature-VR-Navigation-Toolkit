using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    public class DetectArmLength : MonoBehaviour {
        private bool armLengthDetected;

        void OnEnable() {
            MiniatureModel.OnUpdate += detectArmLength;
        }

        void OnDisable() {
            MiniatureModel.OnUpdate -= detectArmLength;
        }

        private void detectArmLength(WIMConfiguration config, WIMData data) {
            if (!config.AutoDetectArmLength || armLengthDetected) return;
            if(!OVRInput.GetDown(config.ConfirmArmLengthButton)) return;
            armLengthDetected = true;
            var controllerPos = GameObject.Find("CustomHandRight").transform.position;
            var headPos = Camera.main.transform.position;
            config.PlayerArmLength = (controllerPos - headPos).magnitude;
        }
    }
}