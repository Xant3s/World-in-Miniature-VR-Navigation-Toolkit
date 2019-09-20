using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDestinationSelection : MonoBehaviour {
    void Start() {
        var collider = gameObject.AddComponent<CapsuleCollider>();
        collider.height = 2.2f;
        collider.radius = .7f;
        collider.isTrigger = true;
    }

    void Update() {
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag != "HandR" || other.tag != "HandL") return;
        // Pickup

        // Spawn new destination indicator

        // Move, rotate

    }

    void OnTriggerStay(Collider other) {
        if (other.tag != "HandR" || other.tag != "HandL") return;
        // Initiate pickup.
        var rightHandPinch = OVRInput.GetDown(OVRInput.RawButton.A) || OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger);
        if (!rightHandPinch) return;

        // Remove existing destination indicator.

        // Spawn new destination indicator.

        // Actually pick up the new destination indicator.

        // Move, rotate until letting go pinch grip.

        // Tell WIM there is a new destination every time the destination indicator is updated.


    }

    void OntriggerExit(Collider other) {
        if (other.tag != "HandR" || other.tag != "HandL") return;

    }
}