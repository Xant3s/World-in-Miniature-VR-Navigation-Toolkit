using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


[CustomEditor(typeof(MovementProvider))]
public class MovementProviderEditor : Editor {
    private MovementProvider movementProvider;


    private void OnEnable() {
        movementProvider = (MovementProvider) target;
    }

    public override VisualElement CreateInspectorGUI() {

        var root = new VisualElement();
        var mappings = new ObjectField {
            label = "Mappings",
            bindingPath = "mappings",
            allowSceneObjects = false,
            objectType = typeof(InputActionAsset)
        };
        root.Add(mappings);

        root.Add(new FloatField {
            label = "Movement Speed",
            bindingPath = "movementSpeed"
        });

        root.Add(new LayerMaskField {
            label = "Ground Layer",
            tooltip = "Layer used to determine whether player is on the ground.",
            bindingPath = "groundLayer"
        });

        //Action action = () => {

        //}

        // TODO: draw binding (action)
        root.Add(base.CreateInspectorGUI());

        mappings.RegisterValueChangedCallback(e => SetupActionMap((InputActionAsset) e.newValue));
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
