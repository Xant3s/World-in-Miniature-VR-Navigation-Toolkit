using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    [Range(0,1)]
    public float test;

    private float endTime;
    private float duration = 5;

    void Start() {
        endTime = Time.time + duration;

    }

    void Update() {
        if (Time.time >= endTime) {
            //Destroy(gameObject);
            this.enabled = false;
        }

        var timeLeft = (endTime - Time.time);
        var timePast = duration - timeLeft;
        var progress = timePast / duration;
        progress = Mathf.Clamp(progress, 0, 1);
        Debug.Log(progress);


        var lr = GetComponent<LineRenderer>();
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {new GradientColorKey(Color.green, 0), new GradientColorKey(Color.green, 1.0f)},
            new GradientAlphaKey[] {new GradientAlphaKey(0, 0), new GradientAlphaKey(1, 1)}
        );
        gradient.mode = GradientMode.Fixed;
        lr.colorGradient = gradient;
    }
}