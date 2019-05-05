using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Model = DEngine.Model;

namespace DEngine.View {

    public class StructNode : BaseNode {

        private List<Model::Attribute> attributes = new List<Model::Attribute>();
        private List<Model::Method> methods = new List<Model::Method>();
        private List<Model::Constructor> constructors = new List<Model::Constructor>();
        private List<Model::BaseModel> implementationsModels = new List<Model::BaseModel>();
        private List<InterfaceNode> interfaces = new List<InterfaceNode>();
        private List<BaseNode> implementations = new List<BaseNode>();
        private List<string> interfaceNames = new List<string>();

        /// <summary>
        /// Scroll position of the list of entities to render
        /// </summary>
        private Vector2 scrollPos;

        public StructNode(string title) {
            windowTitle = title;
        }

        public void Init(Model::StructModel structModel) {
            Name = structModel.GetName();
            windowTitle = structModel.GetCompleteName();
            isEmpty = false;
            attributes = new List<Model::Attribute>();
            methods = new List<Model::Method>();
            constructors = new List<Model::Constructor>();
            implementationsModels = new List<Model::BaseModel>();
            implementations = new List<BaseNode>();
            interfaces = null;
            interfaceNames = new List<string>();
            Type = structModel.Type;

            scrollPos = new Vector2();

            attributes.AddRange(structModel.GetAttributes());
            methods.AddRange(structModel.GetMethods());
            constructors.AddRange(structModel.GetConstructors());
            implementationsModels.AddRange(structModel.GetImplementations());

            if (structModel.GetInterfaceNames() != null) {
                foreach (var interface_ in structModel.GetInterfaceNames()) {
                    string interfaceName = interface_.Name;
                    interfaceNames.Add(interfaceName);
                }
            }

            // Initially set the minimum size for all windows without content
            if (attributes.Count == 0 && methods.Count == 0 && constructors.Count == 0) {
                isEmpty = true;
                windowRect.height = 50;
                return;
            }
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

            //Display Constructors
            if (constructors.Count > 0) {
                GUILayout.Label("Constructors (" + constructors.Count + ")", header);
                foreach (Model::Constructor constructor in constructors) {
                    GUILayout.BeginHorizontal();
                    Drawer.DrawConstructor(constructor);
                    GUILayout.EndHorizontal();
                }
            }

            // Display attributes
            if (attributes.Count > 0) {
                GUILayout.Label("Attributes (" + attributes.Count + ")", header);
                foreach (Model::Attribute attribute in attributes) {
                    GUILayout.BeginHorizontal();
                    Drawer.DrawAttribute(attribute);
                    GUILayout.EndHorizontal();
                }
            }

            // Display Methods
            if (methods?.Count > 0) {
                GUILayout.Label("Methods (" + methods.Count + ")", header);
                foreach (Model::Method method in methods) {
                    GUILayout.BeginHorizontal();
                    Drawer.DrawMethod(method);
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public List<string> GetInterfaceNames() {
            return interfaceNames;
        }

        public List<Model::BaseModel> GetImplementations()
        {
            return implementationsModels;
        }

        public void SetInterfaceNodes(List<InterfaceNode> interfaceNodes) {
            if (interfaces == null)
                interfaces = new List<InterfaceNode>();
            interfaces.AddRange(interfaceNodes);
        }

        public void SetImplementationNodes(List<BaseNode> implementations)
        {
            this.implementations.AddRange(implementations);
        }

        public override void DrawCurves() {

            if (interfaces != null) {
                foreach (InterfaceNode interfaceNode in interfaces) {
                    Rect interfaceRect = interfaceNode.windowRect;
                    interfaceRect.y = interfaceNode.windowRect.y + interfaceNode.windowRect.height / 2;
                    interfaceRect.height = 1;
                    interfaceRect.width = 1;
                    RightPanel.DrawImplementationCurve(windowRect, interfaceRect);
                }
            }

            // Already verified for null at ClassNode Getter
            if (implementations != null)
            {
                foreach (var implementation in implementations)
                {
                    Rect implementationRect = implementation.windowRect;
                    implementationRect.y = implementation.windowRect.y + implementation.windowRect.height / 2;
                    implementationRect.height = 1;
                    implementationRect.width = 1;
                    RightPanel.DrawImplementationCurve(windowRect, implementationRect);
                }
            }
        }
    }
}
