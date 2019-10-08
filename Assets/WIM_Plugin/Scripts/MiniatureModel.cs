using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdvancedDissolve_Example;
using MyBox;
using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabbable))]
[RequireComponent(typeof(DistanceGrabbable))]
public class MiniatureModel : MonoBehaviour {
    public bool PrevAllowWIMScrolling { get; set; } = true;
    public OcclusionHandling prevOcclusionHandling { get; set; } = OcclusionHandling.None;
    public float MaxWIMScaleFactorDelta { get; set; } = 0.005f;  // The maximum value scale factor can be changed by (positive or negative) when adapting to player height.

    public bool IsNewDestination {
        get => isNewDestination;
        set {
            isNewDestination = value;
            if(isNewDestination) onNewDestination();
        }
    }

    public Transform DestinationIndicatorInLevel { get; set; }

    public bool PrevSemiTransparent { get; set; } = false;
    public float PrevTransparency { get; set; } = 0;


    private Transform HMDTransform;
    private Transform fingertipIndexR;
    private Transform OVRPlayerController;
    private PostTravelPathTrace pathTrace;
    private float WIMHeightRelativeToPlayer;
    private Vector3 WIMLevelLocalPosOnTravel;
    private bool isNewDestination = false;
    private bool previewScreenEnabled;
    private Vector3 WIMLevelLocalPos;
    private Material previewScreenMaterial;




    public WIMGenerator Generator;
    public WIMConfiguration Configuration;
    public WIMData Data;
    public WIMSpaceConverter Converter;

    public delegate void Action(WIMConfiguration config, WIMData data);
    public static event Action OnUpdate;
    public static event Action OnNewDestination;



    public MiniatureModel() {
        Generator = new WIMGenerator();
    }

    void Awake() {
        if(!ConfigurationIsThere()) return;
        Data = new WIMData();
        Converter = new WIMSpaceConverterImpl(Configuration, Data);
        Data.WIMLevelTransform = GameObject.Find("WIM Level").transform;
        Data.LevelTransform = GameObject.Find("Level").transform;
        Data.PlayerTransform = GameObject.Find("OVRCameraRig").transform;
        HMDTransform = GameObject.Find("CenterEyeAnchor").transform;
        fingertipIndexR = GameObject.Find("hands:b_r_index_ignore").transform;
        OVRPlayerController = GameObject.Find("OVRPlayerController").transform;
        Assert.IsNotNull(Data.WIMLevelTransform);
        Assert.IsNotNull(HMDTransform);
        Assert.IsNotNull(fingertipIndexR);
        Assert.IsNotNull(Configuration.PlayerRepresentation);
        Assert.IsNotNull(Configuration.DestinationIndicator);
        Assert.IsNotNull(OVRPlayerController);
        Assert.IsNotNull(Data.LevelTransform);
        Assert.IsNotNull(Data.PlayerTransform);
    }

    private bool ConfigurationIsThere() {
        if(Configuration) return true;
        Debug.LogError("WIM configuration missing.");
        return false;
    }

    void Start() {
        if(!ConfigurationIsThere()) return;
        WIMLevelLocalPos = Data.WIMLevelTransform.localPosition;
        Data.PlayerRepresentationTransform = Instantiate(Configuration.PlayerRepresentation, Data.WIMLevelTransform).transform;
        if(Configuration.DestinationSelectionMethod == DestinationSelection.Pickup)
            Data.PlayerRepresentationTransform.gameObject.AddComponent<PickupDestinationSelection>().DoubleTapInterval = Configuration.DoubleTapInterval;
        respawnWIM(false);
    }

    void Update() {
        if(!ConfigurationIsThere()) return;
        checkSpawnWIM();
        if(Configuration.DestinationSelectionMethod == DestinationSelection.Selection) {
            selectDestination();
            selectDestinationRotation();
            checkConfirmTeleport();
        }
        if (previewScreenEnabled) updatePreviewScreen();

        OnUpdate?.Invoke(Configuration, Data);
    }

    public void NewDestination() {
        OnNewDestination?.Invoke(Configuration, Data);
    }

    private void checkSpawnWIM() {
        if (!OVRInput.GetUp(Configuration.ShowWIMButton)) return;
        respawnWIM(false);
    }

