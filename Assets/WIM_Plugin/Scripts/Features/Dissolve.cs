using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    public class Dissolve : MonoBehaviour {
        [Range(0.1f, 10.0f)] public float durationInSeconds = 1.0f;
        private Material mat;
        private bool isInverse;
        private float endTime;
        private static readonly int progressProperty = Shader.PropertyToID("Vector1_461A9E8C");

        private void Start() {
            mat = GetComponent<Renderer>().material;
        }

        private void Update() {
            if(Input.GetKeyUp(KeyCode.A)) {
                Play();
            }
            else if(Input.GetKeyUp(KeyCode.D)) {
                PlayInverse();
            }

            if(!(Time.realtimeSinceStartup < endTime)) return;
            var remainingTime = endTime - Time.realtimeSinceStartup;
            var percent = (durationInSeconds - remainingTime) / durationInSeconds;
            var progress = !isInverse ? percent : 1 - percent;
            mat.SetFloat(progressProperty, progress);
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
            if(!mat) {
                mat = GetComponent<Renderer>().material;
            }

            if(!mat) return;
            mat.SetFloat(progressProperty, progress);
        }
    }
}