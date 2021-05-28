// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace WIMVR.VR {
    [RequireComponent(typeof(HandAnimationController))]
    public class HandAnimationInputDeviceBased : MonoBehaviour {
        [SerializeField] private XRController controller;

        private HandAnimationController animationController;
        private float gripState;
        private float pinchState;


        private void Awake() {
            if(!controller) controller = GetComponentInParent<XRController>();
            animationController = GetComponent<HandAnimationController>();
        }

        private void Update() {
            ReadInputs();
            UpdateAnimations();
        }

        private void ReadInputs() {
            controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out gripState);
            controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out pinchState);
        }

        private void UpdateAnimations() {
            animationController.UpdateGesture(HandAnimationController.Gesture.Grip, gripState);
            animationController.UpdateGesture(HandAnimationController.Gesture.Pinch, pinchState);
        }
    }
}