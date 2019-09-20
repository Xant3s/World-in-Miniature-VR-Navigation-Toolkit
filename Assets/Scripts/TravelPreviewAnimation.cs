using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TravelPreviewAnimation : MonoBehaviour {
    public WIMSpaceConverter Converter { get; set; }
    public Transform DestinationInWIM { get; set; }
    public Transform PlayerRepresentationInWIM { get; set; }
    public Transform WIMLevelTransform { get; set; }
    public GameObject DestinationIndicator { get; set; }
    public float AnimationSpeed { get; set; } = 1.0f;

    private LineRenderer lr;
    private Transform animatedPlayerRepresentation;
    private float animationProgress;
    private float startAnimationProgress;


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
        lr.material = Resources.Load<Material>("Materials/SemiTransparent");
    }

    private void initAnimatedPlayerRepresentation() {
        animatedPlayerRepresentation = GameObject.Instantiate(DestinationIndicator, WIMLevelTransform).transform;
        animatedPlayerRepresentation.name = "Animated Player Travel Representation";
        animatedPlayerRepresentation.gameObject.AddComponent<Rigidbody>().useGravity = false;
        animatedPlayerRepresentation.GetComponent<Renderer>().material =
            Resources.Load<Material>("Materials/SemiTransparent");
    }

    void OnDestroy() {
        DestroyImmediate(animatedPlayerRepresentation.gameObject);
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
        if (animationProgress == 0) {
            // Start animation: Turn towards destination.
            startAnimation();
        }
        else if (animationProgress >= 1) {
            // Reached destination. End animation: align with destination indicator.
            resetAnimation();
        }
        else {
            // In between: walk towards destination indicator.
            advanceWalkAnimation();
            updateRotation();
            updatePosition();
        }
    }

    private void startAnimation() {
        var destinationInLevelSpace = Converter.ConvertToLevelSpace(DestinationInWIM.position);
        var playerPosInLevelSpace = Converter.ConvertToLevelSpace(animatedPlayerRepresentation.position);
        var rotationInLevelSpace = Quaternion.LookRotation(destinationInLevelSpace - playerPosInLevelSpace, Vector3.up);
        var desiredRotation = WIMLevelTransform.rotation * rotationInLevelSpace;
        if (startAnimationProgress >= 1) {
            animationProgress = .01f; // Start the next phase: move towards destination.
            return;
        }

        var step = AnimationSpeed * Time.deltaTime;
        startAnimationProgress += step;
        animatedPlayerRepresentation.rotation =
            Quaternion.Lerp(PlayerRepresentationInWIM.rotation, desiredRotation, startAnimationProgress);
        updatePosition();
    }

    private void updatePosition() {
        var dir = DestinationInWIM.position - PlayerRepresentationInWIM.position;
        animatedPlayerRepresentation.position = PlayerRepresentationInWIM.position + dir * animationProgress;
    }

    private void advanceWalkAnimation() {
        var step = AnimationSpeed * Time.deltaTime;
        animationProgress += step;
    }

    private void updateRotation() {
        var destinationInLevelSpace = Converter.ConvertToLevelSpace(DestinationInWIM.position);
        var playerPosInLevelSpace = Converter.ConvertToLevelSpace(animatedPlayerRepresentation.position);
        var rotationInLevelSpace = Quaternion.LookRotation(destinationInLevelSpace - playerPosInLevelSpace, Vector3.up);
        animatedPlayerRepresentation.rotation = WIMLevelTransform.rotation * rotationInLevelSpace;
    }

    private void resetAnimation() {
        animatedPlayerRepresentation.position = PlayerRepresentationInWIM.position;
        animationProgress = 0;
        startAnimationProgress = 0;
    }
}