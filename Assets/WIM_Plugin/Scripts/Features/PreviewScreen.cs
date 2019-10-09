﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    public class PreviewScreen : MonoBehaviour {
        private WIMConfiguration config;
        private WIMData data;
        private Material previewScreenMaterial;

        void OnEnable() {
            MiniatureModel.OnNewDestinationSelected += ShowPreviewScreen;
            MiniatureModel.OnUpdate += updatePreviewScreen;
        }

        void OnDisable() {
            MiniatureModel.OnNewDestinationSelected -= ShowPreviewScreen;
            MiniatureModel.OnUpdate -= updatePreviewScreen;
        }

        public void ShowPreviewScreen(WIMConfiguration config, WIMData data) {
            if(!config.PreviewScreen) return;
            RemovePreviewScreen();
            var previewScreen = Instantiate(Resources.Load<GameObject>("Prefabs/Preview Screen"));

            if(config.AutoPositionPreviewScreen) {
                previewScreen.GetComponent<FloatAbove>().Target = transform;
            }
            else {
                Destroy(previewScreen.GetComponent<FloatAbove>());
                previewScreen.AddComponent<ClosePreviewScreen>();
            }

            initPreviewScreen(previewScreen);
            data.PreviewScreenEnabled = true;
        }

        private void initPreviewScreen(GameObject previewScreen) {
            Assert.IsNotNull(data.DestinationIndicatorInLevel);
            Assert.IsNotNull(data.DestinationIndicatorInLevel.GetChild(1));
            var camObj = data.DestinationIndicatorInLevel.GetChild(1).gameObject; // Making assumptions on the prefab.
            Assert.IsNotNull(camObj);
            var cam = camObj.GetComponent<Camera>() ?? camObj.AddComponent<Camera>();
            Assert.IsNotNull(cam);
            cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.gray;
            previewScreenMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Unlit"));
            previewScreen.GetComponent<Renderer>().material = previewScreenMaterial;
            previewScreenMaterial.SetTexture("_BaseMap", cam.targetTexture);
        }

        private void updatePreviewScreen(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
            if(!data.PreviewScreenEnabled) return;
            if(!config.PreviewScreen || !data.DestinationIndicatorInLevel) return;
            var cam = data.DestinationIndicatorInLevel.GetChild(1).GetComponent<Camera>();
            Destroy(cam.targetTexture);
            cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
            if(!previewScreenMaterial) {
                Debug.LogError("Preview screen material is null");
                previewScreenMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Unlit"));
                GameObject.FindWithTag("PreviewScreen").GetComponent<Renderer>().material = previewScreenMaterial;
            }

            previewScreenMaterial.SetTexture("_BaseMap", cam.targetTexture);
        }

        public void RemovePreviewScreen() {
            data.PreviewScreenEnabled = false;
            var previewScreen = GameObject.FindGameObjectWithTag("PreviewScreen");
            if(!previewScreen) return;
            previewScreen.transform.parent = null;
            Destroy(previewScreen);
        }
    }


    public class ClosePreviewScreen : MonoBehaviour {
        private float DoubleTapInterval { get; set; } = 2;

        private Transform index;
        private bool firstTap = false;

        void Awake() {
            index = GameObject.FindWithTag("IndexR").transform;
            Assert.IsNotNull(index);
        }

        void OnTriggerEnter(Collider other) {
            if(other.transform != index) return;
            if(transform.root.tag == "HandR") return;
            if(firstTap) {
                var WIM = GameObject.Find("WIM").GetComponent<MiniatureModel>();
                WIM.GetComponent<PreviewScreen>().RemovePreviewScreen();
            }
            else {
                firstTap = true;
                Invoke("resetDoubleTap", DoubleTapInterval);
            }
        }

        private void resetDoubleTap() {
            firstTap = false;
        }
    }
}