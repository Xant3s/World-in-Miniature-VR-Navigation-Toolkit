// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.InputSystem;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Util.XR;

namespace WIMVR.Features.Distance_Grab {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInput))]
    public class DistanceGrabbing : HandInitializer {
        public bool distanceGrabbingEnabled;

        private DistanceGrabber leftDistanceGrabber;
        private DistanceGrabber rightDistanceGrabber;
        private AimAssist leftAimAssist;
        private AimAssist rightAimAssist;
        private UnityEngine.XR.InputDevice leftController;
        private UnityEngine.XR.InputDevice rightController;
        private bool rightGripWasPressedLastFrame;
        private bool leftGripWasPressedLastFrame;
        

        private void Start() {
            if(!distanceGrabbingEnabled) return;
            StartWaitForHands();
        }

        private void Update() {
            if(rightController.isValid) {
                rightController.IsPressed(InputHelpers.Button.Grip, out var rightGrabPressed);
                if(!rightGripWasPressedLastFrame && rightGrabPressed) OnRightDistanceGrabDown();
                else if(rightGripWasPressedLastFrame && !rightGrabPressed) OnRightDistanceGrabUp();
                rightGripWasPressedLastFrame = rightGrabPressed;
            }

            if(leftController.isValid) {
                leftController.IsPressed(InputHelpers.Button.Grip, out var leftGrabPressed);
                if(!leftGripWasPressedLastFrame && leftGrabPressed) OnLeftDistanceGrabDown();
                else if(leftGripWasPressedLastFrame && !leftGrabPressed) OnLeftDistanceGrabUp();
                leftGripWasPressedLastFrame = leftGrabPressed;
            }
        }

        protected override void RightHandInitialized(GameObject rightHand) {
            rightController = XRUtils.FindCorrespondingInputDevice(Hand.RightHand);
            SetupHand(Hand.RightHand, rightHand, out rightDistanceGrabber, out rightAimAssist);
        }

        protected override void LeftHandInitialized(GameObject leftHand) {
            leftController = XRUtils.FindCorrespondingInputDevice(Hand.LeftHand);
            SetupHand(Hand.LeftHand, leftHand, out leftDistanceGrabber, out leftAimAssist);
        }

        private void SetupHand(Hand hand, GameObject obj, out DistanceGrabber distanceGrabber, out AimAssist aimAssist) {
            var aimAssistObj = Instantiate(Resources.Load<GameObject>("AimAssist"), obj.transform);
            aimAssist = aimAssistObj.GetComponent<AimAssist>();
            aimAssist.hand = hand;
            distanceGrabber = obj.AddComponent<DistanceGrabber>();
        }

        private void OnRightDistanceGrabDown() {
            if(!rightHandInitialized && rightDistanceGrabber) return;
            rightAimAssist.GrabButtonDown();
            rightDistanceGrabber.GrabButtonDown();
        }

        private void OnRightDistanceGrabUp() {
            if(!rightHandInitialized && rightDistanceGrabber) return;
            rightAimAssist.GrabButtonUp();
            rightDistanceGrabber.GrabButtonUp();
        }

        private void OnLeftDistanceGrabDown() {
            if(!leftHandInitialized && leftDistanceGrabber) return;
            leftAimAssist.GrabButtonDown();
            leftDistanceGrabber.GrabButtonDown();
        }

        private void OnLeftDistanceGrabUp() {
            if(!leftHandInitialized && leftDistanceGrabber) return;
            leftAimAssist.GrabButtonUp();
            leftDistanceGrabber.GrabButtonUp();
        }
    }
}