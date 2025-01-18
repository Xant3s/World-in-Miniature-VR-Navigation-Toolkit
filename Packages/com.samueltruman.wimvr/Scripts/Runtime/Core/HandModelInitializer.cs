// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEngine;

namespace WIMVR.Core {
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
                RightHandInitialized = TryInitHand("HandR", out var rightHand);
                if(RightHandInitialized) OnRightHandInitialized?.Invoke(rightHand);
            }
            
            if(!LeftHandInitialized) {
                LeftHandInitialized = TryInitHand("HandL", out var leftHand);
                if(LeftHandInitialized) OnLeftHandInitialized?.Invoke(leftHand);
            }

            if(RightHandInitialized && LeftHandInitialized) MiniatureModel.OnUpdate -= WaitForHands;
        }

        private static bool TryInitHand(string handTag, out GameObject hand) {
            hand = GameObject.FindWithTag(handTag);
            return hand;
        }
    }
}