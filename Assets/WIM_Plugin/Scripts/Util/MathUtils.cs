using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    public static class MathUtils {
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

        public static Vector3 GetGroundPosition(Vector3 point) {
            return Physics.Raycast(point, Vector3.down, out var hit) ? hit.point : point;
        }
    }
}