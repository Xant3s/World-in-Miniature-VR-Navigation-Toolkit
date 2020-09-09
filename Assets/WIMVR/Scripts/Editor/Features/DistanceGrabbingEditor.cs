using UnityEditor;
using UnityEngine;
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
            Debug.Log("asdf");
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 0, "Usability");
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw, "Usability");
            initialized = false;
        }
        
        private void Draw(WIMConfiguration config, VisualElement container) {
            var distanceGrabbing = (DistanceGrabbing) target;
            if(!distanceGrabbing) return;
            // var WIM = liveUpdate.GetComponent<MiniatureModel>();
            var distanceGrabbingToggle = new Toggle {
                label="Distance Grabbing (experimental)",
                bindingPath = "DistanceGrabbingEnabled",
                tooltip = "Description. Experimental feature." // TODO
            };
            // distanceGrabbingToggle.RegisterValueChangedCallback(e =>
            //     distanceGrabbingToggle.schedule.Execute(() => LiveUpdate.UpdateAutoGenerateWIM(WIM))); // Delay so that newValue is set on execution.
            container.Add(distanceGrabbingToggle);
        }
    }
}