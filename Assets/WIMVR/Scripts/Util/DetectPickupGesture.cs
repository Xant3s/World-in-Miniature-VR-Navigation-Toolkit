// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;

namespace WIMVR.Util {
    /// <summary>
    /// Detects whether object is being picked up and calls events.
    /// What exactly should happen and how the object is picked up must be specified by events.
    /// This object is assumed to only collide with hands, i.e. its layer only collides with
    /// the 'Fingers' layer.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class DetectPickupGesture : MonoBehaviour {
        public delegate void Action();
        public delegate void HandAction(Hand hand);

        //private Transform thumb;
        //private Transform index;
        //private bool thumbIsGrabbing;
        //private bool indexIsGrabbing;
        //private bool isGrabbing;
        //private bool stoppedGrabbing;
        //private bool indexIsPressed;

        public event Action OnStartGrabbing;
        public event Action OnIsGrabbing;
        public event Action OnStopGrabbing;
        public event HandAction OnStartTouch;
        public event Action OnStopTouch;


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

        #endregion


        //private void Awake() {
        //    thumb = GameObject.FindWithTag("ThumbR")?.transform;
        //    index = GameObject.FindWithTag("IndexR")?.transform;
        //    Assert.IsNotNull(thumb);
        //    Assert.IsNotNull(index);
        //}

        //private void OnEnable() {
        //    MiniatureModel.OnPickupIndexButtonDown += PickupIndexButtonDown;
        //    MiniatureModel.OnPickupIndexButtonUp += PickupIndexButtonUp;
        //    MiniatureModel.OnPickupThumbTouchUp += PickupThumbTouchUp;
        //}

        //private void OnDisable() {
        //    MiniatureModel.OnPickupIndexButtonDown -= PickupIndexButtonDown;
        //    MiniatureModel.OnPickupIndexButtonUp -= PickupIndexButtonUp;
        //    MiniatureModel.OnPickupThumbTouchUp -= PickupThumbTouchUp;
        //}

        //private void PickupIndexButtonDown(WIMConfiguration config, WIMData data) {
        //    indexIsPressed = true;
        //}

        //private void PickupIndexButtonUp(WIMConfiguration config, WIMData data) {
        //    indexIsPressed = false;
        //    StopGrabbing();
        //}

        //private void PickupThumbTouchUp(WIMConfiguration config, WIMData data) {
        //    StopGrabbing();
        //}

        private void Update() {
            if(pinchingHand != Hand.None) OnIsGrabbing?.Invoke();
            //var rightHandPinch = thumbIsGrabbing && indexIsGrabbing && indexIsPressed;
            //if (rightHandPinch && !isGrabbing) {
            //    isGrabbing = true;
            //    stoppedGrabbing = false;
            //    StartGrabbing();
            //}
            //else if (isGrabbing && !rightHandPinch) {
            //    isGrabbing = false;
            //}
        }

        private void OnTriggerEnter(Collider other) {
            // This object's layer only collides with the 'Fingers' layer.
            // So we know the object we just collided with is a finger.
            fingersInside[other.tag] = true;
            var hand = (other.tag.Equals(fingers[0]) || other.tag.Equals(fingers[1])) ? Hand.RightHand : Hand.LeftHand;
            OnStartTouch?.Invoke(hand);
            if(!IsPinched && RIndexInside && RThumbInside) {
                pinchingHand = Hand.RightHand;
                OnStartGrabbing?.Invoke();
            }
            else if(!IsPinched && LIndexInside && LThumbInside) {
                pinchingHand = Hand.LeftHand;
                OnStartGrabbing?.Invoke();
            }
            //if (other.transform == thumb) {
            //    thumbIsGrabbing = true;
            //    OnStartTouch?.Invoke();
            //}
            //else if (other.transform == index) {
            //    indexIsGrabbing = true;
            //    OnStartTouch?.Invoke();
            //}
        }

        private void OnTriggerExit(Collider other) {
            fingersInside[other.tag] = false;
            if(fingersInside.All(kvp => kvp.Value == false)) {
                OnStopTouch?.Invoke();
            }
            switch(pinchingHand) {
                case Hand.RightHand when !RIndexInside || !RThumbInside: // fall-through
                case Hand.LeftHand when !LIndexInside || !LThumbInside:
                    StopGrabbing();
                    break;
                default:
                    return;
            }
            //if (other.transform == thumb) {
            //    thumbIsGrabbing = false;
            //    OnStopTouch?.Invoke();
            //}
            //else if (other.transform == index) {
            //    indexIsGrabbing = false;
            //    OnStopTouch?.Invoke();
            //}
        }

        //private void StartGrabbing() {
        //    OnStartGrabbing?.Invoke();
        //}

        private void StopGrabbing() {
            //if (stoppedGrabbing) return;
            pinchingHand = Hand.None;
            OnStopGrabbing?.Invoke();
            //stoppedGrabbing = true;
        }
    }
}