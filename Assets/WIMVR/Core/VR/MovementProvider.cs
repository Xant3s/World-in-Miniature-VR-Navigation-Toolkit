// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


namespace WIMVR.VR {
    [RequireComponent(typeof(CharacterController))]
    public class MovementProvider : MonoBehaviour {
        public InputActionProperty movementAction;
        public float movementSpeed = 1f;
        public float additionalHeadHeight = .2f;

        private CharacterController characterController;
        private XRRig rig;
        private Vector2 inputAxis;
        private float gravityFactor = 1f;


        private void Awake() {
            characterController = GetComponent<CharacterController>();
            rig = GetComponent<XRRig>();
        }

        private void Start() {
            movementAction.action.performed += ctx => OnNewInput(ctx.action.ReadValue<Vector2>());
            movementAction.action.Enable();
        }

        private void OnNewInput(Vector2 value) => inputAxis = value;

        private void FixedUpdate() {
            // Move character controller to headset.
            characterController.height = rig.cameraInRigSpaceHeight + additionalHeadHeight;
            var capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
            characterController.center = new Vector3(capsuleCenter.x,
                characterController.height / 2 + characterController.skinWidth, capsuleCenter.z);

            // Move.
            var headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
            var direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
            characterController.Move(direction * Time.fixedDeltaTime * movementSpeed);

            // Gravity.
            if(characterController.isGrounded) {
                gravityFactor = 0;
            } else {
                gravityFactor += Physics.gravity.y * Time.fixedDeltaTime;
            }

            characterController.Move(Vector3.up * gravityFactor * Time.fixedDeltaTime);
        }

        private void OnDestroy() => movementAction.action.Disable();
    }
}