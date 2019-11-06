using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Preview Screen")]
    public class PreviewScreenConfiguration : ScriptableObject {
        public bool PreviewScreen;
        public bool AutoPositionPreviewScreen;
    }
}
