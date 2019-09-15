using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    void Start() {
        var lr = GetComponent<LineRenderer>();
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {new GradientColorKey(Color.green, 0), new GradientColorKey(Color.red, 1.0f)},
            new GradientAlphaKey[] {new GradientAlphaKey(.5f, 0), new GradientAlphaKey(1, 1.0f)}
        );
        lr.colorGradient = gradient;
    }
}