﻿using System;
using UnityEngine;


[ExecuteAlways]
[RequireComponent(typeof(Light))]
public class ConeController : MonoBehaviour {
    public Material[] materials;
    private static readonly int coneTip = Shader.PropertyToID("_ConeTip");
    private static readonly int coneDir = Shader.PropertyToID("_ConeDir");
    private static readonly int coneHeight = Shader.PropertyToID("_ConeHeight");
    private static readonly int coneBaseRadius = Shader.PropertyToID("_ConeBaseRadius");
    private static readonly int coneEnabled = Shader.PropertyToID("_ConeEnabled");
    private Light spotLight;


    private void Awake() {
        spotLight = GetComponent<Light>();
        if (spotLight.type != LightType.Spot) {
            Debug.LogError("Light is required to be of type 'Spot'.");
        }
    }

    private void OnEnable() {
        SetConeEnabled(true);
    }

    private void OnDisable() {
        SetConeEnabled(false);
    }

    public void SetConeEnabled(bool value) {
        try {
            foreach (var material in materials) {
                material.SetFloat(coneEnabled, value ? 1 : 0);
            }
        }
        catch (Exception) {
            // ignored
        }
    }

    private void Update() {
        if (materials == null || materials.Length == 0) return;
        foreach (var material in materials) {
            material.SetVector(coneTip, transform.position);
            material.SetVector(coneDir, transform.forward);
            material.SetFloat(coneHeight, spotLight.range);
            material.SetFloat(coneBaseRadius, spotLight.spotAngle);
        }
    }
}