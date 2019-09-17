﻿using System;
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
            //WIM.GetComponents<Collider>().ToList().ForEach(col => col.enabled = false);
            removeAllColliders();
            WIM.gameObject.AddComponent<BoxCollider>().size = WIM.activeAreaBounds / WIM.ScaleFactor;

            removeDissolveScript();
            DestroyImmediate(tmpGO);
            maskController.transform.position = WIM.transform.position;
        }
        else {
            //DestroyImmediate(WIM.GetComponents<Collider>().Last());
            //WIM.GetComponents<Collider>().ToList().ForEach(col => col.enabled = true);
            removeAllColliders();
            // Todo: generate default colliders
            DestroyImmediate(GameObject.Find("Box Mask"));   
            addDissolveScript();
        }
    }

    private void removeAllColliders() {
        while(WIM.GetComponent<Collider>()) {
            DestroyImmediate(WIM.GetComponent<Collider>());
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
        removeAllColliders(WIM.transform);
        adaptScaleFactorToPlayerHeight();
        var levelTransform = GameObject.Find("Level").transform;
        if (WIM.transform.childCount > 0) DestroyImmediate(WIM.transform.GetChild(0).gameObject);
        var WIMLevel = Instantiate(levelTransform, WIM.transform);
        WIMLevel.localPosition = Vector3.zero;
        WIMLevel.name = "WIM Level";
        for(var i = 0; i < WIMLevel.childCount; ++i) {
            var child = WIMLevel.GetChild(i);
            //while(child.GetComponent(typeof(Collider)))
            //    DestroyImmediate(child.GetComponent(typeof(Collider)));
            DestroyImmediate(child.GetComponent(typeof(Rigidbody)));
            DestroyImmediate(child.GetComponent(typeof(OVRGrabbable)));
            child.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Dissolve");
            child.gameObject.AddComponent<Dissolve>();
            child.gameObject.isStatic = false;
        }
        WIM.transform.localScale = new Vector3(WIM.ScaleFactor, WIM.ScaleFactor, WIM.ScaleFactor);
        generateColliders();
    }

    private void generateColliders() {
        // Generate colliders: // TODO: what about child of childs: WIM-WIMLevel-objects-childs-...
        // 1. Copy colliders from actual level (to determine which objects should have a collider)
        // [alternatively don't delete them while generating the WIM]
        // 2. replace every collider with box collider (recursive, possibly multiple colliders per obj)
        var wimLevelTransform = WIM.transform.GetChild(0);
        for(var i = 0; i < wimLevelTransform.childCount; i++) {
            var child = wimLevelTransform.GetChild(i);
            var collider = child.GetComponent<Collider>();
            if(!collider) continue;
            removeAllColliders(child);
            var childBoxCollider = child.gameObject.AddComponent<BoxCollider>();
            // 3. move collider to WIM root (consider scale and position)
            var rootCollider = WIM.gameObject.AddComponent<BoxCollider>();
            rootCollider.center = child.localPosition;
            rootCollider.size = Vector3.zero;
            var bounds = rootCollider.bounds;
            bounds.Encapsulate(childBoxCollider.bounds);
            rootCollider.size = bounds.size / WIM.ScaleFactor;
            removeAllColliders(child);
        }
        // 4. remove every collider that is fully inside another one.
        pruneColliders();
        // 5. extend collider (esp. upwards)
        expandColliders();
    }

    private void pruneColliders() {
        var destoryList = new List<Collider>();
        var colliders = WIM.gameObject.GetComponents<Collider>();
        for(var i = 0; i < colliders.Length; i++) {
            var col = (BoxCollider)colliders[i];
            for(var j = 0; j < colliders.Length; j++) {
                if(i == j) continue;
                var other = colliders[j];
                var skip = false;
                for(var id = 0; id < 8; id++) {
                    if(other.bounds.Contains(getCorner(col, id))) continue;
                    // next collider
                    skip = true;
                    break;
                }
                if(skip) continue;
                destoryList.Add(col);
                break;
            }
        }
        while(destoryList.Count() != 0) {
            DestroyImmediate(destoryList[0]);
            destoryList.RemoveAt(0);
        }
    }

    private void expandColliders() {
        foreach(var boxCollider in WIM.gameObject.GetComponents<BoxCollider>()) {
            boxCollider.size += WIM.expandCollidersBy;
            //boxCollider.center += Vector3.up * WIM.expandCollidersBy.y / 2.0f;
        }
    }

    private Vector3 getCorner(BoxCollider box, int id) {
        var extends = box.bounds.extents;
        var center = box.bounds.center;
        switch(id) {
            case 0:
                // Top right front.
                return center + box.transform.up * extends.y + box.transform.right * extends.x + box.transform.forward * -extends.z;
            case 1:
                // top right back.
                return center + box.transform.up * extends.y + box.transform.right * extends.x + box.transform.forward * +extends.z;
            case 2:
                // top left back.
                return center + box.transform.up * extends.y + box.transform.right * -extends.x + box.transform.forward * +extends.z;
            case 3:
                // top left front.
                return center + box.transform.up * extends.y + box.transform.right * -extends.x + box.transform.forward * -extends.z;
            case 4:
                // bottom right front.
                return center + box.transform.up * -extends.y + box.transform.right * extends.x + box.transform.forward * -extends.z;
            case 5:
                // bottom right back.
                return center + box.transform.up * -extends.y + box.transform.right * extends.x + box.transform.forward * -extends.z;
            case 6:
                // bottom left back.
                return center + box.transform.up * -extends.y + box.transform.right * -extends.x + box.transform.forward * +extends.z;
            case 7:
                // bottom left front.
                return center + box.transform.up * -extends.y + box.transform.right * -extends.x + box.transform.forward * -extends.z;
            default:
                throw new Exception("Bad input.");
        }
    }

    private void removeAllColliders(Transform t) {
        while(t.GetComponent<Collider>()) {
            DestroyImmediate(t.GetComponent<Collider>());
        }
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