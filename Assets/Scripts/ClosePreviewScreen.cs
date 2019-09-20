using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ClosePreviewScreen : MonoBehaviour {
    public float DoubleTapInterval { get; set; } = 2;

    private Transform index;
    private bool firstTap = false;

    void Awake() {
        index = GameObject.FindWithTag("IndexR").transform;
        Assert.IsNotNull(index);
    }

    void OnTriggerEnter(Collider other) {
        if(other.transform != index) return;
        if(firstTap) {
            var WIM = GameObject.Find("WIM").GetComponent<MiniatureModel>();
            WIM.RemovePreviewScreen();
        }
        else {
            firstTap = true;
            Invoke("resetDoubleTap", DoubleTapInterval);
        }
    }

    private void resetDoubleTap() {
        firstTap = false;
    }
}