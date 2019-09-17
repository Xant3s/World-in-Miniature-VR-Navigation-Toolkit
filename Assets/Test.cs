using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Oculus.Platform;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour {

    public Vector3 expand = Vector3.zero;

    [ButtonMethod]
    private void generateColliders() {
        // Generate colliders:
        // 1. Copy colliders from actual level (to determine which objects should have a collider) [alternatively don't delete them while generating the WIM]
        // 2. replace every collider with box collider (recursive, possibly multiple colliders per obj)
        for(var i = 0; i < transform.childCount; i++) {
            var child = transform.GetChild(i);
            var collider = child.GetComponent<Collider>();
            if(!collider)continue;
            removeAllColliders(child);
            var boxCollider = child.gameObject.AddComponent<BoxCollider>();
            // 3. move collider to WIM root (consider scale and position)
            var boxColliderRoot = gameObject.AddComponent<BoxCollider>();
            boxColliderRoot.center = child.localPosition;
            //boxColliderRoot.size = new Vector3(boxCollider.size.x * child.localScale.x,
            //    boxCollider.size.y * child.localScale.y,
            //boxCollider.size.z* child.localScale.z);
        }
        // 4. remove every collider that is fully inside another one.
        pruneColliders();
        // 5. extend collider (esp. upwards)
        expandColliders();
    }

    [ButtonMethod]
    private void pruneColliders() {
        var destoryList = new List<Collider>();
        var colliders = gameObject.GetComponents<Collider>();
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

    [ButtonMethod]
    private void expandColliders() {
        foreach(var boxCollider in gameObject.GetComponents<BoxCollider>()) {
            var bounds = boxCollider.bounds;
            bounds.Expand(expand);
            //boxCollider.size = new Vector3(bounds./2.0f,bounds/2.0f,bounds/2.0f);
            boxCollider.size = bounds.size;
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

    //[ButtonMethod]
    //private void test() {
    //    var child = transform.GetChild(0);
    //    var childBound = child.GetComponent<BoxCollider>().bounds;
    //    var boxCollider = gameObject.AddComponent<BoxCollider>();
    //    boxCollider.size = new Vector3((childBound.size.x/child.localScale.x),(childBound.size.y/child.localScale.y),(childBound.size.z/child.localScale.z));
    //    boxCollider.center = child.localPosition;
    //}

    //[ButtonMethod]
    //private void test2() {
    //    var bounds = CalculateRendererBounds(transform.GetChild(0).gameObject);
    //    var boxCol = gameObject.AddComponent<BoxCollider>();
    //    boxCol.center = bounds.center;
    //    boxCol.size = bounds.size;
    //}

    //Calculates the renderer bounds of the gameobject with its children
    //public static Bounds CalculateRendererBounds(GameObject obj) {
    //    Quaternion prevRot = obj.transform.localRotation;
     
    //    obj.transform.localRotation = Quaternion.identity;
     
    //    Bounds bounds = new Bounds(obj.transform.position, Vector3.one);
     
    //    Renderer[] children = obj.GetComponentsInChildren<Renderer>();
    //    foreach(Renderer child in children)
    //        if (child != null)
    //            bounds.Encapsulate(child.bounds);
     
    //    obj.transform.localRotation = prevRot;
     
    //    return bounds;
    //}

    private void removeAllColliders(Transform t) {
        while(t.GetComponent<Collider>()) {
            DestroyImmediate(t.GetComponent<Collider>());
        }
    }
}