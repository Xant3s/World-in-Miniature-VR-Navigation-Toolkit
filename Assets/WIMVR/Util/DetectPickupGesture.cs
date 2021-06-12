// Author: Samuel Truman (contact@samueltruman.com)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using WIMVR.VR;
using WIMVR.VR.HandSetup.Tags;

namespace WIMVR.Util {
    /// <summary>
    /// Detects whether object is being picked up and calls events.
    /// What exactly should happen and how the object is picked up must be specified by events.
    /// This object is assumed to only collide with the player's fingertips,
    /// i.e. its layer only collides with the 'Fingers' layer.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class DetectPickupGesture : MonoBehaviour {
        public enum Finger {
            LeftIndex,
            LeftThumb,
            RightIndex,
            RightThumb
        }

        #region Events

        public UnityEvent OnStartGrabbing = new UnityEvent();

        public UnityEvent OnIsGrabbing = new UnityEvent();

        public UnityEvent OnStopGrabbing = new UnityEvent();

        public HandEvent OnStartTouch = new HandEvent();

        public UnityEvent OnStopTouch = new UnityEvent();

        #endregion

        #region Private Members

        private Hand pinchingHand = Hand.None;

        private readonly Dictionary<Finger, bool> fingersInside = new Dictionary<Finger, bool> {
            {Finger.LeftIndex, false},
            {Finger.LeftThumb, false},
            {Finger.RightIndex, false},
            {Finger.RightThumb, false}
        };

        private bool RIndexInside => fingersInside[Finger.RightIndex];

        private bool RThumbInside => fingersInside[Finger.RightThumb];

        private bool LIndexInside => fingersInside[Finger.LeftIndex];

        private bool LThumbInside => fingersInside[Finger.LeftThumb];

        private bool IsPinched => pinchingHand != Hand.None;

        private readonly IHandInitializer<InputDevice> controllerInitializer = new XRControllerInitializer();

        private InputDevice leftController;

        private InputDevice rightController;

        #endregion


        private void Start() {
            controllerInitializer.OnLeftHandInitialized += controller => leftController = controller; 
            controllerInitializer.OnRightHandInitialized += controller => rightController = controller; 
            controllerInitializer.StartWaitForHands();
        }

        private void Update() {
            if(pinchingHand != Hand.None) {
                OnIsGrabbing?.Invoke();
                var triggerPressed = IndexPressed(pinchingHand);
                if(!triggerPressed) StopGrabbing();
            }
        }

        private void OnTriggerEnter(Collider other) {
            // This object's layer only collides with the 'Fingers' layer.
            // So we know the object we just collided with is a finger.
            fingersInside[other.GetFingerType()] = true;
            var hand = other.Is(Finger.RightIndex) || other.Is(Finger.RightThumb) ? Hand.RightHand : Hand.LeftHand;
            OnStartTouch?.Invoke(hand);
            if(!IsPinched && RIndexInside && RThumbInside && IndexPressed(Hand.RightHand)) {
                pinchingHand = Hand.RightHand;
                OnStartGrabbing?.Invoke();
            }
            else if(!IsPinched && LIndexInside && LThumbInside && IndexPressed(Hand.LeftHand)) {
                pinchingHand = Hand.LeftHand;
                OnStartGrabbing?.Invoke();
            }
        }

        private bool IndexPressed(Hand hand) {
            var controller = hand == Hand.LeftHand ? leftController : rightController;
            controller.TryGetFeatureValue(CommonUsages.triggerButton, out var triggerPressed);
            return triggerPressed;
        }

        private void OnTriggerExit(Collider other) {
            fingersInside[other.GetFingerType()] = false;
            if(fingersInside.All(kvp => kvp.Value == false)) {
                OnStopTouch?.Invoke();
            }
        }

        private void StopGrabbing() {
            pinchingHand = Hand.None;
            OnStopGrabbing?.Invoke();
        }
    }
    
    internal static class ComponentExtensions {
        public static bool Is(this Component component, DetectPickupGesture.Finger finger) 
            => component.GetComponent(ComponentFromFinger(finger));

        public static DetectPickupGesture.Finger GetFingerType(this Component component) {
            if(component.GetComponent<LeftIndexFingerTip>()) return DetectPickupGesture.Finger.LeftIndex;
            if(component.GetComponent<LeftThumbFingerTip>()) return DetectPickupGesture.Finger.LeftThumb;
            if(component.GetComponent<RightIndexFingerTip>()) return DetectPickupGesture.Finger.RightIndex;
            return DetectPickupGesture.Finger.RightThumb;
        }

        private static Type ComponentFromFinger(DetectPickupGesture.Finger finger) => finger switch {
            DetectPickupGesture.Finger.LeftIndex => typeof(LeftIndexFingerTip),
            DetectPickupGesture.Finger.LeftThumb => typeof(LeftThumbFingerTip),
            DetectPickupGesture.Finger.RightIndex => typeof(RightIndexFingerTip),
            _ => typeof(RightThumbFingerTip)
        };
    }
}