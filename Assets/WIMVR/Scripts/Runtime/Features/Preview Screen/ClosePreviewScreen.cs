// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Util.Haptics;
using WIMVR.Util.XR;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// Closes the preview screen when player enters collider with index finger.
    /// </summary>
    [DisallowMultipleComponent]
    public class ClosePreviewScreen : MonoBehaviour {
        private MiniatureModel WIM;
        private bool once;


        private void Awake() {
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

        private void Start() {
            GetComponent<Renderer>().enabled = true;
        }

        private void OnTriggerEnter(Collider other) {
            if(!other.CompareTag("IndexR") && !other.CompareTag("IndexL")) return;
            if (once) return;
            once = true;
            var hand = other.CompareTag("IndexR") ? Hand.RightHand : Hand.LeftHand;
            StartClosing(hand);
        }

        private void StartClosing(Hand hand) {
            Haptics.Vibrate(XRUtils.TryFindCorrespondingInputDevice(hand), .1f, .1f);
            StartCoroutine(Close(hand, .1f));
        }

        private IEnumerator Close(Hand hand, float time) {
            yield return new WaitForSeconds(time);
            WIM.GetComponent<PreviewScreen>().RemovePreviewScreen();
        }
    }
}