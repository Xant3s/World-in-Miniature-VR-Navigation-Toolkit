// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


namespace WIMVR.Core {
    public class HandPresence : MonoBehaviour {
        [SerializeField] private InputDeviceCharacteristics controllerCharacteristics;
        [SerializeField] private GameObject handModelPrefab;

        private InputDevice targetDevice;
        private GameObject spawnedHandModel;
        private Animator handAnimator;


        private void Start() {
            TryInitialize();
        }

        private void TryInitialize() {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

            if(devices.Count > 0) {
                targetDevice = devices[0];
                spawnedHandModel = Instantiate(handModelPrefab, transform);
                handAnimator = spawnedHandModel.GetComponent<Animator>();
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