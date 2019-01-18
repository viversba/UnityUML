using UnityEngine;
using System.Collections;
using UnityEditor;

//We use scriptable object because in the future we will need the messages that unity calls on scriptable objects
//such as OnDestroy() OnEnable()
public abstract class BaseNode: ScriptableObject {
    
	// Store the Rect of the window, location and size
	public Rect windowRect;
	// Indicates if the decendant of the baseNode has inputs or can only be used as an input to another node
	public bool hasInputs = false;
	// The title of our window
	public string windowTitle = "";
    // Minimun values for width and height;
    float minWidth, minHeight;
    // This will tell the initial position of the mouse
    private Vector2 initialResize;

	//Draw the window of the base node, this is virtual and will be implemented by each subclass of baseNode
	public virtual void DrawWindow() {
		//We want each node to have a title which the user can modify
		windowTitle = EditorGUILayout.TextField("Title", windowTitle);
	}

	//Draws the curves from the inputs of this nodes, this is abstract because it must be implemented by each subclass of this baseNode
	public abstract void DrawCurves();

	//is called to all nodes when a node is deleted so that it is removed if it used as an input
	public virtual void NodeDeleted(BaseNode node) {

	}

    public string GetWindowTitle() {
        return windowTitle;
    }

    public void ResizeWindow(Vector2 newMousePosition, ResizeType type) {

        minWidth = 100f;
        minHeight = 70f;
        // Store the current dimensions in case we need to use them
        Vector2 initialPosition = windowRect.position;
        switch (type) {
            case ResizeType.Top_Left:
                ResizeWindow(newMousePosition, ResizeType.Top);
                ResizeWindow(newMousePosition, ResizeType.Left);
                break;
            case ResizeType.Top_Right:
                ResizeWindow(newMousePosition, ResizeType.Top);
                ResizeWindow(newMousePosition, ResizeType.Right);
                break;
            case ResizeType.Bottom_Left:
                ResizeWindow(newMousePosition, ResizeType.Bottom);
                ResizeWindow(newMousePosition, ResizeType.Left);
                break;
            case ResizeType.Bottom_Right:
                ResizeWindow(newMousePosition, ResizeType.Bottom);
                ResizeWindow(newMousePosition, ResizeType.Right);
                break;
            case ResizeType.Top:
                if (windowRect.height + initialPosition.y - newMousePosition.y >= minHeight) {
                    windowRect.position = new Vector2(windowRect.position.x, newMousePosition.y);
                    windowRect.height += initialPosition.y - newMousePosition.y;
                }
                else {
                    if (windowRect.height != minHeight)
                        windowRect.position = new Vector2(windowRect.position.x, windowRect.position.y + windowRect.height - minHeight);
                    windowRect.height = minHeight;
                }
                break;
            case ResizeType.Bottom:
                if(newMousePosition.y - windowRect.position.y >= minHeight) {
                    windowRect.height = newMousePosition.y - windowRect.position.y;
                }
                else {
                    windowRect.height = minHeight;
                }
                break;
            case ResizeType.Right:
                if (newMousePosition.x - windowRect.position.x >= minWidth) {
                    windowRect.width = newMousePosition.x - windowRect.position.x;
                }
                else {
                    windowRect.width = minWidth;
                }
                break;
            case ResizeType.Left:
                if (windowRect.width + initialPosition.x - newMousePosition.x >= minWidth) {
                    windowRect.position = new Vector2(newMousePosition.x, windowRect.position.y);
                    windowRect.width += initialPosition.x - newMousePosition.x;
                }
                else {
                    if (windowRect.width != minWidth)
                        windowRect.position = new Vector2(windowRect.position.x + windowRect.width - minWidth, windowRect.position.y);
                    windowRect.width = minWidth;
                }
                break;
        }
    }
}

public enum ResizeType{

    Bottom_Left,
    Bottom_Right,
    Top_Left,
    Top_Right,
    Right,
    Left,
    Top,
    Bottom,
    None
}
