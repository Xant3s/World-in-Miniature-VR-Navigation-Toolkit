// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;


namespace WIM_Plugin {
    [DisallowMultipleComponent]
    public class Destroyer : MonoBehaviour {
        private MiniatureModel WIM;

        private void Start() {
            WIM = GameObject.FindWithTag("WIM").GetComponent<MiniatureModel>();
        }

        private void Update() {
            if(!WIM.Data.DestinationIndicatorInWIM) {
                Destroy(gameObject);
            }
        }
    }
}