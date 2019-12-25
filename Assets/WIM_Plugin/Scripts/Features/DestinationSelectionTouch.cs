using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WIM_Plugin {
    [ExecuteAlways]
    public class DestinationSelectionTouch : MonoBehaviour {
        private static readonly string selectionActionName = "Destination Selection Button";
        private static readonly string rotationActionName = "Destination Rotation Thumbstick";
        private static readonly string confirmActionName = "Confirm Travel Button";
        private WIMConfiguration config;
        private WIMData data;

        private void OnEnable() {
            MiniatureModel.OnLateInit += init;
            InputManager.RegisterAction(selectionActionName, destinationSelection);
            InputManager.RegisterAction(rotationActionName, selectDestinationRotation);
            InputManager.RegisterAction(confirmActionName, confirmTeleport);
        }

        private void OnDisable() {
            MiniatureModel.OnLateInit -= init;
            InputManager.UnregisterAction(selectionActionName);
            InputManager.UnregisterAction(rotationActionName);
            InputManager.UnregisterAction(confirmActionName);
        }

        private void init(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
        }

        private void destinationSelection() {
            if (config.DestinationSelectionMethod != DestinationSelection.Selection) return;
            selectDestination();
        }

        private void selectDestination() {
        var WIM = GameObject.Find("WIM").GetComponent<MiniatureModel>();

        // Check if in WIM bounds.
        if (!isInsideWIM(data.FingertipIndexR.position, WIM.gameObject)) return;

        // Remove previous destination point.
        DestinationIndicators.RemoveDestinationIndicators(WIM);

        // Show destination in WIM.
        DestinationIndicators.SpawnDestinationIndicatorInWIM(WIM.Configuration, WIM.Data);

        // Show destination in level.
        DestinationIndicators.SpawnDestinationIndicatorInLevel(WIM.Configuration, WIM.Data);

        // Rotate destination indicator in WIM (align with pointing direction):
        // Get forward vector from fingertip in WIM space. Set to WIM floor. Won't work if floor is uneven.
        var lookAtPoint = data.FingertipIndexR.position + data.FingertipIndexR.right; // fingertip.right because of Oculus prefab
        var pointBFloor = WIM.Converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(lookAtPoint));
        var pointAFloor = WIM.Converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(data.FingertipIndexR.position));
        var fingertipForward = pointBFloor - pointAFloor;
        fingertipForward = Quaternion.Inverse(data.WIMLevelTransform.rotation) * fingertipForward;
        // Get current forward vector in WIM space. Set to floor.
        var currForward = MathUtils.GetGroundPosition(data.DestinationIndicatorInWIM.position + data.DestinationIndicatorInWIM.forward)
                          - MathUtils.GetGroundPosition(data.DestinationIndicatorInWIM.position);
        // Get signed angle between current forward vector and desired forward vector (pointing direction).
        var angle = Vector3.SignedAngle(currForward, fingertipForward, data.WIMLevelTransform.up);
        // Rotate to align with pointing direction.
        data.DestinationIndicatorInWIM.Rotate(Vector3.up, angle);

        // Rotate destination indicator in level.
        updateDestinationRotationInLevel();

        // New destination.
        WIM.NewDestination();
    }

        /// <summary>
        /// Update the destination indicator rotation in level.
        /// Rotation should match destination indicator rotation in WIM.
        /// </summary>
        private void updateDestinationRotationInLevel() {
            data.DestinationIndicatorInLevel.rotation = Quaternion.Inverse(data.WIMLevelTransform.rotation) * data.DestinationIndicatorInWIM.rotation;
        }

        private bool isInsideWIM(Vector3 point, GameObject obj) {
            return GetComponents<Collider>().Any(coll => coll.ClosestPoint(point) == point);
        }

        private void selectDestinationRotation(Vector3 axis) {
            // Only if there is a destination indicator in the WIM.
            var WIM = GameObject.Find("WIM").GetComponent<MiniatureModel>();
            if (!data) data = WIM.Data;
            if (!config) config = WIM.Configuration;
            if (!data || !config) return;
            if (!data.DestinationIndicatorInWIM) return;
            if (config.DestinationSelectionMethod != DestinationSelection.Selection) return;

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

        private void confirmTeleport() {
            if (config.DestinationSelectionMethod != DestinationSelection.Selection) return;
            if (!data.DestinationIndicatorInLevel) return;
            GameObject.Find("WIM").GetComponent<MiniatureModel>().ConfirmTravel();
        }
    }
}