    private void respawnWIM(bool maintainTransformRelativeToPlayer) {
        RemoveDestinationIndicators();

        var WIMLevel = transform.GetChild(0);
        var dissolveFX = Configuration.OcclusionHandlingMethod == OcclusionHandling.None;
        if(Configuration.AllowWIMScrolling) dissolveFX = false;
        if(dissolveFX && !maintainTransformRelativeToPlayer) WIMVisualizationUtils.DissolveWIM(WIMLevel);
        if(maintainTransformRelativeToPlayer) WIMVisualizationUtils.InstantDissolveWIM(WIMLevel);

        WIMLevel.parent = null;
        WIMLevel.name = "WIM Level Old";
        Data.PlayerRepresentationTransform.parent = null;
        var newWIMLevel = Instantiate(WIMLevel.gameObject, transform).transform;
        newWIMLevel.gameObject.name = "WIM Level";
        Data.WIMLevelTransform = newWIMLevel;
        var rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        newWIMLevel.position = Vector3.zero;
        newWIMLevel.localPosition = maintainTransformRelativeToPlayer ? WIMLevelLocalPosOnTravel : WIMLevelLocalPos;
        newWIMLevel.rotation = Quaternion.identity;
        newWIMLevel.localRotation = Quaternion.identity;
        newWIMLevel.localScale = new Vector3(1, 1, 1);
        Data.PlayerRepresentationTransform.parent = newWIMLevel;
        
        if (!maintainTransformRelativeToPlayer) {
            var spawnDistanceZ = ((Configuration.PlayerArmLength <= 0) ? Configuration.WIMSpawnOffset.z : Configuration.PlayerArmLength);
            var spawnDistanceY = (Configuration.WIMSpawnHeight - Configuration.PlayerHeightInCM) / 100;
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

        StartCoroutine(WIMVisualizationUtils.FixResolveBug(WIM, resolveDuration));
    }

    private void destroyOldWIMLevel() {
        Destroy(GameObject.Find("WIM Level Old"));
    }

    private void checkConfirmTeleport() {
        if (!OVRInput.GetUp(Configuration.ConfirmTravelButton)) return;
        if (!DestinationIndicatorInLevel) return;
        ConfirmTeleport();
    }

    public void ConfirmTeleport() {
        RemoveDestinationIndicators();

        // Optional: post travel path trace
        if (Configuration.PostTravelPathTrace)
            createPostTravelPathTrace();

        // Travel.
        WIMLevelLocalPosOnTravel = transform.GetChild(0).localPosition;
        transform.parent = OVRPlayerController; // Maintain transform relative to player.
        WIMHeightRelativeToPlayer =
            transform.position.y - OVRPlayerController.position.y; // Maintain height relative to player.
        var playerHeight = OVRPlayerController.position.y - MathUtils.GetGroundPosition(OVRPlayerController.position).y;
        OVRPlayerController.position = MathUtils.GetGroundPosition(DestinationIndicatorInLevel.position) + Vector3.up * playerHeight;
        OVRPlayerController.rotation = DestinationIndicatorInLevel.rotation;

        respawnWIM(true); // Assist player to orientate at new location.

        // Optional: post travel path trace
        if (Configuration.PostTravelPathTrace)
            initPostTravelPathTrace();
    }

    private void createPostTravelPathTrace() {
        var emptyGO = new GameObject();
        var postTravelPathTraceObj = new GameObject("Post Travel Path Trace");
        pathTrace = postTravelPathTraceObj.AddComponent<PostTravelPathTrace>();
        pathTrace.Converter = Converter;
        pathTrace.TraceDurationInSeconds = Configuration.TraceDuration;
        pathTrace.OldPositionInWIM = Instantiate(emptyGO, Data.WIMLevelTransform).transform;
        pathTrace.OldPositionInWIM.position = Data.PlayerRepresentationTransform.position;
        pathTrace.OldPositionInWIM.name = "PathTraceOldPosition";
        pathTrace.NewPositionInWIM = Instantiate(emptyGO, Data.WIMLevelTransform).transform;
        pathTrace.NewPositionInWIM.position = Data.DestinationIndicatorInWIM.position;
        pathTrace.NewPositionInWIM.name = "PathTraceNewPosition";
        Destroy(emptyGO);
    }

    private void initPostTravelPathTrace() {
        pathTrace.WIMLevelTransform = transform.GetChild(0);
        pathTrace.Init();
    }

    private void selectDestination() {
        // Only if select button is pressed.
        if (!OVRInput.GetDown(Configuration.DestinationSelectionButton)) return;

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
        var pointBFloor = Converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(lookAtPoint));
        var pointAFloor = Converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(fingertipIndexR.position));
        var fingertipForward = pointBFloor - pointAFloor;
        fingertipForward = Quaternion.Inverse(Data.WIMLevelTransform.rotation) * fingertipForward;
        // Get current forward vector in WIM space. Set to floor.
        var currForward = MathUtils.GetGroundPosition(Data.DestinationIndicatorInWIM.position + Data.DestinationIndicatorInWIM.forward)
                          - MathUtils.GetGroundPosition(Data.DestinationIndicatorInWIM.position);
        // Get signed angle between current forward vector and desired forward vector (pointing direction).
        var angle = Vector3.SignedAngle(currForward, fingertipForward, Data.WIMLevelTransform.up);
        // Rotate to align with pointing direction.
        Data.DestinationIndicatorInWIM.Rotate(Vector3.up, angle);

