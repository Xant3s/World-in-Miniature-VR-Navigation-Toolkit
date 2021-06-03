// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Util.Extensions {
    public static class GameObjectExtensions {
        /// <summary>
        /// Returns a component of type T if it exists, otherwise creates component of type T
        /// and returns that.
        /// </summary>
        /// <typeparam name="T">A component type.</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component {
            var t = obj.GetComponent(typeof(T));
            if(t) return t as T;
            return obj.AddComponent<T>();
        }

        public static bool IsChildOf(this GameObject child, GameObject parent) {
            if(child.Equals(parent)) return true;
            for(var i = 0; i < parent.transform.childCount; i++) {
                if(child.IsChildOf(parent.transform.GetChild(i).gameObject)) return true;
            }
            return false;
        }
    }
}