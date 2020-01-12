using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    public class PreviewScreen : MonoBehaviour {
        public PreviewScreenConfiguration Config;
        [HideInInspector] public PreviewScreenData Data;

        private WIMConfiguration WIMConfig;
        private WIMData WIMData;
        private Material previewScreenMaterial;
        private static readonly int baseMap = Shader.PropertyToID("_BaseMap");


        private void Awake() {
            Data = ScriptableObject.CreateInstance<PreviewScreenData>();
        }

        private void OnEnable() {
            MiniatureModel.OnNewDestinationSelected += ShowPreviewScreen;
            MiniatureModel.OnUpdate += updatePreviewScreen;
        }

        private void OnDisable() {
            MiniatureModel.OnNewDestinationSelected -= ShowPreviewScreen;
            MiniatureModel.OnUpdate -= updatePreviewScreen;
        }

        public void ShowPreviewScreen(WIMConfiguration WIMConfig, WIMData WIMData) {
            this.WIMConfig = WIMConfig;
            this.WIMData = WIMData;
            Assert.IsNotNull(this.Config, "Preview screen configuration is missing.");

            if(!this.Config.PreviewScreen || !this.Config.AutoPositionPreviewScreen) return;
            RemovePreviewScreen();
            Data.PreviewScreenTransform = Instantiate(Resources.Load<GameObject>("Prefabs/Preview Screen")).transform;
            Data.PreviewScreenTransform.GetComponent<FloatAbove>().Target = transform;
            initPreviewScreen(Data.PreviewScreenTransform.gameObject);
            Data.PreviewScreenEnabled = true;
        }

        public void ShowPreviewScreenPickup(WIMConfiguration WIMConfig, WIMData WIMData) {
            this.WIMConfig = WIMConfig;
            this.WIMData = WIMData;
            if(!this.Config.PreviewScreen) return;
            Assert.IsFalse(this.Config.AutoPositionPreviewScreen);
            RemovePreviewScreen();
            Data.PreviewScreenTransform = Instantiate(Resources.Load<GameObject>("Prefabs/Preview Screen")).transform;
            Destroy(Data.PreviewScreenTransform.GetComponent<FloatAbove>());
            initPreviewScreen(Data.PreviewScreenTransform.gameObject);
            Data.PreviewScreenEnabled = true;
        }

        private void initPreviewScreen(GameObject previewScreen) {
            Assert.IsNotNull(WIMData.DestinationIndicatorInLevel);
            Assert.IsNotNull(WIMData.DestinationIndicatorInLevel.GetChild(1));
            var camObj = WIMData.DestinationIndicatorInLevel.GetChild(1).gameObject; // Making assumptions on the prefab.
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

        private void updatePreviewScreen(WIMConfiguration WIMConfig, WIMData WIMData) {
            this.WIMConfig = WIMConfig;
            this.WIMData = WIMData;
            Assert.IsNotNull(this.Config, "Preview screen configuration is missing.");

            if (!Data.PreviewScreenEnabled) return;
            if(!this.Config.PreviewScreen || !WIMData.DestinationIndicatorInLevel) return;
            var cam = WIMData.DestinationIndicatorInLevel.GetChild(1).GetComponent<Camera>();
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
            Data.PreviewScreenEnabled = false;
            var previewScreen = GameObject.FindGameObjectWithTag("PreviewScreen");
            if(!previewScreen) return;
            previewScreen.transform.parent = null;
            Destroy(previewScreen);
        }
    }
}