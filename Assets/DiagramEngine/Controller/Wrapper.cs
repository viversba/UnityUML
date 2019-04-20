using DEngine.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DEngine.Controller {

    public class Wrapper {

        /// <summary>
        /// List of entities currently being processed
        /// </summary>
        private List<BaseModel> entities;
        //private bool isClass;
        public BaseModel currentEntity;
        /// <summary>
        /// This will be written to disk
        /// </summary>
        public List<BaseModel> allEntities;
        /// <summary>
        /// List of found Namespaces
        /// </summary>
        public List<NamespaceModel> namespaces;
        /// <summary>
        /// Instance of the current namespace
        /// </summary>
        public NamespaceModel currentNamespace;

        /// <summary>
        /// Temporal variable that will indicate if an entity is a struct in order to dump it
        /// Will be removed when support for structs is available
        /// </summary>
        private bool isStruct;

        public Wrapper() {

            isStruct = false;
            entities = new List<BaseModel>();
            allEntities = new List<BaseModel>();
            // Global namespace will be set to a character that cannot be declared as a real namespace
            // Just to avoid dumb errors
            namespaces = new List<NamespaceModel> { new NamespaceModel("$Global") };
            currentNamespace = namespaces[0];

            currentEntity = null;
        }

        public void AddClass(string name) {

            entities.Add(new ClassModel(name));
            currentEntity = entities[entities.Count - 1];
            int numberOfEntities = entities.Count;
            if (numberOfEntities > 1) {
                currentEntity.SetContainer(entities[numberOfEntities - 2]);
            }
        }

        public void AddInterface(string name) {

            entities.Add(new InterfaceModel(name));
            currentEntity = entities[entities.Count - 1];
            int numberOfEntities = entities.Count;
            if (numberOfEntities > 1) {
                entities[numberOfEntities - 1].SetContainer(entities[numberOfEntities - 2]);
            }
        }

        public void AddStruct(string name) {

            entities.Add(new StructModel(name));
            currentEntity = entities[entities.Count - 1];
            int numberOfEntities = entities.Count;
            if (numberOfEntities > 1) {
                entities[numberOfEntities - 1].SetContainer(entities[numberOfEntities - 2]);
            }
            isStruct = true;
        }

        public void AddNamespace(string[] names) {

            if (names.Length < 0)
                throw new Exception($"Namespaces cannot have empty names + ({nameof(Wrapper)})");


            int namespaceIndex = -1;
            for (int i = 0; i < namespaces.Count; i++) {
                namespaceIndex = namespaces[i].Name == names[0] ? i : namespaceIndex;
            }

            if (namespaceIndex == -1) {
                NamespaceModel newNamespace = new NamespaceModel(names[0]);
                currentNamespace = newNamespace;

                // Global namespace should not have sub-namspaces
                // Check for sub-namespaces
                if (names.Length > 1) {
                    string[] subNames = new string[names.Length - 1];
                    Array.Copy(names, 1, subNames, 0, subNames.Length);
                    currentNamespace.AddChilds(subNames);
                }
            }
            else {
                currentNamespace = namespaces[namespaceIndex];
            }
        }

        public void SetSuperClassName(ImplementedType superClassName) {

            if (currentEntity == null)
                return;
            ClassModel currentClass = (ClassModel)currentEntity;
            currentClass.SetSuperClassName(superClassName);
            currentEntity = currentClass;
        }

        public void FinishNamespace() {

            // Always return to the global namespace
            currentNamespace = namespaces[0];
        }

        public void SetContainer() {

            if (entities.Count > 1) {

                currentEntity.SetContainer(entities[entities.Count - 1]);
            }
        }

        public void FinishEntity() {

            // Add the currentEntity to it's corresponding namespace
            currentNamespace.AddEntity(currentEntity);

            //Check if the last entity is a struct
            if (entities.Count == 1) {
                allEntities.Add(entities[entities.Count - 1]);
            }
            entities.RemoveAt(entities.Count - 1);
            currentEntity = entities.Count > 0 ? entities[entities.Count - 1] : null;
            //isClass = false;
            isStruct = false;
        }

        public void AddMethodTo(Method method) {
            if (currentEntity == null) return;
            currentEntity.AddMethod(method);
        }

        public void AddAttributeTo(Model.Attribute attribute) {
            if (currentEntity == null || currentEntity.Type == EntityTypes.ENUM || currentEntity.Type == EntityTypes.INTERFACE) return;
            currentEntity.AddAttribute(attribute);
        }

        public void AddInterfaceToEntity(ImplementedType interface_) {
            if (currentEntity == null) return;
            currentEntity.AddInterfaceName(interface_);
        }

        public void AddParametersToModel(List<string> parameters) {
            if (currentEntity.Type == EntityTypes.ENUM) return;
            currentEntity.SetParameters(parameters);
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

        #region modifier handling

        public void IsPartial(bool partial_) {
            if (currentEntity == null) return;
            currentEntity.Partial = partial_;
        }

        public void IsStatic(bool static_) {
            if (currentEntity == null) return;

            try {
                ClassModel classModel = (ClassModel)currentEntity;
                classModel.Static = static_;
                currentEntity = classModel;
            }
            catch {
                throw new InvalidCastException($"Cannot convert cast from BaseModel to ClassModel on {nameof(Wrapper)}");
            }
        }

        public void IsSealed(bool sealed_) {
            if (currentEntity == null) return;

            try {
                ClassModel classModel = (ClassModel)currentEntity;
                classModel.Sealed = sealed_;
                currentEntity = classModel;
            }
            catch {
                throw new InvalidCastException($"Cannot convert cast from BaseModel to ClassModel on {nameof(Wrapper)}");
            }
        }

        public void IsAbstract(bool abstract_) {
            if (currentEntity == null) return;

            try {
                ClassModel classModel = (ClassModel)currentEntity;
                classModel.Abstract = abstract_;
                currentEntity = classModel;
            }
            catch {
                throw new InvalidCastException($"Cannot convert cast from BaseModel to ClassModel on {nameof(Wrapper)}");
            }
        }

        public void SetAccessModifier(AccessModifier accessModifier) {
            currentEntity.SetAccessModifier(accessModifier);
        }

        #endregion

        public static T ParseEnum<T>(string value) {
            return (T)Enum.Parse(typeof(T), value, true);
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
        public static void ModifierMatch(List<string> modifiers, ref AccessModifier mod, ref StaticType attributeType) {

            foreach (string modifier in modifiers) {

                if (Enum.IsDefined(typeof(AccessModifier), modifier.ToUpper())) {
                    // It is an access modifier, convert to modifier enum
                    mod = ParseEnum<AccessModifier>(modifier);
                }
                else if (Enum.IsDefined(typeof(StaticType), modifier.ToUpper())) {
                    // If it is a method type, convert to method type enum
                    attributeType = ParseEnum<StaticType>(modifier);
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
                if (selectedEntities[i].Type == EntityTypes.CLASS) {
                    ClassModel classModel = (ClassModel)selectedEntities[i];
                    // The class has a super class
                    if (!string.IsNullOrEmpty(classModel.GetSuperClassName().Name)) {

                        // Check for the Super Class to exist on a local file
                        int index = FindEntityWithName(ref selectedEntities, classModel.GetSuperClassName().GetName());
                        // The fact that the entity was not found doesn't mean it is a class.
                        // But if it is not in the files, then screw it, i'll say it is a class
                        if (index == -1) {
                            var type = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                        from type_ in assembly.GetTypes()
                                        where type_.Name == classModel.GetSuperClassName().GetName()
                                        select type_);
                            Type myType = Type.GetType(classModel.GetSuperClassName().GetName());
                            Debug.Log(type.ToString());
                            // If it doesn't exist, then create a new one because a window of it is still needed
                            ClassModel superClass = new ClassModel(classModel.GetSuperClassName().GetName());
                            classModel.SetSuperClass(superClass);
                            selectedEntities.Add(superClass);
                        }
                        else {
                            //Check if the entity is a class  or an interface and handle each one
                            if (selectedEntities[index].Type == EntityTypes.CLASS) {
                                classModel.SetSuperClass((ClassModel)selectedEntities[index]);
                            }
                            else if (selectedEntities[index].Type == EntityTypes.INTERFACE) {
                                classModel.AddInterfaceName(new ImplementedType(selectedEntities[index].GetName()));
                                classModel.AddInterface(selectedEntities[index] as InterfaceModel);
                                classModel.DeleteSuperClass();
                            }
                        }
                    }

                    // Check if the class implements interfaces
                    if (classModel.GetInterfaceNames() != null) {

                        foreach (var interface_ in classModel.GetInterfaceNames()) {

                            string interfaceName = interface_.Name;
                            // Check for the Interface to exist on a local file
                            int index = FindEntityWithName(ref selectedEntities, interfaceName);

                            // The interface is not defined in a loca file
                            if (index == -1) {
                                InterfaceModel interfaceModel = new InterfaceModel(interfaceName);

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
                else if (selectedEntities[i].Type == EntityTypes.INTERFACE) {
                    InterfaceModel interfaceModel = (InterfaceModel)selectedEntities[i];

                    // Check if the class implements interfaces
                    if (interfaceModel.GetInterfaceNames() != null) {

                        foreach (var interface_ in interfaceModel.GetInterfaceNames()) {

                            string interfaceName = interface_.Name;
                            // Check for the Interface to exist on a local file
                            int index = FindEntityWithName(ref selectedEntities, interfaceName);

                            //Debug.Log(interfaceModel.GetName() + " implements " + interface_);
                            // The interface is not defined in a loca file
                            if (index == -1) {
                                InterfaceModel newInterfaceToAdd = new InterfaceModel(interfaceName);
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
            for (int i = 0; i < entities.Count; i++) {
                // If the entity is a class and the name 
                if (entities[i].GetName() == entityName) {
                    return i;
                }
            }
            // If not found, then return null
            return -1;
        }
    }
}