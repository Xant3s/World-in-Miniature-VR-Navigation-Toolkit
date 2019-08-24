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
    //[SerializeField] private OVRInput.Controller showWIMButton;
    [SerializeField] private OVRInput.RawButton showWIMButton;
    [SerializeField] private Vector3 WIMSpawnOffset;
    [SerializeField] private OVRInput.RawButton destinationSelectButton;
    [SerializeField] private GameObject destinationIndicator;
    [SerializeField] private OVRInput.RawButton confirmTeleportButton;
    

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
        CheckSpawnWIM();
        SelectDestination();
        checkConfirmTeleport();
        updatePlayerRepresentation();
    }

    private void checkConfirmTeleport() {
        if (!OVRInput.GetUp(confirmTeleportButton)) return;
        if (!destinationIndicatorInLevel) return;
        OVRPlayerController.position = destinationIndicatorInLevel.position;
        OVRPlayerController.rotation = destinationIndicatorInLevel.rotation;
        Destroy(destinationIndicatorInLevel.gameObject);
        Destroy(destinationIndicatorInWIM.gameObject);
    }

    private void CheckSpawnWIM() {
        if (!OVRInput.Get(showWIMButton)) return;
        transform.rotation = Quaternion.identity;
        transform.position = HMDTransform.position + HMDTransform.forward * WIMSpawnOffset.z + new Vector3(WIMSpawnOffset.x, WIMSpawnOffset.y, 0);
    }

    private void SelectDestination() {
        if (!OVRInput.GetUp(destinationSelectButton)) return;

        // Check if in WIM bounds.
        if (isInsideWIM(fingertipIndexR.position)) {
            // Remove previous destination point.
            if (destinationIndicatorInWIM) {
                Destroy(destinationIndicatorInWIM.gameObject);
                Destroy(destinationIndicatorInLevel.gameObject);
            }

            // Show destination in WIM.
            destinationIndicatorInWIM = Instantiate(destinationIndicator, WIMLevelTransform).transform;
            //destinationWIM.position = getGroundPosition(fingertipIndexR.position);
            destinationIndicatorInWIM.position = fingertipIndexR.position;

            // Show destination in level.
            var WIMPosition = destinationIndicatorInWIM.position;
            var WIMOffset = WIMPosition - WIMLevelTransform.position;
            var levelOffset = WIMOffset / scaleFactor;
            levelOffset = levelTransform.rotation * levelOffset;
            var levelPosition = levelTransform.position + levelOffset;
            destinationIndicatorInLevel = Instantiate(destinationIndicator, levelTransform).transform;
            destinationIndicatorInLevel.position = levelPosition;
        }
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