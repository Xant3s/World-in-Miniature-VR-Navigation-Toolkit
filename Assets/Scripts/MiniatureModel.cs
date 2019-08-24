using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabbable))]
public class MiniatureModel : MonoBehaviour {
    [SerializeField] private GameObject playerRepresentation;
    [Range(0, 1)] [SerializeField] private float scaleFactor = 0.1f;
    [SerializeField] private OVRInput.RawButton showWIMButton;
    [SerializeField] private Vector3 WIMSpawnOffset;
    [SerializeField] private OVRInput.RawButton destinationSelectButton;
    [SerializeField] private GameObject destinationIndicator;
    [SerializeField] private OVRInput.RawButton confirmTeleportButton;
    [Tooltip("If active, the destination will automatically set to ground level." +
             "This protects the player from being teleported to a location in mid-air.")]
    [SerializeField] private bool destinationAlwaysOnTheGround = true;
    
    private Transform levelTransform;
    private Transform WIMLevelTransform;
    private Transform playerRepresentationTransform;
    private Transform playerTransform;
    private Transform HMDTransform;
    private Transform fingertipIndexR;
    private Transform destinationIndicatorInWIM;
    private Transform destinationIndicatorInLevel;
    private Transform OVRPlayerController;

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
    }

    void Update() {
        checkSpawnWIM();
        SelectDestination();
        checkConfirmTeleport();
        updatePlayerRepresentation();
    }

    private void checkSpawnWIM() {
        if (!OVRInput.GetUp(showWIMButton)) return;
        respawnWIM();
    }

    private void respawnWIM() {
        removeDestinationIndicators();

        var WIMLevel = transform.GetChild(0);
        for (var i = 0; i < WIMLevel.childCount; i++) {
            var d = WIMLevel.GetChild(i).GetComponent<Dissolve>();
            if(d == null) continue;
            d.durationInSeconds = 1f;
            d.Play();
        }
        
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
        transform.position = HMDTransform.position + HMDTransform.forward * WIMSpawnOffset.z +
                             new Vector3(WIMSpawnOffset.x, WIMSpawnOffset.y, 0);
        for (var i = 0; i < newWIMLevel.childCount; i++) {
            var d = newWIMLevel.GetChild(i).GetComponent<Dissolve>();
            if(d == null) continue;
            d.durationInSeconds = 1;
            newWIMLevel.GetChild(i).GetComponent<Renderer>().material.SetFloat("Vector1_461A9E8C", 1);
            d.PlayInverse();
        }

        Invoke("destroyOldWIMLevel", 1.1f);
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

    private void SelectDestination() {
        if (!OVRInput.GetUp(destinationSelectButton)) return;

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
            //destinationIndicatorInWIM.position = ConvertToWIMSpace(getGroundPosition(levelPosition)) + new Vector3(0, destinationIndicator.transform.localScale.y * scaleFactor, 0);
        }

        // Destination indicator rotation.
        var lookPos = fingertipIndexR.position + fingertipIndexR.right;
        lookPos.y = destinationIndicatorInWIM.position.y;
        destinationIndicatorInWIM.LookAt(lookPos);
        destinationIndicatorInLevel.rotation = levelTransform.rotation * destinationIndicatorInWIM.rotation;
    }

    private void removeDestinationIndicators() {
        if (!destinationIndicatorInWIM) return;
        destinationIndicatorInWIM.parent = null;    // Prevent object from being copied with WIM. Destroy is apparently on another thread and thus, the object is not destroyed right away.
        Destroy(destinationIndicatorInWIM.gameObject);
        Destroy(destinationIndicatorInLevel.gameObject);
    }

    private Vector3 ConvertToLevelSpace(Vector3 pointInWIMSpace) {
        var WIMOffset = pointInWIMSpace - WIMLevelTransform.position;
        var levelOffset = WIMOffset / scaleFactor;
        levelOffset = levelTransform.rotation * levelOffset;
        return levelTransform.position + levelOffset;
    }

    private Vector3 ConvertToWIMSpace(Vector3 pointInLevelSpace) {
        var levelOffset = pointInLevelSpace - levelTransform.position;
        var WIMOffset = levelOffset * scaleFactor;
        WIMOffset = WIMLevelTransform.rotation * WIMOffset;
        return WIMLevelTransform.position + WIMOffset;
    }

    bool isInsideWIM(Vector3 point) {
        return GetComponent<Collider>().ClosestPoint(point) == point;
    }

    Vector3 getGroundPosition(Vector3 point) {
        return Physics.Raycast(point, Vector3.down, out var hit) ? hit.point : point;
    }

    private void updatePlayerRepresentation() {
        var eyeOffset = playerTransform.forward.normalized * -0.15f;
        var playerOffset = (playerTransform.position + eyeOffset) - levelTransform.position;
        var playerRepresentationOffset = playerOffset * scaleFactor;
        playerRepresentationOffset = WIMLevelTransform.parent.rotation * playerRepresentationOffset;
        playerRepresentationTransform.position =
            WIMLevelTransform.position + playerRepresentationOffset + new Vector3(0, 0.6f, 0) * scaleFactor;

        var lookAtPos = HMDTransform.position + HMDTransform.forward;
        var lookAtOffset = lookAtPos - levelTransform.position;
        var WIMLookAtOffset = lookAtOffset * scaleFactor;
        WIMLookAtOffset = WIMLevelTransform.parent.rotation * WIMLookAtOffset;
        var y = playerRepresentationTransform.rotation.y;
        var lookPos = WIMLevelTransform.position + WIMLookAtOffset;
        lookPos.y = 0;
        playerRepresentationTransform.LookAt(lookPos);
        playerRepresentationTransform.Rotate(-90, 0, 0);
        playerRepresentationTransform.Rotate(0, -45, 0);
    }
}