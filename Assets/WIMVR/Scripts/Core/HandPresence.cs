// Author: Samuel Truman (contact@samueltruman.com)

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


namespace WIMVR.Core {
    public class HandPresence : MonoBehaviour {
        [SerializeField] private InputDeviceCharacteristics controllerCharacteristics = InputDeviceCharacteristics.None;
        [SerializeField] private GameObject handModelPrefab = null;

        private InputDevice targetDevice;
        private GameObject spawnedHandModel;
        private GameObject WIM;
        private Animator handAnimator;


        private void Awake() {
            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
            WIM = GameObject.FindWithTag("WIM");
        }

        private void Start() {
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

        private void UpdateHandAnimation() {
            if(!handAnimator) return;
            if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out var triggerValue)) {
                handAnimator.SetFloat("Trigger", triggerValue);
            }
            else {
                handAnimator.SetFloat("Trigger", 0);
            }

            if(targetDevice.TryGetFeatureValue(CommonUsages.grip, out var gripValue)) {
                handAnimator.SetFloat("Grip", gripValue);
            }
            else {
                handAnimator.SetFloat("Grip", 0);
            }
        }

        private void Update() {
            if(!targetDevice.isValid) TryInitialize();
            UpdateHandAnimation();

            //if(targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) &&
            //   primaryButtonValue)
            //    Debug.Log("Pressing primary button.");

            //if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > .1f)
            //    Debug.Log("Trigger pressed " + triggerValue);

            //if(targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue) &&
            //   primary2DAxisValue != Vector2.zero)
            //    Debug.Log("Primary touchpad " + primary2DAxisValue);
        }
    }
}