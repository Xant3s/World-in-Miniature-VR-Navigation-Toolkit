// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIM_Plugin {
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class AlignWith : MonoBehaviour {
        public Transform Target;

        private void Update() {
            if(!Target) return;
            transform.position = Target.position;
            transform.rotation = Quaternion.LookRotation(Target.forward, Target.up);
        }
    }
}