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

        Rect dropTargetRect = new Rect(10.0f, 10.0f, 30.0f, 30.0f);


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

            bool generateDiagramButton = false, reloadEntities = false;
            string[] options = { "All!", "Drag & Drop", "CheckBox" };
            selected = GUILayout.Toolbar(selected, options, GUILayout.MinWidth(240));

            switch (selected) {

                // All entities
                case 0:

                    // Render entities only if there are
                    if (selectedEntities_ALL.Count == 0) {
                        GUILayout.Label("There's nothing to render here :(");
                        GUILayout.Label("Add some scripts to the project and reopen the window");
                    }
                    else {

                        GUILayout.BeginHorizontal();
                        reloadEntities = GUILayout.Button(reloadTexture, GUILayout.Width(50));
                        GUILayout.Label("Reload Entities");
                        GUILayout.EndHorizontal();

                        GUILayout.Label("Found Classe/Interfaces (" + selectedEntities_ALL.Count + "): ", EditorStyles.boldLabel);
                        scrollPos_ALL = EditorGUILayout.BeginScrollView(scrollPos_ALL);
                        foreach (var entity in selectedEntities_ALL) {
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
                    selectedEntities = selectedEntities_ALL;
                    break;

                // Drag And Drop
                case 1:
                    if (Event.current.type == EventType.DragExited) {
                        //Debug.Log("DragExit event, mousePos:" + Event.current.mousePosition + "window pos:" + position);

                        foreach (UnityEngine.Object obj in DragAndDrop.objectReferences) {
                            if (obj.GetType() == typeof(MonoScript)) {

                                try {
                                    MonoScript script = (MonoScript)obj;
                                    LoadDroppedEntity(script.text);
                                }
                                catch(Exception e) {
                                    Debug.Log("Error (LeftPanel: DrawLeftPanel): " + e);
                                }
                            }
                            else{
                                Debug.LogWarning("Object " + obj.name + " cannot be processed in the diagram. Please be sure to use scripts only");
                            }
                        }

                        Event.current.Use();
                    }

                    scrollPos_DD = EditorGUILayout.BeginScrollView(scrollPos_DD);
                    foreach (var entity in selectedEntities_DD) {
                        string type = entity.IsClass() ? "(C) " : "(I)  ";
                        GUILayout.Label(type + entity.GetName());
                    }
                    EditorGUILayout.EndScrollView();

                    generateDiagramButton = GUILayout.Button("Generate!");

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
                ClassWrapper.RelateEntities(ref selectedEntities_DD);
            }
        }

        private void LoadAllEntities() {

            selectedEntities_ALL = new List<BaseModel>();
            switch (selected) {
                case 0:
                    foreach (string file in SearchAllFilesInDirectory("./Assets/")) {
                        //var list = EntityWrapper.GetEntitiesFromFile(file);
                        selectedEntities_ALL.AddRange(EntityWrapper.GetEntitiesFromFile(file));
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
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
