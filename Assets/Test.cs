using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public Camera cam;
    public RenderTextureFormat f;

    void Start() {
        //var cam = gameObject.AddComponent<Camera>();
        //cam.depth = -1;
        //cam.targetTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.Default);
        //cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
        //GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        //GetComponent<Renderer>().material.SetTexture("_BaseMap", cam.targetTexture);

        var b = SystemInfo.SupportsBlendingOnRenderTextureFormat(f);
        Debug.Log(b);Debug.Log(SystemInfo.deviceName);
    }

    void Update() {
        //Destroy(cam.targetTexture);
        //cam.targetTexture = new RenderTexture(1600, 900, 0, RenderTextureFormat.Default);
        //GetComponent<Renderer>().material.SetTexture("_BaseMap", cam.targetTexture);
    }
}