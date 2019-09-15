using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TravelPreviewAnimation : MonoBehaviour {
    public WIMSpaceConverter Converter { get; set; }
    public Transform DestinationInWIM { get; set; }
    public Transform PlayerRepresentationInWIM { get; set; }
    public Transform WIM { get; set; }
    public GameObject DestinationIndicator { get; set; }
    public float AnimationSpeed { get; set; } = 1.0f;

    private LineRenderer lr;
    private Transform animatedPlayerRepresentation;
    private float animationProgress;


    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Start() {
        initLineRenderer();
        initAnimatedPlayerRepresentation();
        resetAnimation();
    }

    private void initLineRenderer() {
        lr.widthMultiplier = .001f;
        lr.material = Resources.Load<Material>("Materials/Blue");
    }

    private void initAnimatedPlayerRepresentation() {
        animatedPlayerRepresentation = GameObject.Instantiate(DestinationIndicator, WIM.GetChild(0)).transform;
        animatedPlayerRepresentation.name = "Animated Player Travel Representation";
        animatedPlayerRepresentation.gameObject.AddComponent<Rigidbody>().useGravity = false;
        animatedPlayerRepresentation.GetComponent<Renderer>().material =
            Resources.Load<Material>("Materials/SemiTransparent");
    }

    void Update() {
        updateLineRenderer();
        updateAnimatedPlayerRepresentation();
    }

    private void updateLineRenderer() {
        lr.SetPosition(0, PlayerRepresentationInWIM.position);
        lr.SetPosition(1, DestinationInWIM.position);
    }

    private void updateAnimatedPlayerRepresentation() {
        if (animationProgress >= 1)
            resetAnimation();
        updateRotation();
        updatePosition();
    }

    private void updatePosition() {
        var step = AnimationSpeed * Time.deltaTime;
        animationProgress += step;
        var dir = DestinationInWIM.position - PlayerRepresentationInWIM.position;
        animatedPlayerRepresentation.position = PlayerRepresentationInWIM.position + dir * animationProgress;
    }

    private void updateRotation() {
        var destinationInLevelSpace = Converter.ConvertToLevelSpace(DestinationInWIM.position);
        var playerPosInLevelSpace = Converter.ConvertToLevelSpace(animatedPlayerRepresentation.position);
        var rotationInLevelSpace = Quaternion.LookRotation(destinationInLevelSpace - playerPosInLevelSpace, Vector3.up);
        animatedPlayerRepresentation.rotation = WIM.rotation * rotationInLevelSpace;
    }

    private void resetAnimation() {
        animatedPlayerRepresentation.position = PlayerRepresentationInWIM.position;
        animationProgress = 0;
    }
}