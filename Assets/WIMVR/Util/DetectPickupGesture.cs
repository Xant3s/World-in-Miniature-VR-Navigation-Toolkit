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
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class DetectPickupGesture : MonoBehaviour {
        private enum Finger {
            LeftIndex,
            LeftThumb,
            RightIndex,
            RightThumb,
            None
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
            var fingerType = GetFingerType(other);
            var hand = IsRightHand(fingerType) ? Hand.RightHand : Hand.LeftHand;
            if(fingerType == Finger.None) return;
            fingersInside[fingerType] = true;
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
            var fingerType = GetFingerType(other);
            if(fingerType == Finger.None) return;
            fingersInside[fingerType] = false;
            if(fingersInside.All(kvp => kvp.Value == false)) {
                OnStopTouch?.Invoke();
            }
        }

        private void StopGrabbing() {
            pinchingHand = Hand.None;
            OnStopGrabbing?.Invoke();
        }

        private static Finger GetFingerType(Component component) {
            if(component.GetComponent<LeftIndexFingerTip>()) return Finger.LeftIndex;
            if(component.GetComponent<LeftThumbFingerTip>()) return Finger.LeftThumb;
            if(component.GetComponent<RightIndexFingerTip>()) return Finger.RightIndex;
            if(component.GetComponent<RightThumbFingerTip>()) return Finger.RightThumb;
            return Finger.None;
        }

        private static bool IsRightHand(Finger fingerType) => fingerType == Finger.RightIndex || fingerType == Finger.RightThumb;
    }
}