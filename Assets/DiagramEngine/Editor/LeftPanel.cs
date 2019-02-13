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
using System.Threading.Tasks;

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
        /// Indicates if entities are being loaded in order to display label
        /// </summary>
        private bool loadingEntities;

        private void Awake() {

            selected = 0;
            selectedEntities = new List<BaseModel>();
            selectedEntities_ALL = new List<BaseModel>();
            selectedEntities_DD = new List<BaseModel>();
            loadingEntities = false;

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
            LoadAllEntitiesAsyncCaller();
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
                    if (selectedEntities_ALL.Count == 0 && !loadingEntities) {
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
                        if(loadingEntities)
                            GUILayout.Label("Processing entities...");
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
                        LoadAllEntitiesAsyncCaller();
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
                                    LoadDroppedEntityAsyncCaller(script.text);
                                }
                                catch (Exception e) {
                                    Debug.Log("Error (LeftPanel: DrawLeftPanel): " + e);
                                }
                            }
                        }

                        // This foreach loop handles folders
                        foreach (string path in DragAndDrop.paths) {

                            if (Directory.Exists($"{Directory.GetCurrentDirectory()}/{path}")) {
                                LoadAllEntitiesInFolderAsyncCaller($"{Directory.GetCurrentDirectory()}/{path}");
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
                    if (loadingEntities)
                        GUILayout.Label("Processing entities...");
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

        private async void LoadAllEntitiesAsyncCaller() {
            await LoadAllEntitiesAsync();
        }

        private async void LoadAllEntitiesInFolderAsyncCaller(string folder) {
            await LoadAllEntitiesInFolderAsync(folder);
        }

        private async void LoadDroppedEntityAsyncCaller(string text) {
            await LoadDroppedEntityAsync(text);
        }

        /// <summary>
        /// Async version of the LoadDroppedEntity method
        /// </summary>
        /// <param name="text">text</param>
        private async Task LoadDroppedEntityAsync(string text) {

            loadingEntities = true;
            List<BaseModel> entities = await Task.Run(() => EntityWrapper.GetEntitiesFromText(text));
            if (entities != null || entities.Count != 0) {
                foreach (var entity in entities) {
                    if (ClassWrapper.FindEntityWithName(ref selectedEntities_DD, entity.GetName()) == -1) {
                        selectedEntities_DD.Add(entity);
                    }
                }
            }
            loadingEntities = false;
        }

        /// <summary>
        /// Async version of the LoadAllEntitiesInFolder method
        /// </summary>
        /// <returns>void</returns>
        /// <param name="folder">folder.</param>
        private async Task LoadAllEntitiesInFolderAsync(string folder) {

            switch (selected) {
                case 1:
                    loadingEntities = true;
                    foreach (string file in SearchAllFilesInDirectory(folder)) {
                        List<BaseModel> entities = await Task.Run(() => EntityWrapper.GetEntitiesFromFile(file));
                        if (entities != null || entities.Count != 0) {
                            foreach (var entity in entities) {
                                if (ClassWrapper.FindEntityWithName(ref selectedEntities_DD, entity.GetName()) == -1) {
                                    selectedEntities_DD.Add(entity);
                                }
                            }
                        }
                    }
                    loadingEntities = false;
                    break;
            }
        }

        /// <summary>
        /// Async version of <c>LoadAllEntities()</c> method
        /// </summary>
        private async Task LoadAllEntitiesAsync() {

            loadingEntities = true;

            selectedEntities_ALL = new List<BaseModel>();
            foreach (string file in SearchAllFilesInDirectory("./Assets/")) {
                List<BaseModel> entities = await Task.Run(() => EntityWrapper.GetEntitiesFromFile(file));
                if (entities != null || entities.Count != 0) {
                    foreach (var entity in entities) {
                        if (ClassWrapper.FindEntityWithName(ref selectedEntities_ALL, entity.GetName()) == -1) {
                            selectedEntities_ALL.Add(entity);
                        }
                    }
                }
            }
            ClassWrapper.RelateEntities(ref selectedEntities_ALL);
            DiagramEngine.SaveEntitiesOnDisk(selectedEntities_ALL);

            loadingEntities = false;
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
