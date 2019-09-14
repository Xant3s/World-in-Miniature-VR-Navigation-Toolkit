using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabbable))]
public class MiniatureModel : MonoBehaviour {
    [SerializeField] private GameObject playerRepresentation;
    [Range(0, 1)] [SerializeField] public float scaleFactor = 0.1f;
    [SerializeField] private OVRInput.RawButton showWIMButton;
    [SerializeField] private Vector3 WIMSpawnOffset;
    [SerializeField] private float WIMSpawnAtHeight = 150;
    [SerializeField] private float playerArmLength;
    [SerializeField] public float playerHeightInCM = 170;
    [SerializeField] private OVRInput.RawButton destinationSelectButton;
    [SerializeField] private OVRInput.RawAxis2D destinationRotationThumbstick;
    [SerializeField] private GameObject destinationIndicator;
    [SerializeField] private OVRInput.RawButton confirmTeleportButton;
    [Tooltip("If active, the destination will automatically set to ground level." +
             "This protects the player from being teleported to a location in mid-air.")]
    [SerializeField] private bool destinationAlwaysOnTheGround = true;
    [SerializeField] public bool transparentWIM = true;
    [HideInInspector] public bool transparentWIMprev = false;
    [HideInInspector] public float transparency = 0.33f;
    [Tooltip("At the start of the application, player has to extend the arm and press the confirm teleport button.")]
    [SerializeField] public bool autoDetectArmLength = false;
    [SerializeField] public bool adaptWIMSizeToPlayerHeight = false;
    [SerializeField] public bool travelPreviewAnimation = false;

    public float TravelPreviewAnimationSpeed { get; set; } = 1.0f;
    public float MaxWIMScaleFactorDelta { get; set; } = 0.005f;  // The maximum value scale factor can be changed by (positive or negative) when adapting to player height.

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


    void Awake() {
        levelTransform = GameObject.Find("Level").transform;
        WIMLevelTransform = GameObject.Find("WIM Level").transform;
        playerTransform = GameObject.Find("OVRCameraRig").transform;
        HMDTransform = GameObject.Find("CenterEyeAnchor").transform;
        fingertipIndexR = GameObject.Find("hands:b_r_index_ignore").transform;
        OVRPlayerController = GameObject.Find("OVRPlayerController").transform;
        Assert.IsNotNull(levelTransform);
        Assert.IsNotNull(WIMLevelTransform);
        Assert.IsNotNull(playerTransform);
        Assert.IsNotNull(HMDTransform);
        Assert.IsNotNull(fingertipIndexR);
        Assert.IsNotNull(playerRepresentation);
        Assert.IsNotNull(destinationIndicator);
        Assert.IsNotNull(OVRPlayerController);
    }

    void Start() {
        playerRepresentationTransform = Instantiate(playerRepresentation, WIMLevelTransform).transform;
        respawnWIM();

        //createTravelPreviewAnimation();
    }

    private void createTravelPreviewAnimation() {
        travelPreviewAnimationObj = new GameObject("Travel Preview Animation");
        var travelPreview = travelPreviewAnimationObj.AddComponent<TravelPreviewAnimation>();
        //travelPreview.DestinationInWIM = GameObject.Find("FakeDestination").transform;
        travelPreview.DestinationInWIM = destinationIndicatorInWIM;
        travelPreview.PlayerPositionInWIM = playerRepresentationTransform;
        travelPreview.DestinationIndicator = destinationIndicator;
        travelPreview.AnimationSpeed = TravelPreviewAnimationSpeed;
        travelPreview.Scalefactor = scaleFactor;
    }

    void Update() {
        detectArmLength();
        checkSpawnWIM();
        selectDestination();
        selectDestinationRotation();
        checkConfirmTeleport();
        updatePlayerRepresentationInWIM();
    }

    private void detectArmLength() {
        if (!autoDetectArmLength || armLengthDetected) return;
        if (OVRInput.GetDown(confirmTeleportButton)) {
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
        newWIMLevel.localScale = new Vector3(1,1,1);
        playerRepresentationTransform.parent = newWIMLevel;
        transform.rotation = Quaternion.identity;
        var spawnDistanceZ = ((playerArmLength <= 0)? WIMSpawnOffset.z : playerArmLength);
        var spawnDistanceY = (WIMSpawnAtHeight - playerHeightInCM) / 100;
        var camForwardPosition = HMDTransform.position + HMDTransform.forward;
        camForwardPosition.y = HMDTransform.position.y;
        var camForwardIgnoreY = camForwardPosition - HMDTransform.position;
        transform.position = HMDTransform.position + camForwardIgnoreY * spawnDistanceZ +
                             Vector3.up * spawnDistanceY;
        resolveWIM(newWIMLevel);
        Invoke("destroyOldWIMLevel", 1.1f);
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
        OVRPlayerController.position = destinationIndicatorInLevel.position;
        OVRPlayerController.rotation = destinationIndicatorInLevel.rotation;
        respawnWIM(); // Assist player to orientate at new location.
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
        var levelPosition = convertToLevelSpace(destinationIndicatorInWIM.position);
        destinationIndicatorInLevel = Instantiate(destinationIndicator, levelTransform).transform;
        destinationIndicatorInLevel.position = levelPosition;

        // Optional: Set to ground level to prevent the player from being moved to a location in mid-air.
        if (destinationAlwaysOnTheGround) {
            destinationIndicatorInLevel.position = getGroundPosition(levelPosition) + new Vector3(0, destinationIndicator.transform.localScale.y, 0);
            destinationIndicatorInWIM.position = convertToWimSpace(getGroundPosition(levelPosition)) 
                                                 + WIMLevelTransform.up * destinationIndicator.transform.localScale.y * scaleFactor;
        }

        // Rotate destination indicator in WIM (align with pointing direction):
        // Get forward vector from fingertip in WIM space. Set to WIM floor. Won't work if floor is uneven.
        var lookAtPoint = fingertipIndexR.position + fingertipIndexR.right; // fingertip.right because of Oculus prefab
        var pointBFloor = convertToWimSpace(getGroundPosition(lookAtPoint));
        var pointAFloor = convertToWimSpace(getGroundPosition(fingertipIndexR.position));
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

        // Optional: Travel preview animation.
        if (travelPreviewAnimation) {
            createTravelPreviewAnimation();
        }
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
        destinationIndicatorInWIM.parent = null;    // Prevent object from being copied with WIM. Destroy is apparently on another thread and thus, the object is not destroyed right away.
        Destroy(travelPreviewAnimationObj);
        Destroy(destinationIndicatorInWIM.gameObject);
        Destroy(destinationIndicatorInLevel.gameObject);
    }

    private Vector3 convertToLevelSpace(Vector3 pointInWIMSpace) {
        var WIMOffset = pointInWIMSpace - WIMLevelTransform.position;
        var levelOffset = WIMOffset / scaleFactor;
        levelOffset = Quaternion.Inverse(WIMLevelTransform.rotation) * levelOffset; 
        return levelTransform.position + levelOffset;
    }

    private Vector3 convertToWimSpace(Vector3 pointInLevelSpace) {
        var levelOffset = pointInLevelSpace - levelTransform.position;
        var WIMOffset = levelOffset * scaleFactor;
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
        playerRepresentationTransform.position = convertToWimSpace(Camera.main.transform.position);
        playerRepresentationTransform.position -= WIMLevelTransform.up * 0.7f * scaleFactor;

        // Rotation
        var rotationInLevel = WIMLevelTransform.rotation * playerTransform.rotation;
        playerRepresentationTransform.rotation = rotationInLevel;
    }
}