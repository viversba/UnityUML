using System;
using UnityEngine;

namespace DEngine.View {

    public class GUIDraggableObject {

        /// <summary>
        /// Start position of the drag
        /// </summary>
        private Vector2 dragStartPosition;

        public GUIDraggableObject(Vector2 position) {

            Position = position;
        }

        public bool Dragging { get; private set; }

        public Vector2 Position { get; set; }

        /// <summary>
        /// Drag the specified draggingRect.
        /// </summary>
        /// <param name="draggingRect">Dragging rect.</param>
        public void Drag(Rect draggingRect) {

            if(Event.current.type == EventType.MouseUp) {
                Dragging = false;
            }
            else if(Event.current.type == EventType.MouseDown && draggingRect.Contains(Event.current.mousePosition)) {

                Dragging = true;
                dragStartPosition = Event.current.mousePosition - Position;
                Event.current.Use();
            }

            if (Dragging) {
                Position = Event.current.mousePosition - dragStartPosition;
            }
        }
    }
}
