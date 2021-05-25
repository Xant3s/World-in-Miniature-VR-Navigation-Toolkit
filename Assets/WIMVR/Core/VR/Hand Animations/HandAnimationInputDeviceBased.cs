// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace WIMVR.Core {
    [RequireComponent(typeof(HandAnimationController))]
    public class HandAnimationInputDeviceBased : MonoBehaviour {
        [SerializeField] private XRController controller;

        private HandAnimationController animationController;
        private float gripState;
        private float pinchState;
        private float indexTouchState;
        private float thumbTouchState;


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
            controller.inputDevice.TryGetFeatureValue(CommonUsages.indexTouch, out indexTouchState);
            controller.inputDevice.TryGetFeatureValue(CommonUsages.thumbTouch, out thumbTouchState);
        }

        private void UpdateAnimations() {
            animationController.UpdateGesture(HandAnimationController.Gesture.Grip, gripState);
            animationController.UpdateGesture(HandAnimationController.Gesture.Pinch, pinchState);
            animationController.UpdatePinchGesture(HandAnimationController.PinchFinger.Index, indexTouchState);
            animationController.UpdatePinchGesture(HandAnimationController.PinchFinger.Thumb, thumbTouchState);
        }
    }
}