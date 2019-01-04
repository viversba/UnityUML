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
    private static EditorWindow window = GetWindow<DiagramEngineGUI>();
    //private static DiagramEngineGUI engineGUI = new DiagramEngineGUI();
    private static DiagramEngineGUI engineGUI = (DiagramEngineGUI)ScriptableObject.CreateInstance("DiagramEngineGUI");

    /// <summary>
    /// This is the selected option of the tab system of the left panel
    /// </summary>
    private int selected = 0;
    /// <summary>
    /// This is the current list of entities that is going to be rendered;
    /// </summary>
    private List<BaseModel> selectedEntities;

    EditorGUISplitView horizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
    EditorGUISplitView verticalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Vertical);

    [MenuItem("UML/Open Window")]
    public static void Init() {

        GetInstance().Run();
    }

    private static DiagramEngineGUI GetInstance() {

        return engineGUI;
    }

    private void Run() {

        var window = GetWindow<DiagramEngineGUI>();
        window.position = new Rect(200, 200, 800, 400);
        window.minSize = new Vector2(200, 200);
        selectedEntities = new List<BaseModel>();
        selected = 0;
        LoadAllEntities();
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

        LeftPanel.DrawLeftPanel(ref selected,selectedEntities);
    }

    void DrawRightPanel() {

        RightPanel.DrawRightPanel(ref selected);
    }
}