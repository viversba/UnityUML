using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using DEngine.View;

//inherits from EditorWindow  
public class NodeEditor : EditorWindow {

	//list that stores our windows
	private List<BaseNode> windows = new List<BaseNode>();

	//variable to store our mousePos
	private Vector2 mousePos;

	//variable to store a selected node
	private BaseNode selectedNode;
	//variable to determine if we are on a transition mode
	private bool makeTransitionMode = false;

	//private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

	float PanY;
	float PanX;

	private bool scrollWindow = false;
	private Vector2 scrollStartMousePos;

	//In order to be accessible the window from the menue we add a menu item
	[MenuItem("UML/Node Editor")]
	static void ShowEditor() {
		NodeEditor editor  = EditorWindow.GetWindow<NodeEditor>();

		//editor.stopWatch.Start ();
	}

	void Update()
	{
		//long dTime = stopWatch.ElapsedMilliseconds;

		//float deltaTime = ((float)dTime) / 1000;

		//foreach(BaseNode b in windows)
		//{
		//	b.Tick(deltaTime);

		//}

		//stopWatch.Reset();
		//stopWatch.Start();
		
		Repaint ();
	}

	void OnGUI() {
		//check event
		Event e = Event.current;

		//check mouse position
		mousePos = e.mousePosition;

		if(e.button == 1 && !makeTransitionMode) 
		{
			if(e.type == EventType.MouseDown) 
			{
				bool clickedOnWindow = false;
				int selectedIndex = -1;

				//check to see if he clicked inside a window
				for(int i=0; i<windows.Count; i++) 
				{
					if(windows[i].windowRect.Contains(mousePos)) 
					{
						//if he clicked store the i
						selectedIndex = i;
						//we clicked on a window
						clickedOnWindow = true;
						//we have a window so we don't need to check for another one
						break;
					}
				}

				//if we didn't clicked a window
				if(!clickedOnWindow) {
					//make a new menu for every different case 
					GenericMenu menu = new GenericMenu ();

					menu.AddItem(new GUIContent("Add Input Node"), false, ContextCallback, "inputNode");
					menu.AddItem(new GUIContent("Add Output Node"), false, ContextCallback, "outputNode");
					menu.AddItem(new GUIContent("Add Calculation Node"), false, ContextCallback, "calcNode");
					menu.AddItem(new GUIContent("Add Comparison Node"), false, ContextCallback, "compNode");
                    menu.AddItem(new GUIContent("Add Class Node"), false, ContextCallback, "classNode");
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Add GameObject Active Node"), false, ContextCallback, "goActive");
					menu.AddItem(new GUIContent("Add GameObject Distance Node"), false, ContextCallback, "goDistance");
					menu.AddItem(new GUIContent("Add Timer Node"), false, ContextCallback, "timerNode");

					menu.ShowAsContext ();

                    e.Use();
				} 
					else 
				{
					//if it clicked on a window add items to make transition or delete node
					GenericMenu menu = new GenericMenu ();
			
					menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, "makeTransition");
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");

					//we use it so that it will show
					menu.ShowAsContext ();
					//consumes the event
					e.Use();
				}
			}
		} //if we are in a transition mode and there is a left click
		else if(e.button == 0 && e.type == EventType.MouseDown && makeTransitionMode) 
		{
			bool clickedOnWindow = false;
			int selectedIndex = -1;

			//find which window was clicked
			for(int i=0; i<windows.Count; i++) 
			{
				if(windows[i].windowRect.Contains(mousePos)) {
					selectedIndex = i;
					clickedOnWindow = true;
					break;
				}
			}

			//if there is a click on a window and it's not the window that the transition started from
			if(clickedOnWindow && !windows[selectedIndex].Equals(selectedNode)) 
			{
				//call the set Input on the selected window in order to evaluate if the selected node will be used as input
				windows[selectedIndex].SetInput((BaseInputNode) selectedNode, mousePos);
				makeTransitionMode = false;
				selectedNode = null;
			}

			//if we didn't clicked on a window
			if(!clickedOnWindow) 
			{
				//then stop the transition
				makeTransitionMode = false;
				selectedNode = null;
			}

			e.Use();
		}  //if there is a left click and we are not in a transition mode
		else if(e.button == 0 && e.type == EventType.MouseDown && !makeTransitionMode) 
		{
			bool clickedOnWindow = false;
			int selectedIndex = -1;

			//same as above
			for(int i=0; i<windows.Count; i++)
			{
				if(windows[i].windowRect.Contains(mousePos)) {
					selectedIndex = i;
					clickedOnWindow = true;
					break;
				}
			}

			//if we clicked on a window
			if(clickedOnWindow) 
			{
				//evaluate if an input of the node was clicked
				BaseInputNode nodeToChange = windows[selectedIndex].ClickedOnInput(mousePos);

				//if node to change is not null means that an input was clicked
				if(nodeToChange != null) 
				{
					//so set the node to the transition mode
					selectedNode = nodeToChange;
					makeTransitionMode = true;
				}
			}
		}

