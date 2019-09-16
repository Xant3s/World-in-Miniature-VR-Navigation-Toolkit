using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedDissolve_Example;
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
        updateOcclusionHandling();
        updateCylinderMask();
        updateCutoutViewMask();
        updateScrollingMask();
        updateScrolling();
    }

    void updateScrolling() {
        if(WIM.AllowWIMScrolling == WIM.PrevAllowWIMScrolling) return;
        WIM.PrevAllowWIMScrolling = WIM.AllowWIMScrolling;
        var material = WIM.AllowWIMScrolling
            ? Resources.Load<Material>("Materials/MeltWalls")
            : Resources.Load<Material>("Materials/Dissolve");
        setWIMMaterial(material);
        if(WIM.AllowWIMScrolling) {
            var maskController = new GameObject("Mask Controller");
            var controller = maskController.AddComponent<Controller_Mask_Box>();
            controller.materials = new[] {material};
            var boxMask = new GameObject("Box Mask");
            var tmpGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var mf = tmpGO.GetComponent<MeshFilter>();
            var cubeMesh = Instantiate (mf.sharedMesh) as Mesh;
            boxMask.AddComponent<MeshFilter>().sharedMesh = cubeMesh;
            boxMask.AddComponent<AlignWith>().Target = WIM.transform;
            //boxMask.transform.parent = WIM.transform;
            //boxMask.transform.localPosition = Vector3.zero;
            //boxMask.transform.localScale = WIM.activeAreaBounds;
            controller.box1 = boxMask;
            //controller.invert = true;
            removeDissolveScript();
            DestroyImmediate(tmpGO);

            //boxMask.AddComponent<MeshRenderer>();
        }
        else {
            DestroyImmediate(GameObject.Find("Box Mask"));   
            DestroyImmediate(GameObject.Find("Mask Controller"));
            addDissolveScript();
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
    }

    private void setWIMMaterial(Material material) {
        var WIMLevelTransform = WIM.transform.GetChild(0);
        for(var i = 0; i < WIMLevelTransform.childCount; i++) {
            var child = WIMLevelTransform.GetChild(i);
            var renderer = child.GetComponent<Renderer>();
            if(!renderer) continue;
            renderer.material = material;
        }
    }

    private void removeDissolveScript() {
        var WIMLevelTransform = WIM.transform.GetChild(0);
        for(var i = 0; i < WIMLevelTransform.childCount; i++) {
            var child = WIMLevelTransform.GetChild(i);
            DestroyImmediate(child.GetComponent<Dissolve>());
        }
    }

    private void addDissolveScript() {
        var WIMLevelTransform = WIM.transform.GetChild(0);
        for(var i = 0; i < WIMLevelTransform.childCount; i++) {
            var child = WIMLevelTransform.GetChild(i);
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