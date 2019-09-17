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
        updateOcclusionHandling();
        updateCylinderMask();
        updateCutoutViewMask();
        updateScrollingMask();
        updateScrolling();
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

    void updateScrolling() {
        if(WIM.AllowWIMScrolling == WIM.PrevAllowWIMScrolling) return;
        WIM.PrevAllowWIMScrolling = WIM.AllowWIMScrolling;
        var material = WIM.AllowWIMScrolling
            ? Resources.Load<Material>("Materials/ScrollDissolve")
            : Resources.Load<Material>("Materials/Dissolve");
        setWIMMaterial(material);
        if(WIM.AllowWIMScrolling) {
            var maskController = new GameObject("Box Mask");
            var controller = maskController.AddComponent<Controller_Mask_Box>();
            controller.materials = new[] {material};
            var tmpGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var mf = tmpGO.GetComponent<MeshFilter>();
            var cubeMesh = Instantiate (mf.sharedMesh) as Mesh;
            maskController.AddComponent<MeshFilter>().sharedMesh = cubeMesh;
            maskController.AddComponent<AlignWith>().Target = WIM.transform;
            controller.box1 = maskController;
            controller.invert = true;

            // Collider.
            WIM.removeAllColliders();
            WIM.gameObject.AddComponent<BoxCollider>().size = WIM.activeAreaBounds / WIM.ScaleFactor;

            removeDissolveScript();
            DestroyImmediate(tmpGO);
            maskController.transform.position = WIM.transform.position;
        }
        else {
            DestroyImmediate(GameObject.Find("Box Mask"));   
            WIM.generateNewWIM();
        }
    }

    private void updateOcclusionHandling() {
        if(WIM.occlusionHandling == WIM.prevOcclusionHandling) return;
        WIM.prevOcclusionHandling = WIM.occlusionHandling;
        if(WIM.occlusionHandling != MiniatureModel.OcclusionHandling.None) {
            WIM.AllowWIMScrolling = WIM.PrevAllowWIMScrolling = false;
        }
        Material material;
        cleanupOcclusionHandling();
        switch(WIM.occlusionHandling) {
            case MiniatureModel.OcclusionHandling.Transparency:
                material = Resources.Load<Material>("Materials/Dissolve");
                material.shader = Shader.Find("Shader Graphs/DissolveTransparent");
                addDissolveScript();
                break;
            case MiniatureModel.OcclusionHandling.MeltWalls: {
                material = Resources.Load<Material>("Materials/MeltWalls");
                var maskController = new GameObject("Mask Controller");
                var controller = maskController.AddComponent<Controller_Mask_Cylinder>();
                controller.materials = new[] {material};
                var cylinderMask = new GameObject("Cylinder Mask");
                controller.cylinder1 = cylinderMask;
                cylinderMask.AddComponent<FollowHand>().hand = MiniatureModel.Hand.HAND_R;
                removeDissolveScript();
                break;
            }
            case MiniatureModel.OcclusionHandling.CutoutView: {
                material = Resources.Load<Material>("Materials/MeltWalls");
                var maskController = new GameObject("Mask Controller");
                var controller = maskController.AddComponent<Controller_Mask_Cone>();
                controller.materials = new[] {material};
                var spotlightObj = new GameObject("Spotlight Mask");
                var spotlight = spotlightObj.AddComponent<Light>();
                controller.spotLight1 = spotlight;
                spotlight.type = LightType.Spot;
                spotlightObj.AddComponent<AlignWith>().Target = Camera.main.transform;
                removeDissolveScript();
                break;
            }
            case MiniatureModel.OcclusionHandling.None:
                material = Resources.Load<Material>("Materials/Dissolve");
                material.shader = Shader.Find("Shader Graphs/Dissolve");
                addDissolveScript();
                break;
            default:
                material = Resources.Load<Material>("Materials/Dissolve");
                material.shader = Shader.Find("Shader Graphs/Dissolve");
                addDissolveScript();
                break;
        }
        setWIMMaterial(material);
    }

    void cleanupOcclusionHandling() {
        DestroyImmediate(GameObject.Find("Cylinder Mask"));
        DestroyImmediate(GameObject.Find("Spotlight Mask"));
        DestroyImmediate(GameObject.Find("Mask Controller"));
        if(!WIM.AllowWIMScrolling)
            DestroyImmediate(GameObject.Find("Box Mask")); 
    }

    private void setWIMMaterial(Material material) {
        var WIMLevelTransform = WIM.transform.GetChild(0);
        foreach(Transform child in WIMLevelTransform) {
            var renderer = child.GetComponent<Renderer>();
            if(!renderer) continue;
            renderer.material = material;
        }
    }

    private void removeDissolveScript() {
        var WIMLevelTransform = WIM.transform.GetChild(0);
        foreach(Transform child in WIMLevelTransform) {
            DestroyImmediate(child.GetComponent<Dissolve>());
        }
    }

    private void addDissolveScript() {
        var WIMLevelTransform = WIM.transform.GetChild(0);
        foreach(Transform child in WIMLevelTransform) {
            if(!child.GetComponent<Dissolve>())
                child.gameObject.AddComponent<Dissolve>();
        }
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