﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MiniatureModel))]
public class MiniatureModelEditor : Editor {
    public override void OnInspectorGUI() {
        var myTarget = (MiniatureModel)target;
        if (GUILayout.Button("Generate WIM")) {
            generateWIM();
        }
        DrawDefaultInspector();
        updateWIMTransparency(myTarget);
        if (myTarget.transparentWIM)
            myTarget.transparency = EditorGUILayout.Slider("Transparency", myTarget.transparency, 0, 1);
    }

    private static void updateWIMTransparency(MiniatureModel myTarget) {
        if (myTarget.transparentWIM.Equals(myTarget.transparentWIMprev)) return;
        myTarget.transparentWIMprev = myTarget.transparentWIM;
        var material = (Material) Resources.Load("Materials/Dissolve");
        material.shader = Shader.Find(myTarget.transparentWIM? "Shader Graphs/DissolveTransparent" : "Shader Graphs/Dissolve");
    }

    private void generateWIM() {
        var myTarget = (MiniatureModel)target;
        var levelTransform = GameObject.Find("Level").transform;
        if(myTarget.transform.childCount > 0) DestroyImmediate(myTarget.transform.GetChild(0).gameObject);
        var WIMLevel = Instantiate(levelTransform, myTarget.transform);
        WIMLevel.localPosition = Vector3.zero;
        WIMLevel.name = "WIM Level";
        for (var i = 0; i < WIMLevel.childCount; ++i) {
            var child = WIMLevel.GetChild(i);
            while(child.GetComponent(typeof(Collider)))
                DestroyImmediate(child.GetComponent(typeof(Collider)));
            DestroyImmediate(child.GetComponent(typeof(Rigidbody)));
            DestroyImmediate(child.GetComponent(typeof(OVRGrabbable)));
            child.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Dissolve");
            child.gameObject.AddComponent<Dissolve>();
            child.gameObject.isStatic = false;
        }
        myTarget.transform.localScale = new Vector3(myTarget.scaleFactor, myTarget.scaleFactor, myTarget.scaleFactor);
    }


}