using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGrabber : MonoBehaviour {
    [SerializeField] private OVRInput.RawButton grabButton;
    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private float minDistance = .5f;


    void LateUpdate() {
        var WIMLayerOnly = 1 << 8;
        if (Physics.Raycast(transform.position, transform.GetChild(0).forward, out var hit,
            Mathf.Infinity, WIMLayerOnly)) {
            var grabbable = hit.transform.gameObject.GetComponent<DistanceGrabbable>();
            if (!grabbable) return;
            grabbable.HightlightFX = true;
            if (OVRInput.GetDown(grabButton) || Input.GetKeyDown(KeyCode.Y)) {
                grabbable.MinDistance = minDistance;
                grabbable.SnapSpeed = snapSpeed;
                grabbable.Target = transform;
                grabbable.IsBeingGrabbed = true;
            };
        }
    }
}