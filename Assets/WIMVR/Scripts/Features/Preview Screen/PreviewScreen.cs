// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Features.Pickup_Destination;
using WIMVR.Util;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// Can be used to display a screen next to the miniature model that shows the selected destination.
    /// </summary>
    [DisallowMultipleComponent]
    public class PreviewScreen : MonoBehaviour {
        private static readonly int baseMap = Shader.PropertyToID("_BaseMap");
        public PreviewScreenConfiguration Config;
        [HideInInspector] public PreviewScreenData Data;

        private WIMConfiguration WIMConfig;
        private WIMData WIMData;
        private Material previewScreenMaterial;

        
        public void ShowPreviewScreen(WIMConfiguration WIMConfig, WIMData WIMData) {
            this.WIMConfig = WIMConfig;
            this.WIMData = WIMData;
            Assert.IsNotNull(this.Config, "Preview screen configuration is missing.");

            if(!this.Config.PreviewScreen || !this.Config.AutoPositionPreviewScreen) return;
            RemovePreviewScreen();
            Data.PreviewScreenTransform = Instantiate(Resources.Load<GameObject>("Preview Screen")).transform;
            Data.PreviewScreenTransform.GetComponentInChildren<FloatAbove>().Target = transform;
            InitPreviewScreen(Data.PreviewScreenTransform.gameObject);
            Data.PreviewScreenEnabled = true;
        }

        public void ShowPreviewScreenPickup(WIMConfiguration WIMConfig, WIMData WIMData) {
            this.WIMConfig = WIMConfig;
            this.WIMData = WIMData;
            if(!Config.PreviewScreen) return;
            Assert.IsFalse(this.Config.AutoPositionPreviewScreen);
            RemovePreviewScreen();
            Data.PreviewScreenTransform = Instantiate(Resources.Load<GameObject>("Preview Screen")).transform;
            Destroy(Data.PreviewScreenTransform.GetComponentInChildren<FloatAbove>());
            InitPreviewScreen(Data.PreviewScreenTransform.gameObject);
            Data.PreviewScreenEnabled = true;
        }

        public void RemovePreviewScreen() {
            if(!Data) return;
            Data.PreviewScreenEnabled = false;
            var previewScreen = GameObject.FindGameObjectWithTag("PreviewScreen");
            if(!previewScreen) return;
            previewScreen.transform.parent = null;
            Destroy(previewScreen);
        }


        private void Awake() {
            if(!Config) return;
            Data = ScriptableObject.CreateInstance<PreviewScreenData>();
        }

        private void OnEnable() {
            if(!Config) return;
            MiniatureModel.OnNewDestinationSelected += ShowPreviewScreen;
            DestinationIndicators.OnSpawnDestinationIndicatorInWIM += ConfigurePickupPreviewScreen;
            DestinationIndicators.OnRemoveDestinationIndicators += RemovePreviewScreen;
            PickupDestinationUpdate.OnRemoveDestinationIndicatorExceptWIM += RemovePreviewScreen;
        }

        private void OnDisable() {
            if(!Config) return;
            MiniatureModel.OnNewDestinationSelected -= ShowPreviewScreen;
            DestinationIndicators.OnSpawnDestinationIndicatorInWIM -= ConfigurePickupPreviewScreen;
            DestinationIndicators.OnRemoveDestinationIndicators -= RemovePreviewScreen;
            PickupDestinationUpdate.OnRemoveDestinationIndicatorExceptWIM -= RemovePreviewScreen;
        }

        private void InitPreviewScreen(GameObject previewScreen) {
            Assert.IsNotNull(WIMData.DestinationIndicatorInLevel);
            Assert.IsNotNull(WIMData.DestinationIndicatorInLevel.GetChild(0).GetChild(1));
            var camObj = WIMData.DestinationIndicatorInLevel.GetChild(0).GetChild(1).gameObject; // Making assumptions on the prefab.
            Assert.IsNotNull(camObj);
            var cam = camObj.GetOrAddComponent<Camera>();
            Assert.IsNotNull(cam);
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer("WIM"));
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer("Hands"));
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer("PinchGrabbable"));
            cam.targetTexture = new RenderTexture(1600, 900, 16, RenderTextureFormat.Default);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.gray;
            previewScreenMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            previewScreen.GetComponentInChildren<Renderer>().material = previewScreenMaterial;
            previewScreenMaterial.SetTexture(baseMap, cam.targetTexture);
        }

        private void RemovePreviewScreen(WIMConfiguration config, WIMData data) {
            RemovePreviewScreen();
        }

        private void RemovePreviewScreen(in MiniatureModel WIM) {
            RemovePreviewScreen();
        }

        private void ConfigurePickupPreviewScreen(WIMConfiguration WIMConfig, WIMData WIMData) {
            if(!Config || Config.AutoPositionPreviewScreen || !Config.PreviewScreen) return;
            WIMData.DestinationIndicatorInWIM.GetChild(0).GetChild(1).GetChild(0).gameObject
                .AddComponent<PickupPreviewScreen>();
        }
    }
}