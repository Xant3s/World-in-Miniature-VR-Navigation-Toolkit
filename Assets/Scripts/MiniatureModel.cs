using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class MiniatureModel : MonoBehaviour {
    [SerializeField] private GameObject playerRepresentation;

    [Range(0, 1)] [SerializeField] private float scaleFactor = 0.1f;

    //[SerializeField] private OVRInput.Controller showWIMButton;
    [SerializeField] private OVRInput.RawButton showWIMButton;
    [SerializeField] private Vector3 WIMSpawnOffset;


    private Transform levelTransform;
    private Transform WIMLevelTransform;
    private Transform playerRepresentationTransform;
    private Transform playerTransform;
    private Transform HMDTransform;

    void Awake() {
        levelTransform = GameObject.Find("Level").transform;
        WIMLevelTransform = GameObject.Find("WIM Level").transform;
        playerTransform = GameObject.Find("OVRCameraRig").transform;
        HMDTransform = GameObject.Find("CenterEyeAnchor").transform;
        Assert.IsNotNull(levelTransform);
        Assert.IsNotNull(WIMLevelTransform);
        Assert.IsNotNull(playerTransform);
        Assert.IsNotNull(HMDTransform);
    }

    void Start() {
        playerRepresentationTransform = Instantiate(playerRepresentation, WIMLevelTransform).transform;
    }

    void Update() {
        CheckSpawnWIM();
        updatePlayerRepresentation();
    }

    private void CheckSpawnWIM() {
        // For debugging.
        if (Input.GetKeyUp(KeyCode.Space)) {
            respawn();
            return;
        }

        if (!OVRInput.GetUp(showWIMButton)) return;
        respawn();
    }

    private void respawn() {
        var WIMLevel = transform.GetChild(0);
        for (var i = 0; i < WIMLevel.childCount; i++) {
            var d = WIMLevel.GetChild(i).GetComponent<Dissolve>();
            if(d == null) continue;
            d.durationInSeconds = 1f;
            d.Play();
        }

        WIMLevel.parent = null;
        WIMLevel.name = "WIM Level Old";
        playerRepresentationTransform.parent = null;
        var newWIMLevel = Instantiate(WIMLevel.gameObject, transform).transform;
        newWIMLevel.gameObject.name = "WIM Level";
        WIMLevelTransform = newWIMLevel;
        var rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        newWIMLevel.position = Vector3.zero;
        newWIMLevel.localPosition = Vector3.zero;
        newWIMLevel.rotation = Quaternion.identity;
        newWIMLevel.localRotation = Quaternion.identity;
        newWIMLevel.localScale = new Vector3(1,1,1);
        playerRepresentationTransform.parent = newWIMLevel;
        transform.rotation = Quaternion.identity;
        transform.position = HMDTransform.position + HMDTransform.forward * WIMSpawnOffset.z +
                             new Vector3(WIMSpawnOffset.x, WIMSpawnOffset.y, 0);
        for (var i = 0; i < newWIMLevel.childCount; i++) {
            var d = newWIMLevel.GetChild(i).GetComponent<Dissolve>();
            if(d == null) continue;
            d.durationInSeconds = 1;
            newWIMLevel.GetChild(i).GetComponent<Renderer>().material.SetFloat("Vector1_461A9E8C", 1);
            d.PlayInverse();
        }

        Invoke("destroyOldWIMLevel", 1.1f);
    }

    private void destroyOldWIMLevel() {
        Destroy(GameObject.Find("WIM Level Old"));
    }

    private void updatePlayerRepresentation() {
        var eyeOffset = playerTransform.forward.normalized * -0.15f;
        var playerOffset = (playerTransform.position + eyeOffset) - levelTransform.position;
        var playerRepresentationOffset = playerOffset * scaleFactor;
        playerRepresentationOffset = WIMLevelTransform.parent.rotation * playerRepresentationOffset;
        playerRepresentationTransform.position =
            WIMLevelTransform.position + playerRepresentationOffset + new Vector3(0, 0.6f, 0) * scaleFactor;

        var lookAtPos = HMDTransform.position + HMDTransform.forward;
        var lookAtOffset = lookAtPos - levelTransform.position;
        var WIMLookAtOffset = lookAtOffset * scaleFactor;
        WIMLookAtOffset = WIMLevelTransform.parent.rotation * WIMLookAtOffset;
        var y = playerRepresentationTransform.rotation.y;
        var lookPos = WIMLevelTransform.position + WIMLookAtOffset;
        lookPos.y = 0;
        playerRepresentationTransform.LookAt(lookPos);
        playerRepresentationTransform.Rotate(-90, 0, 0);
        playerRepresentationTransform.Rotate(0, -45, 0);
    }
}