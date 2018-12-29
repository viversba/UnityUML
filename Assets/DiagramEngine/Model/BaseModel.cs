using System;
using System.Collections.Generic;

namespace DEngine.Model {

    public class BaseModel {

        protected string name;
        protected List<Method> methods;
        protected List<Attribute> attributes;
        protected List<InterfaceModel> interfaces;
        protected BaseModel container;

        public BaseModel() {

            name = "Generic";
            methods = new List<Method>();
            attributes = new List<Attribute>();
            interfaces = new List<InterfaceModel>();
            container = null;
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

        ~BaseModel() {
            methods.Clear();
            attributes.Clear();
        }
    }
}
