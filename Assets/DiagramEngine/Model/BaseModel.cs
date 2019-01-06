using System;
using System.Collections.Generic;

namespace DEngine.Model {

    public class BaseModel {

        protected string name;
        protected List<Method> methods;
        protected List<Attribute> attributes;
        protected List<InterfaceModel> interfaces;
        protected BaseModel container;
        protected bool isClass;

        public BaseModel() {

            name = "Generic";
            methods = new List<Method>();
            attributes = new List<Attribute>();
            interfaces = new List<InterfaceModel>();
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

        public void ClearAttributes() {

            attributes.Clear();
        }

        public void AddAttribute(Attribute newAttribute) {
            attributes.Add(newAttribute);
        }

        public void ClearMethods() {

            methods.Clear();
        }

        public void AddMethod(Method newMethod) {
            methods.Add(newMethod);
        }

        public List<Method> GetMethods() {

            return methods;
        }

        public void AddInterface(InterfaceModel interface_) {

            interfaces.Add(interface_);
        }

        public List<Attribute> GetAttributes() {

            return attributes;
        }

        public List<InterfaceModel> GetInterfaces() {

            return interfaces;
        }

        public void SetContainer(BaseModel container) {
            this.container = container;
        }

        public override string ToString() {

            string description = name;
            description += container != null?  "<-" + container.name + "\n": "\n";
            foreach (Method method in methods) {
                description += method.ToString() + "()\n";
            }
            foreach (Attribute attribute in attributes) {
                description += attribute.ToString() + "\n";
            }
            return description;
        }

        ~BaseModel() {
            methods.Clear();
            attributes.Clear();
        }
    }
}
