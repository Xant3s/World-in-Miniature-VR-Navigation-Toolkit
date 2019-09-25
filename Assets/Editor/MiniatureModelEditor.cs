using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedDissolve_Example;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(MiniatureModel))]
public class MiniatureModelEditor : Editor {

    private MiniatureModel WIM;

    public override void OnInspectorGUI() {
        GUILayout.Label("World-in-Miniature (WIM)");
        WIM = (MiniatureModel)target;
        if (GUILayout.Button("Generate WIM")) {
            WIM.generateNewWIM();
        }
        EditorGUI.BeginChangeCheck();
        WIM.AutoGenerateWIM = EditorGUILayout.Toggle("Auto Generate WIM", WIM.AutoGenerateWIM);
        if(EditorGUI.EndChangeCheck()) updateAutoGenerateWIM();
        DrawDefaultInspector();
        if(Application.isPlaying) return;
        if (scrollingPropertyChanged() || occlusionHandlingStrategyChanged()) WIM.ConfigureWIM();
        updateCylinderMask();
        updateCutoutViewMask();
        updateScrollingMask();
    }

    void updateAutoGenerateWIM() {
        var level = GameObject.Find("Level");
        if(!level) {
            Debug.LogWarning("Level not found.");
            return;
        }
        if(WIM.AutoGenerateWIM) {
            // Add script recursive
            level.AddComponent<AutoUpdateWIM>().WIM = WIM;
        }
        else {
            // Destroy script recursive
            DestroyImmediate(level.GetComponent<AutoUpdateWIM>());
        }
    }

    private bool scrollingPropertyChanged() {
        if(WIM.AllowWIMScrolling == WIM.PrevAllowWIMScrolling) return false;
        WIM.PrevAllowWIMScrolling = WIM.AllowWIMScrolling;
        return true;
    }

    private bool occlusionHandlingStrategyChanged() {
        if(WIM.occlusionHandling == WIM.prevOcclusionHandling) return false;
        WIM.prevOcclusionHandling = WIM.occlusionHandling;
        if(WIM.occlusionHandling != MiniatureModel.OcclusionHandling.None) {
            WIM.AllowWIMScrolling = WIM.PrevAllowWIMScrolling = false;
        }
        return true;
    }

    private void updateCylinderMask() {
        if (WIM.occlusionHandling != MiniatureModel.OcclusionHandling.MeltWalls) return;
        var cylinderTransform = GameObject.Find("Cylinder Mask").transform;
        if(!cylinderTransform) return;
        cylinderTransform.localScale = new Vector3(WIM.meltRadius, WIM.meltHeight, 1);
    }

    private void updateCutoutViewMask() {
        if (WIM.occlusionHandling != MiniatureModel.OcclusionHandling.CutoutView) return;
        var spotlightObj = GameObject.Find("Spotlight Mask");
        if(!spotlightObj) return;
        var spotlight = spotlightObj.GetComponent<Light>();
        spotlight.range = WIM.cutoutRange;
        spotlight.spotAngle = WIM.cutoutAngle;

        Color color;
        if(WIM.showCutoutLight) {
            color = WIM.cutoutLightColor;
            color.a = 1;
        } else {
            color  = new Color(0,0,0,0);
        }
        spotlight.color = color;
    }

    private void updateScrollingMask() {
        if(!WIM.AllowWIMScrolling) return;
        var boxMaskObj = GameObject.Find("Box Mask");
        if(!boxMaskObj) return;
        boxMaskObj.transform.localScale = WIM.activeAreaBounds;
    }
}