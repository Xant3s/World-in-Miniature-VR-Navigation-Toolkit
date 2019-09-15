using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    //public RenderTexture rt;

    //void Start() {
    //    rt = new RenderTexture(256, 256, 0, RenderTextureFormat.Default);
    //    //rt.format = RenderTextureFormat.Default;
    //    Debug.Log(rt.format);
    //}

    void Start() {
        var cam = gameObject.AddComponent<Camera>();
        cam.depth = -1;
        cam.targetTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.Default);
        GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        GetComponent<Renderer>().material.SetTexture("_BaseMap", cam.targetTexture);
    }
}