// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using WIMVR.VR;


namespace WIMVR.Editor.VR {
    [CustomEditor(typeof(MovementProvider))]
    public class MovementProviderEditor : UnityEditor.Editor {
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

            root.Add(new FloatField {
                label = "Additional Head Height",
                bindingPath = "additionalHeadHeight"
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

            root.Add(new TextElement() {
                text =
                    $"Modify {MovementProvider.directMovementActionName} in {MovementProvider.playerMovementActionMapName} map to change bindings."
            });

            mappings.RegisterValueChangedCallback(e
                => movementProvider.SetupActionMap((InputActionAsset) e.newValue));

            return root;
        }
    }
}