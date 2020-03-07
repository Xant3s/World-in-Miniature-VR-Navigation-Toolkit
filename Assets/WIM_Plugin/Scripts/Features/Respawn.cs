using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    [ExecuteAlways]
    public class Respawn : MonoBehaviour {
        private static readonly string actionName = "Respawn Button";
        private WIMConfiguration config;
        private WIMData data;


        private void OnEnable() {
            MiniatureModel.OnLateInit += respawn;
            InputManager.RegisterAction(actionName, respawn);
        }

        private void OnDisable() {
            MiniatureModel.OnLateInit -= respawn;
            InputManager.UnregisterAction(actionName);
        }

        private void respawn() {
            respawnWIM(false);
        }

        private void respawn(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
            respawnWIM(false);
        }

        public void respawnWIM(bool maintainTransformRelativeToPlayer) {
            if(!Application.isPlaying) return;
            var WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
            DestinationIndicators.RemoveDestinationIndicators(WIM);

            var WIMLevel = transform.GetChild(0);
            // TODO: decouple dissolve
            var dissolveFX = true;


            // Copy WIM
            WIMLevel.parent = null;
            WIMLevel.name = "WIM Level Old";
            WIMLevel.tag = "WIM Level Old";
            data.PlayerRepresentationTransform.parent = null;
            data.WIMLevelTransform = Instantiate(WIMLevel.gameObject, transform).transform;
            data.WIMLevelTransform.gameObject.name = "WIM Level";
            data.WIMLevelTransform.tag = "Untagged";

            // Copy material
            var mat = new Material(Shader.Find("Shader Graphs/WIM Shader Unlit (Pro)2"));
            mat.CopyPropertiesFromMaterial(WIMLevel.GetComponentInChildren<Renderer>().material);


            // Apply material to old WIM
            foreach (Transform t in WIMLevel) {
                var r = t.GetComponent<Renderer>();
                if(!r) continue;
                r.material = mat;
            }

            // Copy box mask for old WIM    
            var boxMask = GameObject.FindWithTag("Box Mask");
            if (boxMask) {
                var oldBoxMask = Instantiate(boxMask);
                oldBoxMask.GetComponent<AlignWith>().Target = WIMLevel;
                var oldBoxController = oldBoxMask.GetComponent<BoxController>();        // TODO: decouple
                oldBoxController.materials = new[] {mat};
                oldBoxController.SetBoxEnabled(true);
            }

            // Dissolve old WIM
            WIMLevel.gameObject.AddComponent<Dissolve>().materials = new[] { mat };
            if (dissolveFX && !maintainTransformRelativeToPlayer)
                WIMVisualizationUtils.DissolveWIM(WIMLevel);
            if (maintainTransformRelativeToPlayer)
                WIMVisualizationUtils.InstantDissolveWIM(WIMLevel);

            var WIMLayer = LayerMask.NameToLayer("WIM");
            data.WIMLevelTransform.gameObject.layer = WIMLayer;
            foreach (Transform child in data.WIMLevelTransform) {
                child.gameObject.layer = WIMLayer;
            }
            var rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            data.WIMLevelTransform.localPosition = maintainTransformRelativeToPlayer
                ? data.WIMLevelLocalPosOnTravel
                : config.WIMLevelOffset;
            data.WIMLevelTransform.rotation = Quaternion.identity;
            data.WIMLevelTransform.localRotation = Quaternion.identity;
            data.WIMLevelTransform.localScale = new Vector3(1, 1, 1);
            data.PlayerRepresentationTransform.parent = data.WIMLevelTransform;

            if(!maintainTransformRelativeToPlayer) {
                var spawnDistanceZ = config.SpawnDistance;
                var spawnDistanceY = (config.WIMSpawnHeight - config.PlayerHeightInCM) / 100;
                var camForwardPosition = data.HMDTransform.position + data.HMDTransform.forward;
                camForwardPosition.y = data.HMDTransform.position.y;
                var camForwardIgnoreY = camForwardPosition - data.HMDTransform.position;
                transform.rotation = Quaternion.identity;
                transform.position = data.HMDTransform.position + camForwardIgnoreY * spawnDistanceZ +
                                     Vector3.up * spawnDistanceY;
            }
            else {
                transform.position = new Vector3(transform.position.x,
                    data.OVRPlayerController.position.y + data.WIMHeightRelativeToPlayer, transform.position.z);
            }

            if (dissolveFX) {
                resolveWIM(data.WIMLevelTransform);
                Invoke(nameof(destroyOldWIMLevel), 1.1f);
            }
            else {
                destroyOldWIMLevel();
            }

            if(maintainTransformRelativeToPlayer) transform.parent = null;
        }

        private void resolveWIM(Transform WIMLevel) {
            const int resolveDuration = 1;
            var d = WIMLevel.parent.GetComponent<Dissolve>();
            if (!d) return;
            d.durationInSeconds = resolveDuration;
            d.SetProgress(1);
            d.PlayInverse();

            StartCoroutine(WIMVisualizationUtils.FixResolveBug(WIMLevel, resolveDuration));
        }

        private void destroyOldWIMLevel() {
            Destroy(GameObject.FindWithTag("WIM Level Old"));
        }
    }
}