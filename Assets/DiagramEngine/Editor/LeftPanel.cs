using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.
using UnityEditor;
using DEngine.Model;

public class LeftPanel : EditorWindow{

    public static bool DrawLeftPanel(ref int selected, List<BaseModel> entities, ref Vector2 scrollPos) {

        bool generateDiagramButton = false;
        string[] options = { "All!", "Drag & Drop", "CheckBox" };
        selected = GUILayout.Toolbar(selected, options, GUILayout.MinWidth(240));

        switch (selected) {

            case 0:
                GUILayout.Label("Found Classe/Interfaces: ", EditorStyles.boldLabel);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                foreach (var entity in entities) {
                    GUILayout.Label(entity.GetName());
                }
                EditorGUILayout.EndScrollView();
                generateDiagramButton = GUILayout.Button("Generate!");
                break;
            case 1:
                GUILayout.Label("Coming Soon!");
                break;
            case 2:
                GUILayout.Label("Coming Soon!");
                break;
            default:
                break;
        }

        return generateDiagramButton;
    }
}
