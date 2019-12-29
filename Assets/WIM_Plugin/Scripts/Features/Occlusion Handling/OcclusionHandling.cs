using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    [ExecuteAlways]
    internal sealed class OcclusionHandling : MonoBehaviour {
        [HideInInspector] public OcclusionHandlingConfiguration Config;

        private void OnDestroy() {
            var WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            if(!WIM) return;
            WIMGenerator.CleanupOcclusionHandling();
            WIMGenerator.SetWIMMaterial(WIMGenerator.LoadDefaultMaterial(WIM), WIM);
        }
    }
}