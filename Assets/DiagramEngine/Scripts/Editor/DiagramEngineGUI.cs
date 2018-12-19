using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DiagramEngineGUI : EditorWindow {

    /// <summary>
    /// Current instance of the window;
    /// </summary>
    private static EditorWindow window = GetWindow<DiagramEngineGUI>();

    EditorGUISplitView horizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
    EditorGUISplitView verticalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Vertical);

    [MenuItem("UML/Open Window")]
    public static void Init() {
        var window = GetWindow<DiagramEngineGUI>();
        window.position = new Rect(200,200,800,400);
        window.minSize = new Vector2(200,200);
    }

    public void OnGUI() {
        horizontalSplitView.BeginSplitView();
        DrawView1();
        horizontalSplitView.Split();
        //verticalSplitView.BeginSplitView();
        DrawView2();
        //verticalSplitView.Split();
        //DrawView2();
        //verticalSplitView.EndSplitView();
        horizontalSplitView.EndSplitView();
        Repaint();
    }

    void DrawView1() {
        EditorGUILayout.LabelField("A label");
        GUILayout.Button("A Button");
        EditorGUILayout.Foldout(false, "A Foldout");
    }

    void DrawView2() {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Centered text");
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}