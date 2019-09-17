using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabbable))]
[RequireComponent(typeof(DistanceGrabbable))]
public class MiniatureModel : MonoBehaviour, WIMSpaceConverter {
    [Separator("Basic Settings", true)]
    [SerializeField] private GameObject playerRepresentation;
    [SerializeField] private GameObject destinationIndicator;
    [Range(0, 1)] [SerializeField] private float scaleFactor = 0.1f;
    [Tooltip("If active, the destination will automatically set to ground level." +
             "This protects the player from being teleported to a location in mid-air.")]
    [SerializeField] private bool destinationAlwaysOnTheGround = true;

    [Separator("Input", true)]
    [SerializeField] private OVRInput.RawButton showWIMButton;
    [SerializeField] private OVRInput.RawButton destinationSelectButton;
    [SerializeField] private OVRInput.RawAxis2D destinationRotationThumbstick;
    [SerializeField] private OVRInput.RawButton confirmTeleportButton;

    [Separator("Occlusion Handling", true)]
    [Tooltip("Select occlusion handling strategy. Disable for scrolling.")]
    public OcclusionHandling occlusionHandling;
    [ConditionalField(nameof(occlusionHandling), false, OcclusionHandling.Transparency)]
    public float transparency = 0.33f;
    [ConditionalField(nameof(occlusionHandling), false, OcclusionHandling.MeltWalls)]
    public float meltRadius = 1.0f;
    [ConditionalField(nameof(occlusionHandling), false, OcclusionHandling.MeltWalls)]
    public float meltHeight = 2.0f;
    [ConditionalField(nameof(occlusionHandling), false, OcclusionHandling.CutoutView)]
    public float cutoutRange = 10;
    [ConditionalField(nameof(occlusionHandling), false, OcclusionHandling.CutoutView)]
    public float cutoutAngle = 30;
    [ConditionalField(nameof(occlusionHandling), false, OcclusionHandling.CutoutView)]
    public bool showCutoutLight = false;
    [ConditionalField(nameof(showCutoutLight))]
    public Color cutoutLightColor = Color.white;


    [Separator("Orientation Aids", true)] 
    [SerializeField] public bool previewScreen = false;
    [SerializeField] public bool travelPreviewAnimation = false;
    [ConditionalField(nameof(travelPreviewAnimation))][Tooltip("Number between 0 and 1.")]
    public float TravelPreviewAnimationSpeed = 1.0f;
    [SerializeField] public bool postTravelPathTrace = false;
    [ConditionalField(nameof(postTravelPathTrace))]
    public float traceDuration = 1.0f;

    [Separator("Usability", true)]
    [SerializeField] private Vector3 WIMSpawnOffset;
    [SerializeField] private float WIMSpawnAtHeight = 150;
    [SerializeField] public float playerHeightInCM = 170;
    [SerializeField] private float playerArmLength;

    [Tooltip("At the start of the application, player has to extend the arm and press the confirm teleport button.")]
    [SerializeField] public bool autoDetectArmLength = false;
    [ConditionalField(nameof(autoDetectArmLength))]
    public OVRInput.RawButton confirmArmLengthButton = OVRInput.RawButton.A;
    [SerializeField] public bool adaptWIMSizeToPlayerHeight = false;

    [Header("Allow Scaling")]
    [SerializeField] public bool AllowWIMScaling = false;
    [ConditionalField(nameof(AllowWIMScaling))]
    public float minScaleFactor = 0;
    [ConditionalField(nameof(AllowWIMScaling))]
    public float maxScaleFactor = .5f;
    [ConditionalField(nameof(AllowWIMScaling))]
    public OVRInput.RawButton grabButtonL = OVRInput.RawButton.LHandTrigger;
    [ConditionalField(nameof(AllowWIMScaling))]
    public OVRInput.RawButton grabButtonR = OVRInput.RawButton.RHandTrigger;
    [ConditionalField(nameof(AllowWIMScaling))]
    public float ScaleStep = .0001f;
    [ConditionalField(nameof(AllowWIMScaling))][Tooltip("Ignore inter hand distance deltas below this threshold for scaling.")]
    public float interHandDistanceDeltaThreshold = .1f;

