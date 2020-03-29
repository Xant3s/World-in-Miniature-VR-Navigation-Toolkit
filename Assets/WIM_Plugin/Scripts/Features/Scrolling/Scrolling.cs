﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    // Allow scrolling the WIM at runtime.
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class Scrolling : MonoBehaviour {
        private static readonly string scrollingActionName = "Scrolling Axis";
        private static readonly string verticalScrollingActionName = "Vertical Scrolling Axis";
        private static readonly string scrollingTooltip = "Axis used to scroll miniature model. Only used if scrolling is enabled.";
        private static readonly string verticalScrollingTooltip = "Axis used to vertical scroll miniature model. Only used if scrolling is enabled.";
        [HideInInspector] public ScrollingConfiguration ScrollingConfig;
        private WIMData data;
        private Vector2 verticalAxisInput;

        public static void DisableScrolling(in MiniatureModel WIM) {
            var boxMask = GameObject.FindWithTag("Box Mask");
#if UNITY_EDITOR
            if (boxMask) Undo.DestroyObjectImmediate(boxMask);
#else
            GameObject.DestroyImmediate(boxMask);
#endif
            WIM.transform.RemoveAllColliders();
            WIMGenerator.GenerateColliders(WIM);
        }

        internal void Setup() {
            InputManager.RegisterAction(scrollingActionName, ScrollWIM, scrollingTooltip);
            InputManager.RegisterAction(verticalScrollingActionName, UpdateVerticalInput, verticalScrollingTooltip);
            WIMGenerator.OnPreConfigure += DisableScrolling;
            WIMGenerator.OnConfigure += EnableScrolling;
            WIMGenerator.OnConfigure += UpdateScrollingMask;
            PlayerRepresentation.OnUpdatePlayerRepresentationInWIM += AdjustPlayerRepresentationInWIM;
            if (!Application.isPlaying) return;
            MiniatureModel.OnUpdate += ScrollWIM;
        }

        internal void Remove() {
            InputManager.UnregisterAction(scrollingActionName);
            InputManager.UnregisterAction(verticalScrollingActionName);
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
            if (!data.WIMLevelTransform) return;    // TODO: Useless?
            if(!ScrollingConfig.AllowWIMScrolling) return;
            if (ScrollingConfig.AutoScroll) AutoScrollWIM();
            this.data = data;
        }

        private void UpdateVerticalInput(Vector3 input) {
            verticalAxisInput = input;
        }

        private void ScrollWIM(Vector3 scrollingInput) {
            if (!Application.isPlaying) return;
            if (!ScrollingConfig.AllowWIMScrolling) return;
            var input = scrollingInput;
            var direction = new Vector3(input.x, verticalAxisInput.y, input.y);
            if(!ScrollingConfig.AllowVerticalScrolling) direction.y = 0;
            Assert.IsNotNull(data.WIMLevelTransform);
            data.WIMLevelTransform.Translate(Time.deltaTime * ScrollingConfig.ScrollSpeed * -direction, Space.World);
        }

        private void AutoScrollWIM() {
            if(!ScrollingConfig.AllowWIMScrolling || !ScrollingConfig.AutoScroll) return;
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

