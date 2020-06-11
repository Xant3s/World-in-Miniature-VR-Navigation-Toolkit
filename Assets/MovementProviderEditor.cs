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


        mappings.RegisterValueChangedCallback(e
            => movementProvider.SetupActionMap((InputActionAsset) e.newValue));

        return root;
    }
}
