﻿using System;
using System.Linq;
using UnityEngine;

namespace WIM_Plugin {
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class DestinationSelectionTouch : MonoBehaviour {
        private static readonly string selectionActionName = "Destination Selection Button";
        private static readonly string rotationActionName = "Destination Rotation Thumbstick";
        private static readonly string confirmActionName = "Confirm Travel Button";
        private WIMConfiguration config;
        private WIMData data;
        private MiniatureModel WIM;


        private void Awake() {
            if (!Application.isPlaying) return;
            WIM = GameObject.FindWithTag("WIM").GetComponent<MiniatureModel>();
        }

        private void OnEnable() {
            MiniatureModel.OnLateInit += Init;
            InputManager.RegisterAction(selectionActionName, DestinationSelection);
            InputManager.RegisterAction(rotationActionName, SelectDestinationRotation);
            InputManager.RegisterAction(confirmActionName, ConfirmTeleport);
        }

        private void OnDisable() {
            MiniatureModel.OnLateInit -= Init;
            InputManager.UnregisterAction(selectionActionName);
            InputManager.UnregisterAction(rotationActionName);
            InputManager.UnregisterAction(confirmActionName);
        }

        private void Init(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
        }

        private void DestinationSelection() {
            if (config.DestinationSelectionMethod != WIM_Plugin.DestinationSelection.Touch) return;
            SelectDestination();
        }

        private void SelectDestination() {
            if (!Application.isPlaying) return;

            // Check if in WIM bounds.
            if (!IsInsideWIM(data.FingertipIndexR.position, WIM.gameObject)) return;

            // Remove previous destination point.
            DestinationIndicators.RemoveDestinationIndicators(WIM);

            // Show destination in WIM.
            DestinationIndicators.SpawnDestinationIndicatorInWIM(WIM.Configuration, WIM.Data);

            // Show destination in level.
            DestinationIndicators.SpawnDestinationIndicatorInLevel(WIM.Configuration, WIM.Data);

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

        private void SelectDestinationRotation(Vector3 axis) {
            if (!Application.isPlaying) return;
            // Only if there is a destination indicator in the WIM.
            if (!data) data = WIM.Data;
            if (!config) config = WIM.Configuration;
            if (!data || !config) return;
            if (!data.DestinationIndicatorInWIM) return;
            if (config.DestinationSelectionMethod != WIM_Plugin.DestinationSelection.Touch) return;

            // Thumbstick input.
            Vector2 inputRotation = axis;

            if (Math.Abs(inputRotation.magnitude) < 0.01f) return;

            // Rotate destination indicator in WIM via thumbstick.
            var rotationAngle = Mathf.Atan2(inputRotation.x, inputRotation.y) * 180 / Mathf.PI;
            data.DestinationIndicatorInWIM.rotation =
                data.WIMLevelTransform.rotation * Quaternion.Euler(0, rotationAngle, 0);

            // Update destination indicator rotation in level.
            data.DestinationIndicatorInLevel.rotation =
                Quaternion.Inverse(data.WIMLevelTransform.rotation) * data.DestinationIndicatorInWIM.rotation;
        }

        private void ConfirmTeleport() {
            if (!Application.isPlaying) return;
            if (config.DestinationSelectionMethod != WIM_Plugin.DestinationSelection.Touch) return;
            if (!data.DestinationIndicatorInLevel) return;
            WIM.ConfirmTravel();
        }
    }
}