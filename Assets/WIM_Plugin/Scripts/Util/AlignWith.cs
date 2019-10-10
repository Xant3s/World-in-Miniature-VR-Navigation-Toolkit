using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    public class AlignWith : MonoBehaviour {
        public Transform Target;

        void Update() {
            if(!Target) return;
            transform.position = Target.position;
            transform.rotation = Quaternion.LookRotation(Target.forward, Target.up);
        }
    }
}