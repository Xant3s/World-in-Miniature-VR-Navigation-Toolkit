using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    public class Respawn : MonoBehaviour {
        private WIMConfiguration config;
        private WIMData data;
        private static readonly int progress = Shader.PropertyToID("Vector1_461A9E8C");

        private void OnEnable() {
            MiniatureModel.OnLateInit += respawn;
            MiniatureModel.OnUpdate += checkRespawnWIM;
        }

        private void OnDisable() {
            MiniatureModel.OnLateInit -= respawn;
            MiniatureModel.OnUpdate -= checkRespawnWIM;
        }

        private void respawn(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
            respawnWIM(false);
        }

        private void checkRespawnWIM(WIMConfiguration config, WIMData data) {
            if (!OVRInput.GetUp(config.ShowWIMButton)) return;
            respawnWIM(false);
        }

        public void respawnWIM(bool maintainTransformRelativeToPlayer) {
            var WIM = GameObject.Find("WIM").GetComponent<MiniatureModel>();
            DestinationIndicators.RemoveDestinationIndicators(WIM);

            var WIMLevel = transform.GetChild(0);
            // TODO: decouple occlusion handling
            var dissolveFX = WIM.GetComponent<OcclusionHandling>().OcclusionHandlingConfig.OcclusionHandlingMethod == OcclusionHandlingMethod.None;
            // TODO: decouple scrolling
            if(WIM.GetComponent<Scrolling>().ScrollingConfig.AllowWIMScrolling) dissolveFX = false;
            if(dissolveFX && !maintainTransformRelativeToPlayer) WIMVisualizationUtils.DissolveWIM(WIMLevel);
            if(maintainTransformRelativeToPlayer) WIMVisualizationUtils.InstantDissolveWIM(WIMLevel);

            WIMLevel.parent = null;
            WIMLevel.name = "WIM Level Old";
            data.PlayerRepresentationTransform.parent = null;
            data.WIMLevelTransform = Instantiate(WIMLevel.gameObject, transform).transform;
            data.WIMLevelTransform.gameObject.name = "WIM Level";
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

        private void resolveWIM(Transform WIM) {
            const int resolveDuration = 1;
            for(var i = 0; i < WIM.childCount; i++) {
                var d = WIM.GetChild(i).GetComponent<Dissolve>();
                if(d == null) continue;
                d.durationInSeconds = resolveDuration;
                WIM.GetChild(i).GetComponent<Renderer>().material.SetFloat(progress, 1);
                d.PlayInverse();
            }

            StartCoroutine(WIMVisualizationUtils.FixResolveBug(WIM, resolveDuration));
        }

        private void destroyOldWIMLevel() {
            Destroy(GameObject.Find("WIM Level Old"));
        }
    }
}