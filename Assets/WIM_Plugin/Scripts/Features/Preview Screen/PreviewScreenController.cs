using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    public class PreviewScreenController : MonoBehaviour {
        private float DoubleTapInterval { get; } = 2;

        private Transform index;
        private bool firstTap;

        private void Awake() {
            index = GameObject.FindWithTag("IndexR").transform;
            Assert.IsNotNull(index);
        }

        private void OnTriggerEnter(Collider other) {
            if(other.transform != index) return;
            if(transform.root.CompareTag("HandR")) return;
            if(firstTap) {
                var WIM = GameObject.Find("WIM").GetComponent<MiniatureModel>();
                WIM.GetComponent<PreviewScreen>().RemovePreviewScreen();
            }
            else {
                firstTap = true;
                Invoke(nameof(resetDoubleTap), DoubleTapInterval);
            }
        }

        private void resetDoubleTap() {
            firstTap = false;
        }
    }
}