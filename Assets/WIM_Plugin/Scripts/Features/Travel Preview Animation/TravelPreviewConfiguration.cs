using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WIM_Plugin {
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Travel Preview Animation")]
    public class TravelPreviewConfiguration : ScriptableObject {
        public bool TravelPreviewAnimation;
        public float TravelPreviewAnimationSpeed = 1.0f;
    }
}