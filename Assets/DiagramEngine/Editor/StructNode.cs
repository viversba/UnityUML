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
        private List<InterfaceNode> interfaces = new List<InterfaceNode>();
        private List<string> interfaceNames = new List<string>();

        /// <summary>
        /// Scroll position of the list of entities to render
        /// </summary>
        private Vector2 scrollPos;

        public StructNode(string title) {
            windowTitle = title;
        }

        public void Init(Model::StructModel structModel) {
            windowTitle = structModel.GetCompleteName();
            isEmpty = false;
            attributes = new List<Model::Attribute>();
            methods = new List<Model::Method>();
            constructors = new List<Model::Constructor>();
            interfaces = null;
            interfaceNames = new List<string>();
            Type = structModel.Type;

            scrollPos = new Vector2();

            attributes.AddRange(structModel.GetAttributes());
            methods.AddRange(structModel.GetMethods());
            constructors.AddRange(structModel.GetConstructors());

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
                    switch (constructor.modifier) {
                        case Model::AccessModifier.PRIVATE:
                            GUILayout.Label("- ", private_, GUILayout.Width(5));
                            break;
                        case Model::AccessModifier.PUBLIC:
                            GUILayout.Label("+ ", public_, GUILayout.Width(5));
                            break;
                        case Model::AccessModifier.PROTECTED:
                            GUILayout.Label("# ", protected_, GUILayout.Width(5));
                            break;
                    }
                    ClassNode.DrawConstructor(constructor);
                    GUILayout.EndHorizontal();
                }
            }

            // Display attributes
            if (attributes.Count > 0) {
                GUILayout.Label("Attributes (" + attributes.Count + ")", header);
                foreach (Model::Attribute attribute in attributes) {
                    GUILayout.BeginHorizontal();
                    switch (attribute.modifier) {
                        case Model::AccessModifier.PRIVATE:
                            GUILayout.Label("- ", private_, GUILayout.Width(5));
                            break;
                        case Model::AccessModifier.PUBLIC:
                            GUILayout.Label("+ ", public_, GUILayout.Width(5));
                            break;
                        case Model::AccessModifier.PROTECTED:
                            GUILayout.Label("# ", protected_, GUILayout.Width(5));
                            break;
                    }
                    ClassNode.DrawAttribute(attribute);
                    GUILayout.EndHorizontal();
                }
            }

            // Display Methods
            if (methods?.Count > 0) {
                GUILayout.Label("Methods (" + methods.Count + ")", header);
                foreach (Model::Method method in methods) {
                    GUILayout.BeginHorizontal();
                    switch (method.modifier) {
                        case Model::AccessModifier.PRIVATE:
                            GUILayout.Label("- ", private_, GUILayout.Width(5));
                            break;
                        case Model::AccessModifier.PUBLIC:
                            GUILayout.Label("+ ", public_, GUILayout.Width(5));
                            break;
                        case Model::AccessModifier.PROTECTED:
                            GUILayout.Label("# ", protected_, GUILayout.Width(5));
                            break;
                    }
                    ClassNode.DrawMethod(method);
                    GUILayout.EndHorizontal();
                }
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
        }
    }
}
