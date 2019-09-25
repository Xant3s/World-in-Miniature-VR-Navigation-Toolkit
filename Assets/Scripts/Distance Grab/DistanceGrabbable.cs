﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGrabbable : MonoBehaviour {
    public Transform Target { get; set; }

    public bool IsBeingGrabbed { get; set; }

    public bool HightlightFX {
        get => hightlightFX;
        set {
            hightlightFX = value;
            if (GetComponent<Renderer>()) {
                GetComponent<Renderer>().material =
                    hightlightFX ? Resources.Load<Material>("Materials/outline") : defaultMaterial;
            }
            else {
                // WIM
                //for(var i = 0; i<transform.GetChild(0).childCount; i++) {
                //    var child  = transform.GetChild(0).GetChild(i);
                //    var renderer = child.GetComponent<Renderer>();
                //    if (!renderer) continue;
                //    renderer.material = hightlightFX ? Resources.Load<Material>("Materials/outline") : defaultMaterial;
                //}
            }
        }
    }

    private bool hightlightFX;

    public float SnapSpeed { get; set; } = 1f;
    public float MinDistance { get; set; } = .1f;

    private Material defaultMaterial;

    void Awake() {
        defaultMaterial = GetComponentInChildren<Renderer>().material;
    }

    void Update() {
        HightlightFX = false;
        if (!IsBeingGrabbed || !Target) return;

        if (Vector3.Distance(Target.position, transform.position) < MinDistance) {
            IsBeingGrabbed = false;
            Target = null;
            var rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else {
            //if (Physics.Raycast(transform.position, Target.position - transform.position, out var hit,
            //    Mathf.Infinity)) {
            //    if (hit.transform.tag != "Hands") {
            //        IsBeingGrabbed = false;
            //        Target = null;
            //        return;
            //    }
            //}

            var step = SnapSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, Target.position, step);
        }
    }
}