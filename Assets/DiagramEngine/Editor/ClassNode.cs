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

            var header = new GUIStyle();
            var public_ = new GUIStyle();
            var private_ = new GUIStyle();
            var protected_ = new GUIStyle();
            header.normal.textColor = new Color(0.188f, 0.258f, 0.733f, 1f);
            public_.normal.textColor = Color.green;
            private_.normal.textColor = Color.red;
            protected_.normal.textColor = Color.magenta;
            public_.margin = new RectOffset(0, 0, 0, 0);
            private_.margin = new RectOffset(0, 0, 0, 0);
            protected_.margin = new RectOffset(0, 0, 0, 0);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.ExpandWidth(false);

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
                    DrawConstructor(constructor);
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
                    DrawMethod(method);
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

        public static void DrawConstructor(Model::Constructor constructor) {
            //Debug.Log(constructor);
            var red = new GUIStyle();
            red.normal.textColor = Color.red;

            GUILayout.Label(constructor.name);
            GUILayout.Label("(");
            if (constructor.parameters != null && constructor.parameters.Count > 0) {
                for (int i = 0; i < constructor.parameters.Count - 1; i++) {
                    DrawParameter(constructor.parameters[i]);
                    GUILayout.Label(",");
                }
                DrawParameter(constructor.parameters[constructor.parameters.Count - 1]);
            }
            GUILayout.Label(")");
        }

        public static void DrawAttribute(Model::Attribute attribute) {

            var red = new GUIStyle();
            red.normal.textColor = Color.red;
            DrawType(attribute.returnType);
            GUILayout.Label(attribute.name);
        }

        public static void DrawMethod(Model::Method method) {

            var red = new GUIStyle();
            red.normal.textColor = Color.red;

            DrawType(method.returnType);
            GUILayout.Label(method.name);
            GUILayout.Label("(");
            if (method.parameters != null && method.parameters.Count > 0) {
                for (int i = 0; i < method.parameters.Count - 1; i++) {
                    DrawParameter(method.parameters[i]);
                    GUILayout.Label(",");
                }
                DrawParameter(method.parameters[method.parameters.Count - 1]);
            }
            GUILayout.Label(")");
        }

        public static void DrawParameter(Model::Parameter parameter) {

            GUILayout.Label(parameter.type.name);
            if (parameter.type.type != null && parameter.type.type.Count > 0) {
                GUILayout.Label("<");
                for (int i = 0; i < parameter.type.type.Count - 1; i++) {
                    DrawType(parameter.type.type[i]);
                    GUILayout.Label(",");
                }
                DrawType(parameter.type.type[parameter.type.type.Count - 1]);
                GUILayout.Label(">");
            }
            GUILayout.Label(parameter.name);
        }

        public static void DrawType(Model::GenericType type) {

            GUILayout.Label(type.name);
            if (type.type != null && type.type.Count > 0) {
                GUILayout.Label("<");
                for (int i = 0; i < type.type.Count - 1; i++) {
                    DrawType(type.type[i]);
                    GUILayout.Label(",");
                }
                DrawType(type.type[type.type.Count - 1]);
                GUILayout.Label(">");
            }
        }
    }
}