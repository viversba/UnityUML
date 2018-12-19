using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class NodeEditor: EditorWindow {

    private List<BaseNode> windows = new List<BaseNode>();
    private Vector2 mousePos;
    private BaseNode selectedNode;

    private bool makeTransitionMode;

    [MenuItem("UML/Node Editor")]
    public static void Show() { 
        
    }

    public static void DrawNodeCurve(Rect start, Rect end) { 
    

    }
}
