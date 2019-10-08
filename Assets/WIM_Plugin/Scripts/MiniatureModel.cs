using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdvancedDissolve_Example;
using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabbable))]
[RequireComponent(typeof(DistanceGrabbable))]
public class MiniatureModel : MonoBehaviour {
    public bool PrevAllowWIMScrolling { get; set; } = true;
    public OcclusionHandling prevOcclusionHandling { get; set; } = OcclusionHandling.None;

    public bool PrevSemiTransparent { get; set; } = false;
    public float PrevTransparency { get; set; } = 0;


    private Transform HMDTransform;
    private Transform fingertipIndexR;
    private Transform OVRPlayerController;
    private float WIMHeightRelativeToPlayer;
    private Vector3 WIMLevelLocalPosOnTravel;
    private Vector3 WIMLevelLocalPos;


    public WIMGenerator Generator;
    public WIMConfiguration Configuration;
    public WIMData Data;
    public WIMSpaceConverter Converter;

    public delegate void WIMAction(WIMConfiguration config, WIMData data);
    public static event WIMAction OnUpdate;
    public static event WIMAction OnNewDestination;
    public static event WIMAction OnPreTravel;
    public static event WIMAction OnPostTravel;



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
        if (!Data.DestinationIndicatorInLevel) return;
        ConfirmTeleport();
    }

    public void ConfirmTeleport() {
        RemoveDestinationIndicators();

        OnPreTravel?.Invoke(Configuration, Data);

        // Travel.
        WIMLevelLocalPosOnTravel = transform.GetChild(0).localPosition;
        transform.parent = OVRPlayerController; // Maintain transform relative to player.
        WIMHeightRelativeToPlayer =
            transform.position.y - OVRPlayerController.position.y; // Maintain height relative to player.
        var playerHeight = OVRPlayerController.position.y - MathUtils.GetGroundPosition(OVRPlayerController.position).y;
        OVRPlayerController.position = MathUtils.GetGroundPosition(Data.DestinationIndicatorInLevel.position) + Vector3.up * playerHeight;
        OVRPlayerController.rotation = Data.DestinationIndicatorInLevel.rotation;

        respawnWIM(true); // Assist player to orientate at new location.

        OnPostTravel?.Invoke(Configuration, Data);
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
        NewDestination();
    }

    public Transform SpawnDestinationIndicatorInLevel() {
        var levelPosition = Converter.ConvertToLevelSpace(Data.DestinationIndicatorInWIM.position);
        Data.DestinationIndicatorInLevel = Instantiate(Configuration.DestinationIndicator, Data.LevelTransform).transform;
        Data.DestinationIndicatorInLevel.position = levelPosition;

        // Remove frustum.
        Destroy(Data.DestinationIndicatorInLevel.GetChild(1).GetChild(0).gameObject);

        // Optional: Set to ground level to prevent the player from being moved to a location in mid-air.
        if(Configuration.DestinationAlwaysOnTheGround) {
            Data.DestinationIndicatorInLevel.position = MathUtils.GetGroundPosition(levelPosition) +
                                                   new Vector3(0, Configuration.DestinationIndicator.transform.localScale.y, 0);
            Data.DestinationIndicatorInWIM.position = Converter.ConvertToWIMSpace(MathUtils.GetGroundPosition(levelPosition))
                                                 + Data.WIMLevelTransform.up * Configuration.DestinationIndicator.transform.localScale.y *
                                                 Configuration.ScaleFactor;
        }

        // Fix orientation.
        if(Configuration.DestinationSelectionMethod == DestinationSelection.Pickup && Configuration.DestinationAlwaysOnTheGround) {
            Data.DestinationIndicatorInLevel.rotation = Quaternion.Inverse(Data.WIMLevelTransform.rotation) * Data.DestinationIndicatorInWIM.rotation;
            var forwardPos = Data.DestinationIndicatorInLevel.position + Data.DestinationIndicatorInLevel.forward;
            forwardPos.y = Data.DestinationIndicatorInLevel.position.y;
            var forwardInLevel = forwardPos - Data.DestinationIndicatorInLevel.position;
            Data.DestinationIndicatorInLevel.rotation = Quaternion.LookRotation(forwardInLevel, Vector3.up);
            Data.DestinationIndicatorInWIM.rotation = Data.WIMLevelTransform.rotation * Data.DestinationIndicatorInLevel.rotation;
        }

        return Data.DestinationIndicatorInLevel;
    }

    public Transform SpawnDestinationIndicatorInWIM() {
        Assert.IsNotNull(Data.WIMLevelTransform);
        Assert.IsNotNull(Configuration.DestinationIndicator);
        Data.DestinationIndicatorInWIM = Instantiate(Configuration.DestinationIndicator, Data.WIMLevelTransform).transform;
        Data.DestinationIndicatorInWIM.position = fingertipIndexR.position;
        if(Configuration.PreviewScreen && !Configuration.AutoPositionPreviewScreen)
            Data.DestinationIndicatorInWIM.GetChild(1).GetChild(0).gameObject.AddComponent<PickupPreviewScreen>();
        return Data.DestinationIndicatorInWIM;
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
        Data.DestinationIndicatorInLevel.rotation =
            Quaternion.Inverse(Data.WIMLevelTransform.rotation) * Data.DestinationIndicatorInWIM.rotation;
    }

    public void RemoveDestinationIndicators() {
        if (!Data.DestinationIndicatorInWIM) return;
        GetComponent<PreviewScreen>().RemovePreviewScreen();
        // Destroy uses another thread, so make sure they are not copied by removing from parent.
        if(Data.TravelPreviewAnimationObj) {
            Data.TravelPreviewAnimationObj.transform.parent = null;
            Destroy(Data.TravelPreviewAnimationObj);
        }
        Data.DestinationIndicatorInWIM.parent = null;
        Destroy(Data.DestinationIndicatorInWIM.gameObject);
        if(!Data.DestinationIndicatorInLevel) return;
        Data.DestinationIndicatorInLevel.parent = null;
        Destroy(Data.DestinationIndicatorInLevel.gameObject);
    }

    public void RemoveDestinantionIndicatorsExceptWIM() {
        if (!Data.DestinationIndicatorInWIM) return;
        GetComponent<PreviewScreen>().RemovePreviewScreen();
        // Using DestroyImmediate because the WIM is about to being copied and we don't want to copy these objects too.
        DestroyImmediate(Data.TravelPreviewAnimationObj);
        if(Data.DestinationIndicatorInLevel) DestroyImmediate(Data.DestinationIndicatorInLevel.gameObject);
    }

    bool isInsideWIM(Vector3 point) {
        return GetComponents<Collider>().Any(coll => coll.ClosestPoint(point) == point);
    }
}