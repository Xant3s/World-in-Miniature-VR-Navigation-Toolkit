using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PickupDestinationUpdate : MonoBehaviour {
    public float DoubleTapInterval { get; set; } = 2;

    private bool pickupMode = false;
    private Transform thumb;
    private Transform index;
    private bool thumbIsGrabbing;
    private bool indexIsGrabbing;
    private bool isGrabbing;
    private bool stoppedGrabbing;
    private bool firstTap = false;


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
        }
        else if(isGrabbing && !rightHandPinch) {
            isGrabbing = false;
        }

        // Hotfix: for not detection thumb letting go.
        if(isGrabbing && !OVRInput.Get(OVRInput.RawButton.A)) {
            isGrabbing = false;
        }

        if(!isGrabbing && (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))) {
            if(stoppedGrabbing) return;
            stopGrabbing();
            stoppedGrabbing = true;
        }

        // Detect destination confirmation (double tap).

    }

    void OnTriggerEnter(Collider other) {
        var WIM = GameObject.Find("WIM").GetComponent<MiniatureModel>();

        if(other.transform == thumb) {
            thumbIsGrabbing = true;
        }
        else if(other.transform == index) {
            indexIsGrabbing = true;
            if(firstTap) {
                WIM.ConfirmTeleport();
            }
            else {
                firstTap = true;
                Invoke("resetDoubleTap", DoubleTapInterval);
            }
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

        // Remove existing destination indicator (except "this").
        WIM.RemoveDestionantionIndicatorsExceptWIM();

        // Actually pick up the new destination indicator.
        WIM.DestinationIndicatorInWIM.parent = index;
    }

    void stopGrabbing() {
        var WIMTransform = GameObject.Find("WIM").transform;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();

        // Let go.
        WIM.DestinationIndicatorInWIM.parent = WIMTransform.GetChild(0);

        // Create destination indicator in level. Includes snap to ground.
        if(!WIM.DestinationIndicatorInLevel) WIM.SpawnDestinationIndicatorInLevel();

        // New destination
        WIM.IsNewDestination = true;
    }

    private void resetDoubleTap() {
        firstTap = false;
    }
}