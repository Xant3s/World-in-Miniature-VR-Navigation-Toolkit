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
        var rightHandPinch = thumbIsGrabbing && indexIsGrabbing;
        if(rightHandPinch && !isGrabbing) {
            isGrabbing = true;
            startGrabbing();
        } else if(isGrabbing && !rightHandPinch) {
            isGrabbing = false;
        }
        if(!isGrabbing && (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))) {
            stopGrabbing();
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
        // TODO: destination indicator in WIM is responsible for updating its counterpart in the level?
        WIM.SpawnDestinationIndicatorInWIM();
        //var destinationIndicatorInLevel = WIM.SpawnDestinationIndicatorInLevel();

        // Actually pick up the new destination indicator.
        WIM.DestinationIndicatorInWIM.parent = index;
        //destinationIndicatorInWIM.localPosition = Vector3.zero;
        //WIM.DestinationIndicatorInWIM.parent = null;
        //WIM.DestinationIndicatorInWIM.gameObject.AddComponent<AlignWithRight>().Target = index;

        // Move, rotate until letting go pinch grip.

        // Tell WIM there is a new destination every time the destination indicator is updated.
        // Create destination indicator in level.

        // TODO: make destination indicator in WIM grabbable, so it can be changed without creating a new one.
    }

    void stopGrabbing() {
        var WIMTransform = transform.root;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();

        // Let go.
        //Destroy(WIM.DestinationIndicatorInWIM.GetComponent<AlignWithRight>());
        WIM.DestinationIndicatorInWIM.parent = WIMTransform.GetChild(0);

        // Snap to ground

        // Destination indicator in level.
        if(!WIM.DestinationIndicatorInWIM) WIM.SpawnDestinationIndicatorInLevel();

        // New destination
        WIM.IsNewDestination = true;
    }
}