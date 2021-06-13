﻿// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;
using WIMVR.Core;
using WIMVR.Core.Input;

namespace WIMVR.Features {
    /// <summary>
    /// Respawns the WIM, i.e. destroys the old miniature model and creates a new one at specified position.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(WIMInput))]
    public class Respawn : MonoBehaviour {
        public delegate void RespawnAction(in Transform oldWIMTransform, in Transform newWIMTransform, bool maintainTransformRelativeToPlayer);
        public static event RespawnAction OnEarlyRespawn;
        public static event RespawnAction OnLateRespawn;
        public static bool removeOldWIMLevel = true;
        public static Material materialForOldWIM;
        
        private WIMConfiguration config;
        private WIMData data;
        private WIMInput wimInput;
        private Transform oldWIMLevel;


        private void Awake() {
            wimInput = GetComponent<WIMInput>();
            wimInput.respawn.action.performed += ctx => OnRespawn();
            MiniatureModel.OnCleanupWIMBeforeRespawn += DestinationIndicators.RemoveDestinationIndicators;
            MiniatureModel.OnInit += Init;
        }

        private void Start() {
            var WIM = TryFindWIM();
            var shaderName = WIMGenerator.LoadDefaultMaterial(WIM).shader.name;
            materialForOldWIM = new Material(Shader.Find(shaderName));
        }

        private void Init(WIMConfiguration wimConfig, WIMData wimData) {
            config = wimConfig;
            data = wimData;
        }

        private void OnRespawn() => RespawnWIM(false);

        /// <summary>
        /// Respawns the WIM, i.e. destroys the old miniature model and creates a new one at specified position.
        /// </summary>
        /// <param name="maintainTransformRelativeToPlayer">
        /// Whether the miniature model should be spawned at the same relative position to the player.
        /// If false, the miniature model will be spawned at the default position relative to the player.
        /// </param>
        public void RespawnWIM(bool maintainTransformRelativeToPlayer) {
            var WIM = TryFindWIM();
            WIM.CleanupBeforeRespawn();
            CopyWIM(out var oldWIMLevel, out var levelPos);
            OnEarlyRespawn?.Invoke(oldWIMLevel, data.WIMLevelTransform, maintainTransformRelativeToPlayer);
            SetupNewWIM(maintainTransformRelativeToPlayer, levelPos);
            OnLateRespawn?.Invoke(oldWIMLevel, data.WIMLevelTransform, maintainTransformRelativeToPlayer);
            Cleanup(maintainTransformRelativeToPlayer);
        }

        private static MiniatureModel TryFindWIM() {
            var WIM = FindObjectOfType<MiniatureModel>();
            Assert.IsNotNull(WIM, "No miniature model was found in this scene.");
            return WIM;
        }

        private void CopyWIM(out Transform oldWIMLevel, out Vector3 levelPos) {
            // Copy WIM
            oldWIMLevel = transform.GetChild(0);
            levelPos = oldWIMLevel.position;
            oldWIMLevel.parent = null;
            oldWIMLevel.name = "WIM Level Old";
            oldWIMLevel.tag = "Untagged";
            Assert.IsNotNull(data, "Data not found");
            Assert.IsNotNull(data.PlayerRepresentationTransform);
            Assert.IsNotNull(data.PlayerRepresentationTransform.parent);
            data.PlayerRepresentationTransform.parent = null;
            data.WIMLevelTransform = Instantiate(oldWIMLevel.gameObject, transform).transform;
            data.WIMLevelTransform.gameObject.name = "WIM Level";

            // Copy material
            materialForOldWIM.CopyPropertiesFromMaterial(oldWIMLevel.GetComponentInChildren<Renderer>().material);

            // Apply material to old WIM
            foreach (Transform t in oldWIMLevel) {
                var r = t.GetComponent<Renderer>();
                if (!r) continue;
                r.material = materialForOldWIM;
            }
            
            this.oldWIMLevel = oldWIMLevel;
        }

        private void SetupNewWIM(bool maintainTransformRelativeToPlayer, Vector3 levelPos) {
            StopAllMovementOfRigidbody();
            SetNewWIMPositionAndOrientation(maintainTransformRelativeToPlayer, levelPos);
        }

        private void StopAllMovementOfRigidbody() {
            var rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        private void Cleanup(bool maintainTransformRelativeToPlayer) {
            if (maintainTransformRelativeToPlayer) transform.parent = null;
            if (removeOldWIMLevel) Destroy(oldWIMLevel.gameObject);
        }

        private void SetNewWIMPositionAndOrientation(bool maintainTransformRelativeToPlayer, Vector3 levelPos) {
            data.WIMLevelTransform.position = levelPos;
            data.WIMLevelTransform.rotation = Quaternion.identity;
            data.WIMLevelTransform.localRotation = Quaternion.identity;
            data.WIMLevelTransform.localScale = new Vector3(1, 1, 1);
            data.PlayerRepresentationTransform.parent = data.WIMLevelTransform;

            if (!maintainTransformRelativeToPlayer) {
                var spawnDistanceZ = config.SpawnDistance;
                var spawnDistanceY = (config.WIMSpawnHeight - config.PlayerHeightInCM) / 100;
                var camForwardPosition = data.HMDTransform.position + data.HMDTransform.forward;
                camForwardPosition.y = data.HMDTransform.position.y;
                var camForwardIgnoreY = camForwardPosition - data.HMDTransform.position;
                transform.rotation = Quaternion.identity;
                transform.position = data.HMDTransform.position + camForwardIgnoreY * spawnDistanceZ +
                                     Vector3.up * spawnDistanceY;
            } else {
                transform.position = new Vector3(transform.position.x,
                    data.PlayerController.position.y + data.WIMHeightRelativeToPlayer, transform.position.z);
            }
        }

        private void OnDestroy() {
            MiniatureModel.OnInit -= Init;
        }
    }
}