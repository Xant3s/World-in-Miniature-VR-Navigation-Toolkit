using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PostTravelPathTrace : MonoBehaviour {
    public WIMSpaceConverter Converter { get; set; }
    public Vector3 newPositionInWIM { get; set; }
    public Vector3 oldPositionInWIM { get; set; }
    public Transform WIMLevelTransform { get; set; }
    public float TraceDuration { get; set; }

    private LineRenderer lr;
    private float animationProgress;
    private Transform oldTransform;
    private Transform newTransform;


    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    public void Init() {
        lr.widthMultiplier = .001f;
        lr.material = Resources.Load<Material>("Materials/Blue");
        var emptyGO = new GameObject();
        oldTransform = Instantiate(emptyGO, WIMLevelTransform).transform;
        oldTransform.position = oldPositionInWIM;
        newTransform = Instantiate(emptyGO, WIMLevelTransform).transform;
        newTransform.position = newPositionInWIM;
        Destroy(emptyGO);
    }

    void Update() {
        lr.SetPosition(0, oldTransform.position);
        lr.SetPosition(1, newTransform.position);
    }

    void OnDestroy() {
        Destroy(oldTransform.gameObject);
        Destroy(newTransform.gameObject);
    }
}
