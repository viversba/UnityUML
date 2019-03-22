using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Model = DEngine.Model;

namespace DEngine.View {

    public class ClassNode : BaseNode {

        private string superClassName = "";
        private Dictionary<string, int> olakase;
        private List<Model::Attribute> attributes = new List<Model::Attribute>();
        private List<Model::Method> methods = new List<Model::Method>();
        private List<Model::Constructor> constructors = new List<Model::Constructor>();
        private List<InterfaceNode> interfaces = new List<InterfaceNode>();
        private List<string> interfaceNames = new List<string>();
        private ClassNode superClass;

        /// <summary>
        /// Scroll position of the list of entities to render
        /// </summary>
        private Vector2 scrollPos;

        public int numberOfLines;

        public ClassNode(string title) {

            windowTitle = title;
            numberOfLines = 0;
            superClass = null;
        }

        public ClassNode(Model::ClassModel classModel) {
            windowTitle = classModel.GetName();
            attributes.AddRange(classModel.GetAttributes());
            methods.AddRange(classModel.GetMethods());
            constructors.AddRange(classModel.GetConstructors());
            numberOfLines = 0;
            superClass = null;
        }

        public void Init(Model::ClassModel classModel) {
            windowTitle = classModel.GetName();
            isEmpty = false;
            attributes = new List<Model::Attribute>();
            methods = new List<Model::Method>();
            constructors = new List<Model::Constructor>();
            interfaces = null;
            interfaceNames = new List<string>();
            superClassName = "";
            superClass = null;
            Type = classModel.Type;

            scrollPos = new Vector2();

            attributes.AddRange(classModel.GetAttributes());
            methods.AddRange(classModel.GetMethods());
            constructors.AddRange(classModel.GetConstructors());
            superClassName = classModel.GetSuperClassName().name;

            if (classModel.GetInterfaceNames() != null) {
                foreach (var interface_ in classModel.GetInterfaceNames()) {
                    string interfaceName = interface_.name;
                    interfaceNames.Add(interfaceName);
                }
            }

            // Initially set the minimum size for all windows without content
            if (attributes.Count == 0 && methods.Count == 0 && constructors.Count == 0) {
                isEmpty = true;
                windowRect.height = 50;
                return;
            }

            numberOfLines = 0;
        }

        /// <summary>
        /// Assigns the super class window.
        /// </summary>
        /// <param name="superClass">Super class.</param>
        public void SetSuperClassNode(ClassNode superClass) {
            this.superClass = superClass;
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

            //Display Constructors
            if (constructors.Count > 0) {
                GUILayout.Label("Constructors (" + constructors.Count + ")", header);
                foreach (Model::Constructor constructor in constructors) {
                    GUILayout.BeginHorizontal();
                    switch (constructor.modifier)
                    {
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
                    GUILayout.Label(constructor.ToString() + "()");
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
                    DrawAttribute(attribute);
                    GUILayout.EndHorizontal();
                }
            }

            // Display Methods
            if (methods.Count > 0) {
                GUILayout.Label("Methods (" + methods.Count + ")", header);
                foreach (Model::Method method in methods) {
                    GUILayout.BeginHorizontal();
                    switch (method.modifier)
                    {
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
                    GUILayout.Label(method.ToString() + "()");
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public string GetSuperClassName() {
            return superClassName;
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

            if (superClass != null) {
                Rect superClassRect = superClass.windowRect;
                superClassRect.x = superClass.windowRect.x + superClass.windowRect.width / 2;
                //superClassRect.y = superClass.windowRect.y + superClass.windowRect.height / 2;
                superClassRect.y = superClassRect.y + superClassRect.height;
                superClassRect.height = 1;
                superClassRect.width = 1;
                RightPanel.DrawInheritanceCurve(windowRect, superClassRect);
            }

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

        private void DrawAttribute(Model::Attribute attribute) {

            //var red = new GUIStyle();
            //red.normal.textColor = Color.red;

            //GUILayout.Label(attribute.returnType);
            //if(attribute.returnSubTypes != null) {
            //    GUILayout.Label("<");
            //    for(int i = 0; i< attribute.returnSubTypes.Length-1; i++) { 
            //        GUILayout.Label($"{attribute.returnSubTypes[i]}", red);
            //        GUILayout.Label($",");
            //    }
            //    GUILayout.Label(attribute.returnSubTypes[attribute.returnSubTypes.Length - 1], red);
            //    GUILayout.Label(">");
            //}
            //GUILayout.Label(attribute.name);
        }
    }
}