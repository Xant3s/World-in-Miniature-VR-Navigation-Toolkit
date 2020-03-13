using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WIM_Plugin;


namespace WIM_Plugin {
    [DisallowMultipleComponent]
    public class FollowHand : MonoBehaviour {
        public Hand hand;

        private Transform target;

        private void Start() {
            switch(hand) {
                case Hand.LeftHand:
                    target = GameObject.FindWithTag("HandL").transform;
                    break;
                case Hand.RightHand:
                    target = GameObject.FindWithTag("HandR").transform;
                    break;
            }
        }

        private void Update() {
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