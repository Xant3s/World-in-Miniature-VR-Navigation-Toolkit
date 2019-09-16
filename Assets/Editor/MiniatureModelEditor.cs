using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedDissolve_Example;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MiniatureModel))]
public class MiniatureModelEditor : Editor {

    private MiniatureModel WIM;
    //private List<GameObject> occlusionHandlingObjects = new List<GameObject>();

    public override void OnInspectorGUI() {
        WIM = (MiniatureModel)target;
        if (GUILayout.Button("Generate WIM")) {
            generateWIM();
        }
        DrawDefaultInspector();
        updateOcclusionHandling();
        updateMeltRadius();
    }

    private void updateOcclusionHandling() {
        if(WIM.occlusionHandling == WIM.prevOcclusionHandling) return;
        WIM.prevOcclusionHandling = WIM.occlusionHandling;
        Material material;
        cleanupOcclusionHandling();
        switch(WIM.occlusionHandling) {
            case MiniatureModel.OcclusionHandling.Transparency:
                material = Resources.Load<Material>("Materials/Dissolve");
                material.shader = Shader.Find("Shader Graphs/DissolveTransparent");
                addDissolveScript();
                break;
            case MiniatureModel.OcclusionHandling.MeltWalls:
                //if(GameObject.Find("Mask Controller")) return;
                material = Resources.Load<Material>("Materials/MeltWalls");
                var maskController = new GameObject("Mask Controller");
                //occlusionHandlingObjects.Add(maskController);
                var controller = maskController.AddComponent<Controller_Mask_Sphere>();
                controller.materials = new[] {material};
                var sphereMask = new GameObject("Sphere Mask");
                //occlusionHandlingObjects.Add(sphereMask);
                controller.sphere1 = sphereMask;
                var moveController = sphereMask.AddComponent<ObjectSceneMove>();
                moveController.scale = true;
                moveController.layerMask = ~0; // Everything.
                sphereMask.AddComponent<FollowHand>().hand = MiniatureModel.Hand.HAND_R;
                moveController.transform.localPosition = Vector3.zero;
                moveController.transform.localScale = new Vector3(WIM.meltRadius, WIM.meltRadius, WIM.meltRadius);
                removeDissolveScript();
                // TODO: Both hands
                break;
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
        //while(occlusionHandlingObjects.Count != 0) {
        //    DestroyImmediate(occlusionHandlingObjects[0]);
        //    occlusionHandlingObjects.RemoveAt(0);
        //}
        DestroyImmediate(GameObject.Find("Sphere Mask"));
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

    private void updateMeltRadius() {
        if(WIM.occlusionHandling != MiniatureModel.OcclusionHandling.MeltWalls) return;
        if(Math.Abs(WIM.meltRadius - WIM.PrevMeltRadius) < .001f) return;
        WIM.PrevMeltRadius = WIM.meltRadius;
        GameObject.Find("Sphere Mask").transform.localScale = new Vector3(WIM.meltRadius,WIM.meltRadius,WIM.meltRadius);
        // TODO: both hands
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