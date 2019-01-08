using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class RightPanel: EditorWindow{

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

    //private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

    float PanY;
    float PanX;

    private bool scrollWindow = false;
    private Vector2 scrollStartMousePos;

    public void Awake() {

        string filePath = "./Assets/DiagramEngine/Textures/grid_texture.jpg";
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            backgroundTexture = new Texture2D(2, 2);
            backgroundTexture.LoadImage(fileData);
        }
    }

    public void DrawRightPanel(Vector2 maxSize, float begginingOfRightPanel) {

        backgroundTexture.Apply();
        backgroundTexture.wrapMode = TextureWrapMode.Repeat;

        // Draw the texture first
        GUI.DrawTexture(new Rect(begginingOfRightPanel, 0, maxSize.x, maxSize.y), backgroundTexture, ScaleMode.ScaleAndCrop);
        GUI.DrawTextureWithTexCoords(new Rect(begginingOfRightPanel, 0, maxSize.x, maxSize.y), backgroundTexture, new Rect(0, 0, maxSize.x / backgroundTexture.width, maxSize.y / backgroundTexture.height));
        Matrix4x4 oldMatrix = GUI.matrix;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Centered text");
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
