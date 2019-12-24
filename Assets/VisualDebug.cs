using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDebug : MonoBehaviour {

    public void SetColor(Color color) {
        GetComponent<Renderer>().material.color = color;
    }

    public void Test() {
        transform.localScale = new Vector3(.5f, 1,1);
    }

    void Start() {
        //GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }
}