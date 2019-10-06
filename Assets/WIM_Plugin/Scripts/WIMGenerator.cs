using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace WIM_Plugin {
    // Generates the actual WIM. Also takes care of configurating the WIM, i.e. set materials and shaders etc.
    public class WIMGenerator {
        // Load the material appropriate to current WIM configuration.
        internal Material LoadAppropriateMaterial(in MiniatureModel WIM) {
            Material material;
            if (WIM.AllowWIMScrolling) {
                material = Resources.Load<Material>("Materials/ScrollDissolve");
                setBaseColorAlpha(material, WIM.SemiTransparent ? 1 - WIM.transparency : 1 - 0);
                return material;
            }

            switch (WIM.occlusionHandling) {
                case OcclusionHandling.MeltWalls: {
                    material = Resources.Load<Material>("Materials/MeltWalls");
                    var color = material.GetColor("_BaseColor");
                    color.a = 1 - WIM.transparency;
                    material.SetColor("_BaseColor", color);
                    break;
                }
                case OcclusionHandling.CutoutView: {
                    material = Resources.Load<Material>("Materials/MeltWalls");
                    setBaseColorAlpha(material, 1 - WIM.transparency);
                    break;
                }
                case OcclusionHandling.None:
                default:
                    if (WIM.SemiTransparent) {
                        material = Resources.Load<Material>("Materials/Dissolve");
                        material.shader = Shader.Find("Shader Graphs/DissolveTransparent");
                        material.SetFloat("Vector1_964AF7C", 1 - WIM.transparency);
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
        private void setBaseColorAlpha(Material material, float value) {
            var color = material.GetColor("_BaseColor");
            color.a = value;
            material.SetColor("_BaseColor", color);
        }

        public void SetupDissolveScript(in MiniatureModel WIM) {
            if (WIM.AllowWIMScrolling || WIM.occlusionHandling == OcclusionHandling.MeltWalls ||
                WIM.occlusionHandling == OcclusionHandling.CutoutView) {
                RemoveDissolveScript(WIM);
            }
            else {
                AddDissolveScript(WIM);
            }
        }

        public void RemoveDissolveScript(in MiniatureModel WIM) {
            var WIMLevelTransform = WIM.transform.GetChild(0);
            foreach (Transform child in WIMLevelTransform) {
                Object.DestroyImmediate(child.GetComponent<Dissolve>());
            }
        }

        public void AddDissolveScript(in MiniatureModel WIM) {
            var WIMLevelTransform = WIM.transform.GetChild(0);
            foreach (Transform child in WIMLevelTransform) {
                if (!child.GetComponent<Dissolve>())
                    child.gameObject.AddComponent<Dissolve>();
            }
        }

        public void SetWIMMaterial(Material material, in MiniatureModel WIM) {
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

        private void expandColliders(in MiniatureModel WIM) {
            foreach (var boxCollider in WIM.gameObject.GetComponents<BoxCollider>()) {
                boxCollider.size += new Vector3(WIM.expandCollidersX.x + WIM.expandCollidersX.y, 0, 0);
                boxCollider.center += Vector3.left * WIM.expandCollidersX.x / 2.0f;
                boxCollider.center += Vector3.right * WIM.expandCollidersX.y / 2.0f;

                boxCollider.size += new Vector3(0, WIM.expandCollidersY.x + WIM.expandCollidersY.y, 0);
                boxCollider.center += Vector3.up * WIM.expandCollidersY.x / 2.0f;
                boxCollider.center += Vector3.down * WIM.expandCollidersY.y / 2.0f;

                boxCollider.size += new Vector3(0, 0, WIM.expandCollidersZ.x + WIM.expandCollidersZ.y);
                boxCollider.center += Vector3.forward * WIM.expandCollidersZ.x / 2.0f;
                boxCollider.center += Vector3.back * WIM.expandCollidersZ.y / 2.0f;
            }
        }

        private void pruneColliders(in MiniatureModel WIM) {
            var destoryList = new List<Collider>();
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
                    destoryList.Add(col);
                    break;
                }
            }

            while (destoryList.Count() != 0) {
                Object.DestroyImmediate(destoryList[0]);
                destoryList.RemoveAt(0);
            }
        }

        [ExecuteInEditMode]
        internal void GenerateColliders(in MiniatureModel WIM) {
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
                rootCollider.size = bounds.size / WIM.ScaleFactor;
                removeAllColliders(childInWIM);
            }

            // 4. remove every collider that is fully inside another one.
            pruneColliders(WIM);
            // 5. extend collider (esp. upwards)
            expandColliders(WIM);
        }

        private void removeAllColliders(Transform t) {
            Collider collider;
            while (collider = t.GetComponent<Collider>()) {
                // Assignment
                Object.DestroyImmediate(collider);
            }
        }

        public void GenerateNewWIM(in MiniatureModel WIM) {
            removeAllColliders(WIM.transform);
            WIM.adaptScaleFactorToPlayerHeight();
            var levelTransform = GameObject.Find("Level").transform;
            if (WIM.transform.childCount > 0) Object.DestroyImmediate(WIM.transform.GetChild(0).gameObject);
            var WIMLevel = Object.Instantiate(levelTransform, WIM.transform);
            WIMLevel.localPosition = WIM.WIMLevelOffset;
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

            WIM.transform.localScale = new Vector3(WIM.ScaleFactor, WIM.ScaleFactor, WIM.ScaleFactor);
            WIM.ConfigureWIM();
        }

        internal void UpdateTransparency(in MiniatureModel WIM) {
            if (WIM.SemiTransparent == WIM.PrevSemiTransparent &&
                Math.Abs(WIM.transparency - WIM.PrevTransparency) < .1f) return;
            WIM.PrevTransparency = WIM.transparency;
            WIM.PrevSemiTransparent = WIM.SemiTransparent;
            WIM.ConfigureWIM();
        }

        internal void UpdateScrollingMask(in MiniatureModel WIM) {
            if (!WIM.AllowWIMScrolling) return;
            var boxMaskObj = GameObject.Find("Box Mask");
            if (!boxMaskObj) return;
            boxMaskObj.transform.localScale = WIM.activeAreaBounds;
        }

        internal void UpdateAutoGenerateWIM(in MiniatureModel WIM) {
            var level = GameObject.Find("Level");
            if (!level) {
                Debug.LogWarning("Level not found.");
                return;
            }

            if (WIM.AutoGenerateWIM) {
                // Add script recursive
                level.AddComponent<AutoUpdateWIM>().WIM = WIM;
            }
            else {
                // Destroy script recursive
                Object.DestroyImmediate(level.GetComponent<AutoUpdateWIM>());
            }
        }

        internal bool ScrollingPropertyChanged(in MiniatureModel WIM) {
            if (WIM.AllowWIMScrolling == WIM.PrevAllowWIMScrolling) return false;
            WIM.PrevAllowWIMScrolling = WIM.AllowWIMScrolling;
            return true;
        }

        internal bool OcclusionHandlingStrategyChanged(in MiniatureModel WIM) {
            if (WIM.occlusionHandling == WIM.prevOcclusionHandling) return false;
            WIM.prevOcclusionHandling = WIM.occlusionHandling;
            if (WIM.occlusionHandling != OcclusionHandling.None) {
                WIM.AllowWIMScrolling = WIM.PrevAllowWIMScrolling = false;
            }

            return true;
        }

        internal void UpdateCylinderMask(in MiniatureModel WIM) {
            if (WIM.occlusionHandling != OcclusionHandling.MeltWalls) return;
            var cylinderTransform = GameObject.Find("Cylinder Mask").transform;
            if (!cylinderTransform) return;
            cylinderTransform.localScale = new Vector3(WIM.meltRadius, WIM.meltHeight, 1);
        }

        internal void UpdateCutoutViewMask(in MiniatureModel WIM) {
            if (WIM.occlusionHandling != OcclusionHandling.CutoutView) return;
            var spotlightObj = GameObject.Find("Spotlight Mask");
            if (!spotlightObj) return;
            var spotlight = spotlightObj.GetComponent<Light>();
            spotlight.range = WIM.cutoutRange;
            spotlight.spotAngle = WIM.cutoutAngle;

            Color color;
            if (WIM.showCutoutLight) {
                color = WIM.cutoutLightColor;
                color.a = 1;
            }
            else {
                color = new Color(0, 0, 0, 0);
            }

            spotlight.color = color;
        }
    }
}