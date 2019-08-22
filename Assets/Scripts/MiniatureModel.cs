using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MiniatureModel : MonoBehaviour {

    [SerializeField] private GameObject playerRepresentation;
    [Range(0, 1)]
    [SerializeField] private float scaleFactor = 0.1f;

    private Transform levelTransform;
    private Transform WIMLevelTransform;
    private Transform playerRepresentationTransform;
    private Transform playerTransform;

    void Awake() {
        levelTransform = GameObject.Find("Level").transform;
        WIMLevelTransform = GameObject.Find("WIM Level").transform;
        playerTransform = GameObject.Find("OVRCameraRig").transform;
        Assert.IsNotNull(levelTransform);
        Assert.IsNotNull(WIMLevelTransform);
    }

    void Start() {
        playerRepresentationTransform = Instantiate(playerRepresentation, WIMLevelTransform).transform;
    }

    void Update() {
        var playerOffset = playerTransform.position - levelTransform.position;
        var playerRepresentationOffset = playerOffset * scaleFactor;
        playerRepresentationTransform.position = WIMLevelTransform.position + playerRepresentationOffset;
    }
}