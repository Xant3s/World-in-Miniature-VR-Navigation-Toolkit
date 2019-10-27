using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    [CustomEditor(typeof(OcclusionHandling))]
    public class OcclusionHandlingEditor : Editor {
        private MiniatureModel WIM;

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 0, "Occlusion Handling");
            WIM = ((OcclusionHandling) target).GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw, "Occlusion Handling");
        }

        void Draw(WIMConfiguration config) {
            EditorGUI.BeginChangeCheck();
            config.OcclusionHandlingMethod = (OcclusionHandlingMethod) EditorGUILayout.EnumPopup(
                new GUIContent("Occlusion Handling Method",
                    "Select occlusion handling strategy. Disable for scrolling."),
                config.OcclusionHandlingMethod);
            if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
            if(config.OcclusionHandlingMethod == OcclusionHandlingMethod.MeltWalls) {
                config.MeltRadius = EditorGUILayout.FloatField("Melt Radius", config.MeltRadius);
                config.MeltHeight = EditorGUILayout.FloatField("Melt Height", config.MeltHeight);
            }
            else if(config.OcclusionHandlingMethod == OcclusionHandlingMethod.CutoutView) {
                config.CutoutRange =
                    EditorGUILayout.FloatField("Cutout Range", config.CutoutRange);
                config.CutoutAngle =
                    EditorGUILayout.FloatField("Cutout Angle", config.CutoutAngle);
                config.ShowCutoutLight =
                    EditorGUILayout.Toggle("Show Cutout Light", config.ShowCutoutLight);
                if(config.ShowCutoutLight) {
                    config.CutoutLightColor =
                        EditorGUILayout.ColorField("Cutout Light Color", config.CutoutLightColor);
                }
            }
        }

        public override void OnInspectorGUI() {
            if(Application.isPlaying) return;
            WIMGenerator.UpdateCylinderMask(WIM);
            WIMGenerator.UpdateCutoutViewMask(WIM);
        }
    }
}