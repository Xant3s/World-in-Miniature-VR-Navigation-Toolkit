using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    [DisallowMultipleComponent]
    public class FloatAbove : MonoBehaviour {
        public Transform Target;
        [SerializeField] private Vector3 offset;

        private void Start() {
            Assert.IsNotNull(Target);
        }

        private void Update() {
            transform.position = Target.position + offset;
        }
    }
}