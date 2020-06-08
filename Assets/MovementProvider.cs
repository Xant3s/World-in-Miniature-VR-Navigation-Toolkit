using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class MovementProvider : MonoBehaviour {
    public const string playerMovementActionMapName = "Player Movement";
    public const string directMovementActionName = "Direct Movement";

    public InputActionAsset mappings;
    public float movementSpeed = 1f;
    public LayerMask groundLayer = 1;
    public InputAction directMovement;  // Should always have the same state as directMovementAction in mappings. Used to allow inline binding editing in inspector.

    private CharacterController characterController;


    private void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    private void Start() {
        if(mappings == null) Debug.Log("Movement provider requires input mapping.");
        NotifyIfNoBinding();
        BindAction();
    }

    private void OnEnable() {
        var action = GetActionFromMappings();
        if(action == null) return;
        UpdateActionInMappings();
        action = GetActionFromMappings();   // TODO: Update also in edit mode
        action.Enable();
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

    public void UpdateActionInMappings() {
        var actionMap = mappings.FindActionMap(playerMovementActionMapName);
        if (actionMap == null) return;
        if (mappings.FindAction(directMovementActionName) != null) mappings.RemoveAction(directMovementActionName);
        var newAction = actionMap.AddAction(directMovementActionName);
        newAction.expectedControlType = "Vector2";
        foreach (var binding in directMovement.bindings) {
            newAction.AddBinding(binding);
        }
        // TODO: Refactoring: extract method
    }

    private bool HasBinding() {
        if(mappings == null) return false;
        var actionMap = mappings.FindActionMap(playerMovementActionMapName);
        var action = actionMap.FindAction(directMovementActionName);
        var bindings = action.bindings;
        var bindingFound = bindings.Any(b => !string.IsNullOrEmpty(b.path));
        return bindings.Count != 0 && bindingFound;
    }

    public void Test(InputAction.CallbackContext context) {
        Debug.Log(context.ReadValue<Vector2>());
    }
}