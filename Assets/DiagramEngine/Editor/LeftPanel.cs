using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.
using UnityEditor;
using DEngine.Model;
using System;
using System.IO;

public class LeftPanel : EditorWindow{

    /// <summary>
    /// This is the current list of entities that is going to be rendered;
    /// </summary>
    private List<BaseModel> selectedEntities = new List<BaseModel>();
    /// <summary>
    /// Scroll position of the list of entities to render
    /// </summary>
    private Vector2 scrollPos = new Vector2();
    /// <summary>
    /// Selected option of file selection (All, Drag & Drop, Checklist)
    /// </summary>
    private int selected = 0;

    public bool DrawLeftPanel() {

        bool generateDiagramButton = false;
        string[] options = { "All!", "Drag & Drop", "CheckBox" };
        selected = GUILayout.Toolbar(selected, options, GUILayout.MinWidth(240));

        switch (selected) {

            case 0:
                GUILayout.Label("Found Classe/Interfaces: ", EditorStyles.boldLabel);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                foreach (var entity in selectedEntities) {
                    GUILayout.Label(entity.GetName());
                }
                EditorGUILayout.EndScrollView();
                generateDiagramButton = GUILayout.Button("Generate!");
                break;
            case 1:
                GUILayout.Label("Coming Soon!");
                break;
            case 2:
                GUILayout.Label("Coming Soon!");
                break;
            default:
                break;
        }

        return generateDiagramButton;
    }

    private void Awake() {

        selected = 0;
        selectedEntities = new List<BaseModel>();
        LoadAllEntities();
    }

    private void LoadAllEntities() {

        switch (selected) {
            case 0:
                foreach (string file in SearchAllFilesInDirectory("./Assets/")) {
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
            foreach (string file in Directory.GetFiles(directory, "*.cs")) {
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
}
