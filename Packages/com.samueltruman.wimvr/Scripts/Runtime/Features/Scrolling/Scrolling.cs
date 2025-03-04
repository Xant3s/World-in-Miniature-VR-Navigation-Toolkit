﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using WIMVR.Core;
using WIMVR.Util;

namespace WIMVR.Features.Scrolling {
    /// <summary>
    /// Allows to scroll the visible part of the miniature model at runtime.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class Scrolling : MonoBehaviour {
        [HideInInspector] public ScrollingConfiguration ScrollingConfig;
        private WIMData data;
        private Vector2 verticalAxisInput;

        internal void Setup() {
            WIMGenerator.OnPreConfigure += DisableScrolling;
            WIMGenerator.OnConfigure += EnableScrolling;
            WIMGenerator.OnConfigure += UpdateScrollingMask;
            PlayerRepresentation.OnUpdatePlayerRepresentationInWIM += AdjustPlayerRepresentationInWIM;
            if (!Application.isPlaying) return;
            MiniatureModel.OnUpdate += ScrollWIM;
        }

        internal void Remove() {
            WIMGenerator.OnPreConfigure -= DisableScrolling;
            WIMGenerator.OnConfigure -= EnableScrolling;
            WIMGenerator.OnConfigure -= UpdateScrollingMask;
            PlayerRepresentation.OnUpdatePlayerRepresentationInWIM -= AdjustPlayerRepresentationInWIM;
            if (!Application.isPlaying) return;
            MiniatureModel.OnUpdate -= ScrollWIM;
        }

        internal void UpdateScrollingMask(in MiniatureModel WIM) {
            if (!ScrollingConfig || !ScrollingConfig.AllowWIMScrolling) return;
            var boxMaskObj = GameObject.FindWithTag("Box Mask");
            if (!boxMaskObj) return;
            boxMaskObj.transform.localScale = WIM.Configuration.ActiveAreaBounds;
        }

        private static void DisableScrolling(in MiniatureModel WIM) {
            var boxMask = GameObject.FindWithTag("Box Mask");
#if UNITY_EDITOR
            if (boxMask) Undo.DestroyObjectImmediate(boxMask);
#else
            GameObject.DestroyImmediate(boxMask);
#endif
            WIM.transform.RemoveAllColliders();
            WIMGenerator.GenerateColliders(WIM);
        }


        private void OnEnable() {
            if(!ScrollingConfig) return;
            Setup();
        }

        private void OnDisable() {
            Remove();
        }

        private void OnDestroy() {
            var WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            if(!WIM) return;
            DisableScrolling(WIM);
            WIMGenerator.SetWIMMaterial(WIMGenerator.LoadDefaultMaterial(WIM), WIM);
        }

        private void ScrollWIM(WIMConfiguration config, WIMData data) {
            Assert.IsNotNull(ScrollingConfig, "Scrolling configuration is missing.");
            this.data = data;
            if (!data.WIMLevelTransform) return;    // TODO: Useless?
            if(!ScrollingConfig.AllowWIMScrolling) return;
            if (ScrollingConfig.AutoScroll) AutoScrollWIM();
        }

        public void OnScrollWIM(InputValue value) {
            ManuallyScrollWIM(value.Get<Vector2>());
        }

        public void OnScrollWIMVertically(InputValue value) {
            verticalAxisInput = value.Get<Vector2>();
            ManuallyScrollWIM(Vector3.zero);
        }

        private void ManuallyScrollWIM(Vector3 scrollingInput) {
            if (!Application.isPlaying) return;
            if (!ScrollingConfig.AllowWIMScrolling) return;
            var input = scrollingInput;
            var direction = new Vector3(input.x, verticalAxisInput.y, input.y);
            if(!ScrollingConfig.AllowVerticalScrolling) direction.y = 0;
            Assert.IsNotNull(data.WIMLevelTransform);
            data.WIMLevelTransform.Translate(Time.deltaTime * ScrollingConfig.ScrollSpeed * -direction, Space.World);
        }

        private void AutoScrollWIM() {
            if (!ScrollingConfig.AllowWIMScrolling || !ScrollingConfig.AutoScroll ||
                !data.PlayerRepresentationTransform) return;
            Assert.IsNotNull(data.PlayerRepresentationTransform);
            Assert.IsNotNull(data.WIMLevelTransform);
            var scrollOffset = data.DestinationIndicatorInWIM
                ? -data.DestinationIndicatorInWIM.localPosition
                : -data.PlayerRepresentationTransform.localPosition;
            data.WIMLevelTransform.localPosition = scrollOffset;
        }

        private void EnableScrolling(in MiniatureModel WIM) {
            if (!ScrollingConfig || !ScrollingConfig.AllowWIMScrolling) return;
            var maskController = new GameObject("Box Mask");
            maskController.tag = maskController.name;
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(maskController, "Created Box Mask");
#endif
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

        private void AdjustPlayerRepresentationInWIM(WIMConfiguration config, WIMData data) {
            if(ScrollingConfig && ScrollingConfig.AllowWIMScrolling) {
                // Get closest point on active area bounds. Won't have any effect if already inside active area.
                data.PlayerRepresentationTransform.position =
                    data.WIMLevelTransform.GetComponentInParent<Collider>().ClosestPoint(data.PlayerRepresentationTransform.position);
            }
        }
    }
}

