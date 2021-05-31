// Author: Samuel Truman (contact@samueltruman.com)

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using WIMVR.Core;
using WIMVR.Core.Input;
using WIMVR.Util;


namespace WIMVR.Features {
    /// <summary>
    /// Can be used to select a destination by touching it with the index finger and pressing a button.
    /// To confirm another button has to be pressed.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(WIMInput))]
    public class DestinationSelectionTouch : MonoBehaviour {
        private WIMConfiguration config;
        private WIMData data;
        private WIMInput wimInput;
        private MiniatureModel WIM;


        private void Awake() {
            wimInput = GetComponent<WIMInput>();
            WIM = FindObjectOfType<MiniatureModel>();
        }

        private void Start() {
            MiniatureModel.OnLateInitHand += Init;
            wimInput.destinationSelectionTouch.action.performed += ctx => OnDestinationSelectionTouch();
            wimInput.destinationRotation.action.performed += ctx => OnDestinationRotation(ctx.action.ReadValue<Vector2>());
            wimInput.confirmTravel.action.performed += ctx => OnConfirmTravel();
        }

        private void OnDestroy() {
            MiniatureModel.OnLateInitHand -= Init;
        }

        private void Init(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
        }

        private void OnDestinationSelectionTouch() {
            if (config.DestinationSelectionMethod != DestinationSelection.Touch) return;
            SelectDestination();
        }

        private void SelectDestination() {
            // Check if in WIM bounds.
            Assert.IsNotNull(data);
            Assert.IsNotNull(data.FingertipIndexR);
            Assert.IsNotNull(WIM);
            if (!IsInsideWIM(data.FingertipIndexR.position, WIM.gameObject)) return;

            // Remove previous destination point.
            DestinationIndicators.RemoveDestinationIndicators(config, data);
            WIM.CleanupBeforeRespawn();

            // Show destination in WIM.
            DestinationIndicators.SpawnDestinationIndicatorInWIM(config, data);

            // Show destination in level.
            DestinationIndicators.SpawnDestinationIndicatorInLevel(config, data);

            // Rotate destination indicator in WIM (align with pointing direction):
            // Get forward vector from fingertip in WIM space. Set to WIM floor. Won't work if floor is uneven.
            var lookAtPoint =
                data.FingertipIndexR.position + data.FingertipIndexR.right; // fingertip.right because of Oculus prefab
            var pointBFloor = WIM.Converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(lookAtPoint));
            var pointAFloor =
                WIM.Converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(data.FingertipIndexR.position));
            var fingertipForward = pointBFloor - pointAFloor;
            fingertipForward = Quaternion.Inverse(data.WIMLevelTransform.rotation) * fingertipForward;
            // Get current forward vector in WIM space. Set to floor.
            var currForward =
                MathUtils.GetGroundPosition(data.DestinationIndicatorInWIM.position +
                                            data.DestinationIndicatorInWIM.forward)
                - MathUtils.GetGroundPosition(data.DestinationIndicatorInWIM.position);
            // Get signed angle between current forward vector and desired forward vector (pointing direction).
            var angle = Vector3.SignedAngle(currForward, fingertipForward, data.WIMLevelTransform.up);
            // Rotate to align with pointing direction.
            data.DestinationIndicatorInWIM.Rotate(Vector3.up, angle);

            // Rotate destination indicator in level.
            UpdateDestinationRotationInLevel();

            // New destination.
            WIM.NewDestination();
        }

        /// <summary>
        /// Update the destination indicator rotation in level.
        /// Rotation should match destination indicator rotation in WIM.
        /// </summary>
        private void UpdateDestinationRotationInLevel() {
            data.DestinationIndicatorInLevel.rotation = Quaternion.Inverse(data.WIMLevelTransform.rotation) *
                                                        data.DestinationIndicatorInWIM.rotation;
        }

        private bool IsInsideWIM(Vector3 point, GameObject obj) {
            return GetComponents<Collider>().Any(coll => coll.ClosestPoint(point) == point);
        }

        public void OnDestinationRotation(Vector2 inputRotation) {
            // Only if there is a destination indicator in the WIM.
            if(!data) data = WIM.Data;
            if(!config) config = WIM.Configuration;
            if(!data || !config) return;
            if(!data.DestinationIndicatorInWIM) return;
            if(config.DestinationSelectionMethod != global::WIMVR.Util.DestinationSelection.Touch) return;

            if(Math.Abs(inputRotation.magnitude) < 0.01f) return;

            // Rotate destination indicator in WIM via thumbstick.
            var rotationAngle = Mathf.Atan2(inputRotation.x, inputRotation.y) * 180 / Mathf.PI;
            data.DestinationIndicatorInWIM.rotation =
                data.WIMLevelTransform.rotation * Quaternion.Euler(0, rotationAngle, 0);

            // Update destination indicator rotation in level.
            data.DestinationIndicatorInLevel.rotation =
                Quaternion.Inverse(data.WIMLevelTransform.rotation) * data.DestinationIndicatorInWIM.rotation;
        }

        public void OnConfirmTravel() {
            if (config.DestinationSelectionMethod != DestinationSelection.Touch) return;
            if (!data.DestinationIndicatorInLevel) return;
            WIM.ConfirmTravel();
        }
    }
}