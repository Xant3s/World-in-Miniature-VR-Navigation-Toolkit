using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDebug : MonoBehaviour {

    public void Test() {
        transform.localScale = new Vector3(.5f, 1,1);
    }

    public void Reverse() {
        transform.localScale = new Vector3(2, 1, 1);
    }
}