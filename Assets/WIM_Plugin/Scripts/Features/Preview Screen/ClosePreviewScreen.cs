using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    [DisallowMultipleComponent]
    public class ClosePreviewScreen : MonoBehaviour {
        private MiniatureModel WIM;


        private void Awake() {
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

        private void Start() {
            GetComponent<Renderer>().enabled = true;
        }

        private bool once;
        private void OnTriggerEnter(Collider other) {
            if(!other.CompareTag("IndexR") && !other.CompareTag("IndexL")) return;
            if (once) return;
            once = true;
            var hand = other.CompareTag("IndexR") ? Hand.RightHand : Hand.LeftHand;
            StartClosing(hand);
        }

        private void StartClosing(Hand hand) {
            InputManager.SetVibration(frequency: .5f, amplitude: .1f, hand);
            StartCoroutine(Close(hand, .1f));
        }

        private IEnumerator Close(Hand hand, float time) {
            yield return new WaitForSeconds(time);
            InputManager.SetVibration(frequency: 0, amplitude: 0, hand);
            WIM.GetComponent<PreviewScreen>().RemovePreviewScreen();
        }
    }
}