using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PickupDestinationSelection : MonoBehaviour {
    private bool pickupMode = false;
    private Transform thumb;
    private Transform index;
    private bool thumbIsGrabbing;
    private bool indexIsGrabbing;
    private bool isGrabbing;
    private bool stoppedGrabbing;


    void Awake() {
        thumb = GameObject.FindWithTag("ThumbR").transform;
        index = GameObject.FindWithTag("IndexR").transform;
        Assert.IsNotNull(thumb);
        Assert.IsNotNull(index);
    }


    void Start() {
        var collider = gameObject.AddComponent<CapsuleCollider>();
        collider.height = 2.2f;
        collider.radius = .7f;
        collider.isTrigger = true;
    }

    void Update() {
        var rightHandPinch = thumbIsGrabbing && indexIsGrabbing;
        if(rightHandPinch && !isGrabbing) {
            isGrabbing = true;
            stoppedGrabbing = false;
            startGrabbing();
        } else if(isGrabbing && !rightHandPinch) {
            isGrabbing = false;
        }
        if(!isGrabbing && (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))) {
            if(stoppedGrabbing) return;
            stopGrabbing();
            stoppedGrabbing = true;
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.transform == thumb) {
            thumbIsGrabbing = true;
        }
        else if(other.transform == index) {
            indexIsGrabbing = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.transform == thumb) {
            thumbIsGrabbing = false;
        }
        else if(other.transform == index) {
            indexIsGrabbing = false;
        }
    }

    void startGrabbing() {
        var WIMTransform = transform.root;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        Assert.IsNotNull(WIM);

        // Remove existing destination indicator.
        WIM.removeDestinationIndicators();

        // Spawn new destination indicator.
        WIM.SpawnDestinationIndicatorInWIM();

        // Actually pick up the new destination indicator.
        WIM.DestinationIndicatorInWIM.parent = index;

        // TODO: make destination indicator in WIM grabbable, so it can be changed without creating a new one.
    }

    void stopGrabbing() {
        var WIMTransform = transform.root;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();

        // Let go.
        WIM.DestinationIndicatorInWIM.parent = WIMTransform.GetChild(0);

        // Create destination indicator in level. Includes snap to ground.
        if(!WIM.DestinationIndicatorInLevel) WIM.SpawnDestinationIndicatorInLevel();

        // TODO: Fix orientation.

        // New destination
        WIM.IsNewDestination = true;
    }
}