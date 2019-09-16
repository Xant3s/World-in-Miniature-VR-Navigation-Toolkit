using UnityEngine;

public class Billboard : MonoBehaviour {
    void Update() {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(Vector3.right, 90);
    }
}