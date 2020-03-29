// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEngine;


[ExecuteAlways]
public class BoxController : MonoBehaviour {
    private static readonly int boxPosition = Shader.PropertyToID("_BoxPosition");
    private static readonly int boxScale = Shader.PropertyToID("_BoxScale");
    private static readonly int boxRotationMatrix_v1 = Shader.PropertyToID("_BoxRotationMatrix_v1");
    private static readonly int boxRotationMatrix_v2 = Shader.PropertyToID("_BoxRotationMatrix_v2");
    private static readonly int boxRotationMatrix_v3 = Shader.PropertyToID("_BoxRotationMatrix_v3");
    private static readonly int boxRotationMatrix_v4 = Shader.PropertyToID("_BoxRotationMatrix_v4");
    private static readonly int boxEnabled = Shader.PropertyToID("_BoxEnabled");
    public Material[] materials;

    public void SetBoxEnabled(bool value) {
        try {
            foreach (var material in materials) {
                material.SetFloat(boxEnabled, value ? 1 : 0);
            }
        }
        catch (Exception) {
            // ignored
        }
    }


    private void OnEnable() {
        SetBoxEnabled(true);
    }

    private void OnDisable() {
        SetBoxEnabled(false);
    }

    private void Update() {
        if (materials == null || materials.Length == 0) return;
        foreach (var material in materials) {
            material.SetVector(boxPosition, transform.position);
            material.SetVector(boxScale, transform.localScale);
            var mat = Matrix4x4.Rotate(transform.rotation);
            material.SetVector(boxRotationMatrix_v1, mat.GetRow(0));
            material.SetVector(boxRotationMatrix_v2, mat.GetRow(1));
            material.SetVector(boxRotationMatrix_v3, mat.GetRow(2));
            material.SetVector(boxRotationMatrix_v4, mat.GetRow(3));
        }
    }
}