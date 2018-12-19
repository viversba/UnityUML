using UnityEngine;
using System.Collections;
using UnityEditor;

//We use scriptable object because in the future we will need the messages that unity calls on scriptable objects
//such as OnDestroy() OnEnable()
public abstract class BaseNode: ScriptableObject {

	//Store the Rect of the window, location and size
	public Rect windowRect;

	//Indicates if the decendant of the baseNode has inputs or can only be used as an input to another node
	public bool hasInputs = false;

	//the title of our window
	public string windowTitle = "";

	//Draw the window of the base node, this is virtual and will be implemented by each subclass of baseNode
	public virtual void DrawWindow() {
		//We want each node to have a title which the user can modify
		windowTitle = EditorGUILayout.TextField("Title", windowTitle);
	}

	//Draws the curves from the inputs of this nodes, this is abstract because it must be implemented by each subclass of this baseNode
	public abstract void DrawCurves();

	//Is used by the subclasses nodes that has inputs and is called when the window is clicked during a transition
	//we make it virtual because it's optional for nodes that don't have inputs
	//it is passed in the metho the node to be used as input and the click position of the mouse
	public virtual void SetInput(BaseInputNode input, Vector2 clickPos) {

	}

	//is called to all nodes when a node is deleted so that it is removed if it used as an input
	public virtual void NodeDeleted(BaseNode node) {

	}

	//is called when a click happens on a window and returns the input that was clicked or null otherwise
	public virtual BaseInputNode ClickedOnInput(Vector2 pos) {
		return null;
	}

	public abstract void Tick (float deltaTime);
}
