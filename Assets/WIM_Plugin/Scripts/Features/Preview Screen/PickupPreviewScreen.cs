using System;
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
    private bool pickupIndexButtonDown = true;  // True, because this script is instantiated while the button is already pressed.
    private bool pickupThumbButtonDown = true;  // True, because this script is instantiated while the button is already pressed.


    private void Awake() {
        thumb = GameObject.FindWithTag("ThumbR").transform;
        index = GameObject.FindWithTag("IndexR").transform;
        Assert.IsNotNull(thumb);
        Assert.IsNotNull(index);
    }

    private void OnEnable() {
        MiniatureModel.OnPickupIndexButton += pickupIndexButtonEvent;
        MiniatureModel.OnPickupThumbButton += pickupThumbButtonEvent;
    }

    private void OnDisable() {
        MiniatureModel.OnPickupIndexButton -= pickupIndexButtonEvent;
        MiniatureModel.OnPickupThumbButton -= pickupThumbButtonEvent;
    }

    private void pickupIndexButtonEvent(WIMConfiguration config, WIMData data) {
        pickupIndexButtonDown = !pickupIndexButtonDown;
        if (!isGrabbing && !pickupIndexButtonDown)
            stopGrabbing();
    }

    private void pickupThumbButtonEvent(WIMConfiguration config, WIMData data) {
        pickupThumbButtonDown = !pickupThumbButtonDown;
        if(!isGrabbing && !pickupThumbButtonDown)
            stopGrabbing();
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
        var previewScreen = WIMTransform.GetComponent<PreviewScreen>();
        previewScreen.ShowPreviewScreenPickup(WIM.Configuration, WIM.Data);
        var previewScreenTransform = previewScreen.Data.PreviewScreenTransform;
        Assert.IsNotNull(previewScreenTransform);

        // Pick up the new preview screen.
        previewScreenTransform.parent = index;
        previewScreenTransform.localPosition = Vector3.zero;
    }

    private void stopGrabbing() {
        if (stoppedGrabbing) return;
        var WIMTransform = GameObject.Find("WIM").transform;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        var previewScreen = WIM.GetComponent<PreviewScreen>();
        var previewScreenTransform = previewScreen.Data.PreviewScreenTransform;
        Assert.IsNotNull(WIM);

        // Let go.
        if(!previewScreenTransform) return;
        previewScreenTransform.parent = WIMTransform.GetChild(0);
        stoppedGrabbing = true;
    }
}