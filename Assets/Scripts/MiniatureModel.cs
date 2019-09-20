using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabbable))]
[RequireComponent(typeof(DistanceGrabbable))]
public class MiniatureModel : MonoBehaviour, WIMSpaceConverter {
    [Separator("Basic Settings", true)]
    [SerializeField] private GameObject playerRepresentation;
    [SerializeField] private GameObject destinationIndicator;
    [Range(0, 1)] [SerializeField] private float scaleFactor = 0.1f;
    [VectorLabels("Left", "Right")] public Vector2 expandCollidersX = Vector2.zero;
    [VectorLabels("Up", "Down")] public Vector2 expandCollidersY = Vector2.zero;
    [VectorLabels("Front", "Back")] public Vector2 expandCollidersZ = Vector2.zero;
    [Tooltip("If active, the destination will automatically set to ground level." +
             "This protects the player from being teleported to a location in mid-air.")]
    [SerializeField] private bool destinationAlwaysOnTheGround = true;

    [Separator("Input", true)]
    [SerializeField] private OVRInput.RawButton showWIMButton;
    [SerializeField] public DestinationSelection destinationSelectionMethod;
    [ConditionalField(nameof(destinationSelectionMethod), false, DestinationSelection.Selection)]
    [SerializeField] private OVRInput.RawButton destinationSelectButton = OVRInput.RawButton.A;
    [ConditionalField(nameof(destinationSelectionMethod), false, DestinationSelection.Selection)]
    [SerializeField] private OVRInput.RawAxis2D destinationRotationThumbstick = OVRInput.RawAxis2D.RThumbstick;
    [ConditionalField(nameof(destinationSelectionMethod), false, DestinationSelection.Selection)]
    [SerializeField] private OVRInput.RawButton confirmTeleportButton = OVRInput.RawButton.B;
    [ConditionalField(nameof(destinationSelectionMethod), false, DestinationSelection.Pickup)]
    public float DoubleTapInterval = 2;


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
    public bool AutoScroll = false;

    public bool AutoGenerateWIM { get; set; } = false;
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
    public enum DestinationSelection {Selection, Pickup}

    public bool IsNewDestination {
        get => isNewDestination;
        set {
            isNewDestination = value;
            if(isNewDestination) onNewDestination();
        }
    }

    public Transform DestinationIndicatorInWIM {
        get => destinationIndicatorInWIM;
        set {
            Destroy(destinationIndicatorInWIM);
            destinationIndicatorInWIM = value;
        }
    }

    public Transform DestinationIndicatorInLevel { get; set; }

    private GameObject TravelPreviewAnimationObj {
        get => travelPreviewAnimationObj;
        set {
            Destroy(travelPreviewAnimationObj);
            travelPreviewAnimationObj = value;
        }
    }

