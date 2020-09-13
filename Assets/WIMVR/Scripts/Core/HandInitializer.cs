// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections;
using UnityEngine;

namespace WIMVR.Core {
    public class HandInitializer : MonoBehaviour {
        protected bool leftHandInitialized;
        protected bool rightHandInitialized;


        protected void StartWaitForHands(float pauseInterval = 1) {
            StartCoroutine(WaitForHands(pauseInterval));
        }

        protected virtual void LeftHandInitialized(GameObject leftHand) { }

        protected virtual void RightHandInitialized(GameObject rightHand) { }

        private IEnumerator WaitForHands(float pauseInterval) {
            while(!leftHandInitialized || !rightHandInitialized) {
                if(!leftHandInitialized) {
                    leftHandInitialized = TryInitHand("HandL", out var leftHand);
                    if(leftHandInitialized) LeftHandInitialized(leftHand);
                }

                if(!rightHandInitialized) {
                    rightHandInitialized = TryInitHand("HandR", out var rightHand);
                    if(rightHandInitialized) RightHandInitialized(rightHand);
                }

                yield return new WaitForSeconds(pauseInterval);
            }
        }

        private bool TryInitHand(string handTag, out GameObject hand) {
            hand = GameObject.FindWithTag(handTag);
            return hand;
        }
    }
}