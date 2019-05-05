using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using DEngine.Model;
using DEngine.View;
using System;

namespace DEngine.View {

    public class RightPanel : EditorWindow {

        private Texture2D backgroundTexture;

        /// <summary>
        /// List that contains all the current windows
        /// </summary>
        private List<BaseNode> windows = new List<BaseNode>();
        // Instance of the selected window;
        private BaseNode selectedWindow;
        // Variable to store our mousePos
        private Vector2 mousePos;
        // Variable to determine if resize mode is on
        private bool resizeMode;
        // Indicates if the nodes should be showing
        private bool drawNodes;
        // Indicates the position of begginig of the right panel
        private float begginingOfRightPanel;
        // Rect of the right panel
        private Rect rightPanelRect;
        // Current resize type
        private ResizeType resizeType;
        // Current mouse cursor
        MouseCursor cursor;

        /*
         * THESE ARE THE BOOLEANS FOR THE OPTIONS DISPLAYED AT RIGHT CLICK      
         */
        private bool DisplayEmpty { get; set; }

        public void Awake() {

            string texturePath = "./Assets/DiagramEngine/Textures/grid_texture.jpg";
            byte[] fileData;
            drawNodes = false;
            selectedWindow = null;
            resizeMode = false;
            resizeType = ResizeType.None;
            cursor = MouseCursor.Arrow;
            DisplayEmpty = true;

            if (File.Exists(texturePath)) {
                fileData = File.ReadAllBytes(texturePath);
                backgroundTexture = new Texture2D(2, 2);
                backgroundTexture.LoadImage(fileData);
                backgroundTexture.Apply();
                backgroundTexture.wrapMode = TextureWrapMode.Repeat;
            }
        }

        public void DrawRightPanel(Vector2 sizeOfMainWindow) {

            // Draw the texture first
            rightPanelRect = new Rect(begginingOfRightPanel, 0, sizeOfMainWindow.x, sizeOfMainWindow.y);
            GUI.DrawTexture(new Rect(begginingOfRightPanel, 0, sizeOfMainWindow.x, sizeOfMainWindow.y), backgroundTexture, ScaleMode.ScaleAndCrop);
            GUI.DrawTextureWithTexCoords(new Rect(begginingOfRightPanel, 0, sizeOfMainWindow.x, sizeOfMainWindow.y), backgroundTexture, new Rect(0, 0, sizeOfMainWindow.x / backgroundTexture.width, sizeOfMainWindow.y / backgroundTexture.height));

            // Handle clicks inside the panel
            Event e = Event.current;
            mousePos = e.mousePosition;

            if (resizeMode) {
                if (e.type == EventType.MouseUp) {
                    resizeMode = false;
                }
                else {
                    EditorGUIUtility.AddCursorRect(rightPanelRect, cursor);
                    //Debug.Log(mousePos.x + "     " + rightPanelRect.position.x);
                    float CheckForX = mousePos.x - rightPanelRect.position.x < 0 ? rightPanelRect.position.x : mousePos.x;
                    float CheckForY = mousePos.y - rightPanelRect.position.y < 0 ? rightPanelRect.position.y : mousePos.y;
                    selectedWindow.ResizeWindow(new Vector2(CheckForX, CheckForY), resizeType);
                }
            }
            else {

                // Handle window selection
                if (e.button == 0 && e.isMouse) {
                    bool clickedOnWindow = false;
                    foreach (BaseNode window in windows) {
                        if (window.windowRect.Contains(mousePos)) {
                            selectedWindow = window;
                            clickedOnWindow = true;
                            break;
                        }
                    }
                    if (!clickedOnWindow)
                        selectedWindow = null;
                }

                WatchForResize();
            }

            if (drawNodes) {

                //Draw the windows
                BeginWindows();

                for (int i = 0; i < windows.Count; i++) {

                    // Don't draw the window if:
                    // * Is empty and empty windows are not being drawn
                    if (windows[i].IsEmpty() && !DisplayEmpty)
                        continue;


                    //Make sure the window is within the borders
                    //Take care of x axis
                    windows[i].windowRect.x = windows[i].windowRect.x < begginingOfRightPanel ? begginingOfRightPanel : windows[i].windowRect.x;
                    windows[i].windowRect.x = windows[i].windowRect.x > sizeOfMainWindow.x - 20 ? sizeOfMainWindow.x - 20 : windows[i].windowRect.x;
                    //Take care of y axis
                    windows[i].windowRect.y = windows[i].windowRect.y < 0 ? 0 : windows[i].windowRect.y;
                    windows[i].windowRect.y = windows[i].windowRect.y > sizeOfMainWindow.y - 20 ? sizeOfMainWindow.y - 20 : windows[i].windowRect.y;

                    GUIStyle style = new GUIStyle(GUI.skin.window);
                    switch (windows[i].Type) {
                        case EntityTypes.CLASS:
                            style.fontStyle = FontStyle.Bold;
                            style.normal.textColor = new Color(0.168f, 0.552f, 0.003f, 1f);
                            style.onNormal.textColor = new Color(0.168f, 0.552f, 0.003f, 1f);
                            windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle, style);
                            break;
                        case EntityTypes.INTERFACE:
                            style.fontStyle = FontStyle.Italic;
                            style.normal.textColor = new Color(0.552f, 0.317f, 0.003f, 1f);
                            style.onNormal.textColor = new Color(0.552f, 0.317f, 0.003f, 1f);
                            windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle, style);
                            break;
                        case EntityTypes.STRUCT:
                            style.fontStyle = FontStyle.BoldAndItalic;
                            style.normal.textColor = new Color(0.011f, 0.431f, 0.5883f, 1f);
                            style.onNormal.textColor = new Color(0.011f, 0.431f, 0.588f, 1f);
                            windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle, style);
                            break;
                    }


                }

