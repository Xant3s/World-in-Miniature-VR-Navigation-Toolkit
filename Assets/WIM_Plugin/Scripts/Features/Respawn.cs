// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class Respawn : MonoBehaviour {
        public delegate void RespawnAction(in Transform oldWIMTransform, in Transform newWIMTransform, 
            bool maintainTransformRelativeToPlayer);
        public static event RespawnAction OnEarlyRespawn;
        public static event RespawnAction OnLateRespawn;
        public static bool RemoveOldWIMLevel = true;
        public static Material materialForOldWIM;

        private static readonly string actionName = "Respawn Button";
        private static readonly string actionTooltip = "Button used to respawn the miniature model.";
        private WIMConfiguration config;
        private WIMData data;


        private void OnEnable() {
            MiniatureModel.OnLateInit += StartRespawn;
            InputManager.RegisterAction(actionName, StartRespawn, tooltip: actionTooltip);
            var WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
            var shaderName = WIMGenerator.LoadDefaultMaterial(WIM).shader.name;
            materialForOldWIM = new Material(Shader.Find(shaderName + "2"));
        }

        private void OnDisable() {
            MiniatureModel.OnLateInit -= StartRespawn;
            InputManager.UnregisterAction(actionName);
        }

        private void StartRespawn() {
            RespawnWIM(false);
        }

        private void StartRespawn(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
            RespawnWIM(false);
        }

        public void RespawnWIM(bool maintainTransformRelativeToPlayer) {
            if(!Application.isPlaying) return;
            var WIM = GameObject.FindWithTag("WIM")?.GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
            DestinationIndicators.RemoveDestinationIndicators(WIM);

            // Copy WIM
            var WIMLevel = transform.GetChild(0);
            var levelPos = WIMLevel.position;

            WIMLevel.parent = null;
            WIMLevel.name = "WIM Level Old";
            WIMLevel.tag = "WIM Level Old";
            data.PlayerRepresentationTransform.parent = null;
            data.WIMLevelTransform = Instantiate(WIMLevel.gameObject, transform).transform;
            data.WIMLevelTransform.gameObject.name = "WIM Level";
            data.WIMLevelTransform.tag = "Untagged";

            // Copy material
            materialForOldWIM.CopyPropertiesFromMaterial(WIMLevel.GetComponentInChildren<Renderer>().material);

            // Apply material to old WIM
            foreach (Transform t in WIMLevel) {
                var r = t.GetComponent<Renderer>();
                if(!r) continue;
                r.material = materialForOldWIM;
            }

            OnEarlyRespawn?.Invoke(WIMLevel, data.WIMLevelTransform, maintainTransformRelativeToPlayer);

            var WIMLayer = LayerMask.NameToLayer("WIM");
            data.WIMLevelTransform.gameObject.layer = WIMLayer;
            foreach (Transform child in data.WIMLevelTransform) {
                child.gameObject.layer = WIMLayer;
            }
            var rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            data.WIMLevelTransform.position = levelPos;

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

            OnLateRespawn?.Invoke(WIMLevel, data.WIMLevelTransform, maintainTransformRelativeToPlayer);
            if (maintainTransformRelativeToPlayer) transform.parent = null;
            if (RemoveOldWIMLevel) DestroyOldWIMLevel();
        }

        private static void DestroyOldWIMLevel() {
            Destroy(GameObject.FindWithTag("WIM Level Old"));
        }
    }
}