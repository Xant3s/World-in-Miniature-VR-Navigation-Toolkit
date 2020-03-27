﻿using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    [DisallowMultipleComponent]
    public class PickupPreviewScreen : MonoBehaviour {
        public bool HightlightFX {
            get => hightlightFX;
            set {
                hightlightFX = value;
                if(GetComponent<Renderer>()) {
                    material.SetFloat("_Alpha", value ? defaultAlpha + .1f : defaultAlpha);
                }
            }
        }

        private MiniatureModel WIM;
        private Material material;
        private float defaultAlpha = .33f;
        private bool hightlightFX;
        private Transform thumb;
        private Transform index;
        private Transform WIMTransform;
        private bool thumbIsGrabbing;
        private bool indexIsGrabbing;
        private bool isGrabbing;
        private bool stoppedGrabbing;


        private void Awake() {
            thumb = GameObject.FindWithTag("ThumbR")?.transform;
            index = GameObject.FindWithTag("IndexR")?.transform;
            WIMTransform = GameObject.FindWithTag("WIM")?.transform;
            Assert.IsNotNull(WIMTransform);
            WIM = WIMTransform.GetComponent<MiniatureModel>();
            material = GetComponent<Renderer>()?.material;
            defaultAlpha = material.GetFloat("_Alpha");
            Assert.IsNotNull(WIM);
            Assert.IsNotNull(thumb);
            Assert.IsNotNull(index);
        }

        private void OnEnable() {
            MiniatureModel.OnPickupIndexButtonUp += pickupIndexButtonUp;
            MiniatureModel.OnPickupThumbButtonUp += pickupThumbButtonUp;
            MiniatureModel.OnPickupThumbTouchUp += pickupThumbTouchUp;
        }

        private void OnDisable() {
            MiniatureModel.OnPickupIndexButtonUp -= pickupIndexButtonUp;
            MiniatureModel.OnPickupThumbButtonUp -= pickupThumbButtonUp;
            MiniatureModel.OnPickupThumbTouchUp -= pickupThumbTouchUp;
        }

        private void pickupIndexButtonUp(WIMConfiguration config, WIMData data) {
            if(!isGrabbing) stopGrabbing();
        }

        private void pickupThumbButtonUp(WIMConfiguration config, WIMData data) {
            if(!isGrabbing) stopGrabbing();
        }

        private void pickupThumbTouchUp(WIMConfiguration config, WIMData data) {
            if(!isGrabbing) stopGrabbing();
        }

        private void Update() {
            var rightHandPinch = thumbIsGrabbing && indexIsGrabbing;
            if(rightHandPinch && !isGrabbing) {
                isGrabbing = true;
                stoppedGrabbing = false;
                startGrabbing();
            }
            else if(isGrabbing && !rightHandPinch) {
                isGrabbing = false;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if(other.transform == thumb) {
                thumbIsGrabbing = true;
                HightlightFX = true;
                vibrate();
            }
            else if(other.transform == index) {
                indexIsGrabbing = true;
                HightlightFX = true;
                vibrate();
            }
        }

        private void OnTriggerExit(Collider other) {
            if(other.transform == thumb) {
                thumbIsGrabbing = false;
                HightlightFX = false;
            }
            else if(other.transform == index) {
                indexIsGrabbing = false;
                HightlightFX = false;
            }
        }

        private void startGrabbing() {
            // Spawn new preview screen.
            var previewScreen = WIMTransform.GetComponent<PreviewScreen>();
            previewScreen.ShowPreviewScreenPickup(WIM.Configuration, WIM.Data);
            var previewScreenTransform = previewScreen.Data.PreviewScreenTransform;
            Assert.IsNotNull(previewScreenTransform);

            // Pick up the new preview screen.
            previewScreenTransform.parent = index;
            previewScreenTransform.localPosition = Vector3.zero;
        }

        private void stopGrabbing() {
            if(stoppedGrabbing) return;
            var previewScreen = WIM.GetComponent<PreviewScreen>();
            Assert.IsNotNull(WIM);
            var previewScreenTransform = previewScreen.Data.PreviewScreenTransform;
            if(!previewScreenTransform) return;
            previewScreenTransform.gameObject.AddComponent<PreviewScreenController>();
            previewScreenTransform.GetChild(0).gameObject.AddComponent<ClosePreviewScreen>();

            // Let go.
            previewScreenTransform.parent = WIMTransform.GetChild(0);
            stoppedGrabbing = true;
        }

        private void vibrate() {
            InputManager.SetVibration(frequency: .5f, amplitude: .1f, Hand.RightHand);
            Invoke(nameof(stopVibration), time: .1f);
        }

        private void stopVibration() {
            InputManager.SetVibration(frequency: 0, amplitude: 0, Hand.RightHand);
        }
    }
}