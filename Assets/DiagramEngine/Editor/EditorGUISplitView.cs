using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorGUISplitView
{
    public bool Resized { get { return resize; } }

	public enum Direction {
		Horizontal,
		Vertical
	}

	Direction splitDirection;
	public float splitNormalizedPosition;
	bool resize;
	public Vector2 scrollPosition;
	public Rect availableRect;


	public EditorGUISplitView(Direction splitDirection) {
		splitNormalizedPosition = 0.32f;
		this.splitDirection = splitDirection;
	}

	public void BeginSplitView() {
		Rect tempRect;

		if(splitDirection == Direction.Horizontal)
			tempRect = EditorGUILayout.BeginHorizontal (GUILayout.ExpandWidth(true));
		else 
			tempRect = EditorGUILayout.BeginVertical (GUILayout.ExpandHeight(true));
		
		if (tempRect.width > 0.0f) {
			availableRect = tempRect;
		}
		if(splitDirection == Direction.Horizontal)
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(availableRect.width * splitNormalizedPosition));
		else
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(availableRect.height * splitNormalizedPosition));

	}

	public void Split() {
		GUILayout.EndScrollView();
		ResizeSplitFirstView ();
	}

	public void EndSplitView() {

		if(splitDirection == Direction.Horizontal)
			EditorGUILayout.EndHorizontal ();
		else 
			EditorGUILayout.EndVertical ();
	}

	private void ResizeSplitFirstView(){

		Rect resizeHandleRect;
        Texture2D texture;

        if (splitDirection == Direction.Horizontal) {
            texture = (Texture2D)AssetDatabase.LoadMainAssetAtPath("Assets/DiagramEngine/Textures/shadow_vertical.png");
            resizeHandleRect = new Rect(availableRect.width * splitNormalizedPosition, availableRect.y, 5f, availableRect.height);
        }
        else {
            texture = (Texture2D)AssetDatabase.LoadMainAssetAtPath("Assets/DiagramEngine/Textures/shadow_horizontal.png");
            resizeHandleRect = new Rect(availableRect.x, availableRect.height * splitNormalizedPosition, availableRect.width, 5f);
        }


        GUI.DrawTexture(resizeHandleRect, texture);

        if (splitDirection == Direction.Horizontal)
			EditorGUIUtility.AddCursorRect(resizeHandleRect,MouseCursor.ResizeHorizontal);
		else
			EditorGUIUtility.AddCursorRect(resizeHandleRect,MouseCursor.ResizeVertical);

		if( Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition)){
			resize = true;
		}
		if(resize){
			if(splitDirection == Direction.Horizontal)
				splitNormalizedPosition = Event.current.mousePosition.x / availableRect.width;
			else
				splitNormalizedPosition = Event.current.mousePosition.y / availableRect.height;

            //Limiters
            splitNormalizedPosition = splitNormalizedPosition < 0.15f ? 0.15f : splitNormalizedPosition;
            splitNormalizedPosition = splitNormalizedPosition > 0.85f ? 0.85f : splitNormalizedPosition;
        }
		if(Event.current.type == EventType.MouseUp)
			resize = false;        
	}
}

