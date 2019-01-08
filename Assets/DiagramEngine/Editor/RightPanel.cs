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

    //variable to store our mousePos
    private Vector2 mousePos;

    //variable to store a selected node
    private BaseNode selectedNode;
    //variable to determine if we are on a transition mode
    private bool makeTransitionMode = false;
    // Indicates if the nodes should be showing
    private bool drawNodes;
    // Indicates the position of begginig of the right panel
    private float begginingOfRightPanel;

    float PanY;
    float PanX;

    private bool scrollWindow = false;
    private Vector2 scrollStartMousePos;

    public void Awake() {

        string filePath = "./Assets/DiagramEngine/Textures/grid_texture.jpg";
        byte[] fileData;
        drawNodes = false;

        if (File.Exists(filePath)) {
            fileData = File.ReadAllBytes(filePath);
            backgroundTexture = new Texture2D(2, 2);
            backgroundTexture.LoadImage(fileData);
        }
    }

    public void DrawRightPanel(Vector2 maxSize) {

        backgroundTexture.Apply();
        backgroundTexture.wrapMode = TextureWrapMode.Repeat;

        // Draw the texture first
        GUI.DrawTexture(new Rect(begginingOfRightPanel, 0, maxSize.x, maxSize.y), backgroundTexture, ScaleMode.ScaleAndCrop);
        GUI.DrawTextureWithTexCoords(new Rect(begginingOfRightPanel, 0, maxSize.x, maxSize.y), backgroundTexture, new Rect(0, 0, maxSize.x / backgroundTexture.width, maxSize.y / backgroundTexture.height));
        Matrix4x4 oldMatrix = GUI.matrix;

        if (drawNodes) {

            //draw the actual windows
            BeginWindows();

            for (int i = 0; i < windows.Count; i++) {
                windows[i].windowRect.x = windows[i].windowRect.x < begginingOfRightPanel ? begginingOfRightPanel : windows[i].windowRect.x;
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
            }
            EndWindows();
        }

        //GUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        //GUILayout.BeginVertical();
        //GUILayout.FlexibleSpace();
        //GUILayout.Label("Centered text");
        //GUILayout.FlexibleSpace();
        //GUILayout.EndVertical();
        //GUILayout.FlexibleSpace();
        //GUILayout.EndHorizontal();
    }

    public void DrawNodes(List<BaseModel> entities) {

        if (entities != null || entities.Count != 0) {
            drawNodes = true;
            foreach(BaseModel entity in entities) {
                if (entity.IsClass()) {
                    ClassNode classNode = (ClassNode)CreateInstance("ClassNode");
                    classNode.Init((ClassModel) entity);
                    classNode.windowRect = new Rect(100f + begginingOfRightPanel, 100f, 150f, 150f);
                    windows.Add(classNode);
                }
            }
            //windows.AddRange(entities);
        }
        else {
            Debug.LogError("Empty or null list trying to be drawn");
        }
    }

    //function that draws the windows
    void DrawNodeWindow(int id) {
        windows[id].DrawWindow();
        GUI.DragWindow();
    }

    public void SetBegginingOfRightPanel(float begginingOfRightPanel) {
        this.begginingOfRightPanel = begginingOfRightPanel;
    }

    void ContextCallback(object obj) {
    }
}