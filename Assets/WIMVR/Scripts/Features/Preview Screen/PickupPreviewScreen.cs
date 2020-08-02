// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Util;
using WIMVR.Input;
using WIMVR.Util.Haptics;
using WIMVR.Util.XR;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// Used to open the preview screen when the player grabs the destination indicator's view cone.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DetectPickupGesture))]
    public class PickupPreviewScreen : MonoBehaviour {
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

        /// <summary>
        /// The highlight effect displayed when the player touches the destination indicator's view cone.
        /// </summary>
        public bool HightlightFX {
            get => hightlightFX;
            set {
                hightlightFX = value;
                if(GetComponent<Renderer>()) {
                    material.SetFloat("_Alpha", value ? defaultAlpha + .1f : defaultAlpha);
                }
            }
        }


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

        private void Start() {
            var detectPickupGesture = GetComponent<DetectPickupGesture>();
            Assert.IsNotNull(detectPickupGesture);
            detectPickupGesture.OnStartGrabbing.AddListener(StartGrabbing);
            detectPickupGesture.OnStopGrabbing.AddListener(StopGrabbing);

        }

        private void OnEnable() {
            //MiniatureModel.OnPickupIndexButtonUp += PickupIndexButtonUp;
            //MiniatureModel.OnPickupThumbButtonUp += PickupThumbButtonUp;
            //MiniatureModel.OnPickupThumbTouchUp += PickupThumbTouchUp;
        }

        private void OnDisable() {
            //MiniatureModel.OnPickupIndexButtonUp -= PickupIndexButtonUp;
            //MiniatureModel.OnPickupThumbButtonUp -= PickupThumbButtonUp;
            //MiniatureModel.OnPickupThumbTouchUp -= PickupThumbTouchUp;
        }

        //private void PickupIndexButtonUp(WIMConfiguration config, WIMData data) {
        //    if(!isGrabbing) StopGrabbing();
        //}

        //private void PickupThumbButtonUp(WIMConfiguration config, WIMData data) {
        //    if(!isGrabbing) StopGrabbing();
        //}

        //private void PickupThumbTouchUp(WIMConfiguration config, WIMData data) {
        //    if(!isGrabbing) StopGrabbing();
        //}

        //private void Update() {
        //    var rightHandPinch = thumbIsGrabbing && indexIsGrabbing;
        //    if(rightHandPinch && !isGrabbing) {
        //        isGrabbing = true;
        //        stoppedGrabbing = false;
        //        StartGrabbing();
        //    }
        //    else if(isGrabbing && !rightHandPinch) {
        //        isGrabbing = false;
        //    }
        //}

        private void OnTriggerEnter(Collider other) {
            if(other.transform == thumb) {
                thumbIsGrabbing = true;
                HightlightFX = true;
                Vibrate();
            }
            else if(other.transform == index) {
                indexIsGrabbing = true;
                HightlightFX = true;
                Vibrate();
            }
        }

        //private void OnTriggerExit(Collider other) {
        //    if(other.transform == thumb) {
        //        thumbIsGrabbing = false;
        //        HightlightFX = false;
        //    }
        //    else if(other.transform == index) {
        //        indexIsGrabbing = false;
        //        HightlightFX = false;
        //    }
        //}

        private void StartTouch(Hand hand) {
            // Haptic feedback.
            var inputDevice = XRUtils.FindCorrespondingInputDevice(hand);
            Haptics.Vibrate(inputDevice, .1f, .1f);

            // Visual feedback.
            HightlightFX = true;
        }

        private void StopTouch() {
            HightlightFX = false;
        }

        private void StartGrabbing() {
            // Spawn new preview screen.
            var previewScreen = WIMTransform.GetComponent<PreviewScreen>();
            previewScreen.ShowPreviewScreenPickup(WIM.Configuration, WIM.Data);
            var previewScreenTransform = previewScreen.Data.PreviewScreenTransform;
            Assert.IsNotNull(previewScreenTransform);

            // Pick up the new preview screen.
            previewScreenTransform.parent = index;
            previewScreenTransform.localPosition = Vector3.zero;
        }

        private void StopGrabbing() {
            if(stoppedGrabbing) return;
            var previewScreen = WIM.GetComponent<PreviewScreen>();
            Assert.IsNotNull(WIM);
            var previewScreenTransform = previewScreen.Data.PreviewScreenTransform;
            if(!previewScreenTransform) return;
            previewScreenTransform.GetChild(0).gameObject.AddComponent<PreviewScreenController>();
            previewScreenTransform.GetChild(0).GetChild(0).gameObject.AddComponent<ClosePreviewScreen>();

            // Let go.
            previewScreenTransform.parent = WIMTransform.GetChild(0);
            stoppedGrabbing = true;
        }

        private void Vibrate() {
            InputManager.SetVibration(frequency: .5f, amplitude: .1f, Hand.RightHand);
            Invoke(nameof(StopVibration), time: .1f);
        }

        private void StopVibration() {
            InputManager.SetVibration(frequency: 0, amplitude: 0, Hand.RightHand);
        }
    }
}