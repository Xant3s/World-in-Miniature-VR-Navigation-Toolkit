// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.InputSystem;
using WIMVR.Core;
using WIMVR.Core.Input;
using WIMVR.Util;
using WIMVR.Util.XR;

namespace WIMVR.Features.Distance_Grab {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInput))]
    public class DistanceGrabbing : HandInitializer {
        // TODO: allow distance grabbing for just one hand.
        public bool distanceGrabbingEnabled;

        private IButtonListener rightGrabButtonListener;
        private IButtonListener leftGrabButtonListener;
        private DistanceGrabber leftDistanceGrabber;
        private DistanceGrabber rightDistanceGrabber;
        private AimAssist leftAimAssist;
        private AimAssist rightAimAssist;


        private void Start() {
            if(!distanceGrabbingEnabled) return;
            StartWaitForHands();
        }

        private void Update() {
            rightGrabButtonListener?.Update();
            leftGrabButtonListener?.Update();
        }

        protected override void RightHandInitialized(GameObject rightHand) {
            var rightController = XRUtils.FindCorrespondingInputDevice(Hand.RightHand);
            var grabButton = XRUtils.DetectGrabButton(Hand.LeftHand);
            rightGrabButtonListener = new ButtonListener(grabButton, rightController);
            rightGrabButtonListener.OnButtonDown += StartDistanceGrabRight;
            rightGrabButtonListener.OnButtonUp += StopDistanceGrabRight;
            SetupHand(Hand.RightHand, rightHand, out rightDistanceGrabber, out rightAimAssist);
        }

        protected override void LeftHandInitialized(GameObject leftHand) {
            var leftController = XRUtils.FindCorrespondingInputDevice(Hand.LeftHand);
            var grabButton = XRUtils.DetectGrabButton(Hand.LeftHand);
            leftGrabButtonListener = new ButtonListener(grabButton, leftController);
            leftGrabButtonListener.OnButtonDown += StartDistanceGrabLeft;
            leftGrabButtonListener.OnButtonUp += StopDistanceGrabLeft;
            SetupHand(Hand.LeftHand, leftHand, out leftDistanceGrabber, out leftAimAssist);
        }

        private void SetupHand(Hand hand, GameObject obj, out DistanceGrabber distanceGrabber, out AimAssist aimAssist) {
            var aimAssistObj = Instantiate(Resources.Load<GameObject>("AimAssist"), obj.transform);
            aimAssist = aimAssistObj.GetComponent<AimAssist>();
            aimAssist.hand = hand;
            distanceGrabber = obj.AddComponent<DistanceGrabber>();
        }

        private void StartDistanceGrabRight() {
            if(!rightHandInitialized && rightDistanceGrabber) return;
            rightAimAssist.Disable();
            rightDistanceGrabber.StartDistanceGrab();
        }

        private void StopDistanceGrabRight() {
            if(!rightHandInitialized && rightDistanceGrabber) return;
            rightAimAssist.Enable();
            rightDistanceGrabber.StopDistanceGrab();
        }

        private void StartDistanceGrabLeft() {
            if(!leftHandInitialized && leftDistanceGrabber) return;
            leftAimAssist.Disable();
            leftDistanceGrabber.StartDistanceGrab();
        }

        private void StopDistanceGrabLeft() {
            if(!leftHandInitialized && leftDistanceGrabber) return;
            leftAimAssist.Enable();
            leftDistanceGrabber.StopDistanceGrab();
        }
    }
}