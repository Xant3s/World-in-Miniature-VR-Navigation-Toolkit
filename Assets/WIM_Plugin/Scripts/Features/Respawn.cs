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
            DestinationIndicators.RemoveDestinationIndicators(GameObject.Find("WIM").GetComponent<MiniatureModel>());

            var WIMLevel = transform.GetChild(0);
            var dissolveFX = config.OcclusionHandlingMethod == OcclusionHandling.None;
            if(config.AllowWIMScrolling) dissolveFX = false;
            if(dissolveFX && !maintainTransformRelativeToPlayer) WIMVisualizationUtils.DissolveWIM(WIMLevel);
            if(maintainTransformRelativeToPlayer) WIMVisualizationUtils.InstantDissolveWIM(WIMLevel);

            WIMLevel.parent = null;
            WIMLevel.name = "WIM Level Old";
            data.PlayerRepresentationTransform.parent = null;
            var newWIMLevel = Instantiate(WIMLevel.gameObject, transform).transform;
            newWIMLevel.gameObject.name = "WIM Level";
            data.WIMLevelTransform = newWIMLevel;
            var rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            newWIMLevel.position = Vector3.zero;
            newWIMLevel.localPosition = maintainTransformRelativeToPlayer
                ? data.WIMLevelLocalPosOnTravel
                : data.WIMLevelTransform.localPosition;
            newWIMLevel.rotation = Quaternion.identity;
            newWIMLevel.localRotation = Quaternion.identity;
            newWIMLevel.localScale = new Vector3(1, 1, 1);
            data.PlayerRepresentationTransform.parent = newWIMLevel;

            if(!maintainTransformRelativeToPlayer) {
                var spawnDistanceZ = (config.PlayerArmLength <= 0)
                    ? config.WIMSpawnOffset.z
                    : config.PlayerArmLength;
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
                resolveWIM(newWIMLevel);
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