using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TravelPreviewAnimation : MonoBehaviour {

    public Transform DestinationInWIM { get; set; }
    public Transform PlayerPositionInWIM { get; set; }
    public GameObject DestinationIndicator { get; set; }
    public float AnimationSpeed { get; set; } = 1.0f;
    public float Scalefactor { get; set; } = 1.0f;

    private LineRenderer lr;
    private Transform animatedPlayerRepresentation;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Start() {
        lr.widthMultiplier = .003f;
        animatedPlayerRepresentation = GameObject.Instantiate(DestinationIndicator).transform;
        animatedPlayerRepresentation.name = "Animated Player Travel Representation";
        animatedPlayerRepresentation.gameObject.AddComponent<Rigidbody>().useGravity = false;
        animatedPlayerRepresentation.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/SemiTransparent");
        animatedPlayerRepresentation.localScale *= Scalefactor;
        resetAnimatedPlayerRepresentation();
    }

    void Update() {
        updateLineRenderer();
        updateAnimatedPlayerRepresentation();
    }

    void updateLineRenderer() {
        lr.SetPosition(0, PlayerPositionInWIM.position);
        lr.SetPosition(1, DestinationInWIM.position);
    }

    void updateAnimatedPlayerRepresentation() {
        if (Vector3.Distance(animatedPlayerRepresentation.position, DestinationInWIM.position) < 0.1f) {
            resetAnimatedPlayerRepresentation();
        }
        animatedPlayerRepresentation.LookAt(DestinationInWIM);
        var step = AnimationSpeed * Time.deltaTime;
        animatedPlayerRepresentation.position = Vector3.MoveTowards(animatedPlayerRepresentation.position, DestinationInWIM.position, step);
    }

    void resetAnimatedPlayerRepresentation() {
        Debug.Log("reset");
        animatedPlayerRepresentation.position = PlayerPositionInWIM.position;
    }

}