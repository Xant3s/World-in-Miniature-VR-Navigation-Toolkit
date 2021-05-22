// Author: Samuel Truman (contact@samueltruman.com)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;
using WIMVR.Util;
using MathUtils = WIMVR.Util.MathUtils;
using Object = UnityEngine.Object;

namespace WIMVR.Core {
    /// <summary>
    /// Generates the miniature model by cloning the actual level etc.
    /// </summary>
    public static class WIMGenerator {
        public delegate void GeneratorAction(in MiniatureModel WIM);

        private static readonly int alpha = Shader.PropertyToID("_Alpha");
        public static event GeneratorAction OnConfigure;
        public static event GeneratorAction OnPreConfigure;
        public static event GeneratorAction OnAdaptScaleToPlayerHeight;


        /// <summary>
        /// Loads the default WIM material by name.
        /// </summary>
        /// <param name="WIM">The miniature model.</param>
        /// <returns>The loaded material.</returns>
        public static Material LoadDefaultMaterial(MiniatureModel WIM) {
            var material = Resources.Load<Material>("WIM Material");
            var fullyOpaque = 1;
            var semiTransparent = 1 - WIM.Configuration.Transparency;
            material.SetFloat(alpha, WIM.Configuration.SemiTransparent ? semiTransparent : fullyOpaque);
            return material;
        }

        /// <summary>
        /// Applies specified material to every object that is part of the miniature model.
        /// </summary>
        /// <param name="material">The material to apply to the miniature model.</param>
        /// <param name="WIM">The miniature model.</param>
        public static void SetWIMMaterial(Material material, in MiniatureModel WIM) {
            var WIMLevelTransform = WIM.transform.GetChild(0);
            foreach (var renderer in WIMLevelTransform.GetComponentsInChildren<Renderer>()) {
                var newMaterials = renderer.sharedMaterials;
                for (var i = 0; i < newMaterials.Length; i++) {
                    newMaterials[i] = material;
                }
                renderer.sharedMaterials = newMaterials;
            }
        }

        /// <summary>
        /// Generates colliders for miniature model. Therefore, the colliders from the full-sized level are copied.
        /// </summary>
        /// <param name="WIM">The miniature model.</param>
        public static void GenerateColliders(in MiniatureModel WIM) {
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
                childInWIM.RemoveAllColliders();
                var childBoxCollider = childInWIM.gameObject.AddComponent<BoxCollider>();
                // 3. move collider to WIM root (consider scale and position)
                var rootCollider = WIM.gameObject.AddComponent<BoxCollider>();
                rootCollider.center = childInWIM.localPosition;
                rootCollider.size = Vector3.zero;
                var bounds = rootCollider.bounds;
                bounds.Encapsulate(childBoxCollider.bounds);
                rootCollider.size = bounds.size / WIM.Configuration.ScaleFactor;
                childInWIM.RemoveAllColliders();
            }

            // 4. remove every collider that is fully inside another one.
            PruneColliders(WIM);
            // 5. extend collider (esp. upwards)
            ExpandColliders(WIM);
        }

        /// <summary>
        /// Generates a new miniature model.
        /// </summary>
        /// <param name="WIM">The miniature model component.</param>
        public static void GenerateNewWIM(in MiniatureModel WIM) {
            WIM.transform.RemoveAllColliders();
            AdaptScaleFactorToPlayerHeight(WIM);
            var levelTransform = GameObject.FindWithTag("Level").transform;
#if UNITY_EDITOR
            if (WIM.transform.childCount > 0) Undo.DestroyObjectImmediate(WIM.transform.GetChild(0).gameObject);
#else
            if (WIM.transform.childCount > 0) Object.DestroyImmediate(WIM.transform.GetChild(0).gameObject);
#endif
            var WIMLevel = Object.Instantiate(levelTransform, WIM.transform);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(WIMLevel.gameObject, "GenerateNewWIM");
#endif
            WIMLevel.localPosition = WIM.Configuration.WIMLevelOffset;
            WIMLevel.name = "WIM Level";
            WIMLevel.tag = "Untagged";
            WIMLevel.gameObject.isStatic = false;
            var WIMLayer = LayerMask.NameToLayer("WIM");
            WIMLevel.gameObject.layer = WIMLayer;
            foreach (Transform child in WIMLevel) {
                child.gameObject.layer = WIMLayer;
                Object.DestroyImmediate(child.GetComponent(typeof(XRGrabInteractable)));
                Object.DestroyImmediate(child.GetComponent(typeof(OffsetGrabInteractable)));
                Object.DestroyImmediate(child.GetComponent(typeof(Rigidbody)));
                var renderer = child.GetComponent<Renderer>();
                if (renderer) {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }

                child.gameObject.isStatic = false;
            }

            WIM.transform.localScale = new Vector3(WIM.Configuration.ScaleFactor, WIM.Configuration.ScaleFactor, WIM.Configuration.ScaleFactor);
            WIM.transform.RemoveAllColliders();
            GenerateColliders(WIM);
            ConfigureWIM(WIM);
        }

        /// <summary>
        /// Configures the miniature model.
        /// </summary>
        /// <param name="WIM"></param>
        public static void ConfigureWIM(in MiniatureModel WIM) {
            // Cleanup old configuration.
            OnPreConfigure?.Invoke(WIM);

            // Setup new configuration.
            var material = LoadDefaultMaterial(WIM);
            SetWIMMaterial(material, WIM);
            OnConfigure?.Invoke(WIM);
        }

        private static void ExpandColliders(in MiniatureModel WIM) {
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

        private static void PruneColliders(in MiniatureModel WIM) {
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

        private static void AdaptScaleFactorToPlayerHeight(in MiniatureModel WIM) {
            var config = WIM.Configuration;
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

            } else if (heightDelta < 0) {
                const float maxDelta = defaultHeight - minHeight;
                var actualDelta = defaultHeight - playerHeight;
                var factor = actualDelta / maxDelta;
                var resultingScaleFactorDelta = maxScaleFactorDelta * -factor;
                config.ScaleFactor = defaultScaleFactor + resultingScaleFactorDelta;
            }
            if(Math.Abs(heightDelta) < 0.0001) return;
            OnAdaptScaleToPlayerHeight?.Invoke(WIM);
            WIM.transform.localScale = new Vector3(config.ScaleFactor, config.ScaleFactor, config.ScaleFactor);
        }
    }
}