using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGrabber : MonoBehaviour {
    [SerializeField] private OVRInput.RawButton grabButton;
    [SerializeField] private Transform start;
    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private float minDistance = .5f;
    [SerializeField] private bool disableWhileInWIM = true;

    private AimAssist aimAssist;
    private LineRenderer lineRenderer;
    private bool isDisabled;

    void Awake() {
        if (!disableWhileInWIM) return;
        if (!this.enabled) {
            isDisabled = true;
            return;
        };
        aimAssist = gameObject.GetComponentInChildren<AimAssist>();
        lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
    }


    void LateUpdate() {
        var WIMLayerOnly = 1 << 8;
        if (Physics.Raycast(transform.position, start.forward, out var hit,
            Mathf.Infinity, WIMLayerOnly)) {
            var grabbable = hit.transform.gameObject.GetComponent<DistanceGrabbable>();
            if (!grabbable) return;
            grabbable.HightlightFX = true;
            if (OVRInput.GetDown(grabButton) || Input.GetKeyDown(KeyCode.Y)) {
                grabbable.MinDistance = minDistance;
                grabbable.SnapSpeed = snapSpeed;
                grabbable.Target = transform;
                grabbable.IsBeingGrabbed = true;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!disableWhileInWIM || isDisabled || other.name != "WIM") return;
        setEnable(false);
    }

    void OnTriggerExit(Collider other) {
        if (!disableWhileInWIM || isDisabled || other.name != "WIM") return;
        setEnable(true);
    }

    void setEnable(bool value) {
        aimAssist.enabled = value;
        lineRenderer.enabled = value;
        this.enabled = value;
    }
}