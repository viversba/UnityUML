﻿using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Model = DEngine.Model;

namespace DEngine.View {

    public class ClassNode : BaseNode {

        private string superClassName = "";
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
            //windowTitle = "Generic Class";
            //hasInputs = false;
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

            scrollPos = new Vector2();

            attributes.AddRange(classModel.GetAttributes());
            methods.AddRange(classModel.GetMethods());
            constructors.AddRange(classModel.GetConstructors());
            superClassName = classModel.GetSuperClassName();

            if (classModel.GetInterfaceNames() != null) {
                foreach (string interface_ in classModel.GetInterfaceNames()) {
                    interfaceNames.Add(interface_);
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
                    if (constructor.modifier == Model::AccessModifier.PRIVATE) {
                        GUILayout.Label("- ", private_, GUILayout.Width(5));
                        GUILayout.Label(constructor.ToString() + "()");
                    }
                    else if (constructor.modifier == Model::AccessModifier.PUBLIC) {
                        GUILayout.Label("+ ", public_, GUILayout.Width(5));
                        GUILayout.Label(constructor.ToString() + "()");
                    }
                    else if (constructor.modifier == Model::AccessModifier.PROTECTED) {
                        GUILayout.Label("# ", protected_, GUILayout.Width(5));
                        GUILayout.Label(constructor.ToString() + "()");
                    }
                    else {
                        GUILayout.Label(constructor.ToString() + "()");
                    }
                    GUILayout.EndHorizontal();
                    //GUILayout.Label(constructor.ToString());
                }
            }

            // Display attributes
            if (attributes.Count > 0) {
                GUILayout.Label("Attributes (" + attributes.Count + ")", header);
                foreach (Model::Attribute attribute in attributes) {
                    GUILayout.BeginHorizontal();
                    if (attribute.modifier == Model::AccessModifier.PRIVATE) {
                        GUILayout.Label("- ", private_, GUILayout.Width(5));
                        GUILayout.Label(attribute.ToString());
                    }
                    else if (attribute.modifier == Model::AccessModifier.PUBLIC) {
                        GUILayout.Label("+ ", public_, GUILayout.Width(5));
                        GUILayout.Label(attribute.ToString());
                    }
                    else if (attribute.modifier == Model::AccessModifier.PROTECTED) {
                        GUILayout.Label("# ", protected_, GUILayout.Width(5));
                        GUILayout.Label(attribute.ToString());
                    }
                    else {
                        GUILayout.Label(attribute.ToString());
                    }
                    GUILayout.EndHorizontal();
                }
            }

            // Display Methods
            if (methods.Count > 0) {
                GUILayout.Label("Methods (" + methods.Count + ")", header);
                foreach (Model::Method method in methods) {
                    GUILayout.BeginHorizontal();
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
                //Debug.Log(windowTitle);
                foreach (InterfaceNode interfaceNode in interfaces) {
                    //Rect windowRect = this.windowRect;
                    //windowRect.y = this.windowRect.y + this.windowRect.height / 2;
                    //windowRect.height = 1;
                    //windowRect.width = 1;
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