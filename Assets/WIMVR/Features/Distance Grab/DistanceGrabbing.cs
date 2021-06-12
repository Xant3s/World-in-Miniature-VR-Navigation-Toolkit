// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Core.Input;
using WIMVR.Util;
using WIMVR.Util.XR;
using WIMVR.VR;
using InputDevice = UnityEngine.XR.InputDevice;

namespace WIMVR.Features.Distance_Grab {
    [DisallowMultipleComponent]
    public class DistanceGrabbing : MonoBehaviour {
        public bool distanceGrabbingEnabled;

        private readonly IHandInitializer<GameObject> handModelInitializer = new HandModelInitializer();
        private readonly IHandInitializer<InputDevice> controllerInitializer = new XRControllerInitializer();
        private IButtonListener rightGrabButtonListener;
        private IButtonListener leftGrabButtonListener;
        private DistanceGrabber leftDistanceGrabber;
        private DistanceGrabber rightDistanceGrabber;
        private AimAssist leftAimAssist;
        private AimAssist rightAimAssist;


        private void Start() {
            if(!distanceGrabbingEnabled) return;
            handModelInitializer.OnRightHandInitialized += SetupRequiredComponentsForRightHand;
            handModelInitializer.OnLeftHandInitialized += SetupRequiredComponentsForLeftHand;
            handModelInitializer.StartWaitForHands();
            
            controllerInitializer.OnRightHandInitialized += SetupRightGrabButtonForwarding;
            controllerInitializer.OnLeftHandInitialized += SetupLeftGrabButtonForwarding;
            controllerInitializer.StartWaitForHands();
        }

        private void Update() {
            rightGrabButtonListener?.Update();
            leftGrabButtonListener?.Update();
        }

        private void SetupRightGrabButtonForwarding(InputDevice rightController) {
            var grabButton = XRUtils.DetectGrabButton(Hand.LeftHand);
            rightGrabButtonListener = new ButtonListener(grabButton, rightController);
            rightGrabButtonListener.OnButtonDown += () => StartDistanceGrab(rightDistanceGrabber, rightAimAssist);
            rightGrabButtonListener.OnButtonUp += () => StopDistanceGrab(rightDistanceGrabber, rightAimAssist);
        }

        private void SetupLeftGrabButtonForwarding(InputDevice leftController) {
            var grabButton = XRUtils.DetectGrabButton(Hand.LeftHand);
            leftGrabButtonListener = new ButtonListener(grabButton, leftController);
            leftGrabButtonListener.OnButtonDown += () => StartDistanceGrab(leftDistanceGrabber, leftAimAssist);
            leftGrabButtonListener.OnButtonUp += () => StopDistanceGrab(leftDistanceGrabber, leftAimAssist);
        }

        private void SetupRequiredComponentsForRightHand(GameObject rightHand) 
            => SetupHand(Hand.RightHand, rightHand, out rightDistanceGrabber, out rightAimAssist);

        private void SetupRequiredComponentsForLeftHand(GameObject leftHand) 
            => SetupHand(Hand.LeftHand, leftHand, out leftDistanceGrabber, out leftAimAssist);

        private static void SetupHand(Hand hand, GameObject obj, out DistanceGrabber distanceGrabber, out AimAssist aimAssist) {
            var aimAssistObj = Instantiate(Resources.Load<GameObject>("AimAssist"), obj.transform);
            aimAssist = aimAssistObj.GetComponent<AimAssist>();
            aimAssist.hand = hand;
            distanceGrabber = obj.AddComponent<DistanceGrabber>();
        }

        private static void StartDistanceGrab(DistanceGrabber distanceGrabber, AimAssist aimAssist) {
            if(!distanceGrabber) return;
            aimAssist.Disable();
            distanceGrabber.StartDistanceGrab();
        }

        private static void StopDistanceGrab(DistanceGrabber distanceGrabber, AimAssist aimAssist) {
            if(!distanceGrabber) return;
            aimAssist.Enable();
            distanceGrabber.StopDistanceGrab();
        }
    }
}