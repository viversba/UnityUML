using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using DEngine.Model;
using DEngine.View;

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
            EndWindows();
        }
    }


    /// <summary>
    /// Sets the local list of windows so that all entities have a window
    /// </summary>
    /// <param name="entities">List of entities.</param>
    public void CreateWindowList(List<BaseModel> entities) {

        if (entities != null || entities.Count != 0) {
            drawNodes = true;
            foreach (BaseModel entity in entities) {
                // Draw Classes
                if (entity.IsClass()) {
                    ClassNode classNode = (ClassNode)CreateInstance("ClassNode");
                    classNode.Init((ClassModel)entity);
                    classNode.windowRect = new Rect(100f + begginingOfRightPanel, 100f, 150f, 150f);
                    windows.Add(classNode);
                }
                else {
                    // Draw Interfaces
                    InterfaceNode interfaceNode = (InterfaceNode)CreateInstance("InterfaceNode");
                    interfaceNode.Init((InterfaceModel)entity);
                    interfaceNode.windowRect = new Rect(100f + begginingOfRightPanel, 100f, 170f, 150f);
                    windows.Add(interfaceNode);
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