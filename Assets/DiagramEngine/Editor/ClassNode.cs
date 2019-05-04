using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Model = DEngine.Model;

namespace DEngine.View {

    public class ClassNode : BaseNode {

        private const int SPACING = 8;
        private string superClassName = "";
        private List<Model::Attribute> attributes = new List<Model::Attribute>();
        private List<Model::Method> methods = new List<Model::Method>();
        private List<Model::Constructor> constructors = new List<Model::Constructor>();
        private List<InterfaceNode> interfaces = new List<InterfaceNode>();
        private List<ClassNode> classImplementations = new List<ClassNode>();
        private List<StructNode> structImplementations = new List<StructNode>();
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
            windowTitle = classModel.GetCompleteName();
            attributes.AddRange(classModel.GetAttributes());
            methods.AddRange(classModel.GetMethods());
            constructors.AddRange(classModel.GetConstructors());
            numberOfLines = 0;
            superClass = null;
        }

        public void Init(Model::ClassModel classModel) {
            windowTitle = classModel.GetCompleteName();
            isEmpty = false;
            attributes = new List<Model::Attribute>();
            methods = new List<Model::Method>();
            constructors = new List<Model::Constructor>();
            classImplementations = new List<ClassNode>();
            structImplementations = new List<StructNode>();
            interfaces = null;
            interfaceNames = new List<string>();
            superClassName = "";
            superClass = null;
            Type = classModel.Type;

            scrollPos = new Vector2();

            attributes.AddRange(classModel.GetAttributes());
            methods.AddRange(classModel.GetMethods());
            constructors.AddRange(classModel.GetConstructors());
            superClassName = classModel.GetSuperClassName().Name;

            if (classModel.GetInterfaceNames() != null) {
                foreach (var interface_ in classModel.GetInterfaceNames()) {
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

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUIStyle header = new GUIStyle();
            header.normal.textColor = new Color(0.188f, 0.258f, 0.733f, 1f);

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

        public void SetClassImplementationsNodes(List<ClassNode> implementationNodes){
            classImplementations = classImplementations ?? new List<ClassNode>();
            classImplementations.AddRange(implementationNodes);
        }

        public void SetStructImplementationsNodes(List<StructNode> implementationNodes){
            structImplementations = structImplementations ?? new List<StructNode>();
            structImplementations.AddRange(implementationNodes);
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

            // Already verified for null at ClassNode Getter
            if(classImplementations != null) { 
            
                foreach(var implementation in classImplementations) { 
                
                    DrawImple
                }
            }
        }
    }
}