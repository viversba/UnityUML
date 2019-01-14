using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using DEngine.Model;
using DEngine.View;
using System;

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

    float PanY;
    float PanX;

    private bool scrollWindow = false;
    private Vector2 scrollStartMousePos;

    public void Awake() {

        string filePath = "./Assets/DiagramEngine/Textures/grid_texture.jpg";
        byte[] fileData;
        drawNodes = false;
        selectedWindow = null;
        resizeMode = false;
        resizeType = ResizeType.None;
        cursor = MouseCursor.Arrow;

        if (File.Exists(filePath)) {
            fileData = File.ReadAllBytes(filePath);
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

                //Make sure the window is within the borders
                //Take care of x axis
                windows[i].windowRect.x = windows[i].windowRect.x < begginingOfRightPanel ? begginingOfRightPanel : windows[i].windowRect.x;
                windows[i].windowRect.x = windows[i].windowRect.x > sizeOfMainWindow.x - 20 ? sizeOfMainWindow.x - 20 : windows[i].windowRect.x;
                //Take care of y axis
                windows[i].windowRect.y = windows[i].windowRect.y < 0 ? 0 : windows[i].windowRect.y;
                windows[i].windowRect.y = windows[i].windowRect.y > sizeOfMainWindow.y - 20 ? sizeOfMainWindow.y - 20 : windows[i].windowRect.y;

                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
            }

            //draw each curve for every node
            foreach (BaseNode n in windows) {
                n.DrawCurves();
            }

            EndWindows();
        }
    }


    /// <summary>
    /// Sets the local list of windows so that all entities have a window
    /// </summary>
    /// <param name="selectedEntities">List of entities.</param>
    public void CreateWindowList(List<BaseModel> selectedEntities) {

        Debug.Log("Right Panel");
        Debug.Log(rightPanelRect.ToString());

        float separationX = (rightPanelRect.width - begginingOfRightPanel - 50) / selectedEntities.Count;
        float separationY = (rightPanelRect.height - 50) / selectedEntities.Count;
        float startX = begginingOfRightPanel +  50, startY = 50;

        if (selectedEntities != null || selectedEntities.Count != 0) {
            drawNodes = true;
            foreach (BaseModel entity in selectedEntities) {
                // Draw Classes
                if (entity.IsClass()) {
                    ClassNode classNode = (ClassNode)CreateInstance("ClassNode");
                    ClassModel classModel = (ClassModel)entity;
                    classNode.Init((ClassModel)entity);
                    classNode.windowRect = new Rect(startX, startY, 150f, 150f);
                    windows.Add(classNode);
                }
                else {
                    // Draw Interfaces
                    InterfaceNode interfaceNode = (InterfaceNode)CreateInstance("InterfaceNode");
                    interfaceNode.Init((InterfaceModel)entity);
                    interfaceNode.windowRect = new Rect(startX, startY, 170f, 150f);
                    windows.Add(interfaceNode);
                }
                startX += separationX;
                startY += separationY;
            }

            foreach(BaseNode window in windows) {

                // Assign superclass or interface windows
                // Is it a class?
                try {
                    ClassNode classNode = (ClassNode)window;
                    ClassNode superClassNode = (ClassNode)GetWindowWithTitle(classNode.GetSuperClassName()); 
                    classNode.SetSuperClassNode(superClassNode);

                    // Assign Interface Windows
                    if (classNode.GetInterfaceNames() != null) {
                        List<InterfaceNode> interfacesToAdd = new List<InterfaceNode>();
                        foreach (string interface_ in classNode.GetInterfaceNames()) {
                            interfacesToAdd.Add(GetWindowWithTitle(interface_) as InterfaceNode);
                        }
                        //Debug.Log("Adding " + interfacesToAdd.Count + " to " + classNode.windowTitle);
                        classNode.SetInterfaceNodes(interfacesToAdd);
                    }
                }
                catch(Exception e) {
                    // Or an interface?
                    // Assign Interface Windows
                    InterfaceNode interfaceNode = window as InterfaceNode;
                    if (interfaceNode.GetInterfaceNames() != null) {
                        List<InterfaceNode> interfacesToAdd = new List<InterfaceNode>();
                        foreach (string interface_ in interfaceNode.GetInterfaceNames()) {
                            if(GetWindowWithTitle(interface_) == null) {
                                Debug.Log(interface_ + " Es nulo");
                            }
                            interfacesToAdd.Add(GetWindowWithTitle(interface_) as InterfaceNode);
                        }
                        interfaceNode.SetInterfaceNodes(interfacesToAdd);
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
        float minDistance = 2;

        if (selectedWindow != null) {
            Rect windowRect = selectedWindow.windowRect;
            //Check if mouse is in edge or corner
            // Left corner
            if (Mathf.Abs(windowRect.position.x - mousePos.x) <= minDistance) {
                // Top left corner
                if (Mathf.Abs(windowRect.position.y - mousePos.y) <= minDistance) {
                    cursor = MouseCursor.ResizeUpLeft;
                    EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeUpLeft);
                    resizeType = ResizeType.Top_Left;
                }
                else if (Mathf.Abs((windowRect.position.y + windowRect.height) - mousePos.y) <= minDistance) {
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
            else if (Mathf.Abs((windowRect.position.x + windowRect.width) - mousePos.x) <= minDistance) {
                // Top right corner
                if (Mathf.Abs(windowRect.position.y - mousePos.y) <= minDistance) {
                    cursor = MouseCursor.ResizeUpRight;
                    resizeType = ResizeType.Top_Right;
                    EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeUpRight);
                }
                else if (Mathf.Abs((windowRect.position.y + windowRect.height) - mousePos.y) <= minDistance) {
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
            else if (Mathf.Abs(windowRect.position.y - mousePos.y) <= minDistance) {
                cursor = MouseCursor.ResizeVertical;
                resizeType = ResizeType.Top;
                EditorGUIUtility.AddCursorRect(rightPanelRect, MouseCursor.ResizeVertical);
            }
            // Bottom edge
            else if (Mathf.Abs((windowRect.position.y + windowRect.height) - mousePos.y) <= minDistance) {
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

        DrawBezier(start, end, Color.red);

        // Draw the handle for Inheritance type
        float triangleHeight = 5f * 2/3;
        float positionX = end.x - 5;
        float positionY = end.y;
        Vector3[] positions = {
            new Vector3(triangleHeight + positionX, positionY),
            new Vector3(-triangleHeight + positionX, triangleHeight + positionY),
            new Vector3(-triangleHeight + positionX, -triangleHeight + positionY),
            new Vector3(triangleHeight + positionX, positionY)};
        Handles.DrawPolyLine(positions);
    }

    /// <summary>
    /// Draws the implementation curve.
    /// </summary>
    /// <param name="start">Start rect.</param>
    /// <param name="end">End rect.</param>
    public static void DrawImplementationCurve(Rect start, Rect end) {

        DrawBezier(start, end, Color.cyan);

        // Draw the handle for Inheritance type
        float triangleHeight = 5f * 2 / 3;
        float positionX = end.x - 5;
        float positionY = end.y;
        Vector3[] positions = {
            new Vector3(triangleHeight + positionX, positionY),
            new Vector3(-triangleHeight + positionX, triangleHeight + positionY),
            new Vector3(-triangleHeight + positionX, -triangleHeight + positionY),
            new Vector3(triangleHeight + positionX, positionY)};
        Handles.DrawPolyLine(positions);
    }

    // Draw the node curve from the middle of the start Rect to the middle of the end rect 
    public static void DrawBezier(Rect start, Rect end, Color mainColor) {

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

    private BaseNode GetWindowWithTitle(string windowTitle) { 
        
        foreach(BaseNode window in windows) {
            //if(window.windowTitle == windowTitle) {
            //    return window;
            //}
            //Debug.Log(window.windowTitle);
            try {
                ClassNode classNode = (ClassNode)window;
                if (classNode.windowTitle == windowTitle) {
                    return classNode;
                }
            }
            catch {
                InterfaceNode interfaceNode = (InterfaceNode)window;
                if(interfaceNode.windowTitle == ("<<" + windowTitle + ">>")) { 
                    return interfaceNode;
                }
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