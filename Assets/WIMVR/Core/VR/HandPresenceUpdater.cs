// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace WIMVR.VR {
    public class HandPresenceUpdater : MonoBehaviour {
        [SerializeField] private InputDeviceCharacteristics controllerCharacteristics = InputDeviceCharacteristics.None;

        private InputDevice targetDevice;
        private GameObject WIM;


        private void Awake() {
            WIM = GameObject.FindWithTag("WIM");
        }
        
        private void Start() {
            Invoke(nameof(Init), 1);
        }

        private void Update() {
            if(!targetDevice.isValid) TryInitialize();
        }

        private void Init() {
            TryInitialize();
            if(WIM) {
                WIM.SendMessage("HandSpawned");
            }
            else {
                Debug.LogWarning("No miniature model found in the scene.");
            }
        }

        private void TryInitialize() {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

            if(devices.Count > 0) {
                targetDevice = devices[0];
                if(WIM) WIM.SendMessage("ReInitWIM");
            }
        }
    }
}