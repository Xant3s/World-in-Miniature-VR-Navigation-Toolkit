﻿using UnityEngine;

namespace WIM_Plugin {
    [ExecuteInEditMode]
    public class AutoUpdateWIM : MonoBehaviour {
        public MiniatureModel WIM {
            get => wim;
            set {
                wim = value;
                if(wim && !wim.Configuration) {
                    enabled = false;
                }
            }
        }

        private MiniatureModel wim;
        private static bool alreadyUpdatedThisFrame;
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 localScale;
        private int childCount;

        private void Start() {
            // Add to children (recursive).
            foreach(Transform child in transform) {
                if(!child.GetComponent<AutoUpdateWIM>())
                    child.gameObject.AddComponent<AutoUpdateWIM>();
            }

#if UNITY_EDITOR
            UpdateValues();
#endif
        }


#if UNITY_EDITOR
        private void Update() {
            var somethingChanged = GetIsChanged();
            if(!WIM) WIM = GameObject.FindWithTag("WIM").GetComponent<MiniatureModel>();
            if (!somethingChanged || alreadyUpdatedThisFrame || !WIM || !WIM.Configuration.AutoGenerateWIM) return;
            UpdateValues();
            TriggerWIMUpdate();
        }

        private void LateUpdate() {
            alreadyUpdatedThisFrame = false;
        }
#endif

        private void OnDestroy() {
            // Remove from children (recursive).
            foreach(Transform child in transform) {
                DestroyImmediate(child.GetComponent<AutoUpdateWIM>());
            }
        }

        private bool GetIsChanged() {
            var somethingChanged = position != transform.position ||
                                   rotation != transform.rotation ||
                                   localScale != transform.localScale ||
                                   childCount != transform.childCount;
            return somethingChanged;
        }

        private void UpdateValues() {
            position = transform.position;
            rotation = transform.rotation;
            localScale = transform.localScale;
            childCount = transform.childCount;
        }

        private void TriggerWIMUpdate() {
            alreadyUpdatedThisFrame = true;
            WIMGenerator.GenerateNewWIM(WIM);
        }
    }
}