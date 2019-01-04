using UnityEngine;
using System.Collections;
using UnityEditor;

public class GameObjectActive : BaseNode {

	private BaseInputNode input1;
	private Rect input1Rect;
	
	private GameObject controlledObject;

	public override void DrawWindow ()
	{
		base.DrawWindow();
		
		Event e = Event.current;
		
		string input1Title = "None";
		if(input1 != null) {
			input1Title = input1.getResult();
		}
		
		//draw a label
		GUILayout.Label("Input 1: " + input1Title);
		
		//same as before
		if(e.type == EventType.Repaint) {
			input1Rect = GUILayoutUtility.GetLastRect();
			
		}
		
		//we make the object field for the user to drop tha gameobject
		controlledObject = (GameObject) EditorGUILayout.ObjectField(controlledObject, typeof(GameObject), true);
		
		
	}

	public override void Tick(float deltaTime) {
		if (input1 != null) {
			
			if (controlledObject) {
				if (input1.getResult ().Equals ("true")) {
					
					controlledObject.SetActive (true);
					
				} else {
					controlledObject.SetActive (false);
				}
				
			}
			
		}
	}

		public override void SetInput (BaseInputNode input, Vector2 clickPos)
		{
			clickPos.x -= windowRect.x;
			clickPos.y -= windowRect.y;
			
			if(input1Rect.Contains(clickPos)) {
				input1 = input;
				
			}
		}
		
		public override void NodeDeleted (BaseNode node)
		{
			if(node.Equals (input1)) {
				input1 = null;
			}
		}
		
		public override BaseInputNode ClickedOnInput (Vector2 pos)
		{
			BaseInputNode retVal = null;
			
			pos.x -= windowRect.x;
			pos.y -= windowRect.y;
			
			if(input1Rect.Contains(pos)) {
				retVal = input1;
				input1 = null;
			}
			
			return retVal;
		}
		
		public override void DrawCurves ()
		{
			if(input1 != null) {
				Rect rect = windowRect;
				rect.x += input1Rect.x;
				rect.y += input1Rect.y + input1Rect.height / 2;
				rect.width = 1;
				rect.height = 1;
				
				NodeEditor.DrawNodeCurve(input1.windowRect, rect);
			}
		}
	}