    [Header("Allow Scrolling")]
    [ConditionalField(nameof(occlusionHandling), false, OcclusionHandling.None)]
    [SerializeField] public bool AllowWIMScrolling = false;
    [ConditionalField(nameof(AllowWIMScrolling))]
    public Vector3 activeAreaBounds = new Vector3(10,10,10);
    [ConditionalField(nameof(AllowWIMScrolling))]
    public bool AutoScroll = true;

    public bool PrevAllowWIMScrolling { get; set; } = true;
    public OcclusionHandling prevOcclusionHandling { get; set; } = OcclusionHandling.Transparency;
    public float MaxWIMScaleFactorDelta { get; set; } = 0.005f;  // The maximum value scale factor can be changed by (positive or negative) when adapting to player height.

    public float ScaleFactor {
        get => scaleFactor;
        set {
            scaleFactor = Mathf.Clamp(value, minScaleFactor, maxScaleFactor);
            transform.localScale = new Vector3(value,value,value);
        }
    }

    public enum OcclusionHandling{None, Transparency, MeltWalls, CutoutView}
    public enum Hand{NONE, HAND_L, HAND_R}

    private Transform levelTransform;
    private Transform WIMLevelTransform;
    private Transform playerRepresentationTransform;
    private Transform playerTransform;
    private Transform HMDTransform;
    private Transform fingertipIndexR;
    private Transform destinationIndicatorInWIM;
    private Transform destinationIndicatorInLevel;
    private Transform OVRPlayerController;
    private bool armLengthDetected = false;
    private GameObject travelPreviewAnimationObj;
    private PostTravelPathTrace pathTrace;
    private OVRGrabbable grabbable;
    private float prevInterHandDistance;
    private Transform handL;
    private Transform handR;
    private bool handRIsInside;
    private bool handLIsInside;
    private Hand scalingHand = Hand.NONE;
    private Material previewScreenMaterial;



    void Awake() {
        levelTransform = GameObject.Find("Level").transform;
        WIMLevelTransform = GameObject.Find("WIM Level").transform;
        playerTransform = GameObject.Find("OVRCameraRig").transform;
        HMDTransform = GameObject.Find("CenterEyeAnchor").transform;
        fingertipIndexR = GameObject.Find("hands:b_r_index_ignore").transform;
        OVRPlayerController = GameObject.Find("OVRPlayerController").transform;
        handL = GameObject.FindWithTag("HandL").transform;
        handR = GameObject.FindWithTag("HandR").transform;
        grabbable = GetComponent<OVRGrabbable>();
        Assert.IsNotNull(levelTransform);
        Assert.IsNotNull(WIMLevelTransform);
        Assert.IsNotNull(playerTransform);
        Assert.IsNotNull(HMDTransform);
        Assert.IsNotNull(fingertipIndexR);
        Assert.IsNotNull(playerRepresentation);
        Assert.IsNotNull(destinationIndicator);
        Assert.IsNotNull(OVRPlayerController);
        Assert.IsNotNull(grabbable);
    }

    void Start() {
        playerRepresentationTransform = Instantiate(playerRepresentation, WIMLevelTransform).transform;
        respawnWIM();
    }

    void Update() {
        detectArmLength();
        checkSpawnWIM();
        selectDestination();
        selectDestinationRotation();
        checkConfirmTeleport();
        updatePlayerRepresentationInWIM();
        updatePreviewScreen();
        scaleWIM();
        autoScrollWIM();
    }

