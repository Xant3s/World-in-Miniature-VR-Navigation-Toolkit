using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using CommonUsages = UnityEngine.XR.CommonUsages;


[RequireComponent(typeof(CharacterController))]
public class MovementProvider2 : MonoBehaviour {
    [SerializeField] private float speed = 1;
    [SerializeField] private XRNode inputSource = XRNode.LeftHand;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private LayerMask layer = 1;

    public InputAction action;

    private float gravityFactor = 1f;
    private XRRig rig;
    private Vector2 inputAxis;
    private CharacterController characterController;


    private void Start() {
        characterController = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();
    }

    private void Update() {
        var device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate() {
        var headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        var direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

        characterController.Move(direction * Time.fixedDeltaTime * speed);

        // Gravity
        var isGrounded = CheckIfGround();
        if(isGrounded)
            gravityFactor = 0;
        else
            gravityFactor += gravity * Time.fixedDeltaTime;

        characterController.Move(Vector3.up* gravityFactor * Time.fixedDeltaTime);
    }

    private bool CheckIfGround() {
        var rayStart = transform.TransformPoint(characterController.center);
        var rayLength = characterController.center.y + 0.01f;
        var hasHit = Physics.SphereCast(rayStart, characterController.radius, Vector3.down, out RaycastHit hit,
            rayLength, layer);
        return hasHit;
    }
}
