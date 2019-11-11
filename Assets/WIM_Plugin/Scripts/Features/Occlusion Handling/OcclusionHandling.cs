using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    [ExecuteAlways]
    internal sealed class OcclusionHandling : MonoBehaviour {
        public OcclusionHandlingConfiguration Config;

        private void OnDestroy() {
            var WIM = GameObject.Find("WIM")?.GetComponent<MiniatureModel>();
            if(!WIM) return;
            WIMGenerator.CleanupOcclusionHandling();
            WIMGenerator.SetWIMMaterial(WIMGenerator.LoadDefaultMaterial(WIM), WIM);
        }
    }
}