    void scaleWIM() {
        // Only if WIM scaling is enabled and WIM is currently being grabbed with one hand.
        if (!AllowWIMScaling || !grabbable.isGrabbed) return;

        var grabbingHand = getGrabbingHand();
        var oppositeHand = getOppositeHand(grabbingHand);   // This is the potential scaling hand.
        var scaleButton = getGrabButton(oppositeHand);

        // Start scaling if the potential scaling hand (the hand currently not grabbing the WIM) is inside the WIM and starts grabbing.
        // Stop scaling if either hand lets go.
        if (getHandIsInside(oppositeHand) && OVRInput.GetDown(scaleButton)) {
            // Start scaling.
            scalingHand = oppositeHand;
        } else if (OVRInput.GetUp(scaleButton)) {
            // Stop scaling.
            scalingHand = Hand.NONE;
        }

        // Check if currently scaling. Abort if not.
        if (scalingHand == Hand.NONE) return;

        // Scale using inter hand distance delta.
        var currInterHandDistance = Vector3.Distance(handL.position, handR.position);
        var distanceDelta = currInterHandDistance - prevInterHandDistance;
        var deltaBeyondThreshold = Mathf.Abs(distanceDelta) >= interHandDistanceDeltaThreshold;
        if (distanceDelta > 0 && deltaBeyondThreshold) {
            ScaleFactor += ScaleStep;
        }
        else if(distanceDelta < 0 && deltaBeyondThreshold) {
            ScaleFactor -= ScaleStep;
        }
        prevInterHandDistance = currInterHandDistance;
    }

    private OVRInput.RawButton getGrabButton(Hand hand) {
        if (hand == Hand.NONE) return OVRInput.RawButton.None;
        return hand == Hand.HAND_L ? grabButtonL : grabButtonR;
    }

    private Hand getGrabbingHand() {
        return (grabbable.grabbedBy.tag == "HandL") ? Hand.HAND_L : Hand.HAND_R;
    }

    private Hand getOppositeHand(Hand hand) {
        if (hand == Hand.NONE) return Hand.NONE;
        return (hand == Hand.HAND_L) ? Hand.HAND_R : Hand.HAND_L;
    }

    private bool getHandIsInside(Hand hand) {
        switch (hand) {
            case Hand.HAND_L when handLIsInside:
            case Hand.HAND_R when handRIsInside:
                return true;
            default:
                return false;
        }
    }

    private void detectArmLength() {
        if (!autoDetectArmLength || armLengthDetected) return;
        if (OVRInput.GetDown(confirmArmLengthButton)) {
            armLengthDetected = true;
            var controllerPos = GameObject.Find("CustomHandRight").transform.position;
            var headPos = Camera.main.transform.position;
            playerArmLength = (controllerPos - headPos).magnitude;
        }
    }

    private void checkSpawnWIM() {
        if (!OVRInput.GetUp(showWIMButton)) return;
        respawnWIM();
    }

    private void respawnWIM() {
        removeDestinationIndicators();

        var WIMLevel = transform.GetChild(0);
        var dissolveFX = occlusionHandling == OcclusionHandling.None ||
                         occlusionHandling == OcclusionHandling.Transparency;
        if(AllowWIMScrolling) dissolveFX = false;
        if(dissolveFX)
            dissolveWIM(WIMLevel);

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
        newWIMLevel.localScale = new Vector3(1, 1, 1);
        playerRepresentationTransform.parent = newWIMLevel;
        transform.rotation = Quaternion.identity;
        var spawnDistanceZ = ((playerArmLength <= 0) ? WIMSpawnOffset.z : playerArmLength);
        var spawnDistanceY = (WIMSpawnAtHeight - playerHeightInCM) / 100;
        var camForwardPosition = HMDTransform.position + HMDTransform.forward;
        camForwardPosition.y = HMDTransform.position.y;
        var camForwardIgnoreY = camForwardPosition - HMDTransform.position;
        transform.position = HMDTransform.position + camForwardIgnoreY * spawnDistanceZ +
                             Vector3.up * spawnDistanceY;
        if(dissolveFX) {
            resolveWIM(newWIMLevel);
            Invoke("destroyOldWIMLevel", 1.1f);
        }
        else {
            destroyOldWIMLevel();
        }
    }

