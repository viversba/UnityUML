using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Model = DEngine.Model;

namespace DEngine.View {

    public class ClassNode : BaseNode {

        private string superClassName = "";
        private List<Model :: Attribute> attributes = new List<Model :: Attribute>();
        private List<Model :: Method> methods = new List<Model :: Method>();
        private List<Model :: Constructor> constructors = new List<Model :: Constructor>();

        /// <summary>
        /// Scroll position of the list of entities to render
        /// </summary>
        private Vector2 scrollPos;

        public int numberOfLines;

        public ClassNode(string title) {

            windowTitle = title;
            hasInputs = false;
            numberOfLines = 0;
        }

        public ClassNode(Model:: ClassModel classModel) {
            windowTitle = classModel.GetName();
            //windowTitle = "Generic Class";
            //hasInputs = false;
            attributes.AddRange(classModel.GetAttributes());
            methods.AddRange(classModel.GetMethods());
            constructors.AddRange(classModel.GetConstructors());
            numberOfLines = 0;
        }

        public void Init(Model::ClassModel classModel) {
            windowTitle = classModel.GetName();
            //windowTitle = "Generic Class";
            //hasInputs = false;
            attributes = new List<Model::Attribute>();
            methods = new List<Model::Method>();
            constructors = new List<Model::Constructor>();
            superClassName = "";

            scrollPos = new Vector2();

            attributes.AddRange(classModel.GetAttributes());
            methods.AddRange(classModel.GetMethods());
            constructors.AddRange(classModel.GetConstructors());
            superClassName = classModel.GetSuperClassName();
            numberOfLines = 0;
        }

        public void AddLine() {
            numberOfLines++;
        }

        public override void DrawWindow() {

            var header = new GUIStyle();
            var public_ = new GUIStyle();
            var private_ = new GUIStyle();
            var protected_ = new GUIStyle();
            header.normal.textColor = new Color(0.188f, 0.258f, 0.733f, 1f);
            public_.normal.textColor = Color.green;
            private_.normal.textColor = Color.red;
            protected_.normal.textColor = Color.magenta;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
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
                else if (method.modifier == Model::AccessModifier.PROTECTED) {
                    GUILayout.Label("# ", protected_, GUILayout.Width(5));
                    GUILayout.Label(method.ToString() + "()");
                }
                else {
                    GUILayout.Label(method.ToString() + "()");
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Label("Constructors", header);
            foreach (Model :: Constructor constructor in constructors) {
                GUILayout.Label(constructor.ToString());
            }
            EditorGUILayout.EndScrollView();
        }

        public override void Tick(float deltaTime) {
        }

        public override void DrawCurves() {
        }
    }

}