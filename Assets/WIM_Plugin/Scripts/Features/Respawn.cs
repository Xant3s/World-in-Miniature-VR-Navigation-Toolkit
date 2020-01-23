using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    [ExecuteAlways]
    public class Respawn : MonoBehaviour {
        private static readonly string actionName = "Respawn Button";
        private MiniatureModel WIM;
        private WIMConfiguration config;
        private WIMData data;
        private static readonly int progress = Shader.PropertyToID("_Progress");


        private void Awake() {
            if (!Application.isPlaying) return;
            WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

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
            DestinationIndicators.RemoveDestinationIndicators(WIM);

            var WIMLevel = transform.GetChild(0);
            var dissolveFX = true;
            // TODO: decouple occlusion handling
            //var occlusionHandling = WIM.GetComponent<OcclusionHandling>();
            //if(occlusionHandling && occlusionHandling.Config) 
            //    dissolveFX = occlusionHandling.Config.OcclusionHandlingMethod == OcclusionHandlingMethod.None;
            // TODO: decouple scrolling
            //var scrolling = WIM.GetComponent<Scrolling>();
            //if(scrolling && scrolling.ScrollingConfig && scrolling.ScrollingConfig.AllowWIMScrolling) 
            //    dissolveFX = false;
            if(dissolveFX && !maintainTransformRelativeToPlayer) 
                WIMVisualizationUtils.DissolveWIM(WIMLevel);
            if(maintainTransformRelativeToPlayer) 
                WIMVisualizationUtils.InstantDissolveWIM(WIMLevel);

            WIMLevel.parent = null;
            WIMLevel.name = "WIM Level Old";
            WIMLevel.tag = "WIM Level Old";
            data.PlayerRepresentationTransform.parent = null;
            data.WIMLevelTransform = Instantiate(WIMLevel.gameObject, transform).transform;
            data.WIMLevelTransform.gameObject.name = "WIM Level";
            data.WIMLevelTransform.tag = "Untagged";
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

            if(dissolveFX) {
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
            foreach (Transform child in WIMLevel) {
                var d = child.GetComponent<Dissolve>();
                if(d == null) continue;
                d.durationInSeconds = resolveDuration;
                child.GetComponent<Renderer>().material.SetFloat(progress, 1);
                d.PlayInverse();
            }

            StartCoroutine(WIMVisualizationUtils.FixResolveBug(WIMLevel, resolveDuration));
        }

        private void destroyOldWIMLevel() {
            Destroy(GameObject.FindWithTag("WIM Level Old"));
        }
    }
}