    private void resolveWIM(Transform WIM) {
        const int resolveDuration = 1;
        for (var i = 0; i < WIM.childCount; i++) {
            var d = WIM.GetChild(i).GetComponent<Dissolve>();
            if (d == null) continue;
            d.durationInSeconds = resolveDuration;
            WIM.GetChild(i).GetComponent<Renderer>().material.SetFloat("Vector1_461A9E8C", 1);
            d.PlayInverse();
        }
        StartCoroutine(FixResolveBug(WIM, resolveDuration));
    }

    /// <summary>
    /// At the end of the resolve effect the WIM will not be completely resolved due to float precision.
    /// To prevent this, set the dissolve progress to a negative number (everything below 0 will be handled as 0 anyway).
    /// </summary>
    /// <param name="WIM"></param>
    /// <param name="delay"></param>
    IEnumerator FixResolveBug(Transform WIM, float delay) {
        yield return new WaitForSeconds(delay);
        for (var i = 0; i < WIM.childCount; i++) {
            var d = WIM.GetChild(i).GetComponent<Dissolve>();
            if (d == null) continue;
            d.durationInSeconds = 1;
            WIM.GetChild(i).GetComponent<Renderer>().material.SetFloat("Vector1_461A9E8C", -.1f);
        }
    }

    private static void dissolveWIM(Transform WIM) {
        for (var i = 0; i < WIM.childCount; i++) {
            var d = WIM.GetChild(i).GetComponent<Dissolve>();
            if (d == null) continue;
            d.durationInSeconds = 1f;
            d.Play();
        }
    }

    private void destroyOldWIMLevel() {
        Destroy(GameObject.Find("WIM Level Old"));
    }

    private void checkConfirmTeleport() {
        if (!OVRInput.GetUp(confirmTeleportButton)) return;
        if (!destinationIndicatorInLevel) return;
        // Optional: post travel path trace
        if(postTravelPathTrace)
            createPostTravelPathTrace();

        // Travel.
        OVRPlayerController.position = destinationIndicatorInLevel.position;
        OVRPlayerController.rotation = destinationIndicatorInLevel.rotation;
        
        respawnWIM(); // Assist player to orientate at new location.

        // Optional: post travel path trace
        if(postTravelPathTrace)
            initPostTravelPathTrace();
    }

    private void createPostTravelPathTrace() {
        var emptyGO = new GameObject();
        var postTravelPathTraceObj = new GameObject("Post Travel Path Trace");
        pathTrace = postTravelPathTraceObj.AddComponent<PostTravelPathTrace>();
        pathTrace.Converter = this;
        pathTrace.TraceDurationInSeconds = traceDuration;
        pathTrace.oldPositionInWIM = Instantiate(emptyGO, WIMLevelTransform).transform;
        pathTrace.oldPositionInWIM.position = playerRepresentationTransform.position;
        pathTrace.oldPositionInWIM.name = "PathTraceOldPosition";
        pathTrace.newPositionInWIM = Instantiate(emptyGO, WIMLevelTransform).transform;
        pathTrace.newPositionInWIM.position = destinationIndicatorInWIM.position;
        pathTrace.newPositionInWIM.name = "PathTraceNewPosition";
        Destroy(emptyGO);
    }

    private void initPostTravelPathTrace() {
        pathTrace.WIMLevelTransform = transform.GetChild(0);
        pathTrace.Init();
    }

