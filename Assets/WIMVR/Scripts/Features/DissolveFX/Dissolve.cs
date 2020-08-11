// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Features.DissolveFX {
    /// <summary>
    /// Plays the dissolve/resolve effect.
    /// </summary>
    public class Dissolve : MonoBehaviour {
        private static readonly int progressProperty = Shader.PropertyToID("_Progress");
        [Range(0.1f, 10.0f)] public float durationInSeconds = 1.0f;
        public Material[] materials;
        private bool isInverse;
        private float endTime;
        

        private void OnDestroy() {
            SetProgress(0);
        }

        /// <summary>
        /// Plays dissolve effect.
        /// </summary>
        public void Play() {
            isInverse = false;
            endTime = Time.realtimeSinceStartup + durationInSeconds;
        }

        /// <summary>
        /// Plays resolve effect.
        /// </summary>
        public void PlayInverse() {
            isInverse = true;
            endTime = Time.realtimeSinceStartup + durationInSeconds;
        }

        /// <summary>
        /// Sets the dissolve progress to specified value.
        /// </summary>
        /// <param name="progress">new dissolve progress value.</param>
        public void SetProgress(float progress) {
            foreach(var mat in materials) {
                mat.SetFloat(progressProperty, progress);
            }
        }

        private void Update() {
            if (!(Time.realtimeSinceStartup < endTime)) return;
            var remainingTime = endTime - Time.realtimeSinceStartup;
            var percent = (durationInSeconds - remainingTime) / durationInSeconds;
            var progress = !isInverse ? percent : 1 - percent;
            SetProgress(progress);
        }
    }
}