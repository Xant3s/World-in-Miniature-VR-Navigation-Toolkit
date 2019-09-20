using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithRight : MonoBehaviour {

    public Transform Target;

    void Update() {
        if(!Target) return;
        transform.position = Target.position;
        transform.rotation = Quaternion.LookRotation(Target.up);
    }
}