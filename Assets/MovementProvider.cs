using System.Collections;
using System.Collections.Generic;
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

    public InputAction action;



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
        var action = GetAction();
        if(action == null) return;
        action.Enable();
    }

    private void OnDisable() {
        var action = GetAction();
        if(action == null) return;
        action.Disable();
    }

    private void NotifyIfNoBinding() {
        if(!HasBinding()) Debug.Log($"No binding for {directMovementActionName}.");
    }

    private void BindAction() {
        if(!HasBinding()) return;
        var action = GetAction();
        action.performed += Test;
    }

    public InputAction GetAction() {
        var actionMap = mappings.FindActionMap(playerMovementActionMapName);
        var action = actionMap.FindAction(directMovementActionName);
        return action;
    }

    private bool HasBinding() {
        if(mappings == null) return false;
        var actionMap = mappings.FindActionMap(playerMovementActionMapName);
        var action = actionMap.FindAction(directMovementActionName);
        var bindings = action.bindings;
        var bindingFound = bindings.Any(b => !string.IsNullOrEmpty(b.path));
        return bindings.Count != 0 && bindingFound;
    }

    //public void OnDirectMovement(InputValue value) {
    //    if(!Application.isPlaying) return;
    //    Debug.Log(value.Get<Vector2>());
    //}

    public void Test(InputAction.CallbackContext context) {
        Debug.Log(context.ReadValue<Vector2>());
    }
}