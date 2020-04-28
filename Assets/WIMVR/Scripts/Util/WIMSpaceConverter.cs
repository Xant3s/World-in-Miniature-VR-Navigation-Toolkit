// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using WIMVR.Core;

namespace WIMVR.Util {
    /// <summary>
    /// Converts between world space (full-sized level) and miniature model space.
    /// </summary>
    public interface WIMSpaceConverter {
        Vector3 ConvertToLevelSpace(Vector3 pointInWIMSpace);

        Vector3 ConvertToWIMSpace(Vector3 pointInLevelSpace);
    }


    /// <summary>
    /// Implementation. Converts between world space (full-sized level) and miniature model space.
    /// </summary>
    public sealed class WIMSpaceConverterImpl : WIMSpaceConverter {
        private readonly WIMConfiguration config;
        private readonly WIMData data;

        public WIMSpaceConverterImpl(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
        }

        public Vector3 ConvertToLevelSpace(Vector3 pointInWIMSpace) {
            var WIMOffset = pointInWIMSpace - data.WIMLevelTransform.position;
            var levelOffset = WIMOffset / config.ScaleFactor;
            levelOffset = Quaternion.Inverse(data.WIMLevelTransform.rotation) * levelOffset;
            return data.LevelTransform.position + levelOffset;
        }

        public Vector3 ConvertToWIMSpace(Vector3 pointInLevelSpace) {
            var levelOffset = pointInLevelSpace - data.LevelTransform.position;
            var WIMOffset = levelOffset * config.ScaleFactor;
            WIMOffset = data.WIMLevelTransform.rotation * WIMOffset;
            return data.WIMLevelTransform.position + WIMOffset;
        }
    }
}