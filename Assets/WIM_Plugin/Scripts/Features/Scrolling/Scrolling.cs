﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    // Allow scrolling the WIM at runtime.
    public class Scrolling : MonoBehaviour {
        public ScrollingConfiguration ScrollingConfig;

        private WIMConfiguration config;
        private WIMData data;

        private void OnEnable() {
            MiniatureModel.OnUpdate += ScrollWIM;
        }

        private void OnDisable() {
            MiniatureModel.OnUpdate -= ScrollWIM;
        }

        private void ScrollWIM(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
            Assert.IsNotNull(ScrollingConfig, "Scrolling configuration is missing.");


            if (!data.WIMLevelTransform) return;    // TODO: Useless?
            if(!ScrollingConfig.AllowWIMScrolling) return;
            if (ScrollingConfig.AutoScroll) autoScrollWIM();
            else scrollWIM();
        }

        private void scrollWIM() {
            var input = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
            var verticalInput = OVRInput.Get(ScrollingConfig.VerticalScrollingAxis).y;
            var direction = new Vector3(input.x, verticalInput, input.y);
            if(!ScrollingConfig.AllowVerticalScrolling) direction.y = 0;
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

