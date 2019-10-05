using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {
    private MiniatureModel WIM;
    
    void Start() {
        WIM = GameObject.Find("WIM").GetComponent<MiniatureModel>();
    }

    void Update() {
        if(!WIM.DestinationIndicatorInWIM) {
            Destroy(gameObject);
        }
    }
}