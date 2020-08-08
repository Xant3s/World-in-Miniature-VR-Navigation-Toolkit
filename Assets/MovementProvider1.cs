using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(CharacterController))]
public class MovementProvider1 : LocomotionProvider {
    public float speed = 1f;
    public float gravityMultiplier = 1f;
    public List<XRController> controllers = null;

    private CharacterController characterController = null;
    private GameObject head;


    protected override void Awake() {
        characterController = GetComponent<CharacterController>();
        head = GetComponent<XRRig>().cameraGameObject;
    }

    private void Start() {
        PositionController();
    }

    private void FixedUpdate() {
        PositionController();
        CheckForInput();
        ApplyGravity();
    }

    private void PositionController() {
        // Get the head in local, playspace ground
        var headHeight = Mathf.Clamp(head.transform.localPosition.y, 1, 2);
        characterController.height = headHeight;

        // Cut in half, add skin
        var newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;

        // Let's move the capsule in local space as well
        newCenter.x = head.transform.localPosition.x;
        newCenter.z = head.transform.localPosition.z;

        // Apply
        characterController.center = newCenter;
    }

    private void CheckForInput() {
        foreach(var controller in controllers) {
            if(controller.enableInputActions)
                CheckForMovement(controller.inputDevice);
        }
    }

    private void CheckForMovement(InputDevice device) {
        if(device.TryGetFeatureValue(CommonUsages.primary2DAxis, out var position))
            StartMove(position);
    }

    private void StartMove(Vector2 position) {
        // Apply the touch position to the head's forward Vector
        var direction = new Vector3(position.x, 0, position.y);
        var headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);

        // Rotate the input direction by the horizontal head rotation
        direction = Quaternion.Euler(headRotation) * direction;

        // Apply speed and move
        var movement = direction * speed;
        characterController.Move(movement * Time.deltaTime);
    }

    private void ApplyGravity() {
        var gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
        gravity.y *= Time.deltaTime;
        characterController.Move(gravity * Time.deltaTime);
    }
}