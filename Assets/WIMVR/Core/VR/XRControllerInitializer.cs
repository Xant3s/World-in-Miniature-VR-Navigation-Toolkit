// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEngine.XR;
using WIMVR.Util.XR;
using Hand = WIMVR.Util.Hand;

namespace WIMVR.Core {
    /// <summary>
    /// Waits for the XR controllers to be initialized, i.e. no longer in standby.
    /// </summary>
    public class XRControllerInitializer : IHandInitializer<InputDevice> {
        public event Action<InputDevice> OnRightHandInitialized;
        public event Action<InputDevice> OnLeftHandInitialized;
        private bool RightHandInitialized { get; set; }
        private bool LeftHandInitialized { get; set; }


        public void StartWaitForHands() => MiniatureModel.OnUpdate += WaitForHands;

        private void WaitForHands(WIMConfiguration config, WIMData data) {
            if(!RightHandInitialized) {
                RightHandInitialized = TryInitHand(Hand.RightHand, out var rightHand);
                if(RightHandInitialized) OnRightHandInitialized?.Invoke(rightHand);
            }
            
            if(!LeftHandInitialized) {
                LeftHandInitialized = TryInitHand(Hand.LeftHand, out var leftHand);
                if(LeftHandInitialized) OnLeftHandInitialized?.Invoke(leftHand);
            }

            if(RightHandInitialized && LeftHandInitialized) MiniatureModel.OnUpdate -= WaitForHands;
        }

        private static bool TryInitHand(Hand hand, out InputDevice controller) {
            controller = XRUtils.TryFindCorrespondingInputDevice(hand);
            return controller.isValid;
        }
    }
}