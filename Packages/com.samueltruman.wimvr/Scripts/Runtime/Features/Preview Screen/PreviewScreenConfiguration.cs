﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// The preview screen configuration. Modified via GUI.
    /// </summary>
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Preview Screen")]
    public class PreviewScreenConfiguration : ScriptableObject {
        public bool PreviewScreen;
        public bool AutoPositionPreviewScreen;
    }
}
