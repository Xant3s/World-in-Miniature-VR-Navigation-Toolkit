﻿using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    [DisallowMultipleComponent]
    public class PreviewScreen : MonoBehaviour {
        public PreviewScreenConfiguration Config;
        [HideInInspector] public PreviewScreenData Data;

        private WIMConfiguration WIMConfig;
        private WIMData WIMData;
        private Material previewScreenMaterial;
        private static readonly int baseMap = Shader.PropertyToID("_BaseMap");


        private void Awake() {
            if(!Config) return;
            Data = ScriptableObject.CreateInstance<PreviewScreenData>();
        }

        private void OnEnable() {
            if(!Config) return;
            MiniatureModel.OnNewDestinationSelected += ShowPreviewScreen;
            DestinationIndicators.OnSpawnDestinationIndicatorInWIM += configurePickupPreviewScreen;
            DestinationIndicators.OnRemoveDestinationIndicators += RemovePreviewScreen;
            PickupDestinationUpdate.OnRemoveDestinationIndicatorExceptWIM += RemovePreviewScreen;
        }

        private void OnDisable() {
            if(!Config) return;
            MiniatureModel.OnNewDestinationSelected -= ShowPreviewScreen;
            DestinationIndicators.OnSpawnDestinationIndicatorInWIM -= configurePickupPreviewScreen;
            DestinationIndicators.OnRemoveDestinationIndicators -= RemovePreviewScreen;
            PickupDestinationUpdate.OnRemoveDestinationIndicatorExceptWIM -= RemovePreviewScreen;
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
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer("WIM"));
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer("Hands"));
            cam.targetTexture = new RenderTexture(1600, 900, 16, RenderTextureFormat.Default);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.gray;
            previewScreenMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            previewScreen.GetComponent<Renderer>().material = previewScreenMaterial;
            previewScreenMaterial.SetTexture(baseMap, cam.targetTexture);
        }

        public void RemovePreviewScreen() {
            if(!Data) return;
            Data.PreviewScreenEnabled = false;
            var previewScreen = GameObject.FindGameObjectWithTag("PreviewScreen");
            if(!previewScreen) return;
            previewScreen.transform.parent = null;
            Destroy(previewScreen);
        }

        private void RemovePreviewScreen(WIMConfiguration config, WIMData data) {
            RemovePreviewScreen();
        }

        private void RemovePreviewScreen(in MiniatureModel WIM) {
            RemovePreviewScreen();
        }

        private void configurePickupPreviewScreen(WIMConfiguration WIMConfig, WIMData WIMData) {
            if(!Config || Config.AutoPositionPreviewScreen) return;
            WIMData.DestinationIndicatorInWIM.GetChild(1).GetChild(0).gameObject
                .AddComponent<PickupPreviewScreen>();
        }
    }
}