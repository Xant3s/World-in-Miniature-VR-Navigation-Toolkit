// Author: Samuel Truman (contact@samueltruman.com)

using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


namespace WIMVR.Core {

    [RequireComponent(typeof(CharacterController))]
    public class MovementProvider : MonoBehaviour {
        public const string playerMovementActionMapName = "Player Movement";
        public const string directMovementActionName = "Direct Movement";

        public InputActionAsset mappings;
        public float movementSpeed = 1f;
        public float additionalHeadHeight = .2f;
        public LayerMask groundLayer = 1;

        private CharacterController characterController;
        private XRRig rig;
        private Vector2 inputAxis;
        private float gravityFactor = 1f;


        private void Awake() {
            characterController = GetComponent<CharacterController>();
            rig = GetComponent<XRRig>();
        }

        private void Start() {
            if(mappings == null) Debug.Log("Movement provider requires input mapping.");
            NotifyIfNoBinding();
            BindAction();
        }

        private void OnEnable() {
            var action = GetActionFromMappings();
            action?.Enable();
        }

        private void OnDisable() {
            var action = GetActionFromMappings();
            action?.Disable();
        }

        private void NotifyIfNoBinding() {
            if(!HasBinding()) Debug.Log($"No binding for {directMovementActionName}.");
        }

        private void BindAction() {
            if(!HasBinding()) return;
            var action = GetActionFromMappings();
            action.performed += Test;
        }

        public InputAction GetActionFromMappings() {
            var actionMap = mappings.FindActionMap(playerMovementActionMapName);
            var action = actionMap.FindAction(directMovementActionName);
            return action;
        }

        public void CopyBindings(InputAction from, InputAction to) {
            foreach(var binding in from.bindings) {
                to.AddBinding(binding);
            }
        }

        private bool HasBinding() {
            if(mappings == null) return false;
            var bindings = GetActionFromMappings().bindings;
            var bindingFound = bindings.Any(b => !string.IsNullOrEmpty(b.path));
            return bindings.Count != 0 && bindingFound;
        }

        public void SetupActionMap(InputActionAsset mappings) {
            if(mappings == null) return;
            var actionMap = mappings.FindActionMap(playerMovementActionMapName)
                            ?? mappings.AddActionMap(playerMovementActionMapName);
            var action = actionMap.FindAction(directMovementActionName)
                         ?? actionMap.AddAction(directMovementActionName);
            action.expectedControlType = "Vector2";
        }

        public void Test(InputAction.CallbackContext context) {
            inputAxis = context.ReadValue<Vector2>();
        }

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
    }
}