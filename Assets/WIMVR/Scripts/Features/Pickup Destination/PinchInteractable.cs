// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR.Features.Pickup_Destination {
    /// <summary>
    /// Can be interacted with via a pinch gesture,
    /// i.e. simultaneous touching with the index finger and thumb of the same hand.
    /// Callbacks are used to execute logic when the pinch gesture is recognized;
    /// <remarks>Assumes fingertips to be tagged as 'IndexR', 'ThumbR', 'IndexL', and 'ThumbL' respectively.</remarks>
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class PinchInteractable : MonoBehaviour {
        //#region Members

        //private static readonly string[] fingers = {"IndexR", "ThumbR", "IndexL", "ThumbL"};

        //private Hand pinchingHand = Hand.None;


        //private readonly Dictionary<string, bool> fingersInside = new Dictionary<string, bool> {
        //    {fingers[0], false},
        //    {fingers[1], false},
        //    {fingers[2], false},
        //    {fingers[3], false}
        //};


        //private bool RIndexInside => fingersInside[fingers[0]];
        //private bool RThumbInside => fingersInside[fingers[1]];
        //private bool LIndexInside => fingersInside[fingers[2]];
        //private bool LThumbInside => fingersInside[fingers[3]];
        //private bool IsPinched => pinchingHand != Hand.None;

        //#endregion


        //#region Detect Pinch

        //protected void Update() {
        //    if(IsPinched) OnPinch(pinchingHand);
        //}

        //protected void OnTriggerEnter(Collider other) {
        //    fingersInside[other.tag] = true;
        //    if(!IsPinched && RIndexInside && RThumbInside) {
        //        pinchingHand = Hand.RightHand;
        //        OnStartPinch();
        //    }
        //    else if(!IsPinched && LIndexInside && LThumbInside) {
        //        pinchingHand = Hand.LeftHand;
        //        OnStartPinch();
        //    }
        //}

        //protected void OnTriggerExit(Collider other) {
        //    fingersInside[other.tag] = false;
        //    switch(pinchingHand) {
        //        case Hand.RightHand when !RIndexInside || !RThumbInside: // fall-through
        //        case Hand.LeftHand when !LIndexInside || !LThumbInside:
        //            StopPinching();
        //            break;
        //        default:
        //            return;
        //    }
        //}

        //private void StopPinching() {
        //    pinchingHand = Hand.None;
        //    OnStopPinch();
        //}

        //#endregion

        //#region Callbacks

        //protected virtual void OnStartPinch() { }
        //protected virtual void OnStopPinch() { }
        //protected virtual void OnPinch(Hand hand) { }

        //#endregion
    }
}