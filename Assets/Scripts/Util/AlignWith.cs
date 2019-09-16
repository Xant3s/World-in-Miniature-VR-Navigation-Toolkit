using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWith : MonoBehaviour {

    public Transform Target;

    void Update() {
        if(!Target) return;
        transform.position = Target.position;
        //transform.LookAt((Target.position + Target.forward) - Target.position);
        transform.rotation = Quaternion.LookRotation(Target.forward, Target.up);
    }
}