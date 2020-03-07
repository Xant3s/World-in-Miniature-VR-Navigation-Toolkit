using System;
using System.Collections;
using System.Collections.Generic;
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
            if(!Application.isPlaying) return;
            MiniatureModel.OnUpdate += ScrollWIM;
        }

        private void OnDisable() {
            remove();
        }

        internal void remove() {
            InputManager.UnregisterAction(scrollingActionName);
            InputManager.UnregisterAction(verticalScrollingActionName);
            if(!Application.isPlaying) return;
            MiniatureModel.OnUpdate -= ScrollWIM;
        }

        private void OnDestroy() {
            var WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            if(!WIM) return;
            WIMGenerator.DisableScrolling(WIM);
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
    }
}

