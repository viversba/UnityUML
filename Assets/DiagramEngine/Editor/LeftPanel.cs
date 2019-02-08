using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEngine.Model;
using DEngine.Controller;
using DEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DEngine.View {

    public class LeftPanel : EditorWindow {

        /// <summary>
        /// This is the current list of entities that is going to be rendered
        /// </summary>
        private List<BaseModel> selectedEntities = new List<BaseModel>();
        /// <summary>
        /// This is the current list of entities that is going to be rendered when 'All' options is selected
        /// </summary>
        private List<BaseModel> selectedEntities_ALL = new List<BaseModel>();
        /// <summary>
        /// This is the current list of entities that is going to be rendered when 'Drag&Drop' options is selected
        /// </summary>
        private List<BaseModel> selectedEntities_DD = new List<BaseModel>();
        /// <summary>
        /// Scroll position of the list of entities to render when All option is enabled
        /// </summary>
        private Vector2 scrollPos_ALL = new Vector2();
        /// <summary>
        /// Scroll position of the list of entities to render when Drag&Drop option is enabled
        /// </summary>
        private Vector2 scrollPos_DD = new Vector2();
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
        /// The draggable panel.
        /// </summary>


        private void Awake() {

            selected = 0;
            selectedEntities = new List<BaseModel>();
            selectedEntities_ALL = new List<BaseModel>();
            selectedEntities_DD = new List<BaseModel>();

            if (File.Exists(texturePath)) {
                byte[] fileData;
                fileData = File.ReadAllBytes(texturePath);
                reloadTexture = new Texture2D(2, 2);
                reloadTexture.LoadImage(fileData);
                reloadTexture.Apply();
            }

            // Try to load already existent entities from disk
            selectedEntities_ALL = DiagramEngine.LoadEntitiesFromDisk();
            if (selectedEntities_ALL != null)
                return;
            LoadAllEntities();
        }

        /// <summary>
        /// Draws the left panel.
        /// </summary>
        /// <returns><c>true</c>, if the GenerateDiagram button was pressed, <c>false</c> otherwise.</returns>
        public bool DrawLeftPanel() {

            bool generateDiagramButton = false, reloadEntities = false, resetEntitiesButton = false;
            string[] options = { "All!", "Drag & Drop", "CheckBox" };
            selected = GUILayout.Toolbar(selected, options, GUILayout.MinWidth(240));

            switch (selected) {

                // All entities
                case 0:

                    GUILayout.BeginHorizontal();
                    reloadEntities = GUILayout.Button(reloadTexture, GUILayout.Width(50));
                    GUILayout.Label("Reload Entities");
                    GUILayout.EndHorizontal();

                    // Render entities only if there are
                    if (selectedEntities_ALL.Count == 0) {
                        GUILayout.Label("There's nothing to render here :(");
                        GUILayout.Label("Add some scripts to the project and reopen the window");
                    }
                    else {

                        GUILayout.Label("Found Classe/Interfaces (" + selectedEntities_ALL.Count + "): ", EditorStyles.boldLabel);
                        scrollPos_ALL = EditorGUILayout.BeginScrollView(scrollPos_ALL);
                        foreach (var entity in selectedEntities_ALL) {
                            string type = entity.Type == EntityTypes.CLASS ? "(C) " : "(I)  ";
                            GUILayout.Label(type + entity.GetName());
                        }
                        EditorGUILayout.EndScrollView();

                        GUILayout.BeginHorizontal();
                        generateDiagramButton = GUILayout.Button("Generate!");
                        resetEntitiesButton = GUILayout.Button("Reset List");
                        GUILayout.EndHorizontal();

                        if (resetEntitiesButton) {
                            selectedEntities_ALL.Clear();
                        }
                    }

                    if (reloadEntities) {
                        LoadAllEntities();
                    }
                    selectedEntities = selectedEntities_ALL;
                    break;

                // Drag And Drop
                case 1:

                    if (Event.current.type == EventType.DragExited) {

                        Event.current.Use();

                        // This foreach loop handles loose scripts
                        foreach (UnityEngine.Object obj in DragAndDrop.objectReferences) {
                            if (obj.GetType() == typeof(MonoScript)) {

                                try {
                                    MonoScript script = (MonoScript)obj;
                                    LoadDroppedEntity(script.text);
                                }
                                catch (Exception e) {
                                    Debug.Log("Error (LeftPanel: DrawLeftPanel): " + e);
                                }
                            }
                        }

                        // This foreach loop handles folders
                        foreach (string path in DragAndDrop.paths) {

                            if (Directory.Exists($"{Directory.GetCurrentDirectory()}/{path}")) {
                                LoadAllEntitiesInFolder($"{Directory.GetCurrentDirectory()}/{path}");
                            }
                        }

                        if (selectedEntities_DD != null) {
                            ClassWrapper.RelateEntities(ref selectedEntities_DD);
                        }
                    }

                    scrollPos_DD = EditorGUILayout.BeginScrollView(scrollPos_DD);
                    foreach (var entity in selectedEntities_DD) {
                        string type = entity.Type == EntityTypes.CLASS ? "(C) " : "(I)  ";
                        GUILayout.Label(type + entity.GetName());
                    }
                    EditorGUILayout.EndScrollView();

                    GUILayout.BeginHorizontal();
                    generateDiagramButton = GUILayout.Button("Generate!");
                    resetEntitiesButton = GUILayout.Button("Reset List");
                    GUILayout.EndHorizontal();

                    if (resetEntitiesButton) {
                        selectedEntities_DD.Clear();
                    }

                    selectedEntities = selectedEntities_DD;

                    break;
                case 2:
                    GUILayout.Label("Coming Soon!");
                    break;
                default:
                    break;
            }

            return generateDiagramButton;
        }

        /// <summary>
        /// Gets the selected entities.
        /// </summary>
        /// <returns>The selected entities.</returns>
        public List<BaseModel> GetSelectedEntities() {
            return selectedEntities;
        }

        private void LoadDroppedEntity(string text) {

            List<BaseModel> entities = EntityWrapper.GetEntitiesFromText(text);
            if(entities != null || entities.Count != 0) {
                foreach (var entity in entities) {
                    if (ClassWrapper.FindEntityWithName(ref selectedEntities_DD, entity.GetName()) == -1) {
                        selectedEntities_DD.Add(entity);
                    }
                }
            }
        }

        private void LoadAllEntitiesInFolder(string folder) {

            switch (selected) {
                case 1:
                    foreach (string file in SearchAllFilesInDirectory(folder)) {
                        List<BaseModel> entities = EntityWrapper.GetEntitiesFromFile(file);
                        if (entities != null || entities.Count != 0) {
                            foreach (var entity in entities) {
                                if (ClassWrapper.FindEntityWithName(ref selectedEntities_DD, entity.GetName()) == -1) {
                                    selectedEntities_DD.Add(entity);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Finds files recursively from the /Assets folder and loads all entities in them
        /// </summary>
        private void LoadAllEntities() {

            selectedEntities_ALL = new List<BaseModel>();
            foreach (string file in SearchAllFilesInDirectory("./Assets/")) {
                List<BaseModel> entities = EntityWrapper.GetEntitiesFromFile(file);
                if (entities != null || entities.Count != 0) {
                    foreach (var entity in entities) {
                        if (ClassWrapper.FindEntityWithName(ref selectedEntities_ALL, entity.GetName()) == -1) {
                            selectedEntities_ALL.Add(entity);
                        }
                    }
                }
                selectedEntities_ALL.AddRange(EntityWrapper.GetEntitiesFromFile(file));
            }
            ClassWrapper.RelateEntities(ref selectedEntities_ALL);
            DiagramEngine.SaveEntitiesOnDisk(selectedEntities_ALL);
        }

        /// <summary>
        /// Async version of <c>LoadAllEntities()</c> method
        /// </summary>
        private void ALoadAllEntities() {
            selectedEntities_ALL = new List<BaseModel>();
            foreach (string file in SearchAllFilesInDirectory("./Assets/")) {
                List<BaseModel> entities = EntityWrapper.GetEntitiesFromFile(file);
                if (entities != null || entities.Count != 0) {
                    foreach (var entity in entities) {
                        if (ClassWrapper.FindEntityWithName(ref selectedEntities_ALL, entity.GetName()) == -1) {
                            selectedEntities_ALL.Add(entity);
                        }
                    }
                }
                selectedEntities_ALL.AddRange(EntityWrapper.GetEntitiesFromFile(file));
            }
            ClassWrapper.RelateEntities(ref selectedEntities_ALL);
            DiagramEngine.SaveEntitiesOnDisk(selectedEntities_ALL);
        }

        private List<string> SearchAllFilesInDirectory(string directory) {

            List<string> files = new List<string>();
            try {
                foreach (string file in Directory.GetFiles(directory, "*.cs")) {
                    files.Add(file);
                }
                foreach (string dir in Directory.GetDirectories(directory)) {
                    if (dir != "./Assets/DiagramEngine")
                        files.AddRange(SearchAllFilesInDirectory(dir));
                }
            }
            catch (Exception e) {
                Debug.Log("Error (DiagramEngine): " + e);
            }
            return files;
        }
    }
}
