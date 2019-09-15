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
        lr.material = Resources.Load<Material>("Materials/Blue");
        endTime = Time.time + TraceDurationInSeconds;
    }

    void Update() {
        lr.SetPosition(0, oldPositionInWIM.position);
        lr.SetPosition(1, newPositionInWIM.position);


    }

    void OnDestroy() {
        Destroy(oldPositionInWIM.gameObject);
        Destroy(newPositionInWIM.gameObject);
    }
}
