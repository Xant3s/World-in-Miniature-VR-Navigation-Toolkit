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
        if (!OVRInput.Get(showWIMButton)) return;
        dissolve();
    }

    private void dissolve() {
        //var t = transform.GetChild(0);
        //for (var i = 0; i < t.childCount; i++) {
        //    var tt = t.GetChild(i);
        //    var d = tt.GetComponent<Dissolve>();
        //    d.durationInSeconds = .5f;
        //    d.Play();
        //}
        Invoke("spawn", 0.5f);
    }

    private void spawn() {
        transform.rotation = Quaternion.identity;
        transform.position = HMDTransform.position + HMDTransform.forward * WIMSpawnOffset.z + new Vector3(WIMSpawnOffset.x, WIMSpawnOffset.y, 0);
        var t = transform.GetChild(0);
        for (var i = 0; i < t.childCount; i++) {
            var tt = t.GetChild(i);
            var d = tt.GetComponent<Dissolve>();
            d.durationInSeconds = 1;
            d.PlayInverse();
        }
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