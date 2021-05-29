using System;
using System.Collections;
using UnityEngine;

namespace WIMVR.Util.Extensions {
    public static class MonoBehaviourExtensions {
        public static void Invoke(this MonoBehaviour mb, Action f, float delayInSeconds)
            => mb.StartCoroutine(InvokeDelayed(f, delayInSeconds));

        private static IEnumerator InvokeDelayed(Action f, float delayInSeconds) {
            yield return new WaitForSeconds(delayInSeconds);
            f();
        }
    }
}