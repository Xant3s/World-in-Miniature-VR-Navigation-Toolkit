// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Util.Haptics;
using WIMVR.Util.XR;
using WIMVR.VR.HandSetup.Tags;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// Closes the preview screen when player enters collider with index finger.
    /// </summary>
    [DisallowMultipleComponent]
    public class ClosePreviewScreen : MonoBehaviour {
        private MiniatureModel WIM;
        private bool once;


        private void Awake() {
            WIM = FindObjectOfType<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

        private void Start() {
            GetComponent<Renderer>().enabled = true;
        }

        private void OnTriggerEnter(Collider other) {
            if(other.GetComponent<RightIndexFingerTip>() == null && other.GetComponent<LeftIndexFingerTip>() == null) return;
            if(once) return;
            once = true;
            var hand = other.GetComponent<RightIndexFingerTip>() ? Hand.RightHand : Hand.LeftHand;
            StartClosing(hand);
        }

        private void StartClosing(Hand hand) {
            Haptics.Vibrate(XRUtils.TryFindCorrespondingInputDevice(hand), .1f, .1f);
            StartCoroutine(Close(.1f));
        }

        private IEnumerator Close(float time) {
            yield return new WaitForSeconds(time);
            WIM.GetComponent<PreviewScreen>().RemovePreviewScreen();
        }
    }
}