// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEngine;

namespace WIMVR.Features.Occlusion_Handling {
    /// <summary>
    /// Updates the capsule mask that is used to hide parts of the miniature model.
    /// </summary>
    [ExecuteAlways]
    public class CapsuleController : MonoBehaviour {
        private static readonly int p1 = Shader.PropertyToID("_P1");
        private static readonly int p2 = Shader.PropertyToID("_P2");
        private static readonly int r = Shader.PropertyToID("_Radius");
        private static readonly int capsuleEnabled = Shader.PropertyToID("_CapsuleEnabled");
        public Material[] materials;

        public void SetCapsuleEnabled(bool value) {
            try {
                foreach (var material in materials) {
                    material.SetFloat(capsuleEnabled, value ? 1 : 0);
                }
            }
            catch (Exception) {
                // ignored
            }
        }


        private void OnEnable() {
            SetCapsuleEnabled(true);
        }

        private void OnDisable() {
            SetCapsuleEnabled(false);
        }

        private void Update() {
            if (materials == null || materials.Length == 0) return;
            foreach (var material in materials) {
                var position = transform.position;
                var radius = transform.localScale.x;
                var point1 = position + transform.up * transform.localScale.y / 2.0f;
                var point2 = position - transform.up * transform.localScale.y / 2.0f;
                material.SetVector(p1, point1);
                material.SetVector(p2, point2);
                material.SetFloat(r, radius);
            }
        }
    }
}
