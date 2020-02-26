using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    public static class WIMVisualizationUtils {
        internal static void DissolveWIM(Transform WIM) {
            //foreach (Transform child in WIM) {
            //var d = child.GetComponent<Dissolve>();
            var d = WIM.GetComponent<Dissolve>();
            if (!d) return;
            d.durationInSeconds = 1f;
            d.Play();
            //}

        }

        internal static void InstantDissolveWIM(Transform WIM) {
            //foreach (Transform child in WIM) {
            //var d = child.GetComponent<Dissolve>();
            var d = WIM.GetComponent<Dissolve>();
            if (!d) return;
            d.SetProgress(1);
            //}
        }

        /// <summary>
        /// At the end of the resolve effect the WIM will not be completely resolved due to float precision.
        /// To prevent this, set the dissolve progress to a negative number (everything below 0 will be handled as 0 anyway).
        /// </summary>
        /// <param name="WIM"></param>
        /// <param name="delay"></param>
        internal static IEnumerator FixResolveBug(Transform WIM, float delay) {
            yield return new WaitForSeconds(delay);
            //for (var i = 0; i < WIM.childCount; i++) {
            //var d = WIM.GetChild(i).GetComponent<Dissolve>();
            var d = WIM.GetComponent<Dissolve>();
            if (!d) yield break;
            d.durationInSeconds = 1;
            d.SetProgress(-0.1f);
            //}
        }
    }
}