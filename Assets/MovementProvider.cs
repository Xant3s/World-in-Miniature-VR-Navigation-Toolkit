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
        Debug.Log(context.ReadValue<Vector2>());
    }
}