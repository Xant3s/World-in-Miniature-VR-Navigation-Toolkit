// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.Assertions;


namespace WIMVR {
    /// <summary>
    /// Set position to float above target using offset.
    /// </summary>
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