    private void selectDestination() {
        // Only if select button is pressed.
        if (!OVRInput.GetDown(destinationSelectButton)) return;

        // Check if in WIM bounds.
        if (!isInsideWIM(fingertipIndexR.position)) return;

        // Remove previous destination point.
        removeDestinationIndicators();

        // Show destination in WIM.
        destinationIndicatorInWIM = Instantiate(destinationIndicator, WIMLevelTransform).transform;
        destinationIndicatorInWIM.position = fingertipIndexR.position;

        // Show destination in level.
        var levelPosition = ConvertToLevelSpace(destinationIndicatorInWIM.position);
        destinationIndicatorInLevel = Instantiate(destinationIndicator, levelTransform).transform;
        destinationIndicatorInLevel.position = levelPosition;

        // Optional: Set to ground level to prevent the player from being moved to a location in mid-air.
        if (destinationAlwaysOnTheGround) {
            destinationIndicatorInLevel.position = getGroundPosition(levelPosition) + new Vector3(0, destinationIndicator.transform.localScale.y, 0);
            destinationIndicatorInWIM.position = ConvertToWIMSpace(getGroundPosition(levelPosition)) 
                                                 + WIMLevelTransform.up * destinationIndicator.transform.localScale.y * ScaleFactor;
        }

        // Rotate destination indicator in WIM (align with pointing direction):
        // Get forward vector from fingertip in WIM space. Set to WIM floor. Won't work if floor is uneven.
        var lookAtPoint = fingertipIndexR.position + fingertipIndexR.right; // fingertip.right because of Oculus prefab
        var pointBFloor = ConvertToWIMSpace(getGroundPosition(lookAtPoint));
        var pointAFloor = ConvertToWIMSpace(getGroundPosition(fingertipIndexR.position));
        var fingertipForward = pointBFloor - pointAFloor;
        fingertipForward = Quaternion.Inverse(WIMLevelTransform.rotation) * fingertipForward;
        // Get current forward vector in WIM space. Set to floor.
        var currForward = getGroundPosition(destinationIndicatorInWIM.position + destinationIndicatorInWIM.forward)
                          - getGroundPosition(destinationIndicatorInWIM.position);
        // Get signed angle between current forward vector and desired forward vector (pointing direction).
        var angle = Vector3.SignedAngle(currForward, fingertipForward, WIMLevelTransform.up);
        // Rotate to align with pointing direction.
        destinationIndicatorInWIM.Rotate(Vector3.up, angle);

        // Rotate destination indicator in level.
        updateDestinationRotationInLevel();

        // Optional: show preview screen.
        if(previewScreen) showPreviewScreen();

        // Optional: Travel preview animation.
        if(travelPreviewAnimation) createTravelPreviewAnimation();
    }

    void autoScrollWIM() {
        if(!AllowWIMScrolling || !AutoScroll) return;
        var scrollOffset = destinationIndicatorInWIM
            ? -destinationIndicatorInWIM.localPosition
            : -playerRepresentationTransform.localPosition;
        WIMLevelTransform.localPosition = scrollOffset;
    }

    private void createTravelPreviewAnimation() {
        travelPreviewAnimationObj = new GameObject("Travel Preview Animation");
        var travelPreview = travelPreviewAnimationObj.AddComponent<TravelPreviewAnimation>();
        travelPreview.DestinationInWIM = destinationIndicatorInWIM;
        travelPreview.PlayerRepresentationInWIM = playerRepresentationTransform;
        travelPreview.DestinationIndicator = destinationIndicator;
        travelPreview.AnimationSpeed = TravelPreviewAnimationSpeed;
        travelPreview.WIMLevelTransform = WIMLevelTransform;
        travelPreview.Converter = this;
    }

    private void selectDestinationRotation() {
        // Only if there is a destination indicator in the WIM.
        if(!destinationIndicatorInWIM) return;

        // Poll thumbstick input.
        var inputRotation = OVRInput.Get(destinationRotationThumbstick);

        // Only if rotation is changed via thumbstick.
        if (Math.Abs(inputRotation.magnitude) < 0.01f) return;

        // Rotate destination indicator in WIM via thumbstick.
        var rotationAngle = Mathf.Atan2(inputRotation.x, inputRotation.y) * 180 / Mathf.PI;
        destinationIndicatorInWIM.rotation = WIMLevelTransform.rotation * Quaternion.Euler(0, rotationAngle, 0);

        // Update destination indicator rotation in level.
        updateDestinationRotationInLevel();
    }

