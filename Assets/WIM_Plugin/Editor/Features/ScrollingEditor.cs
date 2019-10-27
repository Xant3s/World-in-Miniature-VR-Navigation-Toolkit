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
            if(config.OcclusionHandlingMethod != OcclusionHandlingMethod.None) {
                EditorGUILayout.LabelField("Disable occlusion handling method to use scrolling.");
            }
            else {
                EditorGUI.BeginChangeCheck();
                config.AllowWIMScrolling =
                    EditorGUILayout.Toggle("Allow WIM Scrolling", config.AllowWIMScrolling);
                if(EditorGUI.EndChangeCheck()) WIMGenerator.ConfigureWIM(WIM);

                if(config.AllowWIMScrolling) {
                    EditorGUI.BeginChangeCheck();
                    config.ActiveAreaBounds =
                        EditorGUILayout.Vector3Field("Active Area Bounds", config.ActiveAreaBounds);
                    if(EditorGUI.EndChangeCheck()) WIMGenerator.UpdateScrollingMask(WIM);
                    config.AutoScroll = EditorGUILayout.Toggle("Auto Scroll", config.AutoScroll);
                    if(config.AutoScroll) {
                        config.ScrollSpeed =
                            EditorGUILayout.FloatField("Scroll Speed", config.ScrollSpeed);
                    }

                    if(!config.AutoScroll) {
                        config.AllowVerticalScrolling = EditorGUILayout.Toggle("Allow Vertical Scrolling",
                            config.AllowVerticalScrolling);
                    }
                    else {
                        config.AllowVerticalScrolling = false;
                    }

                    if(config.AllowVerticalScrolling) {
                        config.VerticalScrollingAxis =
                            (OVRInput.RawAxis2D) EditorGUILayout.EnumFlagsField("Vertical Scrolling Axis",
                                config.VerticalScrollingAxis);
                    }
                }
            }
        }
    }
}