using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Model = DEngine.Model;

namespace DEngine.View {

    public class ClassNode : BaseNode {
        private List<Model :: Attribute> attributes = new List<Model :: Attribute>();
        private List<Model :: Method> methods = new List<Model :: Method>();
        private List<Model :: Constructor> constructors = new List<Model :: Constructor>();

        public int numberOfLines;

        public ClassNode(string title) {

            windowTitle = title;
            hasInputs = false;
            numberOfLines = 0;
            Initialize();
        }

        public ClassNode() {
            windowTitle = "Generic Class";
            hasInputs = false;
            numberOfLines = 0;
            Initialize();
        }

        private void Initialize() { 
        }

        public void AddLine() {
            numberOfLines++;
        }

        public override void DrawWindow() {
            base.DrawWindow();

            var header = new GUIStyle();
            var public_ = new GUIStyle();
            var private_ = new GUIStyle();
            var protected_ = new GUIStyle();
            header.normal.textColor = Color.grey;
            public_.normal.textColor = Color.green;
            private_.normal.textColor = Color.red;
            protected_.normal.textColor = Color.magenta;

            GUILayout.Label("Attributes", header);
            foreach (Model :: Attribute attribute in attributes) {
                GUILayout.Label(attribute.ToString());
            }
            GUILayout.Label("Methods", header);
            foreach (Model :: Method method in methods) {
                GUILayout.BeginHorizontal();
                if (method.modifier == Model :: AccessModifier.PRIVATE) {
                    GUILayout.Label("- ", private_, GUILayout.Width(5));
                    GUILayout.Label(method.ToString() + "()");
                }
                else if (method.modifier == Model :: AccessModifier.PUBLIC) {
                    GUILayout.Label("+ ", public_, GUILayout.Width(5));
                    GUILayout.Label(method.ToString() + "()");
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Label("Constructors", header);
            foreach (Model :: Constructor constructor in constructors) {
                GUILayout.Label(constructor.ToString());
            }
        }

        public override void Tick(float deltaTime) {
        }

        public override void DrawCurves() {
        }
    }

}