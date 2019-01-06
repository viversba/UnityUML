using UnityEngine;
using UnityEditor;

public class ScaleTest : EditorWindow {
    private static Texture2D tex;

    [MenuItem("UML/ScaleTest")]
    static void Init() {
        EditorWindow.GetWindow(typeof(ScaleTest));
        tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, new Color(0.25f, 0.4f, 0.25f));
        tex.Apply();
    }

    float zoomScale = 1.0f;
    Vector2 vanishingPoint = new Vector2(0, 21);

    void OnGUI() {

        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);
        GUI.Label(new Rect(200, 200, 100, 100), "A label");
        GUI.TextField(new Rect(20, 20, 70, 30), "");
        Matrix4x4 oldMatrix = GUI.matrix;

        //Scale my gui matrix
        Matrix4x4 Translation = Matrix4x4.TRS(vanishingPoint, Quaternion.identity, Vector3.one);
        Matrix4x4 Scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
        GUI.matrix = Translation * Scale * Translation.inverse;

        // Draw the GUI
        GUI.Button(new Rect(0, 0, position.width * 1.5f, 30), "Foo");
        GUI.Button(new Rect(0, 35, 50, 30), "Bar");
        EditorGUI.FloatField(new Rect(0, 70, 50, 30), 0.0f);

        //reset the matrix
        GUI.matrix = oldMatrix;

        // Just for testing (unscaled controls at the bottom)
        GUILayout.FlexibleSpace();
        vanishingPoint = EditorGUILayout.Vector2Field("vanishing point", vanishingPoint);
        zoomScale = EditorGUILayout.Slider("zoom", zoomScale, 1.0f / 25.0f, 2.0f);
    }

}