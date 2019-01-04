using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RightPanel: EditorWindow{

    public static void DrawRightPanel(ref int selected) {

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
