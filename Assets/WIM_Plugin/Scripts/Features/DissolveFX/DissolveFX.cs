using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WIM_Plugin {
    [ExecuteAlways]
    public class DissolveFX : MonoBehaviour {
        private void OnEnable() {
            Respawn.OnEarlyRespawn += dissolveOldWIM;
            Respawn.OnLateRespawn += resolveNewWIM;
        }

        private void OnDisable() {
            Respawn.OnEarlyRespawn -= dissolveOldWIM;
            Respawn.OnLateRespawn -= resolveNewWIM;
        }

        private void dissolveOldWIM(in Transform oldWIMTransform, in Transform newWIMTransform,
            bool maintainTransformRelativeToPlayer) {
            oldWIMTransform.gameObject.AddComponent<Dissolve>().materials = new[] {Respawn.materialForOldWIM};
            if(!maintainTransformRelativeToPlayer) {
                DissolveWIM(oldWIMTransform);
            } else {
                InstantDissolveWIM(oldWIMTransform);
            }
            Respawn.RemoveOldWIMLevel = false;
        }

        private void resolveNewWIM(in Transform oldWIMTransform, in Transform newWIMTransform,
            bool maintainTransformRelativeToPlayer) {
            resolveWIM(newWIMTransform);
            Invoke(nameof(destroyOldWIMLevel), 1.1f);
        }

        private void destroyOldWIMLevel() {
            Destroy(GameObject.FindWithTag("WIM Level Old"));
        }

        private void resolveWIM(Transform WIMLevel) {
            const int resolveDuration = 1;
            var d = WIMLevel.GetComponent<Dissolve>();
            if(!d) {
                d = WIMLevel.gameObject.AddComponent<Dissolve>();
                d.materials = new[] {WIMLevel.GetComponentInChildren<Renderer>().sharedMaterial};
            }
            d.durationInSeconds = resolveDuration;
            d.SetProgress(1);
            d.PlayInverse();
            StartCoroutine(FixResolveBug(WIMLevel, resolveDuration));
        }

        private static void DissolveWIM(Transform WIM) {
            var d = WIM.GetComponent<Dissolve>();
            if(!d) return;
            d.durationInSeconds = 1f;
            d.Play();
        }

        private static void InstantDissolveWIM(Transform WIM) {
            var d = WIM.GetComponent<Dissolve>();
            if(!d) return;
            d.SetProgress(1);
        }

        /// <summary>
        /// At the end of the resolve effect the WIM will not be completely resolved due to float precision.
        /// To prevent this, set the dissolve progress to a negative number (everything below 0 will be handled as 0 anyway).
        /// </summary>
        /// <param name="WIM"></param>
        /// <param name="delay"></param>
        private static IEnumerator FixResolveBug(Transform WIM, float delay) {
            yield return new WaitForSeconds(delay);
            var d = WIM.GetComponent<Dissolve>();
            if(!d) yield break;
            d.durationInSeconds = 1;
            d.SetProgress(-0.1f);
        }
    }
}