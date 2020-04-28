// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Features.Preview_Screen {
    /// <summary>
    /// The preview screen data. Modified at runtime.
    /// </summary>
    public class PreviewScreenData : ScriptableObject {
        public bool PreviewScreenEnabled;
        public Transform PreviewScreenTransform;
    }
}