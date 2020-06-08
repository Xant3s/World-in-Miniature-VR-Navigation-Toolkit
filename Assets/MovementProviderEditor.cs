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

        // Copy action from input mappings asset to direct movement action. They should always have the same state.
        // Used to allow inline binding editing in inspector.
        movementProvider.directMovement = new InputAction(MovementProvider.playerMovementActionMapName) {
            expectedControlType = "Vector2"
        };
        movementProvider.CopyBindings(movementProvider.GetActionFromMappings(), movementProvider.directMovement);
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
            movementProvider.SetupActionMap((InputActionAsset) e.newValue);
            actionPropertyField.visible = e.newValue != null;
        });

        return root;
    }
}