        // Rotate destination indicator in level.
        updateDestinationRotationInLevel();

        // New destination.
        IsNewDestination = true;
        NewDestination();
    }

    public Transform SpawnDestinationIndicatorInLevel() {
        var levelPosition = Converter.ConvertToLevelSpace(Data.DestinationIndicatorInWIM.position);
        DestinationIndicatorInLevel = Instantiate(Configuration.DestinationIndicator, Data.LevelTransform).transform;
        DestinationIndicatorInLevel.position = levelPosition;

        // Remove frustum.
        Destroy(DestinationIndicatorInLevel.GetChild(1).GetChild(0).gameObject);

        // Optional: Set to ground level to prevent the player from being moved to a location in mid-air.
        if(Configuration.DestinationAlwaysOnTheGround) {
            DestinationIndicatorInLevel.position = MathUtils.GetGroundPosition(levelPosition) +
                                                   new Vector3(0, Configuration.DestinationIndicator.transform.localScale.y, 0);
            Data.DestinationIndicatorInWIM.position = Converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(levelPosition))
                                                 + Data.WIMLevelTransform.up * Configuration.DestinationIndicator.transform.localScale.y *
                                                 Configuration.ScaleFactor;
        }

        // Fix orientation.
        if(Configuration.DestinationSelectionMethod == DestinationSelection.Pickup && Configuration.DestinationAlwaysOnTheGround) {
            DestinationIndicatorInLevel.rotation = Quaternion.Inverse(Data.WIMLevelTransform.rotation) * Data.DestinationIndicatorInWIM.rotation;
            var forwardPos = DestinationIndicatorInLevel.position + DestinationIndicatorInLevel.forward;
            forwardPos.y = DestinationIndicatorInLevel.position.y;
            var forwardInLevel = forwardPos - DestinationIndicatorInLevel.position;
            DestinationIndicatorInLevel.rotation = Quaternion.LookRotation(forwardInLevel, Vector3.up);
            Data.DestinationIndicatorInWIM.rotation = Data.WIMLevelTransform.rotation * DestinationIndicatorInLevel.rotation;
        }

        return DestinationIndicatorInLevel;
    }

    public Transform SpawnDestinationIndicatorInWIM() {
        Assert.IsNotNull(Data.WIMLevelTransform);
        Assert.IsNotNull(Configuration.DestinationIndicator);
        Data.DestinationIndicatorInWIM = Instantiate(Configuration.DestinationIndicator, Data.WIMLevelTransform).transform;
        Data.DestinationIndicatorInWIM.position = fingertipIndexR.position;
        if(Configuration.PreviewScreen && !Configuration.AutoPositionPreviewScreen)
            Data.DestinationIndicatorInWIM.GetChild(1).GetChild(0).gameObject.AddComponent<PickupPreviewScreen>();
        //DestinationIndicatorInWIM.Find("Camera").GetChild(0).gameObject.AddComponent<PickupPreviewScreen>();
        return Data.DestinationIndicatorInWIM;
    }

    void onNewDestination() {
        IsNewDestination = false;

        // Optional: show preview screen.
        if (Configuration.PreviewScreen && Configuration.AutoPositionPreviewScreen) showPreviewScreen(true);
    }

    private void selectDestinationRotation() {
        // Only if there is a destination indicator in the WIM.
        if(!Data.DestinationIndicatorInWIM) return;

        // Poll thumbstick input.
        var inputRotation = OVRInput.Get(Configuration.DestinationRotationThumbstick);

        // Only if rotation is changed via thumbstick.
        if (System.Math.Abs(inputRotation.magnitude) < 0.01f) return;

        // Rotate destination indicator in WIM via thumbstick.
        var rotationAngle = Mathf.Atan2(inputRotation.x, inputRotation.y) * 180 / Mathf.PI;
        Data.DestinationIndicatorInWIM.rotation = Data.WIMLevelTransform.rotation * Quaternion.Euler(0, rotationAngle, 0);

        // Update destination indicator rotation in level.
        updateDestinationRotationInLevel();
    }

    /// <summary>
    /// Update the destination indicator rotation in level.
    /// Rotation should match destination indicator rotation in WIM.
    /// </summary>
    private void updateDestinationRotationInLevel() {
        DestinationIndicatorInLevel.rotation =
            Quaternion.Inverse(Data.WIMLevelTransform.rotation) * Data.DestinationIndicatorInWIM.rotation;
    }

    public void RemoveDestinationIndicators() {
        if (!Data.DestinationIndicatorInWIM) return;
        RemovePreviewScreen();
        // Destroy uses another thread, so make sure they are not copied by removing from parent.
        if(Data.TravelPreviewAnimationObj) {
            Data.TravelPreviewAnimationObj.transform.parent = null;
            Destroy(Data.TravelPreviewAnimationObj);
        }
        Data.DestinationIndicatorInWIM.parent = null;
        Destroy(Data.DestinationIndicatorInWIM.gameObject);
        if(!DestinationIndicatorInLevel) return;
        DestinationIndicatorInLevel.parent = null;
        Destroy(DestinationIndicatorInLevel.gameObject);
    }

    public void RemoveDestinantionIndicatorsExceptWIM() {
        if (!Data.DestinationIndicatorInWIM) return;
        RemovePreviewScreen();
        // Using DestroyImmediate because the WIM is about to being copied and we don't want to copy these objects too.
        DestroyImmediate(Data.TravelPreviewAnimationObj);
        if(DestinationIndicatorInLevel) DestroyImmediate(DestinationIndicatorInLevel.gameObject);
    }

    public Transform showPreviewScreen(bool autoPosition) {
        RemovePreviewScreen();
        var previewScreen = Instantiate(Resources.Load<GameObject>("Prefabs/Preview Screen"));

        if(autoPosition) {
            previewScreen.GetComponent<FloatAbove>().Target = transform;
        }
        else {
            Destroy(previewScreen.GetComponent<FloatAbove>());
            previewScreen.AddComponent<ClosePreviewScreen>();
        }

        initPreviewScreen(previewScreen);
        previewScreenEnabled = true;
        return previewScreen.transform;
    }

    private void initPreviewScreen(GameObject previewScreen) {
        Assert.IsNotNull(DestinationIndicatorInLevel);
        Assert.IsNotNull(DestinationIndicatorInLevel.GetChild(1));
        var camObj = DestinationIndicatorInLevel.GetChild(1).gameObject; // Making assumptions on the prefab.
        Assert.IsNotNull(camObj);
        var cam = camObj.GetOrAddComponent<Camera>();
        Assert.IsNotNull(cam);
        cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.gray;
        previewScreenMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Unlit"));
        previewScreen.GetComponent<Renderer>().material = previewScreenMaterial;
        previewScreenMaterial.SetTexture("_BaseMap", cam.targetTexture);
    }

    private void updatePreviewScreen() {
        if (!Configuration.PreviewScreen || !DestinationIndicatorInLevel) return;
        var cam = DestinationIndicatorInLevel.GetChild(1).GetComponent<Camera>();
        Destroy(cam.targetTexture);
        cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
        if(!previewScreenMaterial) {
            Debug.LogError("Preview screen material is null");
            previewScreenMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Unlit"));
            GameObject.FindWithTag("PreviewScreen").GetComponent<Renderer>().material = previewScreenMaterial;
        }
        previewScreenMaterial.SetTexture("_BaseMap", cam.targetTexture);
    }

    public void RemovePreviewScreen() {
        previewScreenEnabled = false;
        var previewScreen = GameObject.FindGameObjectWithTag("PreviewScreen");
        if(!previewScreen) return;
        previewScreen.transform.parent = null;
        Destroy(previewScreen);
    }

    bool isInsideWIM(Vector3 point) {
        return GetComponents<Collider>().Any(coll => coll.ClosestPoint(point) == point);
    }

    public void ConfigureWIM() {
        // Cleanup old configuration.
        disableScrolling();
        cleanupOcclusionHandling();

        // Setup new configuration.
        var material = Generator.LoadAppropriateMaterial(this);
        Generator.SetWIMMaterial(material, this);
        Generator.SetupDissolveScript(this);
        if (Configuration.AllowWIMScrolling) enableScrolling(material);
        else if (Configuration.OcclusionHandlingMethod == OcclusionHandling.CutoutView) configureCutoutView(material);
        else if (Configuration.OcclusionHandlingMethod == OcclusionHandling.MeltWalls) configureMeltWalls(material);
    }

    private static void configureCutoutView(Material material) {
        var maskController = new GameObject("Mask Controller");
        var controller = maskController.AddComponent<Controller_Mask_Cone>();
        controller.materials = new[] {material};
        var spotlightObj = new GameObject("Spotlight Mask");
        var spotlight = spotlightObj.AddComponent<Light>();
        controller.spotLight1 = spotlight;
        spotlight.type = LightType.Spot;
        spotlightObj.AddComponent<AlignWith>().Target = Camera.main.transform;
    }

    private static void configureMeltWalls(Material material) {
        var maskController = new GameObject("Mask Controller");
        var controller = maskController.AddComponent<Controller_Mask_Cylinder>();
        controller.materials = new[] {material};
        var cylinderMask = new GameObject("Cylinder Mask");
        controller.cylinder1 = cylinderMask;
        cylinderMask.AddComponent<FollowHand>().hand = Hand.HAND_R;
    }

    void cleanupOcclusionHandling() {
        DestroyImmediate(GameObject.Find("Cylinder Mask"));
        DestroyImmediate(GameObject.Find("Spotlight Mask"));
        DestroyImmediate(GameObject.Find("Mask Controller"));
    }

    private void enableScrolling(Material material) {
        var maskController = new GameObject("Box Mask");
        var controller = maskController.AddComponent<Controller_Mask_Box>();
        controller.materials = new[] {material};
        var tmpGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var mf = tmpGO.GetComponent<MeshFilter>();
        var cubeMesh = Instantiate(mf.sharedMesh) as Mesh;
        maskController.AddComponent<MeshFilter>().sharedMesh = cubeMesh;
        maskController.AddComponent<AlignWith>().Target = transform;
        controller.box1 = maskController;
        controller.invert = true;
        removeAllColliders(transform);
        gameObject.AddComponent<BoxCollider>().size = Configuration.ActiveAreaBounds / Configuration.ScaleFactor;
        Generator.RemoveDissolveScript(this);
        DestroyImmediate(tmpGO);
        maskController.transform.position = transform.position;
    }

    private void disableScrolling() {
        DestroyImmediate(GameObject.Find("Box Mask"));
        removeAllColliders(transform);
        Generator.GenerateColliders(this);
    }

    private void removeAllColliders(Transform t) {
        Collider collider;
        while(collider = t.GetComponent<Collider>()) {  // Assignment
            DestroyImmediate(collider);
        }
    }

    internal void adaptScaleFactorToPlayerHeight() {
        if (!Configuration.AdaptWIMSizeToPlayerHeight) return;
        var playerHeight = Configuration.PlayerHeightInCM;
        const float defaultHeight = 170;
        var defaultScaleFactor = Configuration.ScaleFactor;
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
            Configuration.ScaleFactor = defaultScaleFactor + resultingScaleFactorDelta;
            Configuration.ScaleFactor = Mathf.Clamp(Configuration.ScaleFactor, Configuration.MinScaleFactor, Configuration.MaxScaleFactor);
            transform.localScale = new Vector3(Configuration.ScaleFactor,Configuration.ScaleFactor,Configuration.ScaleFactor);

        } else if (heightDelta < 0) {
            const float maxDelta = defaultHeight - minHeight;
            var actualDelta = defaultHeight - playerHeight;
            var factor = actualDelta / maxDelta;
            var resultingScaleFactorDelta = maxScaleFactorDelta * (-factor);
            Configuration.ScaleFactor = defaultScaleFactor + resultingScaleFactorDelta;
            Configuration.ScaleFactor = Mathf.Clamp(Configuration.ScaleFactor, Configuration.MinScaleFactor, Configuration.MaxScaleFactor);
            transform.localScale = new Vector3(Configuration.ScaleFactor,Configuration.ScaleFactor,Configuration.ScaleFactor);
        }
    }
}