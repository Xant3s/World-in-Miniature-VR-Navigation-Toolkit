using System;
using UnityEngine;

namespace WIMVR.Features.Distance_Grab {
    [DisallowMultipleComponent]
    public class DistanceGrabbing : MonoBehaviour {
        
        // TODO add distance grabbing to settings (editor)
        // TODO update hands on enable/disable distance grabbing (in editor? -> runtime)
        // TODO forward input to corresponding hand
        
        // TODO setup when enabled (public bool member)
        
        // TODO setting persistence
        
        public bool DistanceGrabbingEnabled;


        private void Start() {
            Debug.Log(DistanceGrabbingEnabled);
        }

        public void OnDistanceGrabRightHand() {
            Debug.Log("Distance grab right");
        }

        public void OnDistanceGrabLeftHand() {
            Debug.Log("Distance grab left");
        }
    }
}