// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections;
using UnityEngine;


namespace WIMVR {
    /// <summary>
    /// Adds a dissolve-resolve effect when respawning miniature model.
    /// </summary>
    [DisallowMultipleComponent]
    public class DissolveFX : MonoBehaviour {
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

        private void OnEnable() {
            Respawn.OnEarlyRespawn += DissolveOldWIM;
            Respawn.OnLateRespawn += ResolveNewWIM;
            Respawn.RemoveOldWIMLevel = false;
        }

        private void OnDisable() {
            Respawn.OnEarlyRespawn -= DissolveOldWIM;
            Respawn.OnLateRespawn -= ResolveNewWIM;
            Respawn.RemoveOldWIMLevel = true;
        }

        private void Awake() {
            var WIM = GameObject.FindGameObjectWithTag("WIM");
            if(!WIM) return;
            WIM.AddComponent<Dissolve>().materials = new[] {WIM.GetComponentInChildren<Renderer>().sharedMaterial};
        }

        private void DissolveOldWIM(in Transform oldWIMTransform, in Transform newWIMTransform,
            bool maintainTransformRelativeToPlayer) {
            oldWIMTransform.gameObject.AddComponent<Dissolve>().materials = new[] {Respawn.materialForOldWIM};
            if(!maintainTransformRelativeToPlayer) {
                DissolveWIM(oldWIMTransform);
            } else {
                InstantDissolveWIM(oldWIMTransform);
            }
        }

        private void ResolveNewWIM(in Transform oldWIMTransform, in Transform newWIMTransform,
            bool maintainTransformRelativeToPlayer) {
            ResolveWIM(newWIMTransform);
            Invoke(nameof(DestroyOldWIMLevel), 1.1f);
        }

        private void DestroyOldWIMLevel() {
            Destroy(GameObject.FindWithTag("WIM Level Old"));
        }

        private void ResolveWIM(Transform WIMLevel) {
            const int resolveDuration = 1;
            var d = WIMLevel.GetComponentInParent<Dissolve>();
            if(!d) return;
            d.durationInSeconds = resolveDuration;
            d.SetProgress(1);
            d.PlayInverse();
            StartCoroutine(FixResolveBug(WIMLevel, resolveDuration));
        }
    }
}