    private GameObject travelPreviewAnimationObj;
    private Transform levelTransform;
    private Transform WIMLevelTransform;
    private Transform playerRepresentationTransform;
    private Transform playerTransform;
    private Transform HMDTransform;
    private Transform fingertipIndexR;
    private Transform OVRPlayerController;
    private bool armLengthDetected = false;
    private PostTravelPathTrace pathTrace;
    private OVRGrabbable grabbable;
    private float prevInterHandDistance;
    private Transform handL;
    private Transform handR;
    private bool handRIsInside;
    private bool handLIsInside;
    private Hand scalingHand = Hand.NONE;
    private Material previewScreenMaterial;
    private float WIMHeightRelativeToPlayer;
    private bool isNewDestination = false;
    private Transform destinationIndicatorInWIM;


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
        if(destinationSelectionMethod == DestinationSelection.Pickup)
            playerRepresentationTransform.gameObject.AddComponent<PickupDestinationSelection>().DoubleTapInterval = DoubleTapInterval;
        respawnWIM(false);
    }

    void Update() {
        detectArmLength();
        checkSpawnWIM();
        if (destinationSelectionMethod == DestinationSelection.Selection) {
            selectDestination();
            selectDestinationRotation();
            checkConfirmTeleport();
        }
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
        respawnWIM(false);
    }

    private void respawnWIM(bool maintainTransformRelativeToPlayer) {
        RemoveDestinationIndicators();

        var WIMLevel = transform.GetChild(0);
        var dissolveFX = occlusionHandling == OcclusionHandling.None ||
                         occlusionHandling == OcclusionHandling.Transparency;
        if(AllowWIMScrolling) dissolveFX = false;
        if(dissolveFX && !maintainTransformRelativeToPlayer) dissolveWIM(WIMLevel);
        if(maintainTransformRelativeToPlayer) instantDissolveWIM(WIMLevel);

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
        
        if (!maintainTransformRelativeToPlayer) {
            var spawnDistanceZ = ((playerArmLength <= 0) ? WIMSpawnOffset.z : playerArmLength);
            var spawnDistanceY = (WIMSpawnAtHeight - playerHeightInCM) / 100;
            var camForwardPosition = HMDTransform.position + HMDTransform.forward;
            camForwardPosition.y = HMDTransform.position.y;
            var camForwardIgnoreY = camForwardPosition - HMDTransform.position;
            transform.rotation = Quaternion.identity;
            transform.position = HMDTransform.position + camForwardIgnoreY * spawnDistanceZ +
                                 Vector3.up * spawnDistanceY;
        }
        else {
            transform.position = new Vector3(transform.position.x,
                OVRPlayerController.position.y + WIMHeightRelativeToPlayer, transform.position.z);
        }

        if(dissolveFX) {
            resolveWIM(newWIMLevel);
            Invoke("destroyOldWIMLevel", 1.1f);
        }
        else {
            destroyOldWIMLevel();
        }

        if (maintainTransformRelativeToPlayer) transform.parent = null;
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
        foreach (Transform child in WIM) {
            var d = child.GetComponent<Dissolve>();
            if (!d) return;
            d.durationInSeconds = 1f;
            d.Play();
        }
    }

    private static void instantDissolveWIM(Transform WIM) {
        foreach (Transform child in WIM) {
            var d = child.GetComponent<Dissolve>();
            if (!d) return;
            d.SetProgress(1);
        }
    }

    private void destroyOldWIMLevel() {
        Destroy(GameObject.Find("WIM Level Old"));
    }

    private void checkConfirmTeleport() {
        if (!OVRInput.GetUp(confirmTeleportButton)) return;
        if (!DestinationIndicatorInLevel) return;
        ConfirmTeleport();
    }

    public void ConfirmTeleport() {
        RemoveDestinationIndicators();

        // Optional: post travel path trace
        if (postTravelPathTrace)
            createPostTravelPathTrace();

        // Travel.
        transform.parent = OVRPlayerController; // Maintain transform relative to player.
        WIMHeightRelativeToPlayer =
            transform.position.y - OVRPlayerController.position.y; // Maintain height relative to player.
        var playerHeight = OVRPlayerController.position.y - getGroundPosition(OVRPlayerController.position).y;
        OVRPlayerController.position = getGroundPosition(DestinationIndicatorInLevel.position) + Vector3.up * playerHeight;
        OVRPlayerController.rotation = DestinationIndicatorInLevel.rotation;

        respawnWIM(true); // Assist player to orientate at new location.

        // Optional: post travel path trace
        if (postTravelPathTrace)
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
        pathTrace.newPositionInWIM.position = DestinationIndicatorInWIM.position;
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
        RemoveDestinationIndicators();

        // Show destination in WIM.
        SpawnDestinationIndicatorInWIM();

        // Show destination in level.
        SpawnDestinationIndicatorInLevel();

        // Rotate destination indicator in WIM (align with pointing direction):
        // Get forward vector from fingertip in WIM space. Set to WIM floor. Won't work if floor is uneven.
        var lookAtPoint = fingertipIndexR.position + fingertipIndexR.right; // fingertip.right because of Oculus prefab
        var pointBFloor = ConvertToWIMSpace(getGroundPosition(lookAtPoint));
        var pointAFloor = ConvertToWIMSpace(getGroundPosition(fingertipIndexR.position));
        var fingertipForward = pointBFloor - pointAFloor;
        fingertipForward = Quaternion.Inverse(WIMLevelTransform.rotation) * fingertipForward;
        // Get current forward vector in WIM space. Set to floor.
        var currForward = getGroundPosition(DestinationIndicatorInWIM.position + DestinationIndicatorInWIM.forward)
                          - getGroundPosition(DestinationIndicatorInWIM.position);
        // Get signed angle between current forward vector and desired forward vector (pointing direction).
        var angle = Vector3.SignedAngle(currForward, fingertipForward, WIMLevelTransform.up);
        // Rotate to align with pointing direction.
        DestinationIndicatorInWIM.Rotate(Vector3.up, angle);

        // Rotate destination indicator in level.
        updateDestinationRotationInLevel();

        // New destination.
        IsNewDestination = true;
    }

    public Transform SpawnDestinationIndicatorInLevel() {
        var levelPosition = ConvertToLevelSpace(DestinationIndicatorInWIM.position);
        DestinationIndicatorInLevel = Instantiate(destinationIndicator, levelTransform).transform;
        DestinationIndicatorInLevel.position = levelPosition;

        // Optional: Set to ground level to prevent the player from being moved to a location in mid-air.
        if(destinationAlwaysOnTheGround) {
            DestinationIndicatorInLevel.position = getGroundPosition(levelPosition) +
                                                   new Vector3(0, destinationIndicator.transform.localScale.y, 0);
            DestinationIndicatorInWIM.position = ConvertToWIMSpace(getGroundPosition(levelPosition))
                                                 + WIMLevelTransform.up * destinationIndicator.transform.localScale.y *
                                                 ScaleFactor;
        }

        // Fix orientation.
        if(destinationSelectionMethod == DestinationSelection.Pickup && destinationAlwaysOnTheGround) {
            DestinationIndicatorInLevel.rotation = Quaternion.Inverse(WIMLevelTransform.rotation) * DestinationIndicatorInWIM.rotation;
            var forwardPos = DestinationIndicatorInLevel.position + DestinationIndicatorInLevel.forward;
            forwardPos.y = DestinationIndicatorInLevel.position.y;
            var forwardInLevel = forwardPos - DestinationIndicatorInLevel.position;
            DestinationIndicatorInLevel.rotation = Quaternion.LookRotation(forwardInLevel, Vector3.up);
            DestinationIndicatorInWIM.rotation = WIMLevelTransform.rotation * DestinationIndicatorInLevel.rotation;
        }

        return DestinationIndicatorInLevel;
    }

    public Transform SpawnDestinationIndicatorInWIM() {
        DestinationIndicatorInWIM = Instantiate(destinationIndicator, WIMLevelTransform).transform;
        DestinationIndicatorInWIM.position = fingertipIndexR.position;
        return DestinationIndicatorInWIM;
    }

    void onNewDestination() {
        //if (!IsNewDestination) return;
        IsNewDestination = false;

        // Optional: show preview screen.
        if (previewScreen) showPreviewScreen();

        // Optional: Travel preview animation.
        if (travelPreviewAnimation) createTravelPreviewAnimation();
    }

    void autoScrollWIM() {
        if(!AllowWIMScrolling || !AutoScroll) return;
        var scrollOffset = DestinationIndicatorInWIM
            ? -DestinationIndicatorInWIM.localPosition
            : -playerRepresentationTransform.localPosition;
        WIMLevelTransform.localPosition = scrollOffset;
    }

    private void createTravelPreviewAnimation() {
        TravelPreviewAnimationObj = new GameObject("Travel Preview Animation");
        var travelPreview = TravelPreviewAnimationObj.AddComponent<TravelPreviewAnimation>();
        travelPreview.DestinationInWIM = DestinationIndicatorInWIM;
        travelPreview.PlayerRepresentationInWIM = playerRepresentationTransform;
        travelPreview.DestinationIndicator = destinationIndicator;
        travelPreview.AnimationSpeed = TravelPreviewAnimationSpeed;
        travelPreview.WIMLevelTransform = WIMLevelTransform;
        travelPreview.Converter = this;
    }

    private void selectDestinationRotation() {
        // Only if there is a destination indicator in the WIM.
        if(!DestinationIndicatorInWIM) return;

        // Poll thumbstick input.
        var inputRotation = OVRInput.Get(destinationRotationThumbstick);

        // Only if rotation is changed via thumbstick.
        if (Math.Abs(inputRotation.magnitude) < 0.01f) return;

        // Rotate destination indicator in WIM via thumbstick.
        var rotationAngle = Mathf.Atan2(inputRotation.x, inputRotation.y) * 180 / Mathf.PI;
        DestinationIndicatorInWIM.rotation = WIMLevelTransform.rotation * Quaternion.Euler(0, rotationAngle, 0);

        // Update destination indicator rotation in level.
        updateDestinationRotationInLevel();
    }

    /// <summary>
    /// Update the destination indicator rotation in level.
    /// Rotation should match destination indicator rotation in WIM.
    /// </summary>
    private void updateDestinationRotationInLevel() {
        DestinationIndicatorInLevel.rotation =
            Quaternion.Inverse(WIMLevelTransform.rotation) * DestinationIndicatorInWIM.rotation;
    }

    public void RemoveDestinationIndicators() {
        if (!DestinationIndicatorInWIM) return;
        removePreviewScreen();
        // Using DestroyImmediate because the WIM is about to being copied and we don't want to copy these objects too.
        //DestroyImmediate(TravelPreviewAnimationObj);
        //DestroyImmediate(DestinationIndicatorInWIM.gameObject);
        //if(DestinationIndicatorInLevel)
        //    DestroyImmediate(DestinationIndicatorInLevel.gameObject);

        // Destroy uses another thread, so make sure they are not copied by removing from parent.
        if(TravelPreviewAnimationObj) {
            TravelPreviewAnimationObj.transform.parent = null;
            Destroy(TravelPreviewAnimationObj);
        }
        DestinationIndicatorInWIM.parent = null;
        Destroy(DestinationIndicatorInWIM.gameObject);
        if(!DestinationIndicatorInLevel) return;
        DestinationIndicatorInLevel.parent = null;
        Destroy(DestinationIndicatorInLevel.gameObject);
    }

    public void RemoveDestionantionIndicatorsExceptWIM() {
        if (!DestinationIndicatorInWIM) return;
        removePreviewScreen();
        // Using DestroyImmediate because the WIM is about to being copied and we don't want to copy these objects too.
        DestroyImmediate(TravelPreviewAnimationObj);
        if(DestinationIndicatorInLevel) DestroyImmediate(DestinationIndicatorInLevel.gameObject);
    }

    private void showPreviewScreen() {
        removePreviewScreen();
        var previewScreen = Instantiate(Resources.Load<GameObject>("Prefabs/Preview Screen"));
        previewScreen.GetComponent<FloatAbove>().Target = transform;
        var camObj = DestinationIndicatorInLevel.GetChild(1).gameObject; // Making assumptions on the prefab.
        var cam = camObj.gameObject.AddComponent<Camera>();
        cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.gray;
        previewScreenMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Unlit"));
        previewScreen.GetComponent<Renderer>().material = previewScreenMaterial;
        previewScreenMaterial.SetTexture("_BaseMap", cam.targetTexture);
    }

    private void updatePreviewScreen() {
        if (!previewScreen || !DestinationIndicatorInLevel) return;
        var cam = DestinationIndicatorInLevel.GetChild(1).gameObject.GetComponent<Camera>();
        Destroy(cam.targetTexture);
        cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
        if(!previewScreenMaterial) {
            Debug.LogError("Preview screen material is null");
            previewScreenMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Unlit"));
            GameObject.FindWithTag("PreviewScreen").GetComponent<Renderer>().material = previewScreenMaterial;
        }
        previewScreenMaterial.SetTexture("_BaseMap", cam.targetTexture);
    }

    private void removePreviewScreen() {
        var previewScreen = GameObject.FindGameObjectWithTag("PreviewScreen");
        if(!previewScreen) return;
        previewScreen.transform.parent = null;
        Destroy(previewScreen);
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
        playerRepresentationTransform.position = ConvertToWIMSpace(getGroundPosition(Camera.main.transform.position));
        playerRepresentationTransform.position += WIMLevelTransform.up * playerRepresentation.transform.localScale.y * ScaleFactor;

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

    public void generateNewWIM() {
        removeAllColliders(transform);
        adaptScaleFactorToPlayerHeight();
        var levelTransform = GameObject.Find("Level").transform;
        if (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);
        var WIMLevel = Instantiate(levelTransform, transform);
        WIMLevel.localPosition = Vector3.zero;
        WIMLevel.name = "WIM Level";
        foreach(Transform child in WIMLevel) {
            DestroyImmediate(child.GetComponent(typeof(Rigidbody)));
            DestroyImmediate(child.GetComponent(typeof(OVRGrabbable)));
            var renderer = child.GetComponent<Renderer>();
            if(renderer) {
                renderer.material = Resources.Load<Material>("Materials/Dissolve");
                renderer.shadowCastingMode = ShadowCastingMode.Off;
            }
            child.gameObject.AddComponent<Dissolve>();
            child.gameObject.isStatic = false;
        }
        transform.localScale = new Vector3(ScaleFactor, ScaleFactor, ScaleFactor);
        generateColliders();
    }

    private void generateColliders() {
        // Generate colliders: // TODO: what about child of childs: WIM-WIMLevel-objects-childs-...
        // 1. Copy colliders from actual level (to determine which objects should have a collider)
        // [alternatively don't delete them while generating the WIM]
        // 2. replace every collider with box collider (recursive, possibly multiple colliders per obj)
        var wimLevelTransform = transform.GetChild(0);
        foreach(Transform child in wimLevelTransform) {
            var collider = child.GetComponent<Collider>();
            if(!collider) continue;
            removeAllColliders(child);
            var childBoxCollider = child.gameObject.AddComponent<BoxCollider>();
            // 3. move collider to WIM root (consider scale and position)
            var rootCollider = gameObject.AddComponent<BoxCollider>();
            rootCollider.center = child.localPosition;
            rootCollider.size = Vector3.zero;
            var bounds = rootCollider.bounds;
            bounds.Encapsulate(childBoxCollider.bounds);
            rootCollider.size = bounds.size / ScaleFactor;
            removeAllColliders(child);
        }
        // 4. remove every collider that is fully inside another one.
        pruneColliders();
        // 5. extend collider (esp. upwards)
        expandColliders();
    }

    public void removeAllColliders() {
        while(GetComponent<Collider>()) {
            DestroyImmediate(GetComponent<Collider>());
        }
    }

    private void removeAllColliders(Transform t) {
        while(t.GetComponent<Collider>()) {
            DestroyImmediate(t.GetComponent<Collider>());
        }
    }

    private void pruneColliders() {
        var destoryList = new List<Collider>();
        var colliders = gameObject.GetComponents<Collider>();
        for(var i = 0; i < colliders.Length; i++) {
            var col = (BoxCollider)colliders[i];
            for(var j = 0; j < colliders.Length; j++) {
                if(i == j) continue;
                var other = colliders[j];
                var skip = false;
                for(var id = 0; id < 8; id++) {
                    if(other.bounds.Contains(getCorner(col, id))) continue;
                    // next collider
                    skip = true;
                    break;
                }
                if(skip) continue;
                destoryList.Add(col);
                break;
            }
        }
        while(destoryList.Count() != 0) {
            DestroyImmediate(destoryList[0]);
            destoryList.RemoveAt(0);
        }
    }

    private void expandColliders() {
        foreach(var boxCollider in gameObject.GetComponents<BoxCollider>()) {
            boxCollider.size += new Vector3(expandCollidersX.x + expandCollidersX.y, 0,0);
            boxCollider.center += Vector3.left * expandCollidersX.x / 2.0f;
            boxCollider.center += Vector3.right * expandCollidersX.y / 2.0f;

            boxCollider.size += new Vector3(0, expandCollidersY.x + expandCollidersY.y,0);
            boxCollider.center += Vector3.up * expandCollidersY.x / 2.0f;
            boxCollider.center += Vector3.down * expandCollidersY.y / 2.0f;

            boxCollider.size += new Vector3(0, 0,expandCollidersZ.x + expandCollidersZ.y);
            boxCollider.center += Vector3.forward * expandCollidersZ.x / 2.0f;
            boxCollider.center += Vector3.back * expandCollidersZ.y / 2.0f;
        }
    }

    private void adaptScaleFactorToPlayerHeight() {
        if (!adaptWIMSizeToPlayerHeight) return;
        var playerHeight = playerHeightInCM;
        const float defaultHeight = 170;
        var defaultScaleFactor = ScaleFactor;
        const float minHeight = 100;
        const float maxHeight = 200;
        playerHeight = Mathf.Clamp(playerHeight, minHeight, maxHeight);
        var maxScaleFactorDelta = MaxWIMScaleFactorDelta;
        var heightDelta = playerHeight - defaultHeight;
        if (heightDelta > 0) {
            const float maxDelta = maxHeight - defaultHeight;
            var actualDelta = maxHeight - playerHeight;
            var factor = actualDelta / maxDelta;
            var resultingScaleFactorDelta = maxScaleFactorDelta * factor;
            ScaleFactor = defaultScaleFactor + resultingScaleFactorDelta;
        } else if (heightDelta < 0) {
            const float maxDelta = defaultHeight - minHeight;
            var actualDelta = defaultHeight - playerHeight;
            var factor = actualDelta / maxDelta;
            var resultingScaleFactorDelta = maxScaleFactorDelta * (-factor);
            ScaleFactor = defaultScaleFactor + resultingScaleFactorDelta;
        }
    }

    private Vector3 getCorner(BoxCollider box, int id) {
        var extends = box.bounds.extents;
        var center = box.bounds.center;
        switch(id) {
            case 0:
                // Top right front.
                return center + box.transform.up * extends.y + box.transform.right * extends.x + box.transform.forward * -extends.z;
            case 1:
                // top right back.
                return center + box.transform.up * extends.y + box.transform.right * extends.x + box.transform.forward * +extends.z;
            case 2:
                // top left back.
                return center + box.transform.up * extends.y + box.transform.right * -extends.x + box.transform.forward * +extends.z;
            case 3:
                // top left front.
                return center + box.transform.up * extends.y + box.transform.right * -extends.x + box.transform.forward * -extends.z;
            case 4:
                // bottom right front.
                return center + box.transform.up * -extends.y + box.transform.right * extends.x + box.transform.forward * -extends.z;
            case 5:
                // bottom right back.
                return center + box.transform.up * -extends.y + box.transform.right * extends.x + box.transform.forward * -extends.z;
            case 6:
                // bottom left back.
                return center + box.transform.up * -extends.y + box.transform.right * -extends.x + box.transform.forward * +extends.z;
            case 7:
                // bottom left front.
                return center + box.transform.up * -extends.y + box.transform.right * -extends.x + box.transform.forward * -extends.z;
            default:
                throw new Exception("Bad input.");
        }
    }
}