// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.VR {
    public class HandAnimationController : MonoBehaviour {
        public enum Gesture {
            Grip,
            Pinch
        }

        private static readonly int grabPropertyID = Animator.StringToHash("Flex");
        private static readonly int pinchPropertyID = Animator.StringToHash("Pinch");
        private const float animFrames = 4;
        private Animator animator;
        private float lastGripState;
        private float lastPinchState;


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

        private void UpdateLastAnimationState(Gesture gesture, float value) {
            if(gesture == Gesture.Grip) lastGripState = value;
            else if(gesture == Gesture.Pinch) lastPinchState = value;
        }

        private static void GetPropertyIndex(Gesture gesture, out int index) => index = gesture switch {
            Gesture.Grip => grabPropertyID,
            _ => pinchPropertyID
        };
    }
}