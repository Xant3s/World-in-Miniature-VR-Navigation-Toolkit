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
        //var rightHandPinch =
        //    OVRInput.GetDown(OVRInput.RawButton.A) || OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger);
        //var letGo = OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger);
        //if (letGo) pickupMode = false;

        //if (!rightHandPinch) return;
        //pickupMode = true;


        var rightHandPinch = thumbIsGrabbing && indexIsGrabbing;
        if(rightHandPinch && !isGrabbing) {
            isGrabbing = true;
            startGrabbing();
        } else if(isGrabbing && !rightHandPinch) {
            isGrabbing = false;
            stopGrabbing();
        }
    }

    void OnTriggerEnter(Collider other) {
        thumbIsGrabbing = other.transform == thumb;
        indexIsGrabbing = other.transform == index;
    }

    void OnTriggerStay(Collider other) {
        //if(other.tag != "HandR" || other.tag != "HandL") return;

        // Initiate pickup.
        //var rightHandPinch = OVRInput.Get(OVRInput.RawButton.A) && OVRInput.Get(OVRInput.RawButton.RIndexTrigger);
        //if (!rightHandPinch) return;
        //if (!pickupMode) return;

        // Remove existing destination indicator.
        //var WIM = transform.root;
        //Destroy(WIM);

        // Spawn new destination indicator.

        // Actually pick up the new destination indicator.

        // Move, rotate until letting go pinch grip.

        // Tell WIM there is a new destination every time the destination indicator is updated.
    }

    void OnTriggerExit(Collider other) {
        thumbIsGrabbing = !(other.transform == thumb);
        indexIsGrabbing = !(other.transform == index);
    }

    void startGrabbing() {
        var WIMTransform = transform.root;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        Assert.IsNotNull(WIM);

        //WIM.parent = index;

        // Remove existing destination indicator.
        WIM.removeDestinationIndicators();

        // Spawn new destination indicator.
        // TODO: destination indicator in WIM is responsible for updating its counterpart in the level?
        var destinationIndicatorInWIM = WIM.SpawnDestinationIndicatorInWIM();
        var destinationIndicatorInLevel = WIM.SpawnDestinationIndicatorInLevel();

        // Actually pick up the new destination indicator.

        // Move, rotate until letting go pinch grip.

        // Tell WIM there is a new destination every time the destination indicator is updated.
    }

    void stopGrabbing() {
        // Let go.

        // Snap to ground

        // New destination
    }
}