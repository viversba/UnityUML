using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Model = DEngine.Model;


namespace DEngine.View {

    public class InterfaceNode : BaseNode{
    
        private List<Model::Method> methods = new List<Model::Method>();
        /// <summary>
        /// Scroll position of the list of entities to render
        /// </summary>
        private Vector2 scrollPos;
        /// <summary>
        /// List of interfaces of windows that it is attached to;
        /// </summary>
        private List<InterfaceNode> interfaces = new List<InterfaceNode>();
        private List<string> interfaceNames = new List<string>();

        public InterfaceNode(string title) {

            windowTitle = title;
            hasInputs = false;
        }

        public InterfaceNode(Model::InterfaceModel interfaceModel) {
            windowTitle = interfaceModel.GetName();
            methods.AddRange(interfaceModel.GetMethods());
        }

        public void Init(Model::InterfaceModel interfaceModel) {
            windowTitle = "<<" + interfaceModel.GetName() + ">>";
            methods = new List<Model::Method>();
            interfaces = null;
            interfaceNames = new List<string>();

            if(interfaceModel.GetInterfaceNames() != null) { 
                foreach(string interface_ in interfaceModel.GetInterfaceNames()) {
                    interfaceNames.Add(interface_);
                }
            }

            scrollPos = new Vector2();

            methods.AddRange(interfaceModel.GetMethods());
        }

        public override void DrawWindow() {

            var header = new GUIStyle();
            var white = new GUIStyle();
            var public_ = new GUIStyle();
            var private_ = new GUIStyle();
            var protected_ = new GUIStyle();
            header.normal.textColor = Color.black;
            public_.normal.textColor = Color.green;
            private_.normal.textColor = Color.red;
            protected_.normal.textColor = Color.magenta;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.Label("Methods (" + methods.Count + ")", header);
            foreach (Model::Method method in methods) {

                GUILayout.BeginHorizontal(header);
                if (method.modifier == Model::AccessModifier.PRIVATE) {
                    GUILayout.Label("- ", private_, GUILayout.Width(5));
                    GUILayout.Label(method.ToString() + "()");
                }
                else if (method.modifier == Model::AccessModifier.PUBLIC) {
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
            EditorGUILayout.EndScrollView();
        }

        public List<string> GetInterfaceNames() {
            return interfaceNames;
        }

        public void SetInterfaceNodes(List<InterfaceNode> interfaceNodes) {
            if (interfaces == null)
                interfaces = new List<InterfaceNode>();
            interfaces.AddRange(interfaceNodes);
        }

        public override void Tick(float deltaTime) {
        }

        public override void DrawCurves() {
        }
    }
}