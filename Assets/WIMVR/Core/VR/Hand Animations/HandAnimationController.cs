// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Core {
    public class HandAnimationController : MonoBehaviour {
        public enum Gesture {
            Grip,
            Pinch
        }

        public enum PinchFinger {
            Index,
            Thumb
        }

        private static readonly int grabPropertyID = Animator.StringToHash("Flex");
        private static readonly int pinchPropertyID = Animator.StringToHash("Pinch");
        private static readonly int pointLayerID = Animator.StringToHash("Point Layer");
        private static readonly int thumbLayerID = Animator.StringToHash("Thumb Layer");
        private const float animFrames = 4;
        private Animator animator;
        private float lastGripState;
        private float lastPinchState;
        private float lastIndexTouchState;
        private float lastThumbTouchState;


        private void Awake() {
            animator = GetComponentInChildren<Animator>();
        }

        public void UpdateGesture(Gesture gesture, float currentState) {
            GetPropertyIndex(gesture, out var propertyIndex);
            GetLastAnimationState(gesture, out var lastState);
            GetNewAnimState(currentState, lastState, out var newAnimState);
            UpdateLastAnimationState(gesture, newAnimState);
            animator.SetFloat(propertyIndex, newAnimState);
        }

        public void UpdatePinchGesture(PinchFinger finger, float currentState) {
            GetLayerIndex(finger, out var layerID);
            GetLastAnimationState(finger, out var lastState);
            GetNewAnimState(currentState, lastState, out var newAnimState);
            UpdateLastAnimationState(finger, newAnimState);
            animator.SetLayerWeight(layerID, 1 - newAnimState);
        }

        private static void GetNewAnimState(float currentState, float lastState, out float newState) {
            var delta = currentState - lastState;
            newState = delta switch {
                _ when delta > 0 => Mathf.Clamp(lastState + 1 / animFrames, 0, currentState),
                _ when delta < 0 => Mathf.Clamp(lastState - 1 / animFrames, currentState, 1),
                _ => currentState
            };
        }

        private void GetLastAnimationState(Gesture gesture, out float lastState) => lastState = gesture switch {
            Gesture.Grip => lastGripState,
            _ => lastPinchState
        };

        private void GetLastAnimationState(PinchFinger finger, out float lastState) => lastState = finger switch {
            PinchFinger.Index => lastIndexTouchState,
            _ => lastThumbTouchState
        };

        private void UpdateLastAnimationState(Gesture gesture, float value) {
            if(gesture == Gesture.Grip) lastGripState = value;
            else if(gesture == Gesture.Pinch) lastPinchState = value;
        }

        private void UpdateLastAnimationState(PinchFinger finger, float value) {
            if(finger == PinchFinger.Index) lastIndexTouchState = value;
            else if(finger == PinchFinger.Thumb) lastThumbTouchState = value;
        }

        private static void GetPropertyIndex(Gesture gesture, out int index) => index = gesture switch {
            Gesture.Grip => grabPropertyID,
            _ => pinchPropertyID
        };

        private static void GetLayerIndex(PinchFinger finger, out int index) => index = finger switch {
            PinchFinger.Index => pointLayerID,
            _ => thumbLayerID
        };
    }
}