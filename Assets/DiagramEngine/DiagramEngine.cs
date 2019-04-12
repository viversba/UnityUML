using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DEngine.Model;
using System;

namespace DEngine {

    public class DiagramEngine {

        /// <summary>
        /// Static reference to the diagram engine
        /// </summary>
        private static DiagramEngine diagramEngine = new DiagramEngine();
        /// <summary>
        /// The program instance.
        /// </summary>
        private Program program;
        /// <summary>
        /// The storage path.
        /// </summary>
        private static string storagePath = Directory.GetCurrentDirectory() + "/Assets/DiagramEngine/diagram.dat";

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

            // program.Start();
        }

        public static List<BaseModel> LoadEntitiesFromDisk(string path="") {

            List<BaseModel> entities = null;
            if (File.Exists(storagePath) && (new FileInfo(storagePath).Length != 0)) {

                try {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(storagePath, FileMode.Open);

                    entities = bf.Deserialize(file) as List<BaseModel>;
                    file.Close();
                }
                catch (Exception e) {

                    Debug.LogWarning("Error (DiagramEngine.cs : LoadEntitiesFromDisk): " + e);
                    File.Delete(storagePath);
                }
            }
            else {
                //Debug.LogWarning("No entities to load");
            }
            return entities;
        }

        public static void SaveEntitiesOnDisk(List<BaseModel> entities, string path = "") {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(storagePath + path, FileMode.Create);

            if (entities.Count > 0) {
                bf.Serialize(file, entities);
            }

            file.Close();

            //if(File.Exists(storagePath + path)) {
            //    Debug.Log("Saved entities at " + storagePath + path);
            //}
        }
    }
}