﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MiniatureModel))]
public class MiniatureModelEditor : Editor {

    private MiniatureModel WIM;

    public override void OnInspectorGUI() {
        WIM = (MiniatureModel)target;
        if (GUILayout.Button("Generate WIM")) {
            generateWIM();
        }
        DrawDefaultInspector();
        updateWIMTransparency();
    }

    private void updateWIMTransparency() {
        if (WIM.transparentWIM.Equals(WIM.TransparentWIMprev)) return;
        WIM.TransparentWIMprev = WIM.transparentWIM;
        var material = (Material) Resources.Load("Materials/Dissolve");
        material.shader = Shader.Find(WIM.transparentWIM? "Shader Graphs/DissolveTransparent" : "Shader Graphs/Dissolve");
    }

    private void generateWIM() {
        adaptScaleFactorToPlayerHeight();
        var levelTransform = GameObject.Find("Level").transform;
        if (WIM.transform.childCount > 0) DestroyImmediate(WIM.transform.GetChild(0).gameObject);
        var WIMLevel = Instantiate(levelTransform, WIM.transform);
        WIMLevel.localPosition = Vector3.zero;
        WIMLevel.name = "WIM Level";
        for (var i = 0; i < WIMLevel.childCount; ++i)
        {
            var child = WIMLevel.GetChild(i);
            while (child.GetComponent(typeof(Collider)))
                DestroyImmediate(child.GetComponent(typeof(Collider)));
            DestroyImmediate(child.GetComponent(typeof(Rigidbody)));
            DestroyImmediate(child.GetComponent(typeof(OVRGrabbable)));
            child.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Dissolve");
            child.gameObject.AddComponent<Dissolve>();
            child.gameObject.isStatic = false;
        }
        WIM.transform.localScale = new Vector3(WIM.ScaleFactor, WIM.ScaleFactor, WIM.ScaleFactor);
    }

    private void adaptScaleFactorToPlayerHeight() {
        if (!WIM.adaptWIMSizeToPlayerHeight) return;
        var playerHeight = WIM.playerHeightInCM;
        const float defaultHeight = 170;
        var defaultScaleFactor = WIM.ScaleFactor;
        const float minHeight = 100;
        const float maxHeight = 200;
        playerHeight = Mathf.Clamp(playerHeight, minHeight, maxHeight);
        var maxScaleFactorDelta = WIM.MaxWIMScaleFactorDelta;
        var heightDelta = playerHeight - defaultHeight;
        if (heightDelta > 0) {
            const float maxDelta = maxHeight - defaultHeight;
            var actualDelta = maxHeight - playerHeight;
            var factor = actualDelta / maxDelta;
            var resultingScaleFactorDelta = maxScaleFactorDelta * factor;
            WIM.ScaleFactor = defaultScaleFactor + resultingScaleFactorDelta;
        } else if (heightDelta < 0) {
            const float maxDelta = defaultHeight - minHeight;
            var actualDelta = defaultHeight - playerHeight;
            var factor = actualDelta / maxDelta;
            var resultingScaleFactorDelta = maxScaleFactorDelta * (-factor);
            WIM.ScaleFactor = defaultScaleFactor + resultingScaleFactorDelta;
        }
    }
}