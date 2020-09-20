// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WIMVR.Core;
using WIMVR.Util;

namespace WIMVR.Features.Distance_Grab {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInput))]
    public class DistanceGrabbing : HandInitializer {
        
        // TODO add distance grabbing to settings (editor)
        // TODO update hands on enable/disable distance grabbing (in editor? -> runtime)
        // TODO forward input to corresponding hand
        
        // TODO setup when enabled (public bool member)
        
        public bool distanceGrabbingEnabled;

        private DistanceGrabber leftDistanceGrabber;
        private DistanceGrabber rightDistanceGrabber;
        private AimAssist leftAimAssist;
        private AimAssist rightAimAssist;

        private InputAction rightDistanceGrabAction;


        private void Awake() {
            var playerInput = GetComponent<PlayerInput>();
            rightDistanceGrabAction = playerInput.actions["Distance Grab Right Hand"];
            var leftDistanceGrabAction = playerInput.actions["Distance Grab Left Hand"];
            // rightDistanceGrabAction.started += context => OnRightDistanceGrabDown();
            rightDistanceGrabAction.started += context => {
            //     Debug.Log(context.ReadValue<float>());
                // UpdateInteract(context);
                // if(!GetInteract()) return;
                OnRightDistanceGrabDown();
            };
            // rightDistanceGrabAction.canceled += context => OnRightDistanceGrabUp();  
            rightDistanceGrabAction.canceled += context => {
            //     Debug.Log(context.ReadValue<float>());
                // UpdateInteract(context);
                // if(GetInteract()) return;
                OnRightDistanceGrabUp();
            };
            leftDistanceGrabAction.started += context => OnLeftDistanceGrabDown();
            leftDistanceGrabAction.canceled += context => OnLeftDistanceGrabUp();
        }

        // private bool interact;
        // public void UpdateInteract(InputAction.CallbackContext context) {
        //     if(context.started) {
        //         interact = true;
        //     } else if(context.canceled) {
        //         interact = false;
        //     }
        // }
        //
        // public bool GetInteract() {
        //     var temp = interact;
        //     interact = false;
        //     return temp;
        // }

        private void Start() {
            if(!distanceGrabbingEnabled) return;
            StartWaitForHands();
        }

        // private void Update() {
        //     Debug.Log(rightDistanceGrabAction.ReadValue<float>());
        // }

        protected override void RightHandInitialized(GameObject rightHand) {
            // Debug.Log("right hand initialized");
            SetupHand(Hand.RightHand, rightHand, out rightDistanceGrabber, out rightAimAssist);
        }

        protected override void LeftHandInitialized(GameObject leftHand) {
            // Debug.Log("left hand initialized");
            SetupHand(Hand.LeftHand, leftHand, out leftDistanceGrabber, out leftAimAssist);
        }

        private void SetupHand(Hand hand, GameObject obj, out DistanceGrabber distanceGrabber, out AimAssist aimAssist) {
            var aimAssistObj = Instantiate(Resources.Load<GameObject>("AimAssist"), obj.transform);
            aimAssist = aimAssistObj.GetComponent<AimAssist>();
            aimAssist.hand = hand;
            distanceGrabber = obj.AddComponent<DistanceGrabber>();
        }

        // public void OnDistanceGrabRightHand() {
        //     if(!rightHandInitialized && rightDistanceGrabber) return;
        //     Debug.Log("Distance grab right");
        //     // TODO: forward to right distance grabber and aim assist
        //     // rightAimAssist.gr
        // }
        //
        // public void OnDistanceGrabLeftHand() {
        //     if(!leftHandInitialized && leftDistanceGrabber) return;
        //     Debug.Log("Distance grab left");
        //     // TODO: forward to left distance grabber and aim assist
        // }

        private void OnRightDistanceGrabDown() {
            if(!rightHandInitialized && rightDistanceGrabber) return;
            Debug.Log("Distance grab right down");
            rightAimAssist.GrabButtonDown();
        }

        private void OnRightDistanceGrabUp() {
            if(!rightHandInitialized && rightDistanceGrabber) return;
            Debug.Log("Distance grab up");
            rightAimAssist.GrabButtonUp();

        }

        private void OnLeftDistanceGrabDown() {
            if(!leftHandInitialized && leftDistanceGrabber) return;
            Debug.Log("Distance grab left down");
            leftAimAssist.GrabButtonDown();

        }

        private void OnLeftDistanceGrabUp() {
            if(!leftHandInitialized && leftDistanceGrabber) return;
            Debug.Log("Distance grab left up");
            leftAimAssist.GrabButtonUp();

        }
    }
}