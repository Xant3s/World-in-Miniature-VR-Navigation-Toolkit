using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

public class PickupPreviewScreen : MonoBehaviour {
    private bool pickupMode = false;
    private Transform thumb;
    private Transform index;
    private bool thumbIsGrabbing;
    private bool indexIsGrabbing;
    private bool isGrabbing;
    private bool stoppedGrabbing;


    private void Awake() {
        thumb = GameObject.FindWithTag("ThumbR").transform;
        index = GameObject.FindWithTag("IndexR").transform;
        Assert.IsNotNull(thumb);
        Assert.IsNotNull(index);
    }

    private void Update() {
        var rightHandPinch = thumbIsGrabbing && indexIsGrabbing;
        if(rightHandPinch && !isGrabbing) {
            isGrabbing = true;
            stoppedGrabbing = false;
            startGrabbing();
        }
        else if(isGrabbing && !rightHandPinch) {
            isGrabbing = false;
        }

        if(!isGrabbing && (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))) {
            if(stoppedGrabbing) return;
            stopGrabbing();
            stoppedGrabbing = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform == thumb) {
            thumbIsGrabbing = true;
        }
        else if(other.transform == index) {
            indexIsGrabbing = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.transform == thumb) {
            thumbIsGrabbing = false;
        }
        else if(other.transform == index) {
            indexIsGrabbing = false;
        }
    }

    private void startGrabbing() {
        var WIMTransform = GameObject.Find("WIM").transform;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        Assert.IsNotNull(WIM);

        // Spawn new preview screen.
        WIMTransform.GetComponent<PreviewScreen>().ShowPreviewScreenPickup(WIM.Configuration, WIM.Data);
        var previewScreenTransform = WIM.Data.PreviewScreenTransform;
        Assert.IsNotNull(previewScreenTransform);

        // Pick up the new preview screen.
        previewScreenTransform.parent = index;
        previewScreenTransform.localPosition = Vector3.zero;
    }

    private void stopGrabbing() {
        var WIMTransform = GameObject.Find("WIM").transform;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        var previewScreenTransform = WIM.Data.PreviewScreenTransform;
        Assert.IsNotNull(WIM);

        // Let go.
        if(!previewScreenTransform) return;
        previewScreenTransform.parent = WIMTransform.GetChild(0);
    }
}