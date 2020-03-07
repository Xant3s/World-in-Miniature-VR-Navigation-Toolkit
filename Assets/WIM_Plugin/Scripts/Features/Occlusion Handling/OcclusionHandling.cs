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
            CleanupOcclusionHandling(WIM);
            WIMGenerator.SetWIMMaterial(WIMGenerator.LoadDefaultMaterial(WIM), WIM);
        }

        private void OnEnable() {
            WIMGenerator.OnPreConfigure += CleanupOcclusionHandling;
            WIMGenerator.OnConfigure += configureCutoutView;
            WIMGenerator.OnConfigure += configureMeltWalls;
        }

        private void OnDisable() {
            WIMGenerator.OnPreConfigure -= CleanupOcclusionHandling;
            WIMGenerator.OnConfigure += configureCutoutView;
            WIMGenerator.OnConfigure += configureMeltWalls;
        }

        public void CleanupOcclusionHandling(in MiniatureModel WIM) {
            var cylinderMask = GameObject.FindWithTag("Cylinder Mask");
            var spotlightMask = GameObject.FindWithTag("Spotlight Mask");
            DestroyImmediate(cylinderMask);
            DestroyImmediate(spotlightMask);
        }

        private void configureCutoutView(in MiniatureModel WIM) {
            if(!Config || Config.OcclusionHandlingMethod != OcclusionHandlingMethod.CutoutView) return;
            var spotlightObj = new GameObject("Spotlight Mask");
            spotlightObj.tag = spotlightObj.name;
            spotlightObj.AddComponent<Light>().type = LightType.Spot;
            var controller = spotlightObj.AddComponent<ConeController>();
            var material = WIMGenerator.LoadDefaultMaterial(WIM);
            controller.materials = new[] {material};
            controller.SetConeEnabled(true);
            var mainCamera = Camera.main;
            if(mainCamera) spotlightObj.AddComponent<AlignWith>().Target = mainCamera.transform;
        }

        private void configureMeltWalls(in MiniatureModel WIM) {
            if(!Config || Config.OcclusionHandlingMethod != OcclusionHandlingMethod.MeltWalls) return;
            var cylinderMask = new GameObject("Cylinder Mask");
            cylinderMask.tag = cylinderMask.name;
            cylinderMask.AddComponent<FollowHand>().hand = Hand.RightHand;
            var controller = cylinderMask.AddComponent<CapsuleController>();
            var material = WIMGenerator.LoadDefaultMaterial(WIM);
            controller.materials = new[] {material};
            controller.SetCapsuleEnabled(true);
        }

        internal void UpdateCylinderMask(in MiniatureModel WIM) {
            if(!Config) return;
            if(Config.OcclusionHandlingMethod != OcclusionHandlingMethod.MeltWalls) return;
            var cylinderTransform = GameObject.FindWithTag("Cylinder Mask")?.transform;
            if(!cylinderTransform) return;
            cylinderTransform.localScale =
                new Vector3(Config.MeltRadius, Config.MeltHeight, 1);
        }

        internal void UpdateCutoutViewMask(in MiniatureModel WIM) {
            if(!Config) return;
            if(Config.OcclusionHandlingMethod != OcclusionHandlingMethod.CutoutView) return;
            var spotlightObj = GameObject.FindWithTag("Spotlight Mask");
            if(!spotlightObj) return;
            var spotlight = spotlightObj.GetComponent<Light>();
            spotlight.range = Config.CutoutRange;
            spotlight.spotAngle = Config.CutoutAngle;

            Color color;
            if(Config.ShowCutoutLight) {
                color = Config.CutoutLightColor;
                color.a = 1;
            }
            else {
                color = new Color(0, 0, 0, 0);
            }

            spotlight.color = color;
        }
    }
}