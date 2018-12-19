using UnityEngine;
using UnityEditor;

public abstract class BaseNode: ScriptableObject{

    /// <summary>
    /// Rectangle containing the node
    /// </summary>
    public Rect windowRect;

    public bool hasInputs;

    /// <summary>
    /// Title of the window
    /// </summary>
    public string windowTitle = "";

    public virtual void DrawWindow() {

        windowTitle = EditorGUILayout.TextField("Title", windowTitle);
    }

    public abstract void DrawCurves();

    public virtual void SetInput(BaseInputNode inputNode, Vector2 clickPos) { 
    
    }

    public virtual void NodeDeleted(BaseNode node) {}

    /// <summary>
    /// Called when a click happens on a window. 
    /// Tells the input that it has clicked, or if it doesn't, then returns null
    /// </summary>
    /// <returns>The on input.</returns>
    /// <param name="pos">Position.</param>
    public virtual BaseInputNode ClickedOnInput(Vector2 pos) {

        return null;
    }
}
