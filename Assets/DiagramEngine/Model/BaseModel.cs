using System;
using System.Collections.Generic;
using UnityEngine;

namespace DEngine.Model {

    [Serializable]
    public class BaseModel {

        /// <summary>
        /// Gets or sets the type of entity.
        /// </summary>
        /// <value>The type.</value>
        [SerializeField]
        public EntityTypes Type { get; set; }

        public bool Partial { get; set; }

        /// <summary>
        /// Name of the entity.
        /// </summary>
        [SerializeField]
        protected string name;

        /// <summary>
        /// List of contained methods
        /// </summary>
        [SerializeField]
        protected List<Method> methods;

        /// <summary>
        /// List of attributes
        /// </summary>
        [SerializeField]
        protected List<Attribute> attributes;

        /// <summary>
        /// List of interfaces implemented
        /// </summary>
        [NonSerialized]
        protected List<InterfaceModel> interfaces;

        /// <summary>
        /// List of interface names
        /// </summary>
        [SerializeField]
        protected List<ImplementedType> interfaceNames;

        /// <summary>
        /// Container entity
        /// </summary>
        [NonSerialized]
        protected BaseModel container;

        [SerializeField]
        protected List<string> parameters;

        [SerializeField]
        protected AccessModifier accessModifier;

        public BaseModel() {

            name = "Generic";
            methods = null;
            attributes = null;
            interfaces = null;
            interfaceNames = null;
            container = null;
            parameters = null;
            Partial = false;
            accessModifier = AccessModifier.NONE;
        }

        public BaseModel(List<Dictionary<string, int>> ola) {

        }

        public string GetName() {
            return name;
        }

        public string GetCompleteName() {
            string completeName = name;
            if (parameters != null && parameters.Count > 0) {
                completeName += "<";
                for (int i = 0; i < parameters.Count - 1; i++) {
                    completeName += parameters[i];
                    completeName += ",";
                }
                completeName += parameters[parameters.Count - 1];
                completeName += ">";
            }
            return completeName;
        }

        public void SetName(string name) {

            this.name = name;
        }

        public void SetContainer(BaseModel container) {
            this.container = container;
        }

        public void ClearAttributes() {

            attributes.Clear();
        }

        public void AddAttribute(Attribute newAttribute) {
            attributes = attributes ?? new List<Attribute>();
            attributes.Add(newAttribute);
        }

        public void SetParameters(List<string> parameters) {
            this.parameters = parameters ?? new List<string>();
        }

        public void SetAccessModifier(AccessModifier accessModifier) {
            this.accessModifier = accessModifier;
        }

        public List<Attribute> GetAttributes() {
            return attributes ?? new List<Attribute>();
        }

        public void ClearMethods() {
            methods.Clear();
        }

        public void AddMethod(Method newMethod) {
            methods = methods ?? new List<Method>();
            methods.Add(newMethod);
        }

        public List<Method> GetMethods() {

            return methods ?? new List<Method>();
        }

        public void AddInterfaceName(ImplementedType baseName) {

            interfaceNames = interfaceNames ?? new List<ImplementedType>();
            if (!interfaceNames.Contains(baseName))
                interfaceNames.Add(baseName);
        }

        public void AddInterface(InterfaceModel interface_) {

            interfaces = interfaces ?? new List<InterfaceModel>();
            interfaces.Add(interface_);
        }

        public List<InterfaceModel> GetInterfaces() {
            return interfaces;
        }

        public List<ImplementedType> GetInterfaceNames() {
            return interfaceNames;
        }

        public override string ToString() {

            string description = name;
            description += container != null ? "<-" + container.name + "\n" : "\n";
            if (methods != null) {
                foreach (Method method in methods) {
                    description += method.ToString() + "()\n";
                }
            }
            if (attributes != null) {
                foreach (Attribute attribute in attributes) {
                    description += attribute.ToString() + "\n";
                }
            }
            return description;
        }
    }
}