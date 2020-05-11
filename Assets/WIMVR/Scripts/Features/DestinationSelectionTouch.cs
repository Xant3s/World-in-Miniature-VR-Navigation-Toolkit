// Author: Samuel Truman (contact@samueltruman.com)

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using WIMVR.Core;
using WIMVR.Util;


namespace WIMVR.Features {
    /// <summary>
    /// Can be used to select a destination by touching it with the index finger and pressing a button.
    /// To confirm another button has to be pressed.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class DestinationSelectionTouch : MonoBehaviour {
        private WIMConfiguration config;
        private WIMData data;
        private MiniatureModel WIM;


        private void Start() {
            if (!Application.isPlaying) return;
            WIM = GameObject.FindWithTag("WIM").GetComponent<MiniatureModel>();
        }

        private void OnEnable() {
            MiniatureModel.OnLateInitHand += Init;
        }

        private void OnDisable() {
            MiniatureModel.OnLateInitHand -= Init;
        }

        private void Init(WIMConfiguration config, WIMData data) {
            this.config = config;
            this.data = data;
        }

        private void OnDestinationSelectionTouch() {
            if (config.DestinationSelectionMethod != global::WIMVR.Util.DestinationSelection.Touch) return;
            SelectDestination();
        }

        private void SelectDestination() {
            if (!Application.isPlaying) return;

            // Check if in WIM bounds.
            Assert.IsNotNull(data);
            Assert.IsNotNull(data.FingertipIndexR);
            Assert.IsNotNull(WIM);
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

        public void OnDestinationRotation(InputValue value) {
            if(!Application.isPlaying) return;
            // Only if there is a destination indicator in the WIM.
            if(!data) data = WIM.Data;
            if(!config) config = WIM.Configuration;
            if(!data || !config) return;
            if(!data.DestinationIndicatorInWIM) return;
            if(config.DestinationSelectionMethod != global::WIMVR.Util.DestinationSelection.Touch) return;

            // Thumbstick input.
            var inputRotation = value.Get<Vector2>();

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
            if (!Application.isPlaying) return;
            if (config.DestinationSelectionMethod != DestinationSelection.Touch) return;
            if (!data.DestinationIndicatorInLevel) return;
            WIM.ConfirmTravel();
        }
    }
}