using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FloatAbove : MonoBehaviour {

    public Transform Target = null;
    [SerializeField] private Vector3 offset;

    void Start() {
        Assert.IsNotNull(Target);
    }

    void Update() {
        transform.position = Target.position + offset;
    }
}