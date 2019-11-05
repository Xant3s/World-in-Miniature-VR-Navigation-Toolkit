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
            var occlusionHandling = (OcclusionHandling) target;
            if(!occlusionHandling.OcclusionHandlingConfig) {
                EditorGUILayout.HelpBox("Occlusion handling configuration missing. Create an occlusion handling configuration asset and add it to the OcclusionHandling script.", MessageType.Error);
                occlusionHandling.OcclusionHandlingConfig = (OcclusionHandlingConfiguration) EditorGUILayout.ObjectField("Configuration", occlusionHandling.OcclusionHandlingConfig, typeof(OcclusionHandlingConfiguration), false);
                return;
            }
            EditorGUI.BeginChangeCheck();
            occlusionHandling.OcclusionHandlingConfig.OcclusionHandlingMethod = (OcclusionHandlingMethod) EditorGUILayout.EnumPopup(
                new GUIContent("Occlusion Handling Method",
                    "Select occlusion handling strategy. Disable for scrolling."),
                occlusionHandling.OcclusionHandlingConfig.OcclusionHandlingMethod);
            if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
            if(occlusionHandling.OcclusionHandlingConfig.OcclusionHandlingMethod == OcclusionHandlingMethod.MeltWalls) {
                occlusionHandling.OcclusionHandlingConfig.MeltRadius = EditorGUILayout.FloatField("Melt Radius", occlusionHandling.OcclusionHandlingConfig.MeltRadius);
                occlusionHandling.OcclusionHandlingConfig.MeltHeight = EditorGUILayout.FloatField("Melt Height", occlusionHandling.OcclusionHandlingConfig.MeltHeight);
            }
            else if(occlusionHandling.OcclusionHandlingConfig.OcclusionHandlingMethod == OcclusionHandlingMethod.CutoutView) {
                occlusionHandling.OcclusionHandlingConfig.CutoutRange =
                    EditorGUILayout.FloatField("Cutout Range", occlusionHandling.OcclusionHandlingConfig.CutoutRange);
                occlusionHandling.OcclusionHandlingConfig.CutoutAngle =
                    EditorGUILayout.FloatField("Cutout Angle", occlusionHandling.OcclusionHandlingConfig.CutoutAngle);
                occlusionHandling.OcclusionHandlingConfig.ShowCutoutLight =
                    EditorGUILayout.Toggle("Show Cutout Light", occlusionHandling.OcclusionHandlingConfig.ShowCutoutLight);
                if(occlusionHandling.OcclusionHandlingConfig.ShowCutoutLight) {
                    occlusionHandling.OcclusionHandlingConfig.CutoutLightColor =
                        EditorGUILayout.ColorField("Cutout Light Color", occlusionHandling.OcclusionHandlingConfig.CutoutLightColor);
                }
            }
        }

        public override void OnInspectorGUI() {
            if(Application.isPlaying) return;
            WIMGenerator.UpdateCylinderMask(WIM);
            WIMGenerator.UpdateCutoutViewMask(WIM);
            DrawDefaultInspector();
        }
    }
}