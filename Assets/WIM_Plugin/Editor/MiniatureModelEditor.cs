using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedDissolve_Example;
using UnityEditor;
using UnityEngine;
using System.Linq;
using WIM_Plugin;

// The custom inspector. Displays only relevant settings. Should not contain logic.
[CustomEditor(typeof(MiniatureModel))]
public class MiniatureModelEditor : Editor {

    private MiniatureModel WIM;

    public override void OnInspectorGUI() {
        GUILayout.Label("World-in-Miniature (WIM)");
        WIM = (MiniatureModel)target;
        if (GUILayout.Button("Generate WIM")) {
            WIM.Generator.GenerateNewWIM(WIM);
        }
        EditorGUI.BeginChangeCheck();
        WIM.AutoGenerateWIM = EditorGUILayout.Toggle("Auto Generate WIM", WIM.AutoGenerateWIM);
        if(EditorGUI.EndChangeCheck()) WIM.Generator.UpdateAutoGenerateWIM(WIM);

        WIM.Configuration.TestBool = EditorGUILayout.Toggle("Test", WIM.Configuration.TestBool);

        DrawDefaultInspector();
        if(Application.isPlaying) return;
        if (WIM.Generator.ScrollingPropertyChanged(WIM) || WIM.Generator.OcclusionHandlingStrategyChanged(WIM)) WIM.ConfigureWIM();
        WIM.Generator.UpdateCylinderMask(WIM);
        WIM.Generator.UpdateCutoutViewMask(WIM);
        WIM.Generator.UpdateScrollingMask(WIM);
        WIM.Generator.UpdateTransparency(WIM);
    }
}