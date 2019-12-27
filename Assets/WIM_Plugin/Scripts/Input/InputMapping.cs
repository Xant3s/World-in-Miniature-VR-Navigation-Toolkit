using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace WIM_Plugin {
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