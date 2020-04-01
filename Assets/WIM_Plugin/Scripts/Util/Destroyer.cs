// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine;


namespace WIMVR {
    /// <summary>
    /// Destroys gameobject if there is no destination indicator in the miniature model.
    /// </summary>
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