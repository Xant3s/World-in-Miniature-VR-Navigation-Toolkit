// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using WIMVR.Features.Distance_Grab;

namespace WIMVR.Editor.Features {
    [CustomEditor(typeof(DistanceGrabbing))]
    public class DistanceGrabbingEditor : UnityEditor.Editor {
        private static bool initialized;


        private void OnEnable() {
            if(initialized) return;
            initialized = true;
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 0, "Usability");
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw, "Usability");
            initialized = false;
        }
        
        private void Draw(WIMConfiguration config, VisualElement container) {
            var distanceGrabbing = (DistanceGrabbing) target;
            if(!distanceGrabbing) return;
            var distanceGrabbingToggle = new Toggle {
                label="Distance Grabbing (experimental)",
                bindingPath = "distanceGrabbingEnabled",
                tooltip = "Description. Experimental feature." // TODO
            };
            container.Add(distanceGrabbingToggle);
            container.Bind(new SerializedObject(distanceGrabbing));
        }
    }
}