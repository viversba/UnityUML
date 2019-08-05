using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEngine.Model;
using DEngine.Controller;
using DEngine;
using System;

namespace DEngine.View{

    public class DiagramEngineGUI : EditorWindow {

        /// <summary>
        /// Current instance of the window;
        /// </summary>
        public static EditorWindow Window { get; private set; }

        public static int MinSizeOfLeftPanelInPixels { get; set; } = 150;

#pragma warning disable 0649

        /// <summary>
        /// Instance of itself
        /// </summary>
        private static DiagramEngineGUI engineGUI;

#pragma warning restore 0649

        /// <summary>
        /// This is the current list of entities that is going to be rendered;
        /// </summary>
        private List<BaseModel> selectedEntities = new List<BaseModel>();
        /// <summary>
        /// Reference to the right panel;
        /// </summary>
        private LeftPanel leftPanel;
        /// <summary>
        /// Reference to the right panel
        /// </summary>
        private RightPanel rightPanel;
        /// <summary>
        /// Tells if the Generate Diagram button has already been pressed
        /// </summary>
        private bool drawNodes;

        private float begginingOfRightPanel;

        EditorGUISplitView horizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);

#pragma warning disable 0414

        EditorGUISplitView verticalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Vertical);

#pragma warning restore 0414

        [MenuItem("DEngine/Open Window")]
        public static void Init() {

            GetInstance().Run();
        }

        private static DiagramEngineGUI GetInstance() {

            DiagramEngineGUI engine = engineGUI ?? (DiagramEngineGUI)CreateInstance("DiagramEngineGUI");
            return engine;
        }

        private void Run() {

            Window = GetWindow<DiagramEngineGUI>();
            Window.titleContent.text = "DEngine";
            Window.position = new Rect(200, 100, 900, 600);
            Window.minSize = new Vector2(300, 200);


            drawNodes = false;
            leftPanel = leftPanel ?? (LeftPanel)ScriptableObject.CreateInstance("LeftPanel");
            rightPanel = rightPanel ?? (RightPanel)ScriptableObject.CreateInstance("RightPanel");

            begginingOfRightPanel = horizontalSplitView.splitNormalizedPosition * position.width;
        }

        public void OnGUI() {

            horizontalSplitView.Min_Width = MinSizeOfLeftPanelInPixels / Window.position.width;

            horizontalSplitView.BeginSplitView();
            drawNodes = leftPanel.DrawLeftPanel();
            horizontalSplitView.Split();
            //verticalSplitView.BeginSplitView();
            //DrawRightPanel();
            //verticalSplitView.Split();

            // Leave this line uncommented if you want the panels to increase size on window resize as percentages
            begginingOfRightPanel = horizontalSplitView.splitNormalizedPosition * position.width;

            rightPanel.SetBegginingOfRightPanel(begginingOfRightPanel);
            if (drawNodes) {
                selectedEntities = leftPanel.GetSelectedEntities();
                DiagramEngine.SaveEntitiesOnDisk(selectedEntities);
                rightPanel.CreateWindowList(selectedEntities);
                drawNodes = false;
            }
            rightPanel.DrawRightPanel(new Vector2(position.width, position.height));
            //verticalSplitView.EndSplitView();
            horizontalSplitView.EndSplitView();
            Repaint();
        }
    }
}