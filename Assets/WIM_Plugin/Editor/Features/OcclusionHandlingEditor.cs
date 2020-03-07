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

        void Draw(WIMConfiguration WIMConfig) {
            if(!target) return;
            ref var config = ref ((OcclusionHandling) target).Config;
            if(!config) {
                EditorGUILayout.HelpBox("Occlusion handling configuration missing. Create an occlusion handling configuration asset and add it to the OcclusionHandling script.", MessageType.Error);
                EditorGUI.BeginChangeCheck();
                config = (OcclusionHandlingConfiguration) EditorGUILayout.ObjectField("Configuration", config, typeof(OcclusionHandlingConfiguration), false);
                if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
                return;
            }
            EditorGUI.BeginChangeCheck();
            config.OcclusionHandlingMethod = (OcclusionHandlingMethod) EditorGUILayout.EnumPopup(
                new GUIContent("Occlusion Handling Method",
                    "Select occlusion handling strategy."),
                config.OcclusionHandlingMethod);
            if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
            if(config.OcclusionHandlingMethod == OcclusionHandlingMethod.MeltWalls) {
                config.MeltRadius = EditorGUILayout.FloatField("Melt Radius", config.MeltRadius);
                config.MeltHeight = EditorGUILayout.FloatField("Melt Height", config.MeltHeight);
            }
            else if(config.OcclusionHandlingMethod == OcclusionHandlingMethod.CutoutView) {
                config.CutoutRange = EditorGUILayout.FloatField("Cutout Range", config.CutoutRange);
                config.CutoutAngle = EditorGUILayout.FloatField("Cutout Angle", config.CutoutAngle);
                config.ShowCutoutLight = EditorGUILayout.Toggle("Show Cutout Light", config.ShowCutoutLight);
                if(config.ShowCutoutLight) {
                    config.CutoutLightColor = EditorGUILayout.ColorField("Cutout Light Color", config.CutoutLightColor);
                }
            }
            EditorUtility.SetDirty(config);
        }

        public override void OnInspectorGUI() {
            if(Application.isPlaying) return;
            var occlusionHandling = (OcclusionHandling)target;
            occlusionHandling.UpdateCutoutViewMask(WIM);
            occlusionHandling.UpdateCylinderMask(WIM);
            DrawDefaultInspector();
            EditorGUI.BeginChangeCheck();
            occlusionHandling.Config = (OcclusionHandlingConfiguration) EditorGUILayout.ObjectField("Config", occlusionHandling.Config, typeof(OcclusionHandlingConfiguration), false);
            if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
            ref var config = ref ((OcclusionHandling) target).Config;
            if(!config)           
                EditorGUILayout.HelpBox("Occlusion handling configuration missing. Create an occlusion handling configuration asset and add it to the OcclusionHandling script.", MessageType.Error);
        }
    }
}