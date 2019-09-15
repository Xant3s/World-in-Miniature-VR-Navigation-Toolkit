using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PostTravelPathTrace : MonoBehaviour {
    public WIMSpaceConverter Converter { get; set; }
    public Transform newPositionInWIM { get; set; }
    public Transform oldPositionInWIM { get; set; }
    public Transform WIMLevelTransform { get; set; }
    public float TraceDurationInSeconds { get; set; }

    private LineRenderer lr;
    private float animationProgress;
    private float endTime;


    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    public void Init() {
        oldPositionInWIM = WIMLevelTransform.Find("PathTraceOldPosition").transform;
        newPositionInWIM = WIMLevelTransform.Find("PathTraceNewPosition").transform;
        lr.widthMultiplier = .001f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {new GradientColorKey(Color.green, 0), new GradientColorKey(Color.green, 1.0f)},
            new GradientAlphaKey[] {new GradientAlphaKey(0, 0), new GradientAlphaKey(1, 1)}
        );
        lr.colorGradient = gradient;
        endTime = Time.time + TraceDurationInSeconds;
    }

    void Update() {
        if (Time.time >= endTime) {
            Destroy(gameObject);
        }

        var timeLeft = (endTime - Time.time);
        var timePast = TraceDurationInSeconds - timeLeft;
        var progress = timePast / TraceDurationInSeconds;
        progress = Mathf.Clamp(progress, 0, 1);

        var dir = newPositionInWIM.position - oldPositionInWIM.position;
        var currPos = oldPositionInWIM.position + dir * progress;
        lr.SetPosition(0, currPos);
        lr.SetPosition(1, newPositionInWIM.position);
    }

    void OnDestroy() {
        Destroy(oldPositionInWIM.gameObject);
        Destroy(newPositionInWIM.gameObject);
    }
}