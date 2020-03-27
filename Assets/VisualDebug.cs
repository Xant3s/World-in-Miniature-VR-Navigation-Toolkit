using UnityEngine;

public class VisualDebug : MonoBehaviour {

    public void Test() {
        transform.localScale = new Vector3(.5f, 1,1);
    }

    public void Reverse() {
        transform.localScale = new Vector3(2, 1, 1);
    }

    public void Iterative() {
        transform.localScale = new Vector3(transform.localScale.x / 2.0f, 1,1);
    }
}