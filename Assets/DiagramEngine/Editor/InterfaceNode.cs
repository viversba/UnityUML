﻿using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Model = DEngine.Model;

namespace DEngine.View {

    public class InterfaceNode : BaseNode {

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
        }

        public InterfaceNode(Model::InterfaceModel interfaceModel) {
            windowTitle = interfaceModel.GetCompleteName();
            methods.AddRange(interfaceModel.GetMethods());
        }

        public void Init(Model::InterfaceModel interfaceModel) {
            Name = interfaceModel.GetName();
            isEmpty = true;
            windowTitle = "<<" + interfaceModel.GetCompleteName() + ">>";
            methods = new List<Model::Method>();
            interfaces = null;
            interfaceNames = new List<string>();
            Type = interfaceModel.Type;

            if (interfaceModel.GetInterfaceNames() != null) {

                foreach (var interface_ in interfaceModel.GetInterfaceNames()) {
                    string interfaceName = interface_.Name;
                    interfaceNames.Add(interfaceName);

                    isEmpty = false;
                }
            }

            scrollPos = new Vector2();

            methods.AddRange(interfaceModel.GetMethods());

            // Initially set the minimum size for all windows without content
            if (methods?.Count == 0) {
                isEmpty = true;
                windowRect.height = 50;
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

            if (methods?.Count > 0) {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                GUILayout.Label("Methods (" + methods.Count + ")", header);
                foreach (Model::Method method in methods) {
                    GUILayout.BeginHorizontal();
                    Drawer.DrawMethod(method);
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
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