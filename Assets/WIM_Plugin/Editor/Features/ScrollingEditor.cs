﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;


namespace WIM_Plugin {
    [CustomEditor(typeof(Scrolling))]
    public class ScrollingEditor : Editor {

        private MiniatureModel WIM;

        private void OnEnable() {
            MiniatureModelEditor.OnDraw.AddCallback(Draw, 4);
            WIM = ((Scrolling) target).GetComponent<MiniatureModel>();
            Assert.IsNotNull(WIM);
        }

        private void OnDisable() {
            MiniatureModelEditor.OnDraw.RemoveCallback(Draw);   
        }

        private void Draw(WIMConfiguration WIMConfig) {
            MiniatureModelEditor.Separator("Scrolling");
            if(!target) return;
            ref var config = ref ((Scrolling) target).ScrollingConfig;
            if(!config) {
                EditorGUILayout.HelpBox("Scrolling configuration missing. Create a scrolling configuration asset and add it to the scrolling script.", MessageType.Error);
                EditorGUI.BeginChangeCheck();
                config = (ScrollingConfiguration)EditorGUILayout.ObjectField("Configuration", config, typeof(ScrollingConfiguration), false);
                if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);
                return;
            }

            EditorGUI.BeginChangeCheck();
            config.AllowWIMScrolling = EditorGUILayout.Toggle("Allow WIM Scrolling", config.AllowWIMScrolling);
            if (EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);

            if (config.AllowWIMScrolling) {
                EditorGUI.BeginChangeCheck();
                WIMConfig.ActiveAreaBounds =
                    EditorGUILayout.Vector3Field("Active Area Bounds", WIMConfig.ActiveAreaBounds);
                if (EditorGUI.EndChangeCheck()) WIMGenerator.UpdateScrollingMask(WIM);
                config.AutoScroll = EditorGUILayout.Toggle("Auto Scroll", config.AutoScroll);
                if (config.AutoScroll) {
                    config.ScrollSpeed = EditorGUILayout.FloatField("Scroll Speed", config.ScrollSpeed);
                }

                if (!config.AutoScroll) {
                    config.AllowVerticalScrolling = EditorGUILayout.Toggle("Allow Vertical Scrolling",
                        config.AllowVerticalScrolling);
                }
                else {
                    config.AllowVerticalScrolling = false;
                }
            }
            EditorUtility.SetDirty(config);
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var scrolling = (Scrolling) target;
            EditorGUI.BeginChangeCheck();
            scrolling.ScrollingConfig = (ScrollingConfiguration) EditorGUILayout.ObjectField("Config", scrolling.ScrollingConfig, typeof(ScrollingConfiguration), false);
            if(EditorGUI.EndChangeCheck()) {
                WIMGenerator.ConfigureWIM(WIM);
                scrolling.remove();
                if(scrolling.ScrollingConfig) {
                    scrolling.setup();
                }
            }
            if(!scrolling.ScrollingConfig)
                EditorGUILayout.HelpBox("Scrolling configuration missing. Create a scrolling configuration asset and add it to the scrolling script.", MessageType.Error);

        }
    }
}