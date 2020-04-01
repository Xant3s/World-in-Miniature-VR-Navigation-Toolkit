// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;

namespace WIMVR {
    /// <summary>
    /// Gameobject will always face the main camera.
    /// </summary>
    [DisallowMultipleComponent]
    public class Billboard : MonoBehaviour {
        private void Update() {
            var mainCamera = Camera.main;
            if(!mainCamera) return;
            transform.LookAt(mainCamera.transform);
            transform.Rotate(Vector3.right, 90);
        }
    }
}