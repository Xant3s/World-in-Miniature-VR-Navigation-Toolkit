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
    private bool thumbIsTouching;
    private bool indexIsTouching;
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
        var radius = GameObject.Find("WIM").GetComponent<MiniatureModel>().Configuration.ScaleFactor * 1.0f;
        var colliders = Physics.OverlapCapsule(capLowerCenter, capUpperCenter, radius, LayerMask.GetMask("Hands"));
        thumbIsTouching = colliders.Contains(thumb.GetComponent<Collider>());
        indexIsTouching = colliders.Contains(index.GetComponent<Collider>());
        var thumbAndIndexTouching = thumbIsTouching && indexIsTouching;
        var thumbAndIndexPressed = OVRInput.Get(OVRInput.RawButton.A) && OVRInput.Get(OVRInput.RawButton.RIndexTrigger);

        if(!isGrabbing && thumbAndIndexTouching && thumbAndIndexPressed) {
            isGrabbing = true;
            startGrabbing();
        }
        else if(isGrabbing && !thumbAndIndexPressed) {
            isGrabbing = false;
            stopGrabbing();
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
        WIM.Data.DestinationIndicatorInWIM.parent = index;
        var midPos = thumb.position + (index.position - thumb.position) / 2.0f;
        WIM.Data.DestinationIndicatorInWIM.position = midPos;
        WIM.Data.DestinationIndicatorInWIM.rotation = WIM.Data.PlayerRepresentationTransform.rotation;
    }

    void stopGrabbing() {
        var WIMTransform = transform.root;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        Assert.IsNotNull(WIM);

        // Let go.
        if(!WIM.Data.DestinationIndicatorInWIM) return;
        WIM.Data.DestinationIndicatorInWIM.parent = WIMTransform.GetChild(0);

        // Make destination indicator in WIM grabbable, so it can be changed without creating a new one.
        Invoke("allowUpdates", 1);

        // Create destination indicator in level. Includes snap to ground.
        if (!WIM.DestinationIndicatorInLevel) WIM.SpawnDestinationIndicatorInLevel();

        // New destination
        WIM.IsNewDestination = true;
        WIM.NewDestination();
    }

    private void allowUpdates() {
        var WIMTransform = transform.root;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        Assert.IsNotNull(WIM);
        Assert.IsNotNull(WIM.Data.DestinationIndicatorInWIM);
        WIM.Data.DestinationIndicatorInWIM.gameObject.AddComponent<PickupDestinationUpdate>().DoubleTapInterval = DoubleTapInterval;
    }
}