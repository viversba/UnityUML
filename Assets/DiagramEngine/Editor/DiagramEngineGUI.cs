using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEngine.Model;
using System;

public class DiagramEngineGUI : EditorWindow {

    /// <summary>
    /// Current instance of the window;
    /// </summary>
    private static EditorWindow window;
    /// <summary>
    /// Instance of itself
    /// </summary>
    //private static DiagramEngineGUI engineGUI = (DiagramEngineGUI)ScriptableObject.CreateInstance("DiagramEngineGUI");
    private static DiagramEngineGUI engineGUI;
    /// <summary>
    /// This is the current list of entities that is going to be rendered;
    /// </summary>
    private List<BaseModel> selectedEntities = new List<BaseModel>();
    /// <summary>
    /// Reference to the right panel;
    /// </summary>
    private LeftPanel leftPanel;
    /// <summary>
    /// Reference to the right panel
    /// </summary>
    private RightPanel rightPanel;

    EditorGUISplitView horizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
    EditorGUISplitView verticalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Vertical);

    [MenuItem("UML/Open Window")]
    public static void Init() {

        GetInstance().Run();
    }

    private static DiagramEngineGUI GetInstance() {

        DiagramEngineGUI engine = engineGUI == null ? (DiagramEngineGUI)CreateInstance("DiagramEngineGUI") : engineGUI;
        return engine;
    }

    private void Awake() {
        //engineGUI = (DiagramEngineGUI)CreateInstance(typeof(DiagramEngineGUI));
    }

    private void Run() {

        var window = GetWindow<DiagramEngineGUI>();
        window.position = new Rect(200, 200, 800, 400);
        window.minSize = new Vector2(200, 200);

        leftPanel = leftPanel ?? (LeftPanel)ScriptableObject.CreateInstance("LeftPanel");
        rightPanel = rightPanel ?? (RightPanel)ScriptableObject.CreateInstance("RightPanel");
    }

    public void OnGUI() {
        horizontalSplitView.BeginSplitView();
        leftPanel.DrawLeftPanel();
        horizontalSplitView.Split();
        //verticalSplitView.BeginSplitView();
        //DrawRightPanel();
        //verticalSplitView.Split();
        float begginingOfRightPanel = horizontalSplitView.splitNormalizedPosition * position.width;
        rightPanel.DrawRightPanel(maxSize, begginingOfRightPanel);
        //verticalSplitView.EndSplitView();
        horizontalSplitView.EndSplitView();
        Repaint();
    }
}