using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AimAssist : MonoBehaviour {

    [SerializeField] private OVRInput.RawButton grabButton;
    [SerializeField] private float length = 1.0f;

    void Update() {
        var lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + transform.forward * length);
        lr.enabled = !OVRInput.Get(grabButton);
    }
}