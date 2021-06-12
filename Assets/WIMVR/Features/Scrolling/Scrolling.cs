// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Core.Input;
using WIMVR.Features.Scrolling.Tags;
using WIMVR.Util;
using WIMVR.Util.Extensions;

namespace WIMVR.Features.Scrolling {
    /// <summary>
    /// Allows to scroll the visible part of the miniature model at runtime.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(WIMInput))]
    public class Scrolling : MonoBehaviour {
        [HideInInspector] public ScrollingConfiguration ScrollingConfig;

        private MiniatureModel WIM;
        private WIMData data;
        private WIMInput wimInput;
        private Vector2 verticalAxisInput;


        private void Awake() {
            WIM = FindObjectOfType<MiniatureModel>();
            wimInput = GetComponent<WIMInput>();
            wimInput.scrollWIM.action.performed += ctx => OnScrollWIM(ctx.action.ReadValue<Vector2>());
            wimInput.scrollWIMVertically.action.performed += ctx => OnScrollWIMVertically(ctx.action.ReadValue<Vector2>());
        }

        private void Start() {
            if(!ScrollingConfig || !ScrollingConfig.AllowWIMScrolling) return;
            EnableScrolling(WIM);
            UpdateScrollingMask(WIM);
            PlayerRepresentation.OnUpdatePlayerRepresentationInWIM += AdjustPlayerRepresentationInWIM;
            MiniatureModel.OnUpdate += ScrollWIM;
        }
        
        private static void EnableScrolling(in MiniatureModel WIM) {
            var maskController = new GameObject("Box Mask");
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(maskController, "Created Box Mask");
#endif
            maskController.AddComponent<BoxMask>();
            maskController.AddComponent<AlignWith>().Target = WIM.transform;
            var controller = maskController.AddComponent<BoxController>();
            var material = WIMGenerator.LoadDefaultMaterial(WIM);
            controller.materials = new[] {material};
            controller.SetBoxEnabled(true);
            WIM.transform.RemoveAllColliders();
            WIM.gameObject.AddComponent<BoxCollider>().size =
                WIM.Configuration.ActiveAreaBounds / WIM.Configuration.ScaleFactor;
            maskController.transform.position = WIM.transform.position;
        }

        public void DisableScrolling() {
            UnregisterCallbacks();
            RemoveBoxMask();
            WIM.transform.RemoveAllColliders();
            WIMGenerator.GenerateColliders(WIM);
        }

        private void UnregisterCallbacks() {
            PlayerRepresentation.OnUpdatePlayerRepresentationInWIM -= AdjustPlayerRepresentationInWIM;
            MiniatureModel.OnUpdate -= ScrollWIM;
        }

        private static void RemoveBoxMask() {
            var boxMask = FindObjectOfType<BoxMask>()?.gameObject;
#if UNITY_EDITOR
            if(boxMask) Undo.DestroyObjectImmediate(boxMask);
#else
            GameObject.DestroyImmediate(boxMask);
#endif
        }

        private void OnScrollWIM(Vector2 value) => ManuallyScrollWIM(value);

        private void OnScrollWIMVertically(Vector2 value) {
            verticalAxisInput = value;
            ManuallyScrollWIM(Vector3.zero);
        }

        private void ManuallyScrollWIM(Vector3 scrollingInput) {
            if(!ScrollingConfig || !ScrollingConfig.AllowWIMScrolling) return;
            var direction = new Vector3(scrollingInput.x, verticalAxisInput.y, scrollingInput.y);
            if(!ScrollingConfig.AllowVerticalScrolling) direction.y = 0;
            Assert.IsNotNull(data.WIMLevelTransform);
            data.WIMLevelTransform.Translate(Time.deltaTime * ScrollingConfig.ScrollSpeed * -direction, Space.World);
        }

        private void ScrollWIM(WIMConfiguration wimConfig, WIMData wimData) {
            data = wimData;
            if(!wimData.WIMLevelTransform) return;
            if(ScrollingConfig.AutoScroll) AutoScrollWIM();
        }

        private void AutoScrollWIM() {
            if(!data.PlayerRepresentationTransform) return;
            Assert.IsNotNull(data.PlayerRepresentationTransform);
            Assert.IsNotNull(data.WIMLevelTransform);
            var scrollOffset = data.DestinationIndicatorInWIM
                ? -data.DestinationIndicatorInWIM.localPosition
                : -data.PlayerRepresentationTransform.localPosition;
            data.WIMLevelTransform.localPosition = scrollOffset;
        }

        private static void UpdateScrollingMask(in MiniatureModel WIM) {
            var boxMaskObj = FindObjectOfType<BoxMask>().gameObject;
            if(!boxMaskObj) return;
            boxMaskObj.transform.localScale = WIM.Configuration.ActiveAreaBounds;
        }

        private void AdjustPlayerRepresentationInWIM(WIMConfiguration config, WIMData data) {
            if(ScrollingConfig && ScrollingConfig.AllowWIMScrolling) {
                // Get closest point on active area bounds. Won't have any effect if already inside active area.
                data.PlayerRepresentationTransform.position =
                    data.WIMLevelTransform.GetComponentInParent<Collider>().ClosestPoint(data.PlayerRepresentationTransform.position);
            }
        }
    }
}

