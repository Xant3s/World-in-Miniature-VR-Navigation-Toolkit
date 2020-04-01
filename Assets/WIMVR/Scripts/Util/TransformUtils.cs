// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR {
    /// <summary>
    /// A collection of extension methods for the Transform component.
    /// </summary>
    public static class TransformUtils {
        /// <summary>
        /// Removes all colliders attached to this transform. Does not touch colliders in children.
        /// </summary>
        /// <param name="t">The Transform to be freed of all colliders.</param>
        public static void RemoveAllColliders(this Transform t) {
            Collider collider;
            // ReSharper disable once AssignmentInConditionalExpression
            while (collider = t.GetComponent<Collider>()) { // Assignment
                Object.DestroyImmediate(collider);
            }
        }
    }
}