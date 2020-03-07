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
                WIMVisualizationUtils.DissolveWIM(oldWIMTransform);
            } else {
                WIMVisualizationUtils.InstantDissolveWIM(oldWIMTransform);
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
            StartCoroutine(WIMVisualizationUtils.FixResolveBug(WIMLevel, resolveDuration));
        }
    }
}