// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using UnityEngine;

namespace WIMVR.Input {
    /// <summary>
    /// Stores platform dependent input mapping. Serializable.
    /// For each supported platform, a platform specific input mapper has to be provided.
    /// Will be eventually replaced by the new Unity input manager.
    /// </summary>
    [CreateAssetMenu(menuName = "WIM/Input Mapping")]
    public class InputMapping : ScriptableObject {
        public List<string> Keys = new List<string>();
        public List<int> Values = new List<int>();

        public bool HasKey(string key) {
            return Keys.Contains(key);
        }

        public void Add(string key, int value) {
            Keys.Add(key);
            Values.Add(value);
        }

        public void Remove(string key) {
            var index = Keys.FindIndex(k => k.Equals(key));
            Debug.Assert(index != -1);
            Values.RemoveAt(index);
            Keys.RemoveAt(index);
        }

        public void Clear() {
            Keys.Clear();
            Values.Clear();
        }

        public void Set(string key, int value) {
            if (!HasKey(key)) {
                Add(key, value);
                return;
            }

            var index = Keys.FindIndex(k => k.Equals(key));
            Debug.Assert(index != -1);
            Values[index] = value;
        }

        public int Get(string key) {
            var index = Keys.FindIndex(k => k.Equals(key));
            Debug.Assert(index != -1);
            return Values[index];
        }
    }
}