		//if we are in a transition mode and there is a selected node
		if(makeTransitionMode && selectedNode != null)
		{
			//draw the curve from the selected node to the mouse position
			Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);

			DrawNodeCurve(selectedNode.windowRect, mouseRect);

				Repaint ();
		}


		GUI.BeginGroup(new Rect(PanX, PanY, 100000, 100000));

		//draw each curve for every node
		foreach(BaseNode n in windows) {
			n.DrawCurves();
		}

		//draw the actual windows
		BeginWindows ();

		for(int i=0; i<windows.Count; i++) 
		{
			windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
		}
		EndWindows ();
		GUI.EndGroup();

		if(e.keyCode == KeyCode.A && e.type == EventType.KeyDown) {
			if(scrollWindow == true) {
				scrollWindow = false;
			} else {
				scrollStartMousePos = e.mousePosition;
				scrollWindow = true;
			}
		}
		
		if(e.button == 2) {
			if( e.type == EventType.MouseDown) {
				scrollStartMousePos = e.mousePosition;
				scrollWindow = true;
			} else if(e.type == EventType.MouseUp) {
				scrollWindow = false;
			}
		}
		
		if(scrollWindow) {
			Vector2 mouseDiff = e.mousePosition - scrollStartMousePos;
			PanX += mouseDiff.x / 100;
			PanY += mouseDiff.y / 100;
			
		}

	
	}

	//function that draws the windows
	void DrawNodeWindow(int id) 
	{
		windows[id].DrawWindow();
		GUI.DragWindow();
	}


	//Is called when a selection from the context menu is made
	void ContextCallback(object obj)
	{
		//make the passed object to a string
		string clb = obj.ToString();

        //add the node we want
        if (clb.Equals("inputNode"))
        {

            InputNode inputNode = (InputNode)ScriptableObject.CreateInstance("InputNode");
            inputNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 150);

            windows.Add(inputNode);

        }
        else if (clb.Equals("outputNode"))
        {
            OutputNode outputNode = (OutputNode)ScriptableObject.CreateInstance("OutputNode");
            outputNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            windows.Add(outputNode);

        }
        else if (clb.Equals("calcNode"))
        {
            CalcNode calcNode = (CalcNode)ScriptableObject.CreateInstance("CalcNode");
            calcNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            windows.Add(calcNode);

        }
        else if (clb.Equals("compNode"))
        {
            ComparisonNode compNode = (ComparisonNode)ScriptableObject.CreateInstance("ComparisonNode");
            compNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            windows.Add(compNode);

        }
        else if (clb.Equals("goActive"))
        {
            GameObjectActive goNode = (GameObjectActive)ScriptableObject.CreateInstance("GameObjectActive");
            goNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);
            windows.Add(goNode);
        }
        else if (clb.Equals("goDistance"))
        {
            GameObjectDistance goDistance = (GameObjectDistance)ScriptableObject.CreateInstance("GameObjectDistance");
            goDistance.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);
            windows.Add(goDistance);

        }
        else if (clb.Equals("timerNode"))
        {

            TimerNode tNode = (TimerNode)ScriptableObject.CreateInstance("TimerNode");
            tNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);
            windows.Add(tNode);


        } //if it's a transition
        else if (clb.Equals("makeTransition"))
        {
            bool clickedOnWindow = false;
            int selectedIndex = -1;
            //find the window that it was clicked
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }

            //and make it the selected node of the transition
            if (clickedOnWindow)
            {
                selectedNode = windows[selectedIndex];
                makeTransitionMode = true;
            }

        }
        else if (clb.Equals("deleteNode")) //if it's a delete node
        {
            bool clickedOnWindow = false;
            int selectedIndex = -1;

            //find the selected node
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }


            if (clickedOnWindow)
            {
                //delete it from our list
                BaseNode selNode = windows[selectedIndex];
                windows.RemoveAt(selectedIndex);

                //then pass it to all our nodes that is deleted
                foreach (BaseNode n in windows)
                {
                    n.NodeDeleted(selNode);
                }
            }
        }
        else if (clb.Equals("classNode"))
        {
            ClassNode classNode = (ClassNode)ScriptableObject.CreateInstance("ClassNode");
            //ClassNode classNode = new ClassNode();
            classNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 80 + 22 * classNode.numberOfLines);
            Debug.Log("Number of lines: " + classNode.numberOfLines);
            windows.Add(classNode);
        } 

		//we use else if instead of a switch because:
		/*Selecting from a set of multiple cases is faster with if statements than with switch 
		 */
	}

	//draw the node curve from the middle of the start Rect to the middle of the end rect 
	public static void DrawNodeCurve(Rect start, Rect end) 
	{
		
		Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
		Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
		Vector3 startTan = startPos + Vector3.right * 50;
		Vector3 endTan = endPos + Vector3.left * 50;
		Color shadowCol = new Color(0, 0, 0, 0.06f);
		
		for (int i = 0; i < 3; i++) {// Draw a shadow
			Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
		}
		
		Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
	}

}
