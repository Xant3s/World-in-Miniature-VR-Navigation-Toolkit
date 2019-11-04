using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdvancedDissolve_Example;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace WIM_Plugin {
    // Generates the actual WIM. Also takes care of configurating the WIM, i.e. set materials and shaders etc.
    public static class WIMGenerator {
        private static readonly int transparency = Shader.PropertyToID("Vector1_964AF7C");
        private static readonly int baseColor = Shader.PropertyToID("_BaseColor");

        // Load the material appropriate to current WIM configuration.
        private static Material LoadAppropriateMaterial(in MiniatureModel WIM) {
            Material material;
            if (WIM.GetComponent<Scrolling>().ScrollingConfig.AllowWIMScrolling) {
                material = Resources.Load<Material>("Materials/ScrollDissolve");
                setBaseColorAlpha(material, WIM.Configuration.SemiTransparent ? 1 - WIM.Configuration.Transparency : 1 - 0);
                return material;
            }

            switch (WIM.Configuration.OcclusionHandlingMethod) {
                case OcclusionHandlingMethod.MeltWalls: {
                    material = Resources.Load<Material>("Materials/MeltWalls");
                    var color = material.GetColor(baseColor);
                    color.a = 1 - WIM.Configuration.Transparency;
                    material.SetColor(baseColor, color);
                    break;
                }
                case OcclusionHandlingMethod.CutoutView: {
                    material = Resources.Load<Material>("Materials/MeltWalls");
                    setBaseColorAlpha(material, 1 - WIM.Configuration.Transparency);
                    break;
                }
                default:
                    if (WIM.Configuration.SemiTransparent) {
                        material = Resources.Load<Material>("Materials/Dissolve");
                        material.shader = Shader.Find("Shader Graphs/DissolveTransparent");
                        material.SetFloat(transparency, 1 - WIM.Configuration.Transparency);
                    }
                    else {
                        material = Resources.Load<Material>("Materials/Dissolve");
                        material.shader = Shader.Find("Shader Graphs/Dissolve");
                    }

                    break;
            }

            return material;
        }

        // Set the color.alpha of the given material.
        private static void setBaseColorAlpha(Material material, float value) {
            var color = material.GetColor(baseColor);
            color.a = value;
            material.SetColor(baseColor, color);
        }

        private static void SetupDissolveScript(in MiniatureModel WIM) {
            if (WIM.GetComponent<Scrolling>().ScrollingConfig.AllowWIMScrolling || WIM.Configuration.OcclusionHandlingMethod == OcclusionHandlingMethod.MeltWalls ||
                WIM.Configuration.OcclusionHandlingMethod == OcclusionHandlingMethod.CutoutView) {
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

        private static void SetWIMMaterial(Material material, in MiniatureModel WIM) {
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
            var levelChildTransforms = GameObject.Find("Level").GetComponentsInChildren<Transform>();
            Assert.AreNotEqual(wimLevelTransform, GameObject.Find("Level").transform);
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
            var levelTransform = GameObject.Find("Level").transform;
            if (WIM.transform.childCount > 0) Object.DestroyImmediate(WIM.transform.GetChild(0).gameObject);
            var WIMLevel = Object.Instantiate(levelTransform, WIM.transform);
            WIMLevel.localPosition = WIM.Configuration.WIMLevelOffset;
            WIMLevel.name = "WIM Level";
            WIMLevel.gameObject.isStatic = false;
            foreach (Transform child in WIMLevel) {
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
            var scalingConfig = WIM.GetComponent<Scaling>().ScalingConfig;
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
                config.ScaleFactor = Mathf.Clamp(config.ScaleFactor, scalingConfig.MinScaleFactor, scalingConfig.MaxScaleFactor);
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
            if (!WIM.GetComponent<Scrolling>().ScrollingConfig.AllowWIMScrolling) return;
            var boxMaskObj = GameObject.Find("Box Mask");
            if (!boxMaskObj) return;
            boxMaskObj.transform.localScale = WIM.Configuration.ActiveAreaBounds;
        }

        internal static void UpdateAutoGenerateWIM(in MiniatureModel WIM) {
            var level = GameObject.Find("Level");
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
            if (WIM.Configuration.OcclusionHandlingMethod != OcclusionHandlingMethod.MeltWalls) return;
            var cylinderTransform = GameObject.Find("Cylinder Mask")?.transform;
            if (!cylinderTransform) return;
            cylinderTransform.localScale = new Vector3(WIM.Configuration.MeltRadius, WIM.Configuration.MeltHeight, 1);
        }

        internal static void UpdateCutoutViewMask(in MiniatureModel WIM) {
            if (WIM.Configuration.OcclusionHandlingMethod != OcclusionHandlingMethod.CutoutView) return;
            var spotlightObj = GameObject.Find("Spotlight Mask");
            if (!spotlightObj) return;
            var spotlight = spotlightObj.GetComponent<Light>();
            spotlight.range = WIM.Configuration.CutoutRange;
            spotlight.spotAngle = WIM.Configuration.CutoutAngle;

            Color color;
            if (WIM.Configuration.ShowCutoutLight) {
                color = WIM.Configuration.CutoutLightColor;
                color.a = 1;
            }
            else {
                color = new Color(0, 0, 0, 0);
            }

            spotlight.color = color;
        }

        private static void configureCutoutView(Material material) {
            var maskController = new GameObject("Mask Controller");
            var controller = maskController.AddComponent<Controller_Mask_Cone>();
            controller.materials = new[] {material};
            var spotlightObj = new GameObject("Spotlight Mask");
            var spotlight = spotlightObj.AddComponent<Light>();
            controller.spotLight1 = spotlight;
            spotlight.type = LightType.Spot;
            var mainCamera = Camera.main;
            if(mainCamera)
                spotlightObj.AddComponent<AlignWith>().Target = mainCamera.transform;
        }

        private static void configureMeltWalls(Material material) {
            var maskController = new GameObject("Mask Controller");
            var controller = maskController.AddComponent<Controller_Mask_Cylinder>();
            controller.materials = new[] {material};
            var cylinderMask = new GameObject("Cylinder Mask");
            controller.cylinder1 = cylinderMask;
            cylinderMask.AddComponent<FollowHand>().hand = Hand.HAND_R;
        }

        private static void cleanupOcclusionHandling() {
            Object.DestroyImmediate(GameObject.Find("Cylinder Mask"));
            Object.DestroyImmediate(GameObject.Find("Spotlight Mask"));
            Object.DestroyImmediate(GameObject.Find("Mask Controller"));
        }

        private static void enableScrolling(Material material, in MiniatureModel WIM) {
            var maskController = new GameObject("Box Mask");
            var controller = maskController.AddComponent<Controller_Mask_Box>();
            controller.materials = new[] {material};
            var tmpGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var mf = tmpGO.GetComponent<MeshFilter>();
            var cubeMesh = Object.Instantiate(mf.sharedMesh) as Mesh;
            maskController.AddComponent<MeshFilter>().sharedMesh = cubeMesh;
            maskController.AddComponent<AlignWith>().Target = WIM.transform;
            controller.box1 = maskController;
            controller.invert = true;
            removeAllColliders(WIM.transform);
            WIM.gameObject.AddComponent<BoxCollider>().size = WIM.Configuration.ActiveAreaBounds / WIM.Configuration.ScaleFactor;
            RemoveDissolveScript(WIM);
            Object.DestroyImmediate(tmpGO);
            maskController.transform.position = WIM.transform.position;
        }

        private static void disableScrolling(in MiniatureModel WIM) {
            Object.DestroyImmediate(GameObject.Find("Box Mask"));
            removeAllColliders(WIM.transform);
            GenerateColliders(WIM);
        }

        public static void ConfigureWIM(in MiniatureModel WIM) {
            // Cleanup old configuration.
            disableScrolling(WIM);
            cleanupOcclusionHandling();

            // Setup new configuration.
            var s = WIM.GetComponent<Scrolling>();
            if(WIM.Configuration.OcclusionHandlingMethod != OcclusionHandlingMethod.None) s.ScrollingConfig.AllowWIMScrolling = false;
            var material = LoadAppropriateMaterial(WIM);
            SetWIMMaterial(material, WIM);
            SetupDissolveScript(WIM);
            if (s.ScrollingConfig.AllowWIMScrolling) enableScrolling(material, WIM);
            else if (WIM.Configuration.OcclusionHandlingMethod == OcclusionHandlingMethod.CutoutView) configureCutoutView(material);
            else if (WIM.Configuration.OcclusionHandlingMethod == OcclusionHandlingMethod.MeltWalls) configureMeltWalls(material);
            UpdateScrollingMask(WIM);
        }
    }
}