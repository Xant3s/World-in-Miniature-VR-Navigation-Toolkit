// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Core;
using WIMVR.Util.Extensions;

namespace WIMVR.Features.DissolveFX {
    /// <summary>
    /// Adds a dissolve-resolve effect when respawning miniature model.
    /// </summary>
    [DisallowMultipleComponent]
    public class DissolveFX : MonoBehaviour {
        private void OnEnable() {
            Respawn.OnEarlyRespawn += DissolveOldWIM;
            Respawn.OnLateRespawn += ResolveNewWIM;
            Respawn.removeOldWIMLevel = false;
        }

        private void OnDisable() {
            Respawn.OnEarlyRespawn -= DissolveOldWIM;
            Respawn.OnLateRespawn -= ResolveNewWIM;
            Respawn.removeOldWIMLevel = true;
        }

        private void Awake() {
            var WIM = FindObjectOfType<MiniatureModel>().gameObject;
            if(!WIM) return;
            WIM.AddComponent<Dissolve>().materials = new[] {WIM.GetComponentInChildren<Renderer>().sharedMaterial};
        }

        private void DissolveOldWIM(in Transform oldWIMTransform, in Transform newWIMTransform,
            bool maintainTransformRelativeToPlayer) {
            var oldWIM = oldWIMTransform.gameObject;
            PlayDissolveEffect(oldWIMTransform, maintainTransformRelativeToPlayer);
            this.Invoke(() => Destroy(oldWIM), 1.1f);
        }

        private static void PlayDissolveEffect(Transform oldWIMTransform, bool maintainTransformRelativeToPlayer) {
            oldWIMTransform.gameObject.AddComponent<Dissolve>().materials = new[] {Respawn.materialForOldWIM};
            if(!maintainTransformRelativeToPlayer) {
                DissolveWIM(oldWIMTransform);
            } else {
                InstantDissolveWIM(oldWIMTransform);
            }
        }

        private static void DissolveWIM(Transform WIM) {
            var dissolve = WIM.GetComponent<Dissolve>();
            if(!dissolve) return;
            dissolve.durationInSeconds = 1f;
            dissolve.Play();
        }

        private static void InstantDissolveWIM(Transform WIM) {
            var d = WIM.GetComponent<Dissolve>();
            if(!d) return;
            d.SetProgress(1);
        }

        private void ResolveNewWIM(in Transform oldWIMTransform, in Transform newWIMTransform, bool maintainTransformRelativeToPlayer)
            => ResolveWIM(newWIMTransform);

        private static void ResolveWIM(Transform WIMLevel) {
            const int resolveDuration = 1;
            var d = WIMLevel.GetComponentInParent<Dissolve>();
            if(!d) return;
            d.durationInSeconds = resolveDuration;
            d.SetProgress(1);
            d.PlayInverse();
        }
    }
}