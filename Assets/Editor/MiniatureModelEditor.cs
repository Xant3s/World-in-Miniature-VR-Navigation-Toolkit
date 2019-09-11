using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MiniatureModel))]
public class MiniatureModelEditor : Editor {
    public override void OnInspectorGUI() {
        var myTarget = (MiniatureModel)target;
        GUILayout.Label("Test");
        if (GUILayout.Button("Generate WIM")) {
            generateWIM();
        }
        DrawDefaultInspector();
        updateWIMTransparency(myTarget);
        if (myTarget.transparentWIM)
            myTarget.transparency = EditorGUILayout.Slider("Transparency", myTarget.transparency, 0, 1);
        updatePreviewScreen(myTarget);
    }

    private void updatePreviewScreen(MiniatureModel myTarget) {
        if (myTarget.showPreviewScreen.Equals(myTarget.showPreviewScreenPrev)) return;
        myTarget.showPreviewScreenPrev = myTarget.showPreviewScreen;
        destroyAllObjectsWithTag("PreviewScreen");
        if (myTarget.showPreviewScreen) {
            var previewScreen = Instantiate(Resources.Load<GameObject>("Prefabs/Preview Screen"));
            previewScreen.GetComponent<FloatAbove>().Target = myTarget.transform;
            if (myTarget.destinationIndicatorInWIM) {
                var camObj = myTarget.destinationIndicatorInWIM.GetChild(1).gameObject; // Making assumptions on the prefab.
                var cam = camObj.gameObject.AddComponent<Camera>();
                cam.targetTexture = Resources.Load<RenderTexture>("Prefabs/Textures/PreviewTexture");
            }
        }
    }

    private static void destroyAllObjectsWithTag(string tag) {
        var list = GameObject.FindGameObjectsWithTag(tag);
        foreach (var obj in list) {
            DestroyImmediate(obj);
        }
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