using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    [RequireComponent(typeof(LineRenderer))]
    public class AimAssist : MonoBehaviour {
        [SerializeField] private Hand hand;
        [SerializeField] private float length = 10.0f;
        private LineRenderer lr;


        private void Awake() {
            lr = GetComponent<LineRenderer>();
        }

        private void OnEnable() {
            if (hand == Hand.HAND_L) {
                MiniatureModel.OnLeftGrabButtonDown += grabButtonDown;
                MiniatureModel.OnLeftGrabButtonUp += grabButtonUp;
            }
            else if (hand == Hand.HAND_R) {
                MiniatureModel.OnRightGrabButtonDown += grabButtonDown;
                MiniatureModel.OnRightGrabButtonUp += grabButtonUp;
            }
        }

        private void OnDisable() {
            if (hand == Hand.HAND_L) {
                MiniatureModel.OnLeftGrabButtonDown -= grabButtonDown;
                MiniatureModel.OnLeftGrabButtonUp -= grabButtonUp;
            }
            else if (hand == Hand.HAND_R) {
                MiniatureModel.OnRightGrabButtonDown -= grabButtonDown;
                MiniatureModel.OnRightGrabButtonUp -= grabButtonUp;
            }
        }

        private void grabButtonDown(WIMConfiguration config, WIMData data) {
            lr.enabled = false;
        }

        private void grabButtonUp(WIMConfiguration config, WIMData data) {
            lr.enabled = true;
        }

        private void Start() {
            // Check if enabled.
            var grabber = gameObject.GetComponentInParent<DistanceGrabber>();
            if (grabber == null || !grabber.enabled) {
                gameObject.GetComponent<LineRenderer>().enabled = false;
                this.enabled = false;
            }
        }

        private void Update() {
            var position = transform.position;
            lr.SetPosition(0, position);
            lr.SetPosition(1, position + transform.forward * length);
        }
    }
}