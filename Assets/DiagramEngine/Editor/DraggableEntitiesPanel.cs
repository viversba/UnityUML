using System;
using UnityEngine;

namespace DEngine.View {

    public class DraggableEntitiesPanel: GUIDraggableObject{

        private string name;
        private int value;

        public DraggableEntitiesPanel(string name, int value, Vector2 position): base(position) {

            this.name = name;
            this.value = value;
        }

        public void OnGUI() {

            Rect drawRect = new Rect(Position.x, Position.y, 100f, 100f);
            Rect dragRect;
            GUILayout.BeginArea(drawRect, GUI.skin.GetStyle("Box"));

            GUILayout.Label(name, GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));
            dragRect = GUILayoutUtility.GetLastRect();
            dragRect = new Rect(dragRect.x + Position.x, dragRect.y + Position.y, dragRect.width, dragRect.height);

            if (Dragging) {
                GUILayout.Label("Woooooooo");
            }
            else if (GUILayout.Button("Yes!")) {
                Debug.Log("Yes. It is " + value + "!");
            }

            GUILayout.EndArea();
            Drag(dragRect);
        }
    }
}
