// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEngine;

namespace WIMVR {
    /// <summary>
    /// A collection of math utils.
    /// </summary>
    public static class MathUtils {
        /// <summary>
        /// Get corner position of a box collider specified by ID.
        /// </summary>
        /// <param name="box">The box collider.</param>
        /// <param name="id">Specifies which corner of the box collider should be returned.</param>
        /// <returns>The corner of the box collider specified by ID.</returns>
        public static Vector3 GetCorner(BoxCollider box, int id) {
            var extends = box.bounds.extents;
            var center = box.bounds.center;
            switch (id) {
                case 0:
                    // Top right front.
                    return center + box.transform.up * extends.y + box.transform.right * extends.x +
                           box.transform.forward * -extends.z;
                case 1:
                    // top right back.
                    return center + box.transform.up * extends.y + box.transform.right * extends.x +
                           box.transform.forward * +extends.z;
                case 2:
                    // top left back.
                    return center + box.transform.up * extends.y + box.transform.right * -extends.x +
                           box.transform.forward * +extends.z;
                case 3:
                    // top left front.
                    return center + box.transform.up * extends.y + box.transform.right * -extends.x +
                           box.transform.forward * -extends.z;
                case 4:
                    // bottom right front.
                    return center + box.transform.up * -extends.y + box.transform.right * extends.x +
                           box.transform.forward * -extends.z;
                case 5:
                    // bottom right back.
                    return center + box.transform.up * -extends.y + box.transform.right * extends.x +
                           box.transform.forward * -extends.z;
                case 6:
                    // bottom left back.
                    return center + box.transform.up * -extends.y + box.transform.right * -extends.x +
                           box.transform.forward * +extends.z;
                case 7:
                    // bottom left front.
                    return center + box.transform.up * -extends.y + box.transform.right * -extends.x +
                           box.transform.forward * -extends.z;
                default:
                    throw new Exception("Bad input.");
            }
        }

        /// <summary>
        /// Raycast straight down to get closest point on the floor.
        /// </summary>
        /// <param name="point">Start position.</param>
        /// <returns>First raycast hit position straight down from provided start point.</returns>
        public static Vector3 GetGroundPosition(Vector3 point) {
            var layerMask = ~(1 << LayerMask.NameToLayer ("WIM") | 1 << LayerMask.NameToLayer ("Hands")); // Ignore both WIM and hands.
            return Physics.Raycast(point, Vector3.down, out var hit, Mathf.Infinity, layerMask) ? hit.point : point;
        }
    }
}