                //draw each curve for every node
                foreach (BaseNode n in windows) {
                    n.DrawCurves();
                }

                EndWindows();

                DrawConventions();

            }

            //DrawOptionsPanel();
        }

        /// <summary>
        /// Draws the options panel, when a Right click is made.
        /// </summary>
        public void DrawOptionsPanel() {

            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 1 && rightPanelRect.Contains(e.mousePosition)) {

                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent($"{(DisplayEmpty ? "" : "√  ")}Ignore Empty Windows", "Don't draw the empty windows"), false, MenuOptionsCallback, "ignoreEmpty");

                menu.ShowAsContext();

                e.Use();
            }
        }

        public void MenuOptionsCallback(object opt) {

            string option = opt.ToString();
            if (option.Equals("ignoreEmpty"))
                DisplayEmpty = !DisplayEmpty;
        }

        /// <summary>
        /// Sets the local list of windows so that all entities have a window
        /// </summary>
        /// <param name="selectedEntities">List of entities.</param>
        public void CreateWindowList(List<BaseModel> selectedEntities) {

            windows.Clear();

            float startX = begginingOfRightPanel + 50;
            float startY = 70;

            float availableHeight = rightPanelRect.height;

            float windowXPosition = startX;
            float windowYPosition = startY;

            if (selectedEntities != null || selectedEntities.Count != 0) {
                drawNodes = true;
                foreach (BaseModel entity in selectedEntities) {

                    // Draw Classes
                    if (entity.Type == EntityTypes.CLASS) {
                        ClassNode classNode = (ClassNode)CreateInstance("ClassNode");
                        classNode.windowRect = new Rect(windowXPosition, windowYPosition, 150f, 150f);
                        classNode.Init((ClassModel)entity);
                        windows.Add(classNode);

                        // Validate limits
                        if (windowXPosition + 155f <= rightPanelRect.width - 150f) {
                            windowXPosition += 155f;
                        }
                        else {
                            windowXPosition = startX;
                            if (windowYPosition + 155f < availableHeight - 150f) {
                                windowYPosition += 155f;
                            }
                            else {
                                windowYPosition = startY;
                            }
                        }
                    }
                    else if (entity.Type == EntityTypes.INTERFACE) {
                        // Draw Interfaces
                        InterfaceNode interfaceNode = (InterfaceNode)CreateInstance("InterfaceNode");
                        interfaceNode.windowRect = new Rect(windowXPosition, windowYPosition, 170f, 150f);
                        interfaceNode.Init((InterfaceModel)entity);
                        windows.Add(interfaceNode);

                        // Validate limits
                        if (windowXPosition + 175f <= rightPanelRect.width - 170f) {
                            windowXPosition += 175f;
                        }
                        else {
                            windowXPosition = startX;
                            if (windowYPosition + 155f < availableHeight - 150f) {
                                windowYPosition += 155f;
                            }
                            else {
                                windowYPosition = startY;
                            }
                        }
                    }
                    else if (entity.Type == EntityTypes.STRUCT) {
                        // Draw structs
                        StructNode structNode = (StructNode)CreateInstance("StructNode");
                        structNode.windowRect = new Rect(windowXPosition, windowYPosition, 150f, 150f);
                        structNode.Init((StructModel)entity);
                        windows.Add(structNode);

                        // Validate limits
                        if (windowXPosition + 155f <= rightPanelRect.width - 150f) {
                            windowXPosition += 155f;
                        }
                        else {
                            windowXPosition = startX;
                            if (windowYPosition + 155f < availableHeight - 150f) {
                                windowYPosition += 155f;
                            }
                            else {
                                windowYPosition = startY;
                            }
                        }
                    }
                }

                foreach (BaseNode window in windows) {

                    // Assign superclass or interface windows
                    // Is it a class?
                    if (window.Type == EntityTypes.CLASS) {
                        ClassNode classNode = (ClassNode)window;
                        ClassNode superClassNode = (ClassNode)GetWindowWithTitle(classNode.GetSuperClassName());
                        classNode.SetSuperClassNode(superClassNode);

                        // Assign Interface Windows
                        if (classNode.GetInterfaceNames() != null) {
                            List<InterfaceNode> interfacesToAdd = new List<InterfaceNode>();
                            foreach (string interface_ in classNode.GetInterfaceNames()) {
                                interfacesToAdd.Add(GetWindowWithTitle(interface_) as InterfaceNode);
                            }
                            classNode.SetInterfaceNodes(interfacesToAdd);
                        }

                        // Assign implementation windows
                        if (classNode.GetImplementations() != null && classNode.GetImplementations().Count > 0) {
                            List<BaseNode> implementations = new List<BaseNode>();
                            foreach (var implementation in classNode.GetImplementations()) {
                                BaseNode implementationNode = GetWindowWithTitle(implementation.GetName());
                                if (implementationNode != null){
                                    implementations.Add(implementationNode);
                                }
                            }
                            classNode.SetImplementationNodes(implementations);
                        }
                    }
                    else if (window.Type == EntityTypes.INTERFACE) {
                        // Or an interface?
                        // Assign Interface Windows
                        InterfaceNode interfaceNode = window as InterfaceNode;
                        if (interfaceNode.GetInterfaceNames() != null) {
                            List<InterfaceNode> interfacesToAdd = new List<InterfaceNode>();
                            foreach (string interface_ in interfaceNode.GetInterfaceNames()) {
                                interfacesToAdd.Add(GetWindowWithTitle(interface_) as InterfaceNode);
                            }
                            interfaceNode.SetInterfaceNodes(interfacesToAdd);
                        }
                    }
                    else if (window.Type == EntityTypes.STRUCT) {
                        // Or an interface?
                        // Assign Interface Windows
                        StructNode structNode = window as StructNode;
                        if (structNode.GetInterfaceNames() != null) {
                            List<InterfaceNode> interfacesToAdd = new List<InterfaceNode>();
                            foreach (string interface_ in structNode.GetInterfaceNames()) {
                                interfacesToAdd.Add(GetWindowWithTitle(interface_) as InterfaceNode);
                            }
                            structNode.SetInterfaceNodes(interfacesToAdd);
                        }

                        // Assign implementation windows
                        if (structNode.GetImplementations() != null && structNode.GetImplementations().Count > 0){
                            List<BaseNode> implementations = new List<BaseNode>();
                            foreach (var implementation in structNode.GetImplementations()){
                                BaseNode implementationNode = GetWindowWithTitle(implementation.GetName());
                                if (implementationNode != null){
                                    implementations.Add(implementationNode);
                                }
                            }
                            structNode.SetImplementationNodes(implementations);
                        }
                    }
                }
            }
            else {
                Debug.LogError("Empty or null list trying to be drawn");
            }
        }

        public void WatchForResize() {


            Event e = Event.current;
            mousePos = e.mousePosition;
            float minDistance = 5;

            if (selectedWindow != null) {
                Rect windowRect = selectedWindow.windowRect;
                //Check if mouse is in edge or corner
                // Left corner
                if (mousePos.x >= windowRect.position.x && mousePos.x - windowRect.position.x <= minDistance) {
                    // Top left corner
                    if (mousePos.y >= windowRect.position.y && mousePos.y - windowRect.position.y <= minDistance) {
                        cursor = MouseCursor.ResizeUpLeft;
                        EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeUpLeft);
                        resizeType = ResizeType.Top_Left;
                    }
                    else if ((windowRect.position.y + windowRect.height) >= mousePos.y && (windowRect.position.y + windowRect.height) - mousePos.y <= minDistance) {
                        // Bottom left corner
                        cursor = MouseCursor.ResizeUpRight;
                        EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeUpRight);
                        resizeType = ResizeType.Bottom_Left;
                    }
                    else {
                        cursor = MouseCursor.ResizeHorizontal;
                        EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeHorizontal);
                        resizeType = ResizeType.Left;
                    }
                }// Right corner
                else if ((windowRect.position.x + windowRect.width) >= mousePos.x && (windowRect.position.x + windowRect.width) - mousePos.x <= minDistance) {
                    // Top right corner
                    if (windowRect.position.y <= mousePos.y && mousePos.y - windowRect.position.y <= minDistance) {
                        cursor = MouseCursor.ResizeUpRight;
                        resizeType = ResizeType.Top_Right;
                        EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeUpRight);
                    }
                    else if ((windowRect.position.y + windowRect.height) >= mousePos.y && (windowRect.position.y + windowRect.height) - mousePos.y <= minDistance) {
                        // Bottom right corner
                        cursor = MouseCursor.ResizeUpLeft;
                        resizeType = ResizeType.Bottom_Right;
                        EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeUpLeft);
                    }
                    else {
                        cursor = MouseCursor.ResizeHorizontal;
                        resizeType = ResizeType.Right;
                        EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeHorizontal);
                    }
                }// Top edge
                else if (windowRect.position.y <= mousePos.y && mousePos.y - windowRect.position.y <= minDistance) {
                    cursor = MouseCursor.ResizeVertical;
                    resizeType = ResizeType.Top;
                    EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeVertical);
                }
                // Bottom edge
                else if ((windowRect.position.y + windowRect.height) > mousePos.y && (windowRect.position.y + windowRect.height) - mousePos.y <= minDistance) {
                    cursor = MouseCursor.ResizeVertical;
                    resizeType = ResizeType.Bottom;
                    EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeVertical);
                }
                else {
                    resizeType = ResizeType.None;
                }

                // Set On and off resize mode
                if (e.button == 0 && e.type == EventType.MouseDown && resizeType != ResizeType.None) {
                    resizeMode = true;
                    selectedWindow.ResizeWindow(mousePos, resizeType);
                }
            }
        }

        public void SetBegginingOfRightPanel(float begginingOfRightPanel) {
            this.begginingOfRightPanel = begginingOfRightPanel;
        }

        public void SetRightPanelRect(Rect rightPanelRect) {
            this.rightPanelRect = rightPanelRect;
        }

        /// <summary>
        /// Draws the inheritance curve.
        /// </summary>
        /// <param name="start">Start rect.</param>
        /// <param name="end">End rect.</param>
        public static void DrawInheritanceCurve(Rect start, Rect end) {

            DrawVerticalBezier(start, end, Color.red);

            // Draw the handle for Inheritance type
            float triangleHeight = 5f * 2 / 3;
            float positionX = end.x;
            float positionY = end.y + 5;

            Handles.color = new Color(0.8f, 0.8f, 0.8f, 1);
            Vector3[] positions = {
            new Vector3(positionX, positionY - triangleHeight),
            new Vector3(positionX - triangleHeight, positionY + triangleHeight),
            new Vector3(positionX + triangleHeight, positionY + triangleHeight),
            new Vector3(positionX, positionY - triangleHeight)};
            Handles.DrawPolyLine(positions);
            Handles.color = Color.white;
        }

        /// <summary>
        /// Draws the implementation curve.
        /// </summary>
        /// <param name="start">Start rect.</param>
        /// <param name="end">End rect.</param>
        public static void DrawImplementationCurve(Rect start, Rect end) {

            // Draw the handle for Inheritance type
            float rectHeight = 5f * 2 / 3;
            float positionX = end.x - 5;
            float positionY = end.y;

            Handles.color = new Color(0.8f, 0.8f, 0.8f, 1);
            Vector3[] positions = {
            new Vector3(rectHeight + positionX, positionY),
            new Vector3(-rectHeight + positionX, rectHeight + positionY),
            new Vector3(-rectHeight + positionX, -rectHeight + positionY),
            new Vector3(rectHeight + positionX, positionY)};
            //Handles.DrawRectangle(1, new Vector3(end.x, end.y), Quaternion.identity, 10);
            Vector3[] handles = {
                new Vector3(end.x, end.y),
                new Vector3(end.x - 10, end.y - 5),
                new Vector3(end.x - 20, end.y),
                new Vector3(end.x - 10, end.y + 5)
            };

            end.x -= 20;
            DrawHorizontalBezier(start, end, Color.white);

            Handles.DrawSolidRectangleWithOutline(handles, Color.green, Color.green);
            //Handles.DrawPolyLine(positions);
            Handles.color = Color.white;
        }

        // Draw the node curve from the middle of the start Rect to the middle of the end rect 
        public static void DrawVerticalBezier(Rect start, Rect end, Color mainColor) {

            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.down * 50;
            Vector3 endTan = endPos + Vector3.up * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++) {// Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }
            Handles.DrawBezier(startPos, endPos, startTan, endTan, mainColor, null, 1);
        }

        // Draw the node curve from the middle of the start Rect to the middle of the end rect 
        public static void DrawHorizontalBezier(Rect start, Rect end, Color mainColor) {

            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++) {// Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }
            Handles.DrawBezier(startPos, endPos, startTan, endTan, mainColor, null, 1);
        }

        private void DrawConventions() {

            //Color for the 'Conventions' title
            var whiteText = new GUIStyle();
            var header = new GUIStyle();
            whiteText.normal.textColor = Color.white;
            header.fontSize = 20;
            header.normal.textColor = Color.white;

            GUILayout.BeginArea(new Rect(begginingOfRightPanel + 30, 10, 200, 100));

            GUILayout.Label("Conventions:", header);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Imlpementation  ", whiteText);
            Handles.color = Color.cyan;
            Handles.DrawLine(new Vector3(100f, 30f, 0f), new Vector3(130f, 30f, 0f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Inheritance  ", whiteText);
            Handles.color = Color.red;
            Handles.DrawLine(new Vector3(100f, 45f, 0f), new Vector3(130f, 45f, 0f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Composition  ", whiteText);
            Handles.color = Color.white;
            Handles.DrawLine(new Vector3(100f, 60f, 0f), new Vector3(130f, 60f, 0f));
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            Handles.color = Color.white;
        }

        private BaseNode GetWindowWithTitle(string windowTitle) {

            foreach (BaseNode window in windows) {
                if (window.Type == EntityTypes.CLASS) {
                    ClassNode classNode = (ClassNode)window;
                    if (string.Equals(classNode.Name, windowTitle))
                        return classNode;
                }
                else if (window.Type == EntityTypes.INTERFACE) {
                    InterfaceNode interfaceNode = (InterfaceNode)window;
                    if (string.Equals(interfaceNode.Name, windowTitle))
                        return interfaceNode;
                }
                else if (window.Type == EntityTypes.STRUCT) {
                    StructNode structNode = (StructNode)window;
                    if (string.Equals(structNode.Name, windowTitle))
                        return structNode;
                }
            }
            return null;
        }

        //function that draws the windows
        private void DrawNodeWindow(int id) {
            windows[id].DrawWindow();
            if (!resizeMode) {
                GUI.DragWindow();
            }
        }

        void ContextCallback(object obj) {
        }
    }
}

