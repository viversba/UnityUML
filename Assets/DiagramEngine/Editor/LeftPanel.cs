﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEngine.Model;
using DEngine.Controller;
using DEngine;
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
    /// <summary>
    /// Reload texture path
    /// </summary>
    string texturePath = "./Assets/DiagramEngine/Textures/reload.png";
    /// <summary>
    /// The reload texture.
    /// </summary>
    Texture2D reloadTexture;


    /// <summary>
    /// Draws the left panel.
    /// </summary>
    /// <returns><c>true</c>, if the GenerateDiagram button was pressed, <c>false</c> otherwise.</returns>
    public bool DrawLeftPanel() {

        bool generateDiagramButton = false, reloadEntities = false;
        string[] options = { "All!", "Drag & Drop", "CheckBox" };
        selected = GUILayout.Toolbar(selected, options, GUILayout.MinWidth(240));

        switch (selected) {

            case 0:
               
                // Render entities only if there are
                if(selectedEntities.Count == 0) {
                    GUILayout.Label("There's nothing to render here :(");
                    GUILayout.Label("Add some scripts and reopen the window");
                }
                else {

                    GUILayout.BeginHorizontal();
                    reloadEntities = GUILayout.Button(reloadTexture, GUILayout.Width(50));
                    GUILayout.Label("Reload Entities");
                    GUILayout.EndHorizontal();

                    GUILayout.Label("Found Classe/Interfaces (" + selectedEntities.Count + "): ", EditorStyles.boldLabel);
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                    foreach (var entity in selectedEntities) {
                        string type = entity.IsClass() ? "(C) " : "(I)  ";
                        GUILayout.Label(type + entity.GetName());
                    }
                    EditorGUILayout.EndScrollView();
                    generateDiagramButton = GUILayout.Button("Generate!");

                    if (reloadEntities) {
                        Debug.Log("Reloading entities...");
                        LoadAllEntities();
                    }
                }
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

        if (File.Exists(texturePath)) {
            byte[] fileData;
            fileData = File.ReadAllBytes(texturePath);
            reloadTexture = new Texture2D(2, 2);
            reloadTexture.LoadImage(fileData);
            reloadTexture.Apply();
        }

        // Try to load already existent entities from disk
        selectedEntities = DiagramEngine.LoadEntitiesFromDisk();
        if (selectedEntities != null)
            return;
        LoadAllEntities();
    }

    /// <summary>
    /// Gets the selected entities.
    /// </summary>
    /// <returns>The selected entities.</returns>
    public List<BaseModel> GetSelectedEntities() {
        return selectedEntities;
    }

    private void LoadAllEntities() {

        selectedEntities = new List<BaseModel>();
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
        ClassWrapper.RelateEntities(ref selectedEntities);
        DiagramEngine.SaveEntitiesOnDisk(selectedEntities);
    }

    private List<string> SearchAllFilesInDirectory(string directory) {

        List<string> files = new List<string>();
        try {
            foreach (string file in Directory.GetFiles(directory, "*.cs")) {
                files.Add(file);
            }
            foreach (string dir in Directory.GetDirectories(directory)) {
                if(dir != "./Assets/DiagramEngine")
                    files.AddRange(SearchAllFilesInDirectory(dir));
            }
        }
        catch (Exception e) {
            Debug.Log("Error (DiagramEngine): " + e);
        }
        return files;
    }
}
