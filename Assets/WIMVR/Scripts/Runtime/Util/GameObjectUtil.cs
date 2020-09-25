// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

public static class GameObjectUtil {
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
}