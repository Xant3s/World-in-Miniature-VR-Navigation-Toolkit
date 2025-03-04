﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Features.Travel_Preview_Animation {
    /// <summary>
    /// The travel preview animation configuration. Modified via GUI.
    /// </summary>
    [CreateAssetMenu(menuName = "WIM/Feature Configuration/Travel Preview Animation")]
    public class TravelPreviewConfiguration : ScriptableObject {
        public bool TravelPreviewAnimation;
        public float TravelPreviewAnimationSpeed = 1.0f;
    }
}