using UnityEngine;
using UnityEditor;
using System.Collections;

public class CalcNode: BaseInputNode {

	//variables for our input nodes
	private BaseInputNode input1;
	private Rect input1Rect;

	private BaseInputNode input2;
	private Rect input2Rect;

	//the calculation types we want to have
	private CalculationType calculationType;

	public enum CalculationType {
		Addition,
		Subtraction,
		Multiplication,
		Division
	}

	//give a title to the window and set it to have input
	public CalcNode() {
		windowTitle = "Calculation Node";
		hasInputs = true;
	}

	public override void DrawWindow() {
		base.DrawWindow();

		//check for events
		Event e = Event.current;
		//make a popup for the user to select the calculation type
		calculationType = (CalculationType) EditorGUILayout.EnumPopup("Calculation Type", calculationType);


		string input1Title = "None";

		//if there is input get the result
		if(input1) 
		{
			input1Title = input1.getResult();
		}

		//draw a label
		GUILayout.Label("Input 1: " + input1Title);
		
		if(e.type == EventType.Repaint) {
			input1Rect = GUILayoutUtility.GetLastRect();
			
		}
		
		string input2Title = "None";
		if(input2) {
			input2Title = input2.getResult();
		}
		
		GUILayout.Label("Input 2: " + input2Title);
		
		if(e.type == EventType.Repaint) {
			input2Rect = GUILayoutUtility.GetLastRect();
			
		}
	}


	public override void SetInput (BaseInputNode input, Vector2 clickPos)
	{
		clickPos.x -= windowRect.x;
		clickPos.y -= windowRect.y;
		
		if(input1Rect.Contains(clickPos)) {
			input1 = input;
			
		} else if(input2Rect.Contains(clickPos)) {
			input2 = input;
		}
	}

	public override void DrawCurves ()
	{
		if(input1) {
			Rect rect = windowRect;
			rect.x += input1Rect.x;
			rect.y += input1Rect.y + input2Rect.height / 2;
			rect.width = 1;
			rect.height = 1;

			NodeEditor.DrawNodeCurve(input1.windowRect, rect);
		}

		if(input2) {
			Rect rect = windowRect;
			rect.x += input2Rect.x;
			rect.y += input2Rect.y + input2Rect.height / 2;
			rect.width = 1;
			rect.height = 1;

			NodeEditor.DrawNodeCurve(input2.windowRect, rect);
		}
	}


	public override void Tick(float deltaTime)
	{

		float input1Value = 0;
		float input2Value = 0;
		
		if(input1) 
		{
			//get the result from the first input
			string input1Raw = input1.getResult();
			//try to make it a float
			float.TryParse(input1Raw, out input1Value);
		}

		//same as above
		if(input2) {
			string input2Raw = input2.getResult();
			float.TryParse(input2Raw, out input2Value);
		}

		//by default the result is falce
		string result = "false";

		//switch statement for each calculation type
		switch(calculationType) {
		case CalculationType.Addition:

			result = (input1Value + input2Value).ToString();
			break;


		case CalculationType.Division:
			result = (input1Value / input2Value).ToString();
			break;

		case CalculationType.Multiplication:
			result = (input1Value * input2Value).ToString();
			break;

		case CalculationType.Subtraction:
			result = (input1Value - input2Value).ToString();
			break;
		}

		nodeResult = result;

	}

	public override BaseInputNode ClickedOnInput (Vector2 pos)
	{
		BaseInputNode retVal = null;

		pos.x -= windowRect.x;
		pos.y -= windowRect.y;
		
		if(input1Rect.Contains(pos)) {
			retVal = input1;
			input1 = null;

		} else if(input2Rect.Contains(pos)) {
			retVal = input2;
			input2 = null;
		}

		return retVal;
	}

	public override void NodeDeleted (BaseNode node)
	{
		if(node.Equals (input1)) {
			input1 = null;
		}

		if(node.Equals(input2)) {
			input2 = null;
		}
	}



}
