using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class PickupDestinationUpdate : MonoBehaviour {
    public float DoubleTapInterval { get; set; } = 2;

    private enum TapState{None, FirstTap, WaitingForSecondTap, SecondTap}

    private TapState tapState;
    private bool pickupMode = false;
    private Transform thumb;
    private Transform index;
    private bool thumbIsTouching;
    private bool indexIsTouching;
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
        var capLowerCenter = transform.position - transform.up * transform.localScale.y;
        var capUpperCenter = transform.position + transform.up * transform.localScale.y;
        var radius = GameObject.Find("WIM").GetComponent<MiniatureModel>().Configuration.ScaleFactor * 1.0f;
        var colliders = Physics.OverlapCapsule(capLowerCenter, capUpperCenter, radius, LayerMask.GetMask("Hands"));
        thumbIsTouching = colliders.Contains(thumb.GetComponent<Collider>());
        indexIsTouching = colliders.Contains(index.GetComponent<Collider>());
        var thumbAndIndexTouching = thumbIsTouching && indexIsTouching;
        var thumbAndIndexPressed = OVRInput.Get(OVRInput.RawButton.A) && OVRInput.Get(OVRInput.RawButton.RIndexTrigger);

        if(!isGrabbing) {
            // Handle double tap
            switch(tapState) {
                case TapState.None when indexIsTouching && !thumbIsTouching:
                    // First tap
                    tapState = TapState.FirstTap;
                    Invoke("resetDoubleTap", DoubleTapInterval);
                    break;
                case TapState.FirstTap when !indexIsTouching:
                    // Tapped once, now outside 
                    tapState = TapState.WaitingForSecondTap;
                    break;
                case TapState.WaitingForSecondTap when indexIsTouching && !thumbIsTouching:
                    // 2nd tap
                    tapState = TapState.SecondTap;
                    var WIM = GameObject.Find("WIM").GetComponent<MiniatureModel>();
                    WIM.ConfirmTeleport();
                    break;
            }
        }

        if (!isGrabbing && thumbAndIndexTouching && thumbAndIndexPressed) {
            isGrabbing = true;
            resetDoubleTap();
            startGrabbing();
        }
        else if(isGrabbing && !thumbAndIndexPressed) {
            isGrabbing = false;
            stopGrabbing();
        }
    }

    void startGrabbing() {
        var WIMTransform = GameObject.Find("WIM").transform;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        Assert.IsNotNull(WIM);

        // Remove existing destination indicator (except "this").
        WIM.RemoveDestionantionIndicatorsExceptWIM();

        // Actually pick up the new destination indicator.
        WIM.Data.DestinationIndicatorInWIM.parent = index;
        var midPos = thumb.position + (index.position - thumb.position) / 2.0f;
        WIM.Data.DestinationIndicatorInWIM.position = midPos;
    }

    void stopGrabbing() {
        var WIMTransform = GameObject.Find("WIM").transform;
        var WIM = WIMTransform.GetComponent<MiniatureModel>();
        Assert.IsNotNull(WIM);

        // Let go. 
        WIM.Data.DestinationIndicatorInWIM.parent = WIMTransform.GetChild(0);

        // Create destination indicator in level. Includes snap to ground.
        if(!WIM.DestinationIndicatorInLevel) WIM.SpawnDestinationIndicatorInLevel();

        // New destination
        WIM.IsNewDestination = true;
    }

    private void resetDoubleTap() {
        //firstTap = false;
        tapState = TapState.None;
        CancelInvoke("resetDoubleTap");
    }
}