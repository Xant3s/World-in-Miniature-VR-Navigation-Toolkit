using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    public class Dissolve : MonoBehaviour {
        [Range(0.1f, 10.0f)] public float durationInSeconds = 1.0f;
        //private Material mat;
        private bool isInverse;
        private float endTime;
        private static readonly int progressProperty = Shader.PropertyToID("_Progress");


        //private void Awake() {
        //    mat = GetComponent<Renderer>()?.material;
        //    Assert.IsNotNull(mat);
        //}

        private void Start() {
            //mat = GetComponent<Renderer>().material;
            var boxMask = GameObject.FindWithTag("BoxMask").GetComponent<BoxController>();
            var capsuleMask = GameObject.FindWithTag("CapsuleMask").GetComponent<CapsuleController>();

            var newMats  = boxMask.materials.ToList();
            newMats.Add(GetComponent<Renderer>().material);
            boxMask.materials = newMats.ToArray();
        }

        private void Update() {
            if (!(Time.realtimeSinceStartup < endTime)) return;
            var remainingTime = endTime - Time.realtimeSinceStartup;
            var percent = (durationInSeconds - remainingTime) / durationInSeconds;
            //var mat = GetComponent<Renderer>().material;
            var progress = !isInverse ? percent : 1 - percent;
            GetComponent<Renderer>().material.SetFloat(progressProperty, progress);
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
            //if (!mat) {
            //    mat = GetComponent<Renderer>().material;
            //}

            //if (!mat) return;
            GetComponent<Renderer>().material.SetFloat(progressProperty, progress);
        }
    }
}