// Author: Samuel Truman (contact@samueltruman.com)

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class HandSetupWindow : EditorWindow {
    private VisualElement root;
    private GameObject leftHandPrefab;
    private GameObject rightHandPrefab;
    
    
    [MenuItem("Window/wimVR/Hand Setup")]
    public static void ShowExample() {
        var window = GetWindow<HandSetupWindow>();
        window.titleContent = new GUIContent("wimVR Hand Setup");
    }

    public void OnEnable() {
        root = rootVisualElement;
        LoadHierarchy();
        LoadStyle();
    }

    private void LoadHierarchy() {
        var visualTree = Resources.Load<VisualTreeAsset>("HandSetupWindow");
        visualTree.CloneTree(root);
    }

    private void LoadStyle() {
        var style = Resources.Load<StyleSheet>("HandSetupWindowStyle");
        root.styleSheets.Add(style);
    }
}