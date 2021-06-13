// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEngine;
using WIMVR.Core;
using WIMVR.VR.HandSetup.Tags;
using Object = UnityEngine.Object;

namespace WIMVR.VR {
    /// <summary>
    /// Waits for the hand models to be initialized.
    /// </summary>
    public sealed class HandModelInitializer : IHandInitializer<GameObject> {
        public event Action<GameObject> OnRightHandInitialized;
        public event Action<GameObject> OnLeftHandInitialized;
        private bool RightHandInitialized { get; set; }
        private bool LeftHandInitialized { get; set; }


        public void StartWaitForHands() => MiniatureModel.OnUpdate += WaitForHands;

        private void WaitForHands(WIMConfiguration config, WIMData data) {
            if(!RightHandInitialized) {
                var rightHand = Object.FindObjectOfType<RightHand>().gameObject;
                RightHandInitialized = rightHand;
                if(RightHandInitialized) OnRightHandInitialized?.Invoke(rightHand);
            }
            
            if(!LeftHandInitialized) {
                var leftHand = Object.FindObjectOfType<LeftHand>().gameObject;
                LeftHandInitialized = leftHand;
                if(LeftHandInitialized) OnLeftHandInitialized?.Invoke(leftHand);
            }

            if(RightHandInitialized && LeftHandInitialized) MiniatureModel.OnUpdate -= WaitForHands;
        }
    }
}