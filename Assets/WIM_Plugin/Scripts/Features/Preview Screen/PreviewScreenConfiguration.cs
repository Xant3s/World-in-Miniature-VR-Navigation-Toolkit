// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIM_Plugin {
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Preview Screen")]
    public class PreviewScreenConfiguration : ScriptableObject {
        public bool PreviewScreen;
        public bool AutoPositionPreviewScreen;
    }
}
