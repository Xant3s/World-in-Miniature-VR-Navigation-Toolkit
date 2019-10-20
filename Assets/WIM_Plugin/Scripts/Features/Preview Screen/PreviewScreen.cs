using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    public class PreviewScreen : MonoBehaviour {
        private WIMConfiguration config;
        private WIMData data;
        private Material previewScreenMaterial;
        private static readonly int baseMap = Shader.PropertyToID("_BaseMap");

        private void OnEnable() {
            MiniatureModel.OnNewDestinationSelected += ShowPreviewScreen;
            MiniatureModel.OnUpdate += updatePreviewScreen;
        }

        private void OnDisable() {
            MiniatureModel.OnNewDestinationSelected -= ShowPreviewScreen;
            MiniatureModel.OnUpdate -= updatePreviewScreen;
        }

        public void ShowPreviewScreen(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
            if(!config.PreviewScreen || !config.AutoPositionPreviewScreen) return;
            RemovePreviewScreen();
            data.PreviewScreenTransform = Instantiate(Resources.Load<GameObject>("Prefabs/Preview Screen")).transform;
            data.PreviewScreenTransform.GetComponent<FloatAbove>().Target = transform;
            initPreviewScreen(data.PreviewScreenTransform.gameObject);
            data.PreviewScreenEnabled = true;
        }

        public void ShowPreviewScreenPickup(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
            if(!config.PreviewScreen) return;
            Assert.IsFalse(config.AutoPositionPreviewScreen);
            RemovePreviewScreen();
            data.PreviewScreenTransform = Instantiate(Resources.Load<GameObject>("Prefabs/Preview Screen")).transform;
            Destroy(data.PreviewScreenTransform.GetComponent<FloatAbove>());
            data.PreviewScreenTransform.gameObject.AddComponent<PreviewScreenController>();
            initPreviewScreen(data.PreviewScreenTransform.gameObject);
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
            previewScreenMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Unlit")); // TODO: Universal Render Pipeline in newer Unity versions.
            previewScreen.GetComponent<Renderer>().material = previewScreenMaterial;
            previewScreenMaterial.SetTexture(baseMap, cam.targetTexture);
        }

        private void updatePreviewScreen(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;

            if (!data.PreviewScreenEnabled) return;
            if(!config.PreviewScreen || !data.DestinationIndicatorInLevel) return;
            var cam = data.DestinationIndicatorInLevel.GetChild(1).GetComponent<Camera>();
            Destroy(cam.targetTexture);
            cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
            if(!previewScreenMaterial) {
                Debug.LogError("Preview screen material is null");
                previewScreenMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Unlit"));
                GameObject.FindWithTag("PreviewScreen").GetComponent<Renderer>().material = previewScreenMaterial;
            }

            previewScreenMaterial.SetTexture(baseMap, cam.targetTexture);
        }

        public void RemovePreviewScreen() {
            data.PreviewScreenEnabled = false;
            var previewScreen = GameObject.FindGameObjectWithTag("PreviewScreen");
            if(!previewScreen) return;
            previewScreen.transform.parent = null;
            Destroy(previewScreen);
        }
    }
}