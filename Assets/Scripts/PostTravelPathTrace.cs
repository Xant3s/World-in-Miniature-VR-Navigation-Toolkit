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

    // TODO self destruct


    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    public void Init() {
        oldPositionInWIM = WIMLevelTransform.Find("PathTraceOldPosition").transform;
        newPositionInWIM = WIMLevelTransform.Find("PathTraceNewPosition").transform;
        lr.widthMultiplier = .001f;
        //lr.material = Resources.Load<Material>("Materials/Blue");
        lr.material = new Material(Shader.Find("Sprites/Default"));
        endTime = Time.time + TraceDurationInSeconds;
    }

    void Update() {
        if (Time.time >= endTime) {
            Destroy(gameObject);
        }

        lr.SetPosition(0, oldPositionInWIM.position);
        lr.SetPosition(1, newPositionInWIM.position);

        var timeLeft = (endTime - Time.time);
        var timePast = TraceDurationInSeconds - timeLeft;
        var progress = timePast / TraceDurationInSeconds;
        progress = Mathf.Clamp(progress, 0, 1);

        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1-progress, progress), new GradientAlphaKey(1, 1.0f) }
        );
        //gradient.SetKeys(
        //    new GradientColorKey[]{new GradientColorKey(new Color(0,1,0,1), 0), new GradientColorKey(new Color(1,0,0,1), 1)}, 
        //    new GradientColorKey[]{new GradientColorKey(new Color(0,1,0,1), 0), new GradientColorKey(new Color(1,0,0,1), 1)} 
        //    );
        lr.colorGradient = gradient;
    }

    void OnDestroy() {
        Destroy(oldPositionInWIM.gameObject);
        Destroy(newPositionInWIM.gameObject);
    }
}
