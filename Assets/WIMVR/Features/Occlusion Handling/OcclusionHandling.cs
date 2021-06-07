// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEngine;
using WIMVR.Core;
using WIMVR.Features.Occlusion_Handling.Tags;
using WIMVR.Util;

namespace WIMVR.Features.Occlusion_Handling {
    /// <summary>
    /// Adds occlusion handling strategies.
    /// To Deal with occlusion, parts of the miniature model are hidden.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class OcclusionHandling : MonoBehaviour {
        [HideInInspector] public OcclusionHandlingConfiguration config;


        private void OnEnable() {
            WIMGenerator.OnPreConfigure += CleanupOcclusionHandling;
            WIMGenerator.OnConfigure += ConfigureCutoutView;
            WIMGenerator.OnConfigure += ConfigureMeltWalls;
        }

        private void OnDisable() {
            WIMGenerator.OnPreConfigure -= CleanupOcclusionHandling;
            WIMGenerator.OnConfigure -= ConfigureCutoutView;
            WIMGenerator.OnConfigure -= ConfigureMeltWalls;
        }

        public void UpdateCylinderMask(in MiniatureModel WIM) {
            if(!config) return;
            if(config.OcclusionHandlingMethod != OcclusionHandlingMethod.MeltWalls) return;
            var cylinderTransform = FindObjectOfType<CylinderMask>()?.transform;
            if(!cylinderTransform) return;
            cylinderTransform.localScale = new Vector3(config.MeltRadius, config.MeltHeight, 1);
        }

        public void UpdateCutoutViewMask(in MiniatureModel WIM) {
            if(!config) return;
            if(config.OcclusionHandlingMethod != OcclusionHandlingMethod.CutoutView) return;
            var spotlightObj = FindObjectOfType<SpotlightMask>();
            if(!spotlightObj) return;
            var spotlight = spotlightObj.GetComponent<Light>();
            spotlight.range = config.CutoutRange;
            spotlight.spotAngle = config.CutoutAngle;

            Color color;
            if(config.ShowCutoutLight) {
                color = config.CutoutLightColor;
                color.a = 1;
            }
            else {
                color = new Color(0, 0, 0, 0);
            }

            spotlight.color = color;
        }

        private static void CleanupOcclusionHandling(in MiniatureModel WIM) {
            var cylinderMask = FindObjectOfType<CylinderMask>()?.gameObject;
            var spotlightMask = FindObjectOfType<SpotlightMask>()?.gameObject;

#if UNITY_EDITOR
            if(cylinderMask) Undo.DestroyObjectImmediate(cylinderMask);
            if(spotlightMask) Undo.DestroyObjectImmediate(spotlightMask);
#else
            GameObject.DestroyImmediate(cylinderMask);
            GameObject.DestroyImmediate(spotlightMask);
#endif
        }

        private void OnDestroy() {
            var WIM = FindObjectOfType<MiniatureModel>()?.GetComponent<MiniatureModel>();
            if(!WIM) return;
            CleanupOcclusionHandling(WIM);
            WIMGenerator.SetWIMMaterial(WIMGenerator.LoadDefaultMaterial(WIM), WIM);
        }

        private void ConfigureCutoutView(in MiniatureModel WIM) {
            if(!config || config.OcclusionHandlingMethod != OcclusionHandlingMethod.CutoutView) return;
            if (FindObjectOfType<SpotlightMask>()) return;
            var spotlightObj = new GameObject("Spotlight Mask");
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(spotlightObj, "Spotlight Mask");
#endif
            spotlightObj.AddComponent<SpotlightMask>();
            spotlightObj.AddComponent<Light>().type = LightType.Spot;
            var controller = spotlightObj.AddComponent<ConeController>();
            var material = WIMGenerator.LoadDefaultMaterial(WIM);
            controller.materials = new[] {material};
            controller.SetConeEnabled(true);
            var mainCamera = Camera.main;
            if(mainCamera) spotlightObj.AddComponent<AlignWith>().Target = mainCamera.transform;
        }

        private void ConfigureMeltWalls(in MiniatureModel WIM) {
            if(!config || config.OcclusionHandlingMethod != OcclusionHandlingMethod.MeltWalls) return;
            if (FindObjectOfType<CylinderMask>()) return;
            var cylinderMask = new GameObject("Cylinder Mask");
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(cylinderMask, "configureMeltWalls");
#endif
            cylinderMask.AddComponent<CylinderMask>();
            cylinderMask.AddComponent<FollowHand>().hand = Hand.RightHand;
            var controller = cylinderMask.AddComponent<CapsuleController>();
            var material = WIMGenerator.LoadDefaultMaterial(WIM);
            controller.materials = new[] {material};
            controller.SetCapsuleEnabled(true);
        }
    }
}