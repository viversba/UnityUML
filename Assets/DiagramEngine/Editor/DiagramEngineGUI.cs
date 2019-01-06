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
    //private static DiagramEngineGUI engineGUI = new DiagramEngineGUI();
    private static DiagramEngineGUI engineGUI = (DiagramEngineGUI)ScriptableObject.CreateInstance("DiagramEngineGUI");

    /// <summary>
    /// This is the selected option of the tab system of the left panel
    /// </summary>
    private int selected = 0;
    /// <summary>
    /// This is the current list of entities that is going to be rendered;
    /// </summary>
    private List<BaseModel> selectedEntities = new List<BaseModel>();
    /// <summary>
    /// Position of the left panel scroll
    /// </summary>
    private Vector2 scrollPos = new Vector2();

    EditorGUISplitView horizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
    EditorGUISplitView verticalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Vertical);

    [MenuItem("UML/Open Window")]
    public static void Init() {

        GetInstance().Run();
    }

    private static DiagramEngineGUI GetInstance() {

        return engineGUI;
    }

    private void Awake() {

        selected = 0;
        selectedEntities = new List<BaseModel>();
        LoadAllEntities();
        RightPanel.Init();
    }

    private void Run() {

        var window = GetWindow<DiagramEngineGUI>();
        window.position = new Rect(200, 200, 800, 400);
        window.minSize = new Vector2(200, 200);
    }

    public void OnGUI() {
        horizontalSplitView.BeginSplitView();
        DrawLeftPanel();
        horizontalSplitView.Split();
        //verticalSplitView.BeginSplitView();
        //DrawRightPanel();
        //verticalSplitView.Split();
        DrawRightPanel();
        //verticalSplitView.EndSplitView();
        horizontalSplitView.EndSplitView();
        Repaint();
    }

    private void LoadAllEntities() {

        switch (selected) {
            case 0:
                foreach(string file in SearchAllFilesInDirectory("./Assets/")) {
                    var list = EntityWrapper.GetEntitiesFromFile(file);
                    selectedEntities.AddRange(EntityWrapper.GetEntitiesFromFile(file));
                }
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    }

    private List<string> SearchAllFilesInDirectory(string directory) {

        List<string> files = new List<string>();
        try {
            foreach (string file in Directory.GetFiles(directory,"*.cs")) {
                files.Add(file);
            }
            foreach (string dir in Directory.GetDirectories(directory)) {
                files.AddRange(SearchAllFilesInDirectory(dir));
            }
        }
        catch (Exception e) {
            Debug.Log("Error (DiagramEngine): " + e);
        }
        return files;
    }

    void DrawLeftPanel() {

        bool generateDiagram;
        if(selectedEntities.Count == 0) {
            generateDiagram = LeftPanel.DrawLeftPanel(ref selected, new List<BaseModel>(), ref scrollPos);
            return;
        }
        generateDiagram = LeftPanel.DrawLeftPanel(ref selected,selectedEntities, ref scrollPos);
    }

    void DrawRightPanel() {

        float begginingOfRightPanel = horizontalSplitView.splitNormalizedPosition * position.width;
        RightPanel.DrawRightPanel(ref selected, maxSize, begginingOfRightPanel);
    }

    private void OnDestroy() {

        scrollPos = new Vector2();
    }
}