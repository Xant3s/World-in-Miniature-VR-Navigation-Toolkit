using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    // Allow scrolling the WIM at runtime.
    public class Scrolling : MonoBehaviour {
        private WIMConfiguration config;
        private WIMData data;

        void OnEnable() {
            MiniatureModel.OnUpdate += ScrollWIM;
        }

        void OnDisable() {
            MiniatureModel.OnUpdate -= ScrollWIM;
        }

        void ScrollWIM(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;

            if (!data.WIMLevelTransform) return;    // TODO: Useless?
            if(!config.AllowWIMScrolling) return;
            if (config.AutoScroll) autoScrollWIM();
            else scrollWIM();
        }

        private void scrollWIM() {
            var input = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
            var direction = new Vector3(input.x, 0, input.y);
            data.WIMLevelTransform.Translate(-direction * config.ScrollSpeed * Time.deltaTime, Space.World);
        }
        
        private void autoScrollWIM() {
            if(!config.AllowWIMScrolling || !config.AutoScroll) return;
            var scrollOffset = data.DestinationIndicatorInWIM
                ? -data.DestinationIndicatorInWIM.localPosition
                : -data.PlayerRepresentationTransform.localPosition;
            data.WIMLevelTransform.localPosition = scrollOffset;
        }
    }
}

