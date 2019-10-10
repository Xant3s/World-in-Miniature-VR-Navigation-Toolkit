using UnityEngine;

namespace WIM_Plugin {
    public class Billboard : MonoBehaviour {
        private void Update() {
            var mainCamera = Camera.main;
            if(!mainCamera) return;
            transform.LookAt(mainCamera.transform);
            transform.Rotate(Vector3.right, 90);
        }
    }
}