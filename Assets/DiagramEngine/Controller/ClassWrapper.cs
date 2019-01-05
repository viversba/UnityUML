using DEngine.Model;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DEngine.Controller {

    public class ClassWrapper {

        private List<BaseModel> entities;
        private bool isClass;
        public BaseModel currentEntity;
        /// <summary>
        /// This will be written to disk
        /// </summary>
        public List<BaseModel> allEntities;

        public ClassWrapper() {

            isClass = false;
            entities = new List<BaseModel>();
            allEntities = new List<BaseModel>();
            currentEntity = null;
        }

        public void AddClass(string name) {

            entities.Add(new ClassModel(name));
            currentEntity = entities[entities.Count - 1];
            int numberOfEntities = entities.Count;
            if (numberOfEntities > 1) {
                currentEntity.SetContainer(entities[numberOfEntities - 2]);
            }
            isClass = true;
        }

        public void AddInterface(string name) {

            entities.Add(new InterfaceModel(name));
            currentEntity = entities[entities.Count - 1];
            int numberOfEntities = entities.Count;
            if (numberOfEntities > 1) {
                entities[numberOfEntities - 1].SetContainer(entities[numberOfEntities - 2]);
            }
            isClass = false;
        }

        public void SetContainer() {

            if (entities.Count > 1) {

                currentEntity.SetContainer(entities[entities.Count - 1]);
            }
        }

        public void FinishEntity() {

            if(entities.Count == 1) {
                allEntities.Add(entities[entities.Count - 1]);
            }
            entities.RemoveAt(entities.Count - 1);
            currentEntity = entities.Count > 0 ? entities[entities.Count - 1] : null;
            isClass = false;
        }

        public void AddMethodTo(Method method) {
            if (currentEntity == null)
                return;
            currentEntity.AddMethod(method);
        }

        public void AddAttributeTo(Model.Attribute attribute) {
            if (currentEntity == null)
                return;
            currentEntity.AddAttribute(attribute);
        }

        public void AddInterface(InterfaceModel interface_) {
            if (currentEntity == null)
                return;
            currentEntity.AddInterface(interface_);
        }

        public ClassModel GetClass() {

            return currentEntity as ClassModel;
        }

        public void AddConstructor(Constructor constructor) {
            if (currentEntity == null)
                return;
            var currentClass = (ClassModel)currentEntity;
            currentClass.AddConstructor(constructor);
            currentEntity = currentClass;
        }

        public static T ParseEnum<T>(string value) {
            return (T)Enum.Parse(typeof(T),value, true);
        }

        /// <summary>
        /// Receives a list of modifiers and matches them with defined enums
        /// </summary>
        /// <param name="modifiers">List containing all the modifiers to check</param>
        /// <param name="mod"><c>out</c> variable that will contain the Access Modifier</param>
        /// <param name="methodType"><c>out</c> variable that will contain the Method Type</param>
        public static void ModifierMatch(List<string> modifiers, ref AccessModifier mod, ref MethodType methodType) {

            foreach (string modifier in modifiers) {

                if (Enum.IsDefined(typeof(AccessModifier), modifier.ToUpper())) {
                    // It is an access modifier, convert to modifier enum
                    mod = ParseEnum<AccessModifier>(modifier);
                }
                else if (Enum.IsDefined(typeof(MethodType), modifier.ToUpper())) {
                    // If it is a method type, convert to method type enum
                    methodType = ParseEnum<MethodType>(modifier);
                }
            }
        }

        /// <summary>
        /// Receives a list of modifiers and matches them with defined enums
        /// </summary>
        /// <param name="modifiers">List containing all the modifiers to check</param>
        /// <param name="mod"><c>out</c> variable that will contain the Access Modifier</param>
        /// <param name="attributeType"><c>out</c> variable that will contain the Method Type</param>
        public static void ModifierMatch(List<string> modifiers, ref AccessModifier mod, ref AttributeType attributeType) {

            foreach (string modifier in modifiers) {

                if (Enum.IsDefined(typeof(AccessModifier), modifier.ToUpper())) {
                    // It is an access modifier, convert to modifier enum
                    mod = ParseEnum<AccessModifier>(modifier);
                }
                else if (Enum.IsDefined(typeof(AttributeType), modifier.ToUpper())) {
                    // If it is a method type, convert to method type enum
                    attributeType = ParseEnum<AttributeType>(modifier);
                }
            }
        }
    }

}
