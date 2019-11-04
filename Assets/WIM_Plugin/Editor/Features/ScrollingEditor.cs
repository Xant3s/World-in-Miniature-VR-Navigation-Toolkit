using System;
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

        private void Draw(WIMConfiguration config) {
            MiniatureModelEditor.Separator("Scrolling");
            //var scrollingConfig = ((Scrolling)target).ScrollingConfig;
            var s = (Scrolling) target;
            if(!s.ScrollingConfig) {
                EditorGUILayout.HelpBox("Scrolling configuration missing. Create a scrolling configuration asset and add it to the scrolling script.", MessageType.Error);
                ((Scrolling)target).ScrollingConfig = (ScrollingConfiguration) EditorGUILayout.ObjectField("Configuration", ((Scrolling)target).ScrollingConfig, typeof(ScrollingConfiguration), false);
                return;
            }
            if(config.OcclusionHandlingMethod != OcclusionHandlingMethod.None) {
                EditorGUILayout.LabelField("Disable occlusion handling method to use scrolling.");
            }
            else {
                EditorGUI.BeginChangeCheck();
                s.ScrollingConfig.AllowWIMScrolling =
                    EditorGUILayout.Toggle("Allow WIM Scrolling", s.ScrollingConfig.AllowWIMScrolling);
                if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);

                if(s.ScrollingConfig.AllowWIMScrolling) {
                    EditorGUI.BeginChangeCheck();
                    config.ActiveAreaBounds =
                        EditorGUILayout.Vector3Field("Active Area Bounds", config.ActiveAreaBounds);
                    if(EditorGUI.EndChangeCheck()) WIMGenerator.UpdateScrollingMask(WIM);
                    s.ScrollingConfig.AutoScroll = EditorGUILayout.Toggle("Auto Scroll", s.ScrollingConfig.AutoScroll);
                    if(s.ScrollingConfig.AutoScroll) {
                        s.ScrollingConfig.ScrollSpeed =
                            EditorGUILayout.FloatField("Scroll Speed", s.ScrollingConfig.ScrollSpeed);
                    }

                    if(!s.ScrollingConfig.AutoScroll) {
                        s.ScrollingConfig.AllowVerticalScrolling = EditorGUILayout.Toggle("Allow Vertical Scrolling",
                            s.ScrollingConfig.AllowVerticalScrolling);
                    }
                    else {
                        s.ScrollingConfig.AllowVerticalScrolling = false;
                    }

                    if(s.ScrollingConfig.AllowVerticalScrolling) {
                        s.ScrollingConfig.VerticalScrollingAxis =
                            (OVRInput.RawAxis2D) EditorGUILayout.EnumFlagsField("Vertical Scrolling Axis",
                                s.ScrollingConfig.VerticalScrollingAxis);
                    }
                }
            }
        }
    }
}