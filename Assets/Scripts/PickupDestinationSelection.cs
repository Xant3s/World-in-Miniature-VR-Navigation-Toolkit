using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class PickupDestinationSelection : MonoBehaviour {

    public float DoubleTapInterval { get; set; } = 2;

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

    void Update() {
        var capLowerCenter = transform.position - transform.up * transform.localScale.y;
        var capUpperCenter = transform.position + transform.up * transform.localScale.y;
        var radius = GameObject.Find("WIM").GetComponent<MiniatureModel>().ScaleFactor * 1.0f;
        var colliders = Physics.OverlapCapsule(capLowerCenter, capUpperCenter, radius, LayerMask.GetMask("Hands"));
        thumbIsGrabbing = colliders.Contains(thumb.GetComponent<Collider>());
        indexIsGrabbing = colliders.Contains(index.GetComponent<Collider>());

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

    void startGrabbing() {
        var WIMTransform = transform.root;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        Assert.IsNotNull(WIM);

        // Remove existing destination indicator.
        WIM.RemoveDestinationIndicators();

        // Spawn new destination indicator.
        WIM.SpawnDestinationIndicatorInWIM();

        // Actually pick up the new destination indicator.
        WIM.DestinationIndicatorInWIM.parent = index;
    }

    void stopGrabbing() {
        var WIMTransform = transform.root;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        Assert.IsNotNull(WIM);

        // Let go.
        if(!WIM.DestinationIndicatorInWIM) return;
        WIM.DestinationIndicatorInWIM.parent = WIMTransform.GetChild(0);

        // Make destination indicator in WIM grabbable, so it can be changed without creating a new one.
        Invoke("allowUpdates", 1);

        // Create destination indicator in level. Includes snap to ground.
        if(!WIM.DestinationIndicatorInLevel) WIM.SpawnDestinationIndicatorInLevel();

        // New destination
        WIM.IsNewDestination = true;
    }

    private void allowUpdates() {
        var WIMTransform = transform.root;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        WIM.DestinationIndicatorInWIM.gameObject.AddComponent<PickupDestinationUpdate>().DoubleTapInterval = DoubleTapInterval;
    }
}