    /// <summary>
    /// Update the destination indicator rotation in level.
    /// Rotation should match destination indicator rotation in WIM.
    /// </summary>
    private void updateDestinationRotationInLevel() {
        destinationIndicatorInLevel.rotation =
            Quaternion.Inverse(WIMLevelTransform.rotation) * destinationIndicatorInWIM.rotation;
    }

    private void removeDestinationIndicators() {
        if (!destinationIndicatorInWIM) return;
        removePreviewScreen();
        // Using DestroyImmediate because the WIM is about to being copied and we don't want to copy these objects too.
        DestroyImmediate(travelPreviewAnimationObj);
        DestroyImmediate(destinationIndicatorInWIM.gameObject);
        DestroyImmediate(destinationIndicatorInLevel.gameObject);
    }

    private void showPreviewScreen() {
        removePreviewScreen();
        var previewScreen = Instantiate(Resources.Load<GameObject>("Prefabs/Preview Screen"));
        previewScreen.GetComponent<FloatAbove>().Target = transform;
        var camObj = destinationIndicatorInLevel.GetChild(1).gameObject; // Making assumptions on the prefab.
        var cam = camObj.gameObject.AddComponent<Camera>();
        cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.gray;
        previewScreen.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        previewScreenMaterial = previewScreen.GetComponent<Renderer>().material;
        previewScreenMaterial.SetTexture("_BaseMap", cam.targetTexture);
    }

    private void updatePreviewScreen() {
        if (!previewScreen || !destinationIndicatorInLevel) return;
        var cam = destinationIndicatorInLevel.GetChild(1).gameObject.GetComponent<Camera>();
        Destroy(cam.targetTexture);
        cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
        previewScreenMaterial.SetTexture("_BaseMap", cam.targetTexture);
    }

    private void removePreviewScreen() {
        Destroy(GameObject.FindGameObjectWithTag("PreviewScreen"));
    }

    public Vector3 ConvertToLevelSpace(Vector3 pointInWIMSpace) {
        var WIMOffset = pointInWIMSpace - WIMLevelTransform.position;
        var levelOffset = WIMOffset / ScaleFactor;
        levelOffset = Quaternion.Inverse(WIMLevelTransform.rotation) * levelOffset; 
        return levelTransform.position + levelOffset;
    }

    public Vector3 ConvertToWIMSpace(Vector3 pointInLevelSpace) {
        var levelOffset = pointInLevelSpace - levelTransform.position;
        var WIMOffset = levelOffset * ScaleFactor;
        WIMOffset = WIMLevelTransform.rotation * WIMOffset;
        return WIMLevelTransform.position + WIMOffset;
    }

    bool isInsideWIM(Vector3 point) {
        return GetComponents<Collider>().Any(coll => coll.ClosestPoint(point) == point);
    }

    Vector3 getGroundPosition(Vector3 point) {
        return Physics.Raycast(point, Vector3.down, out var hit) ? hit.point : point;
    }

    private void updatePlayerRepresentationInWIM() {
        // Position.
        playerRepresentationTransform.position = ConvertToWIMSpace(Camera.main.transform.position);
        playerRepresentationTransform.position -= WIMLevelTransform.up * 0.7f * ScaleFactor;

        // Rotation
        var rotationInLevel = WIMLevelTransform.rotation * playerTransform.rotation;
        playerRepresentationTransform.rotation = rotationInLevel;
    }

    void OnTriggerEnter(Collider other) {
        const int handLayer = 9;
        if (other.gameObject.layer != handLayer) return;
        if (other.transform.root.tag == "HandL") {
            handLIsInside = true;
        }
        else {
            handRIsInside = true;
        }
    }

    void OnTriggerExit(Collider other) {
        const int handLayer = 9;
        if (other.gameObject.layer != handLayer) return;
        if (other.transform.root.tag == "HandL") {
            handLIsInside = false;
        }
        else {
            handRIsInside = false;
        }
    }
}