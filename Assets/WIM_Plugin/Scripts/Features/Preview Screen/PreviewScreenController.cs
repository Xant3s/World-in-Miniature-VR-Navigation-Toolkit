using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    public class PreviewScreenController : MonoBehaviour {
        private float DoubleTapInterval { get; } = 2;

        private MiniatureModel WIM;
        private Transform index;
        private bool firstTap;

        private void Awake() {
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(index);
            Assert.IsNotNull(WIM);
        }

        private void OnTriggerEnter(Collider other) {
            if(other.transform != index) return;
            if(transform.root.CompareTag("HandR")) return;
            if(firstTap) {
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