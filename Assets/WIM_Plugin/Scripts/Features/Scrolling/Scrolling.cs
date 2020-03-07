using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    // Allow scrolling the WIM at runtime.
    [ExecuteAlways]
    public class Scrolling : MonoBehaviour {
        [HideInInspector] public ScrollingConfiguration ScrollingConfig;

        private static readonly string scrollingActionName = "Scrolling Axis";
        private static readonly string verticalScrollingActionName = "Vertical Scrolling Axis";
        private WIMData data;
        private Vector2 verticalAxisInput;


        private void OnEnable() {
            if(!ScrollingConfig) return;
            setup();
        }

        internal void setup() {
            InputManager.RegisterAction(scrollingActionName, scrollWIM);
            InputManager.RegisterAction(verticalScrollingActionName, updateVerticalInput);
            WIMGenerator.OnPreConfigure += DisableScrolling;
            WIMGenerator.OnConfigure += EnableScrolling;
            WIMGenerator.OnConfigure += UpdateScrollingMask;
            Respawn.OnEarlyRespawn += EnableScrollingForOldWIM;
            if (!Application.isPlaying) return;
            MiniatureModel.OnUpdate += ScrollWIM;
        }

        private void OnDisable() {
            remove();
        }

        internal void remove() {
            InputManager.UnregisterAction(scrollingActionName);
            InputManager.UnregisterAction(verticalScrollingActionName);
            WIMGenerator.OnPreConfigure -= DisableScrolling;
            WIMGenerator.OnConfigure -= EnableScrolling;
            WIMGenerator.OnConfigure -= UpdateScrollingMask;
            Respawn.OnEarlyRespawn -= EnableScrollingForOldWIM;
            if (!Application.isPlaying) return;
            MiniatureModel.OnUpdate -= ScrollWIM;
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
            if (ScrollingConfig.AutoScroll) autoScrollWIM();
            this.data = data;
        }

        private void updateVerticalInput(Vector3 input) {
            verticalAxisInput = input;
        }

        private void scrollWIM(Vector3 scrollingInput) {
            if (!Application.isPlaying) return;
            if (!ScrollingConfig.AllowWIMScrolling) return;
            var input = scrollingInput;
            var direction = new Vector3(input.x, verticalAxisInput.y, input.y);
            if(!ScrollingConfig.AllowVerticalScrolling) direction.y = 0;
            Assert.IsNotNull(data.WIMLevelTransform);
            data.WIMLevelTransform.Translate(Time.deltaTime * ScrollingConfig.ScrollSpeed * -direction, Space.World);
        }
        
        private void autoScrollWIM() {
            if(!ScrollingConfig.AllowWIMScrolling || !ScrollingConfig.AutoScroll) return;
            var scrollOffset = data.DestinationIndicatorInWIM
                ? -data.DestinationIndicatorInWIM.localPosition
                : -data.PlayerRepresentationTransform.localPosition;
            data.WIMLevelTransform.localPosition = scrollOffset;
        }

        internal void UpdateScrollingMask(in MiniatureModel WIM) {
            if (!ScrollingConfig || !ScrollingConfig.AllowWIMScrolling) return;
            var boxMaskObj = GameObject.FindWithTag("Box Mask");
            if (!boxMaskObj) return;
            boxMaskObj.transform.localScale = WIM.Configuration.ActiveAreaBounds;
        }

        public static void DisableScrolling(in MiniatureModel WIM) {
            var boxMask = GameObject.FindWithTag("Box Mask");
            DestroyImmediate(boxMask);
            WIMGenerator.RemoveAllColliders(WIM.transform);
            WIMGenerator.GenerateColliders(WIM);
        }

        private void EnableScrolling(in MiniatureModel WIM) {
            if (!ScrollingConfig.AllowWIMScrolling) return;
            var maskController = new GameObject("Box Mask");
            maskController.tag = maskController.name;
            maskController.AddComponent<AlignWith>().Target = WIM.transform;
            var controller = maskController.AddComponent<BoxController>();
            var material = WIMGenerator.LoadDefaultMaterial(WIM);
            controller.materials = new[] {material};
            controller.SetBoxEnabled(true);
            WIMGenerator.RemoveAllColliders(WIM.transform);
            WIM.gameObject.AddComponent<BoxCollider>().size =
                WIM.Configuration.ActiveAreaBounds / WIM.Configuration.ScaleFactor;
            maskController.transform.position = WIM.transform.position;
        }

        private void EnableScrollingForOldWIM(in Transform oldWIMTransform, in Transform newWIMTransform,
            bool maintainTransformRelativeToPlayer) {
            var boxMask = GameObject.FindWithTag("Box Mask");
            if(!boxMask) return;
            var oldBoxMask = Instantiate(boxMask, oldWIMTransform);
            oldBoxMask.GetComponent<AlignWith>().Target = oldWIMTransform;
            var oldBoxController = oldBoxMask.GetComponent<BoxController>();
            oldBoxController.materials = new[] { Respawn.materialForOldWIM };
            oldBoxController.SetBoxEnabled(true);
        }
    }
}

