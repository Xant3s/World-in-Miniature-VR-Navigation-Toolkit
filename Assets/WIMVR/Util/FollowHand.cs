// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.VR.HandSetup.Tags;

namespace WIMVR.Util {
    /// <summary>
    /// Transform follows specified target.
    /// </summary>
    [DisallowMultipleComponent]
    public class FollowHand : MonoBehaviour {
        public Hand hand;

        private Transform target;


        private void Start() {
            TryGetTarget();
        }

        private void TryGetTarget() {
            if(target) return;
            target = hand switch {
                Hand.LeftHand => FindObjectOfType<LeftHand>()?.transform,
                Hand.RightHand => FindObjectOfType<RightHand>()?.transform,
                _ => target
            };
        }

        private void Update() {
            TryGetTarget();
            if(!target) return;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.position = target.position;
            transform.rotation = Quaternion.LookRotation(target.forward, target.up);
            // Todo Hardcoded:
            transform.Rotate(Vector3.right, 90);
            transform.position -= transform.up * .1f;
            transform.position -= transform.forward * .05f;
        }
    }
}