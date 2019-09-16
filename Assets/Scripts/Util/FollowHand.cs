using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHand : MonoBehaviour {
    public MiniatureModel.Hand hand;

    private Transform target;

    void Start() {
        if(hand == MiniatureModel.Hand.HAND_L) {
            target = GameObject.FindWithTag("HandL").transform;
        } else if(hand == MiniatureModel.Hand.HAND_R) {
            target = GameObject.FindWithTag("HandR").transform;
        }
    }

    void Update() {
        if(!target) return;
        transform.localPosition = Vector3.zero;
        transform.position = target.position;
    }
}