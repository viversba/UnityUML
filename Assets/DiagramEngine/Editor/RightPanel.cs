using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class RightPanel: EditorWindow{

    private static Texture2D backgroundTexture;

    public static void Init() {

        string filePath = "./Assets/DiagramEngine/Textures/grid_texture.jpg";
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            backgroundTexture = new Texture2D(2, 2);
            backgroundTexture.LoadImage(fileData);
        }
    }

    public static void DrawRightPanel(ref int selected, Vector2 maxSize, float begginingOfRightPanel) {

        backgroundTexture.Apply();
        backgroundTexture.wrapMode = TextureWrapMode.Repeat;

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
