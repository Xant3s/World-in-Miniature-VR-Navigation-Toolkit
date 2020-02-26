using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WIM_Plugin {
    public class Dissolve : MonoBehaviour {
        [Range(0.1f, 10.0f)] public float durationInSeconds = 1.0f;
        public Material[] materials;
        private bool isInverse;
        private float endTime;
        private static readonly int progressProperty = Shader.PropertyToID("_Progress");


        private void Update() {
            if (!(Time.realtimeSinceStartup < endTime)) return;
            var remainingTime = endTime - Time.realtimeSinceStartup;
            var percent = (durationInSeconds - remainingTime) / durationInSeconds;
            var progress = !isInverse ? percent : 1 - percent;
            SetProgress(progress);
            //Debug.Log(progress);
        }

        public void Play() {
            isInverse = false;
            endTime = Time.realtimeSinceStartup + durationInSeconds;
        }

        public void PlayInverse() {
            isInverse = true;
            endTime = Time.realtimeSinceStartup + durationInSeconds;
        }

        public void SetProgress(float progress) {
            foreach(var mat in materials) {
                mat.SetFloat(progressProperty, progress);
            }
        }
    }
}