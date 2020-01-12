using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    public class ClosePreviewScreen : MonoBehaviour {
        private MiniatureModel WIM;
        private Transform index;


        private void Awake() {
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(index);
            Assert.IsNotNull(WIM);
        }

        private void Start() {
            GetComponent<Renderer>().enabled = true;
        }

        private bool once;
        private void OnTriggerEnter(Collider other) {
            if (other.transform != index) return;
            if (transform.root.CompareTag("HandR")) return;
            if (once) return;
            once = true;
            startClosing();
        }

        private void startClosing() {
            InputManager.SetVibration(frequency: .5f, amplitude: .1f, Hand.RightHand);
            Invoke(nameof(close), time: .1f);
        }

        private void close() {
            InputManager.SetVibration(frequency: 0, amplitude: 0, Hand.RightHand);
            WIM.GetComponent<PreviewScreen>().RemovePreviewScreen();
        }
    }
}