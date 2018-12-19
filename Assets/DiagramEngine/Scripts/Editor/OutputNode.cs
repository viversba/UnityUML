using UnityEngine;
using System.Collections;
using UnityEditor;

//inherits from base node
public class OutputNode: BaseNode {

	//variable we use for visualizing the result
	private string result = "";

	//The output node has only one input
	private BaseInputNode inputNode;
	private Rect inputNodeRect;

	//we give a title to the output node
	//and can take an input
	public OutputNode() {
		windowTitle = "Output Node";
		hasInputs = true;
	}

	public override void Tick(float deltaTime) {
		
	}
	
	public override void DrawWindow ()
	{
		base.DrawWindow();

		//check for events
		Event e = Event.current;

		//variable to help us visualize if there is an input node attached
		string input1Title = "None";

		//if there is an input node
		if(inputNode) 
		{
			//the input title is the result of the attached node
			input1Title = inputNode.getResult();
		}

		//draw a label with our value
		GUILayout.Label("Input 1: " + input1Title);

		//We check when the window is repainted
		if(e.type == EventType.Repaint) 
		{
			//and we store the rect of the last item, which is the input rect
			inputNodeRect = GUILayoutUtility.GetLastRect();
			
		}

		//draw a label with the result
		GUILayout.Label("Result: " + result);
	}


	public override void DrawCurves ()
	{
		if(inputNode) 
		{
			//draws a curve from the Input node to this node
			Rect rect = windowRect;
			rect.x += inputNodeRect.x;
			rect.y += inputNodeRect.y + inputNodeRect.height / 2;
			rect.width = 1;
			rect.height = 1;
			
			NodeEditor.DrawNodeCurve(inputNode.windowRect, rect);
		}
	}


	//we call it when we want to delete this node and checks if the deleted node is it's input and if it is, removes the input
	public override void NodeDeleted (BaseNode node)
	{
		if(node.Equals (inputNode)) {
			inputNode = null;
		}

	}

	//Is called when a click happens in the window
	public override BaseInputNode ClickedOnInput (Vector2 pos)
	{

		BaseInputNode retVal = null;

		//convert the global input position to local space
		pos.x -= windowRect.x;
		pos.y -= windowRect.y;

		//if the click is on top of the input label it returns the associated input
		if(inputNodeRect.Contains(pos)) {
			retVal = inputNode;
			inputNode = null;
			
		}

		return retVal;
	}

	//is called when the window is clicked during a transition, if the click happens on top of a input label
	//it associates the passed node with that input
	public override void SetInput (BaseInputNode input, Vector2 clickPos)
	{
		clickPos.x -= windowRect.x;
		clickPos.y -= windowRect.y;
		
		if(inputNodeRect.Contains(clickPos)) {
			inputNode = input;
			
		}
	}
}
