// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using WIMVR.Core;
using WIMVR.Util.XR;

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
        #region Events
        public UnityEvent OnStartGrabbing = new UnityEvent();
        public UnityEvent OnIsGrabbing = new UnityEvent();
        public UnityEvent OnStopGrabbing = new UnityEvent();
        public HandEvent OnStartTouch = new HandEvent();
        public UnityEvent OnStopTouch = new UnityEvent();
        #endregion

        #region Private Members

        private static readonly string[] fingers = { "IndexR", "ThumbR", "IndexL", "ThumbL" };

        private Hand pinchingHand = Hand.None;

        private readonly Dictionary<string, bool> fingersInside = new Dictionary<string, bool> {
            {fingers[0], false},
            {fingers[1], false},
            {fingers[2], false},
            {fingers[3], false}
        };


        private bool RIndexInside => fingersInside[fingers[0]];
        private bool RThumbInside => fingersInside[fingers[1]];
        private bool LIndexInside => fingersInside[fingers[2]];
        private bool LThumbInside => fingersInside[fingers[3]];
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
            fingersInside[other.tag] = true;
            var hand = (other.tag.Equals(fingers[0]) || other.tag.Equals(fingers[1])) ? Hand.RightHand : Hand.LeftHand;
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
            fingersInside[other.tag] = false;
            if(fingersInside.All(kvp => kvp.Value == false)) {
                OnStopTouch?.Invoke();
            }
        }

        private void StopGrabbing() {
            pinchingHand = Hand.None;
            OnStopGrabbing?.Invoke();
        }
    }
}