using UnityEngine;

namespace WIM_Plugin {
    public class Billboard : MonoBehaviour {
        void Update() {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(Vector3.right, 90);
        }
    }
}