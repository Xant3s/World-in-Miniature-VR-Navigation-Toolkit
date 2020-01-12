using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    public class PreviewScreenController : MonoBehaviour {
        private float DoubleTapInterval { get; } = 2;

        private MiniatureModel WIM;
        private Transform index;
        private bool firstTap;
        private bool ready; // Index finger is very likely to be inside object on creation of this script. Ready once finger is no longer within collider.

        private void Awake() {
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(index);
            Assert.IsNotNull(WIM);
        }

        private void OnDestroy() {
            CancelInvoke();
        }

        private void OnTriggerEnter(Collider other) {
            if (!ready) return;
            if (other.transform != index) return;
            if (transform.root.CompareTag("HandR")) return;
            if (firstTap) {
                WIM.GetComponent<PreviewScreen>().RemovePreviewScreen();
            }
            else {
                vibrate();
                firstTap = true;
                Invoke(nameof(resetDoubleTap), DoubleTapInterval);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (ready) return;
            if (other.transform != index) return;
            if (transform.root.CompareTag("HandR")) return;
            ready = true;
        }

        private void resetDoubleTap() {
            firstTap = false;
        }

        private bool isVibrating;
        private void vibrate() {
            if (isVibrating) return;
            isVibrating = true;
            InputManager.SetVibration(frequency: .5f, amplitude: .1f, Hand.RightHand);
            Invoke(nameof(stopVibration), time: .1f);
        }

        private void stopVibration() {
            isVibrating = false;
            InputManager.SetVibration(frequency: 0, amplitude: 0, Hand.RightHand);
        }
    }
}