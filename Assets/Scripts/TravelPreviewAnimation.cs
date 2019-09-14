﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TravelPreviewAnimation : MonoBehaviour {

    public WIMSpaceConverter converter { get; set; }
    public Transform DestinationInWIM { get; set; }
    public Transform PlayerRepresentationInWIM { get; set; }
    public Transform WIM { get; set; }
    public GameObject DestinationIndicator { get; set; }
    public float AnimationSpeed { get; set; } = 1.0f;
    public float Scalefactor { get; set; } = 1.0f;


    private LineRenderer lr;
    private Transform animatedPlayerRepresentation;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Start() {
        lr.widthMultiplier = .001f;
        lr.material = Resources.Load<Material>("Materials/Blue");
        animatedPlayerRepresentation = GameObject.Instantiate(DestinationIndicator, WIM.GetChild(0)).transform;
        animatedPlayerRepresentation.name = "Animated Player Travel Representation";
        animatedPlayerRepresentation.gameObject.AddComponent<Rigidbody>().useGravity = false;
        animatedPlayerRepresentation.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/SemiTransparent");
        resetAnimatedPlayerRepresentation();
    }

    void Update() {
        updateLineRenderer();
        updateAnimatedPlayerRepresentation();
    }

    void updateLineRenderer() {
        lr.SetPosition(0, PlayerRepresentationInWIM.position);
        lr.SetPosition(1, DestinationInWIM.position);
    }

    void updateAnimatedPlayerRepresentation() {
        if (Vector3.Distance(animatedPlayerRepresentation.position, DestinationInWIM.position) < 0.1f) {
            resetAnimatedPlayerRepresentation();
        }

        var destinationInLevelSpace = converter.ConvertToLevelSpace(DestinationInWIM.position);
        var playerPosInLevelSpace = converter.ConvertToLevelSpace(animatedPlayerRepresentation.position);
        //var playerPosInLevelSpace = animatedPlayerRepresentation.position;
        var rotationInLevelSpace = Quaternion.LookRotation(destinationInLevelSpace - playerPosInLevelSpace, Vector3.up);
        animatedPlayerRepresentation.rotation = WIM.rotation * rotationInLevelSpace;

        //animatedPlayerRepresentation.LookAt(DestinationInWIM);
        //animatedPlayerRepresentation.rotation = WIM.rotation * animatedPlayerRepresentation.rotation;
        
        var step = AnimationSpeed * Time.deltaTime;
        //animatedPlayerRepresentation.position = Vector3.MoveTowards(animatedPlayerRepresentation.position, DestinationInWIM.position, step);
        var posInLevelSpace = Vector3.MoveTowards(playerPosInLevelSpace, destinationInLevelSpace, step);
        animatedPlayerRepresentation.position = converter.ConvertToWIMSpace(posInLevelSpace);
    }

    void resetAnimatedPlayerRepresentation() {
        animatedPlayerRepresentation.position = PlayerRepresentationInWIM.position;
    }
}