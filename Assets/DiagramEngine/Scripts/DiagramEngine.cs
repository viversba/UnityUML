using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEngine {

    public class DiagramEngine {

        /// <summary>
        /// Static reference to the diagram engine
        /// </summary>
        private static DiagramEngine diagramEngine = new DiagramEngine();
        private Program program;

        /// <summary>
        /// Private constructor
        /// </summary>
        private DiagramEngine() {

            program = new Program();
        }

        public static DiagramEngine getInstance() {

            return diagramEngine;
        }

        public void Run() {

            program.Start();
        }
    }
}