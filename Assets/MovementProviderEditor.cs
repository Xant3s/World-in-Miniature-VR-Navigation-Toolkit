using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


[CustomEditor(typeof(MovementProvider))]
public class MovementProviderEditor : Editor {
    private MovementProvider movementProvider;


    private void OnEnable() {
        movementProvider = (MovementProvider) target;
        if(movementProvider.mappings == null) return;
        movementProvider.directMovement = new InputAction(MovementProvider.playerMovementActionMapName) {
            expectedControlType = "Vector2"
        };
        foreach(var binding in movementProvider.GetActionFromMappings().bindings) {
            movementProvider.directMovement.AddBinding(binding);
        }
    }

    public override VisualElement CreateInspectorGUI() {
        var root = new VisualElement();
        root.Add(new FloatField {
            label = "Movement Speed",
            bindingPath = "movementSpeed"
        });

        root.Add(new LayerMaskField {
            label = "Ground Layer",
            tooltip = "Layer used to determine whether player is on the ground.",
            bindingPath = "groundLayer"
        });

        var mappings = new ObjectField {
            label = "Mappings",
            bindingPath = "mappings",
            allowSceneObjects = false,
            objectType = typeof(InputActionAsset)
        };
        root.Add(mappings);

        var actionPropertyField = new PropertyField {
            label = MovementProvider.directMovementActionName,
            bindingPath = "directMovement",
            visible = movementProvider.mappings != null
        };
        root.Add(actionPropertyField);


        //actionPropertyField.RegisterCallback<FocusOutEvent>(e =>
        //{
        //    //movementProvider.action3 = new InputAction(MovementProvider.playerMovementActionMapName) {
        //    //    expectedControlType = "Vector2"
        //    //};
        //    //foreach(var binding in movementProvider.GetActionFromMappings().bindings) {
        //    //    movementProvider.action3.AddBinding(binding);
        //    //}
        //    //movementProvider.UpdateActionInMappings();
        //    Debug.Log("adsef");
        //});

        //actionPropertyField.RegisterCallback<ChangeEvent<PropertyField>>(e => {
        //    Debug.Log("adsef");
        //});


        mappings.RegisterValueChangedCallback(e => {
            SetupActionMap((InputActionAsset) e.newValue);
            actionPropertyField.visible = e.newValue != null;
        });

        return root;
    }

    private void SetupActionMap(InputActionAsset mappings) {
        if(mappings == null) return;
        var actionMap = mappings.FindActionMap(MovementProvider.playerMovementActionMapName)
                        ?? mappings.AddActionMap(MovementProvider.playerMovementActionMapName);
        var action = actionMap.FindAction(MovementProvider.directMovementActionName)
                     ?? actionMap.AddAction(MovementProvider.directMovementActionName);
        action.expectedControlType = "Vector2";
    }
}
