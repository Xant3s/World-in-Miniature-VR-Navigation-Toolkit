using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour {
    //[SerializeField] [Range(0, 1)] private float progress;
    [Range(0.1f, 10.0f)] public float durationInSeconds = 1.0f;
    private Material mat;
    private bool isInverse;
    private float endTime;

    void Start() {
        mat = GetComponent<Renderer>().material;
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.A)) {
            Play();
        } else if (Input.GetKeyUp(KeyCode.D)) {
            PlayInverse();
        }

        if (Time.realtimeSinceStartup < endTime) {
            var remainingTime = endTime - Time.realtimeSinceStartup;
            var percent = (durationInSeconds - remainingTime) / durationInSeconds;
            var progress = (!isInverse) ? percent : 1 - percent;
            mat.SetFloat("Vector1_461A9E8C", progress);
        }
    }

    public void Play() {
        isInverse = false;
        endTime = Time.realtimeSinceStartup + durationInSeconds;
    }

    public void PlayInverse() {
        isInverse = true;
        endTime = Time.realtimeSinceStartup + durationInSeconds;
    }

    public void SetProgress(float progress) {
        mat.SetFloat("Vector1_461A9E8C", progress);
    }
}