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

        /// <summary>
        /// Temporal variable that will indicate if an entity is a struct in order to dump it
        /// Will be removed when support for structs is available
        /// </summary>
        private bool isStruct;

        public ClassWrapper() {

            isClass = false;
            isStruct = false;
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
            currentEntity.SetTypeOfEntity(true);
            isClass = true;
        }

        public void SetSuperClassName(string superClassName) {

            if (currentEntity == null)
                return;
            ClassModel currentClass = (ClassModel)currentEntity;
            currentClass.SetSuperClassName(superClassName);
            currentEntity = currentClass;
        }

        public void AddInterface(string name) {

            entities.Add(new InterfaceModel(name));
            currentEntity = entities[entities.Count - 1];
            int numberOfEntities = entities.Count;
            if (numberOfEntities > 1) {
                entities[numberOfEntities - 1].SetContainer(entities[numberOfEntities - 2]);
            }
            currentEntity.SetTypeOfEntity(false);
            isClass = false;
        }

        public void AddStruct(string name) {

            entities.Add(new StructModel(name));
            currentEntity = entities[entities.Count - 1];
            int numberOfEntities = entities.Count;
            if (numberOfEntities > 1) {
                entities[numberOfEntities - 1].SetContainer(entities[numberOfEntities - 2]);
            }
            currentEntity.SetTypeOfEntity(false);
            isClass = false;
            isStruct = true;
        }

        public void SetContainer() {

            if (entities.Count > 1) {

                currentEntity.SetContainer(entities[entities.Count - 1]);
            }
        }

        public void FinishEntity() {

            //Check if the last entity is a struct
            if (entities.Count == 1 && !isStruct) {
                allEntities.Add(entities[entities.Count - 1]);
            }
            entities.RemoveAt(entities.Count - 1);
            currentEntity = entities.Count > 0 ? entities[entities.Count - 1] : null;
            isClass = false;
            isStruct = false;
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

        public void AddInterfaceToEntity(string interface_) {
            if (currentEntity == null)
                return;
            currentEntity.AddInterfaceName(interface_);
        }

        public void SetInterfaceBase(string interface_) {

            if (currentEntity == null)
                return;
        }

        public ClassModel GetClass() {

            return currentEntity as ClassModel;
        }

        public void AddConstructor(Constructor constructor) {
            if (currentEntity == null)
                return;

            if (!isStruct) {
                var currentClass = (ClassModel)currentEntity;
                currentClass.AddConstructor(constructor);
                currentEntity = currentClass;
            }
            else {
                var currentStruct = (StructModel)currentEntity;
                currentStruct.AddConstructor(constructor);
                currentEntity = currentStruct;
            }
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


        /// <summary>
        /// Given a list of BaseModels it relates all elements as they are connected
        /// </summary>
        /// <param name="selectedEntities">List of entities</param>
        public static void RelateEntities(ref List<BaseModel> selectedEntities) {

            for (int i = 0; i < selectedEntities.Count; i++) {

                // Checks for class
                if (selectedEntities[i].IsClass()) {
                    ClassModel classModel = (ClassModel)selectedEntities[i];
                    // The class has a super class
                    if (classModel.GetSuperClassName() != "") {
                        // Check for the Super Class to exist on a local file
                        int index = FindEntityWithName(ref selectedEntities, classModel.GetSuperClassName());
                        // The fact that the entity was not found doesn't mean it is a class.
                        // But if it is not in the files, the screw it, i'll say it is a class
                        if(index == -1) {
                            // If it doesn't exist, then create a new one because a window of it is still needed
                            ClassModel superClass = new ClassModel(classModel.GetSuperClassName());
                            superClass.SetTypeOfEntity(true);
                            classModel.SetSuperClass(superClass);
                            selectedEntities.Add(superClass);
                        }
                        else {
                            //Check if the entity is a class  or an interface and handle each one
                            if (selectedEntities[index].IsClass()) {
                                classModel.SetSuperClass((ClassModel)selectedEntities[index]);
                            }
                            else {
                                //InterfaceModel interfaceModel = new InterfaceModel(classModel.GetSuperClassName());
                                //interfaceModel.SetTypeOfEntity(false);
                                classModel.AddInterfaceName(selectedEntities[index].GetName());
                                classModel.AddInterface(selectedEntities[index] as InterfaceModel);
                                classModel.DeleteSuperClass();
                            }
                        }
                    }

                    // Check if the class implements interfaces
                    if(classModel.GetInterfaceNames() != null) { 

                        foreach(string interface_ in classModel.GetInterfaceNames()) {
                            // Check for the Interface to exist on a local file
                            int index = FindEntityWithName(ref selectedEntities, interface_);

                            // The interface is not defined in a loca file
                            if(index == -1) {
                                InterfaceModel interfaceModel = new InterfaceModel(interface_);
                                interfaceModel.SetTypeOfEntity(false);

                                selectedEntities[i].AddInterface(interfaceModel);
                                selectedEntities.Add(interfaceModel);    
                            }
                            else {
                                //The interface is found on index
                                selectedEntities[i].AddInterface(selectedEntities[index] as InterfaceModel);
                            }
                        }
                    }
                }
                else {
                    InterfaceModel interfaceModel = (InterfaceModel)selectedEntities[i];

                    // Check if the class implements interfaces
                    if (interfaceModel.GetInterfaceNames() != null) {

                        foreach (string interface_ in interfaceModel.GetInterfaceNames()) {
                            // Check for the Interface to exist on a local file
                            int index = FindEntityWithName(ref selectedEntities, interface_);

                            //Debug.Log(interfaceModel.GetName() + " implements " + interface_);
                            // The interface is not defined in a loca file
                            if (index == -1) {
                                InterfaceModel newInterfaceToAdd = new InterfaceModel(interface_);
                                newInterfaceToAdd.SetTypeOfEntity(false);
                                selectedEntities[i].AddInterface(newInterfaceToAdd);
                                selectedEntities.Add(newInterfaceToAdd);
                            }
                            else {
                                //The interface is found on index
                                selectedEntities[i].AddInterface(selectedEntities[index] as InterfaceModel);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the index of the Class with a name X in a list of BaseEntities
        /// </summary>
        /// <returns>The class with name.</returns>
        /// <param name="entities">List of entities.</param>
        /// <param name="entityName">Class name.</param>
        public static int FindEntityWithName(ref List<BaseModel> entities, string entityName) {

            // Iterate trough all the models to find class with name "className"
            for (int i=0; i<entities.Count; i++) { 
                // If the entity is a class and the name 
                if(entities[i].GetName() == entityName) {
                    return i;
                }
            }
            // If not found, then return null
            return -1;
        }
    }
}
