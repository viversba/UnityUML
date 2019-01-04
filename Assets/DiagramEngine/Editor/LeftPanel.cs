using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEngine.Model;

public class LeftPanel : EditorWindow{

    public static void DrawLeftPanel(ref int selected, List<BaseModel> entities) {

        string[] options = { "All!", "Drag & Drop", "CheckBox" };
        selected = GUILayout.Toolbar(selected, options, GUILayout.MinWidth(240));

        switch (selected) {

            case 0:
                foreach(var entity in entities) {
                    GUILayout.Label(entity.GetName());
                }
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
    }
}
