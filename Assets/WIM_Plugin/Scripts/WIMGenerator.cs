using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace WIM_Plugin {
    // Generates the actual WIM. Also takes care of configurating the WIM, i.e. set materials and shaders etc.
    public static class WIMGenerator {
        private static readonly int alpha = Shader.PropertyToID("_Alpha");
        private static readonly int baseMapColor = Shader.PropertyToID("_BaseMapColor");

        // Load the material appropriate to current WIM configuration.
        public static Material LoadAppropriateMaterial(in MiniatureModel WIM) {
            Material material;
            var scrollingConfig = WIM.GetComponent<Scrolling>()?.ScrollingConfig;
            if (scrollingConfig && scrollingConfig.AllowWIMScrolling) {
                material = Resources.Load<Material>("Materials/CombinedCutout");
                setBaseMapColorAlpha(material, WIM.Configuration.SemiTransparent ? 1 - WIM.Configuration.Transparency : 1);
                return material;
            }

            var occlusionHandlingConfig = WIM.GetComponent<OcclusionHandling>()?.Config;
            if(occlusionHandlingConfig) {
                switch (occlusionHandlingConfig.OcclusionHandlingMethod) {
                    case OcclusionHandlingMethod.MeltWalls: {
                            material = Resources.Load<Material>("Materials/CombinedCutout");
                            setBaseMapColorAlpha(material, WIM.Configuration.SemiTransparent ? 1 - WIM.Configuration.Transparency : 1);
                        break;
                    }
                    case OcclusionHandlingMethod.CutoutView: {
                        material = Resources.Load<Material>("Materials/CombinedCutout");
                        setBaseMapColorAlpha(material, WIM.Configuration.SemiTransparent ? 1 - WIM.Configuration.Transparency : 1);
                        break;
                    }
                    default:
                        material = LoadDefaultMaterial(WIM);
                        break;
                }
            }
            else {
                material = LoadDefaultMaterial(WIM);
            }
            return material;
        }

        public static Material LoadDefaultMaterial(MiniatureModel WIM) {
            Material material;
            material = Resources.Load<Material>("Materials/WIM Material");
            var fullyOpaque = 1;
            var semiTransparent = 1 - WIM.Configuration.Transparency;
            material.SetFloat(alpha, WIM.Configuration.SemiTransparent ? semiTransparent : fullyOpaque);
            return material;
        }


        // Set the color.alpha of the given material. 0 equals to fully transparent.
        private static void setBaseMapColorAlpha(Material material, float value) {
            var color = material.GetColor(baseMapColor);
            color.a = value;
            material.SetColor(baseMapColor, color);
        }

        private static void SetupDissolveScript(in MiniatureModel WIM) {
            var occlusionHandlingConfig = WIM.GetComponent<OcclusionHandling>()?.Config;
            var scrollingConfig = WIM.GetComponent<Scrolling>()?.ScrollingConfig;
            if (scrollingConfig && scrollingConfig.AllowWIMScrolling) {
                RemoveDissolveScript(WIM);
            } else if(occlusionHandlingConfig &&
                      (occlusionHandlingConfig.OcclusionHandlingMethod == OcclusionHandlingMethod.MeltWalls ||
                      occlusionHandlingConfig.OcclusionHandlingMethod == OcclusionHandlingMethod.CutoutView)) {
                RemoveDissolveScript(WIM);
            }
            else {
                AddDissolveScript(WIM);
            }
        }

        private static void RemoveDissolveScript(in MiniatureModel WIM) {
            var WIMLevelTransform = WIM.transform.GetChild(0);
            foreach (Transform child in WIMLevelTransform) {
                Object.DestroyImmediate(child.GetComponent<Dissolve>());
            }
        }

        private static void AddDissolveScript(in MiniatureModel WIM) {
            var WIMLevelTransform = WIM.transform.GetChild(0);
            foreach (Transform child in WIMLevelTransform) {
                if (!child.GetComponent<Dissolve>())
                    child.gameObject.AddComponent<Dissolve>();
            }
        }

        public static void SetWIMMaterial(Material material, in MiniatureModel WIM) {
            var WIMLevelTransform = WIM.transform.GetChild(0);
            foreach (Transform child in WIMLevelTransform) {
                var renderer = child.GetComponent<Renderer>();
                if (!renderer) continue;
                var newMaterials = renderer.sharedMaterials;
                for (var i = 0; i < newMaterials.Length; i++) {
                    newMaterials[i] = material;
                }

                renderer.sharedMaterials = newMaterials;
            }
        }

        private static void expandColliders(in MiniatureModel WIM) {
            foreach (var boxCollider in WIM.gameObject.GetComponents<BoxCollider>()) {
                boxCollider.size += new Vector3(WIM.Configuration.ExpandCollidersX.x + WIM.Configuration.ExpandCollidersX.y, 0, 0);
                boxCollider.center += Vector3.left * WIM.Configuration.ExpandCollidersX.x / 2.0f;
                boxCollider.center += Vector3.right * WIM.Configuration.ExpandCollidersX.y / 2.0f;

                boxCollider.size += new Vector3(0, WIM.Configuration.ExpandCollidersY.x + WIM.Configuration.ExpandCollidersY.y, 0);
                boxCollider.center += Vector3.up * WIM.Configuration.ExpandCollidersY.x / 2.0f;
                boxCollider.center += Vector3.down * WIM.Configuration.ExpandCollidersY.y / 2.0f;

                boxCollider.size += new Vector3(0, 0, WIM.Configuration.ExpandCollidersZ.x + WIM.Configuration.ExpandCollidersZ.y);
                boxCollider.center += Vector3.forward * WIM.Configuration.ExpandCollidersZ.x / 2.0f;
                boxCollider.center += Vector3.back * WIM.Configuration.ExpandCollidersZ.y / 2.0f;
            }
        }

        private static void pruneColliders(in MiniatureModel WIM) {
            var destroyList = new List<Collider>();
            var colliders = WIM.gameObject.GetComponents<Collider>();
            for (var i = 0; i < colliders.Length; i++) {
                var col = (BoxCollider) colliders[i];
                for (var j = 0; j < colliders.Length; j++) {
                    if (i == j) continue;
                    var other = colliders[j];
                    var skip = false;
                    for (var id = 0; id < 8; id++) {
                        if (other.bounds.Contains(MathUtils.GetCorner(col, id))) continue;
                        // next collider
                        skip = true;
                        break;
                    }

                    if (skip) continue;
                    destroyList.Add(col);
                    break;
                }
            }

            while (destroyList.Count != 0) {
                Object.DestroyImmediate(destroyList[0]);
                destroyList.RemoveAt(0);
            }
        }

        private static void GenerateColliders(in MiniatureModel WIM) {
            // Generate colliders:
            // 1. Copy colliders from actual level (to determine which objects should have a collider)
            // [alternatively don't delete them while generating the WIM]
            // 2. replace every collider with box collider (recursive, possibly multiple colliders per obj)
            var wimLevelTransform = WIM.transform.GetChild(0);
            Assert.IsNotNull(wimLevelTransform);
            var WIMChildTransforms = wimLevelTransform.GetComponentsInChildren<Transform>();
            var level = GameObject.FindWithTag("Level")?.transform;
            Assert.IsNotNull(level);
            Assert.AreNotEqual(wimLevelTransform, level);
            var levelChildTransforms = level.GetComponentsInChildren<Transform>();
            if (WIMChildTransforms.Length != levelChildTransforms.Length) return;
            var i = 0;
            foreach (var childInWIM in WIMChildTransforms) {
                Transform childEquivalentInLevel;
                try {
                    childEquivalentInLevel = levelChildTransforms.ElementAt(i);
                    Assert.IsNotNull(childEquivalentInLevel);
                }
                catch {
                    continue;
                }

                if (!childEquivalentInLevel) continue;
                Assert.AreNotEqual(childEquivalentInLevel, childInWIM);
                var collider = childEquivalentInLevel.GetComponent<Collider>();
                i++;
                if (!collider) continue;
                removeAllColliders(childInWIM);
                var childBoxCollider = childInWIM.gameObject.AddComponent<BoxCollider>();
                // 3. move collider to WIM root (consider scale and position)
                var rootCollider = WIM.gameObject.AddComponent<BoxCollider>();
                rootCollider.center = childInWIM.localPosition;
                rootCollider.size = Vector3.zero;
                var bounds = rootCollider.bounds;
                bounds.Encapsulate(childBoxCollider.bounds);
                rootCollider.size = bounds.size / WIM.Configuration.ScaleFactor;
                removeAllColliders(childInWIM);
            }

            // 4. remove every collider that is fully inside another one.
            pruneColliders(WIM);
            // 5. extend collider (esp. upwards)
            expandColliders(WIM);
        }

        private static void removeAllColliders(Transform t) {
            Collider collider;
            // ReSharper disable once AssignmentInConditionalExpression
            while (collider = t.GetComponent<Collider>()) { // Assignment
                Object.DestroyImmediate(collider);
            }
        }

        public static void GenerateNewWIM(in MiniatureModel WIM) {
            removeAllColliders(WIM.transform);
            adaptScaleFactorToPlayerHeight(WIM);
            var levelTransform = GameObject.FindWithTag("Level").transform;
            if (WIM.transform.childCount > 0) Object.DestroyImmediate(WIM.transform.GetChild(0).gameObject);
            var WIMLevel = Object.Instantiate(levelTransform, WIM.transform);
            GameObject.DestroyImmediate(WIMLevel.GetComponent<AutoUpdateWIM>());
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo (WIMLevel.gameObject, "GenerateNewWIM");
#endif
            WIMLevel.localPosition = WIM.Configuration.WIMLevelOffset;
            WIMLevel.name = "WIM Level";
            WIMLevel.tag = "Untagged";
            WIMLevel.gameObject.isStatic = false;
            var WIMLayer = LayerMask.NameToLayer("WIM");
            WIMLevel.gameObject.layer = WIMLayer;
            foreach (Transform child in WIMLevel) {
                child.gameObject.layer = WIMLayer;
                Object.DestroyImmediate(child.GetComponent(typeof(Rigidbody)));
                Object.DestroyImmediate(child.GetComponent(typeof(OVRGrabbable)));
                Object.DestroyImmediate(child.GetComponent(typeof(AutoUpdateWIM)));
                var renderer = child.GetComponent<Renderer>();
                if (renderer) {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }

                child.gameObject.isStatic = false;
            }

            WIM.transform.localScale = new Vector3(WIM.Configuration.ScaleFactor, WIM.Configuration.ScaleFactor, WIM.Configuration.ScaleFactor);
            ConfigureWIM(WIM);
        }

        private static void adaptScaleFactorToPlayerHeight(in MiniatureModel WIM) {
            var config = WIM.Configuration;
            // TODO: decouple scaling.
            var scalingConfig = WIM.GetComponent<Scaling>()?.ScalingConfig;
            if (!config.AdaptWIMSizeToPlayerHeight) return;
            var playerHeight = config.PlayerHeightInCM;
            const float defaultHeight = 170;
            var defaultScaleFactor = config.ScaleFactor;
            const float minHeight = 100;
            const float maxHeight = 200;
            playerHeight = Mathf.Clamp(playerHeight, minHeight, maxHeight);
            var maxScaleFactorDelta = config.MaxWIMScaleFactorDelta;
            var heightDelta = playerHeight - defaultHeight;
            if (heightDelta > 0) {
                const float maxDelta = maxHeight - defaultHeight;
                var actualDelta = maxHeight - playerHeight;
                var factor = actualDelta / maxDelta;
                var resultingScaleFactorDelta = maxScaleFactorDelta * factor;
                config.ScaleFactor = defaultScaleFactor + resultingScaleFactorDelta;
                if(scalingConfig) config.ScaleFactor = Mathf.Clamp(config.ScaleFactor, scalingConfig.MinScaleFactor, scalingConfig.MaxScaleFactor);
                WIM.transform.localScale = new Vector3(config.ScaleFactor,config.ScaleFactor,config.ScaleFactor);

            } else if (heightDelta < 0) {
                const float maxDelta = defaultHeight - minHeight;
                var actualDelta = defaultHeight - playerHeight;
                var factor = actualDelta / maxDelta;
                var resultingScaleFactorDelta = maxScaleFactorDelta * -factor;
                config.ScaleFactor = defaultScaleFactor + resultingScaleFactorDelta;
                config.ScaleFactor = Mathf.Clamp(config.ScaleFactor, scalingConfig.MinScaleFactor, scalingConfig.MaxScaleFactor);
                WIM.transform.localScale = new Vector3(config.ScaleFactor,config.ScaleFactor,config.ScaleFactor);
            }
        }

        internal static void UpdateScrollingMask(in MiniatureModel WIM) {
            var scrollingConfig = WIM.GetComponent<Scrolling>()?.ScrollingConfig;
            if (scrollingConfig && !scrollingConfig.AllowWIMScrolling) return;
            var boxMaskObj = GameObject.FindWithTag("Box Mask");
            if (!boxMaskObj) return;
            boxMaskObj.transform.localScale = WIM.Configuration.ActiveAreaBounds;
        }

        internal static void UpdateAutoGenerateWIM(in MiniatureModel WIM) {
            var level = GameObject.FindWithTag("Level");
            if (!level) {
                Debug.LogWarning("Level not found.");
                return;
            }

            if (WIM.Configuration.AutoGenerateWIM) {
                // Add script recursive
                level.AddComponent<AutoUpdateWIM>().WIM = WIM;
            }
            else {
                // Destroy script recursive
                Object.DestroyImmediate(level.GetComponent<AutoUpdateWIM>());
            }
        }

        internal static void UpdateCylinderMask(in MiniatureModel WIM) {
            var occlusionHandling = WIM.GetComponent<OcclusionHandling>();
            if(!occlusionHandling || !occlusionHandling.Config) return;
            if (occlusionHandling.Config.OcclusionHandlingMethod != OcclusionHandlingMethod.MeltWalls) return;
            var cylinderTransform = GameObject.FindWithTag("Cylinder Mask")?.transform;
            if (!cylinderTransform) return;
            cylinderTransform.localScale = new Vector3(occlusionHandling.Config.MeltRadius, occlusionHandling.Config.MeltHeight, 1);
        }

        internal static void UpdateCutoutViewMask(in MiniatureModel WIM) {
            var occlusionHandling = WIM.GetComponent<OcclusionHandling>();
            if(!occlusionHandling || !occlusionHandling.Config) return;
            if (occlusionHandling.Config.OcclusionHandlingMethod != OcclusionHandlingMethod.CutoutView) return;
            var spotlightObj = GameObject.FindWithTag("Spotlight Mask");
            if (!spotlightObj) return;
            var spotlight = spotlightObj.GetComponent<Light>();
            spotlight.range = occlusionHandling.Config.CutoutRange;
            spotlight.spotAngle = occlusionHandling.Config.CutoutAngle;

            Color color;
            if (occlusionHandling.Config.ShowCutoutLight) {
                color = occlusionHandling.Config.CutoutLightColor;
                color.a = 1;
            }
            else {
                color = new Color(0, 0, 0, 0);
            }

            spotlight.color = color;
        }

        private static void configureCutoutView(Material material) {
            var spotlightObj = new GameObject("Spotlight Mask");
            spotlightObj.tag = spotlightObj.name;
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo (spotlightObj, "Spotlight Mask");
#endif
            spotlightObj.AddComponent<Light>().type = LightType.Spot;
            var controller = spotlightObj.AddComponent<ConeController>();
            controller.materials = new[] {material};
            controller.SetConeEnabled(true);
            var mainCamera = Camera.main;
            if(mainCamera) spotlightObj.AddComponent<AlignWith>().Target = mainCamera.transform;
        }

        private static void configureMeltWalls(Material material) {
            var cylinderMask = new GameObject("Cylinder Mask");
            cylinderMask.tag = cylinderMask.name;
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo (cylinderMask, "configureMeltWalls");
#endif
            cylinderMask.AddComponent<FollowHand>().hand = Hand.RightHand;
            var controller = cylinderMask.AddComponent<CapsuleController>();
            controller.materials = new[] {material};
            controller.SetCapsuleEnabled(true);
        }

        public static void CleanupOcclusionHandling() {
            var cylinderMask = GameObject.FindWithTag("Cylinder Mask");
            var spotlightMask = GameObject.FindWithTag("Spotlight Mask");

#if UNITY_EDITOR
            if(cylinderMask) Undo.DestroyObjectImmediate (cylinderMask);
            if(spotlightMask) Undo.DestroyObjectImmediate (spotlightMask);
#else
            GameObject.DestroyImmediate(cylinderMask);
            GameObject.DestroyImmediate(spotlightMask);
#endif
        }

        private static void enableScrolling(Material material, in MiniatureModel WIM) {
            var maskController = new GameObject("Box Mask");
            maskController.tag = maskController.name;
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo (maskController, "Created Box Mask");
#endif
            maskController.AddComponent<AlignWith>().Target = WIM.transform;
            var controller = maskController.AddComponent<BoxController>();
            controller.materials = new[] {material};
            controller.SetBoxEnabled(true);
            removeAllColliders(WIM.transform);
            WIM.gameObject.AddComponent<BoxCollider>().size = WIM.Configuration.ActiveAreaBounds / WIM.Configuration.ScaleFactor;
            RemoveDissolveScript(WIM);
            maskController.transform.position = WIM.transform.position;
        }

        public static void DisableScrolling(in MiniatureModel WIM) {
            var boxMask = GameObject.FindWithTag("Box Mask");
#if UNITY_EDITOR
            if(boxMask) Undo.DestroyObjectImmediate (boxMask);
#else
            GameObject.DestroyImmediate(boxMask);
#endif
            removeAllColliders(WIM.transform);
            GenerateColliders(WIM);
        }

        public static void ConfigureWIM(in MiniatureModel WIM) {
            // Cleanup old configuration.
            DisableScrolling(WIM);
            CleanupOcclusionHandling();

            // Setup new configuration.
            var scrollingConfig = WIM.GetComponent<Scrolling>()?.ScrollingConfig;
            var occlusionHandlingConfig = WIM.GetComponent<OcclusionHandling>()?.Config;
            var material = LoadAppropriateMaterial(WIM);
            SetWIMMaterial(material, WIM);
            SetupDissolveScript(WIM);
            if (scrollingConfig && scrollingConfig.AllowWIMScrolling) enableScrolling(material, WIM);
            if (occlusionHandlingConfig && occlusionHandlingConfig.OcclusionHandlingMethod == OcclusionHandlingMethod.CutoutView) configureCutoutView(material);
            if (occlusionHandlingConfig && occlusionHandlingConfig.OcclusionHandlingMethod == OcclusionHandlingMethod.MeltWalls) configureMeltWalls(material);
            UpdateScrollingMask(WIM);
        }
    }
}