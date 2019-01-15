using System;
using System.Collections.Generic;

namespace DEngine.Model {

    [Serializable]
    public class BaseModel {

        protected string name;
        protected List<Method> methods;
        protected List<Attribute> attributes;
        protected List<InterfaceModel> interfaces;
        protected List<string> interfaceNames;
        protected BaseModel container;
        protected bool isClass;

        public BaseModel() {

            name = "Generic";
            methods = null;
            attributes = null;
            interfaces = null;
            interfaceNames = null;
            container = null;
            isClass = false;
        }

        /// <summary>
        /// Set the type of entity to an interface or a class
        /// </summary>
        /// <param name="isClass"><c>true</c> if the entity is a class and false for an interface.</param>
        public void SetTypeOfEntity(bool isClass) {
            this.isClass = isClass;
        }

        public string GetName() {

            return name;
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
            if (attributes == null)
                attributes = new List<Attribute>();
            attributes.Add(newAttribute);
        }

        public List<Attribute> GetAttributes() {

            return attributes ?? new List<Attribute>();
        }

        public void ClearMethods() {

            methods.Clear();
        }

        public void AddMethod(Method newMethod) {
            if (methods == null)
                methods = new List<Method>();
            methods.Add(newMethod);
        }

        public List<Method> GetMethods() {

            return methods ?? new List<Method>();
        }

        public void AddInterfaceName(string baseName) {

            if (interfaceNames == null) {
                interfaceNames = new List<string>();
            }
            if(!interfaceNames.Contains(baseName))
                interfaceNames.Add(baseName);
        }

        public void AddInterface(InterfaceModel interface_) {

            if (interfaces == null) {
                interfaces = new List<InterfaceModel>();
            }
            interfaces.Add(interface_);
        }

        public List<InterfaceModel> GetInterfaces() {
            return interfaces;
        }

        public List<string> GetInterfaceNames() {
            return interfaceNames;
        }

        public override string ToString() {

            string description = name;
            description += container != null?  "<-" + container.name + "\n": "\n";
            if(methods != null) {
                foreach (Method method in methods) {
                    description += method.ToString() + "()\n";
                }
            }
            if(attributes != null) {
                foreach (Attribute attribute in attributes) {
                    description += attribute.ToString() + "\n";
                }
            }
            return description;
        }

        public bool IsClass() {
            return isClass;
        }
    }
}
