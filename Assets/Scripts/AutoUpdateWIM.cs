using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoUpdateWIM : MonoBehaviour {
    public MiniatureModel WIM { get; set; }

    private static bool alreadyUpdatedThisFrame = false;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 localScale;
    private int childCount;

    void Start() {
        // Add to children (recursive).
        foreach(Transform child in transform) {
            if(!child.GetComponent<AutoUpdateWIM>())
                child.gameObject.AddComponent<AutoUpdateWIM>();
        }

#if UNITY_EDITOR
        updateValues();
#endif
    }


#if UNITY_EDITOR
    void Update() {
        var somethingChanged = getIsChanged();
        if(!somethingChanged || alreadyUpdatedThisFrame || !WIM || !WIM.AutoGenerateWIM) return;
        updateValues();
        triggerWIMUpdate();
    }

    void LateUpdate() {
        alreadyUpdatedThisFrame = false;
    }
#endif

    void OnDestroy() {
        // Remove from children (recursive).
        foreach(Transform child in transform) {
            DestroyImmediate(child.GetComponent<AutoUpdateWIM>());
        }
    }

    private bool getIsChanged() {
        var somethingChanged = position != transform.position ||
                               rotation != transform.rotation ||
                               localScale != transform.localScale ||
                               childCount != transform.childCount;
        return somethingChanged;
    }

    private void updateValues() {
        position = transform.position;
        rotation = transform.rotation;
        localScale = transform.localScale;
        childCount = transform.childCount;
    }

    private void triggerWIMUpdate() {
        alreadyUpdatedThisFrame = true;
        WIM.generateNewWIM();
    }
}