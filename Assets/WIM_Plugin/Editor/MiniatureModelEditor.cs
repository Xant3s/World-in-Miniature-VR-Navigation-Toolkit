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
    private GUIStyle headerStyle;

    public override void OnInspectorGUI() {
        WIM = (MiniatureModel) target;
        headerStyle = new GUIStyle(GUI.skin.label) {
            //alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        GUILayout.Label("World-in-Miniature (WIM)");
        if (GUILayout.Button("Generate WIM")) WIM.Generator.GenerateNewWIM(WIM);
        EditorGUI.BeginChangeCheck();
        WIM.AutoGenerateWIM = EditorGUILayout.Toggle("Auto Generate WIM", WIM.AutoGenerateWIM);
        if (EditorGUI.EndChangeCheck()) WIM.Generator.UpdateAutoGenerateWIM(WIM);

        separator("Basic Settings");
        WIM.Configuration.PlayerRepresentation = (GameObject) EditorGUILayout.ObjectField("Player Representation",
            WIM.Configuration.PlayerRepresentation, typeof(GameObject), false);
        WIM.Configuration.DestinationIndicator = (GameObject) EditorGUILayout.ObjectField("Destination Indicator",
            WIM.Configuration.DestinationIndicator, typeof(GameObject), false);
        WIM.Configuration.ScaleFactor = EditorGUILayout.Slider("Scale Factor", WIM.Configuration.ScaleFactor, 0, 1);
        WIM.Configuration.WIMLevelOffset =
            EditorGUILayout.Vector3Field("WIM Level Offset", WIM.Configuration.WIMLevelOffset);
        WIM.Configuration.ExpandCollidersX =
            EditorGUILayout.Vector2Field("Expand Colliders X", WIM.Configuration.ExpandCollidersX);


        WIM.Configuration.ExpandCollidersX =
            NamedVectorField("Expand Colliders X", WIM.Configuration.ExpandCollidersX, "Left", "Right");


        GUILayout.Label("----------------------------");

        DrawDefaultInspector();
        if (Application.isPlaying) return;
        if (WIM.Generator.ScrollingPropertyChanged(WIM) || WIM.Generator.OcclusionHandlingStrategyChanged(WIM))
            WIM.ConfigureWIM();
        WIM.Generator.UpdateCylinderMask(WIM);
        WIM.Generator.UpdateCutoutViewMask(WIM);
        WIM.Generator.UpdateScrollingMask(WIM);
        WIM.Generator.UpdateTransparency(WIM);
    }

    private Vector2 NamedVectorField(string text, Vector2 vector, string xAxisName = "X", string yAxisName = "Y") {
        var namedFloatFieldStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleRight};
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(text);
        var tmpVec = vector;
        GUILayout.Label(xAxisName, namedFloatFieldStyle);
        tmpVec.x = EditorGUILayout.FloatField(tmpVec.x);
        GUILayout.Label(yAxisName, namedFloatFieldStyle);
        tmpVec.y = EditorGUILayout.FloatField(tmpVec.y);
        EditorGUILayout.EndHorizontal();
        return tmpVec;
    }

    private Vector3 NamedVectorField(string text, Vector3 vector, string xAxisName = "X", string yAxisName = "Y", string zAxisName = "Z") {
        var namedFloatFieldStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleRight};
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(text);
        var tmpVec = vector;
        GUILayout.Label(xAxisName, namedFloatFieldStyle);
        tmpVec.x = EditorGUILayout.FloatField(tmpVec.x);
        GUILayout.Label(yAxisName, namedFloatFieldStyle);
        tmpVec.y = EditorGUILayout.FloatField(tmpVec.y);
        GUILayout.Label(zAxisName, namedFloatFieldStyle);
        tmpVec.z = EditorGUILayout.FloatField(tmpVec.z);
        EditorGUILayout.EndHorizontal();
        return tmpVec;
    }

    private void separator(string text = "", ushort space = 10) {
        GUILayout.Space(space);
        GUILayout.Label(text, headerStyle);
    }
}