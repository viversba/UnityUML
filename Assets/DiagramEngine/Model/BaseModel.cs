using System;
using System.Collections.Generic;

namespace DEngine.Model {

    public class BaseModel {

        private string name;
        private List<Method> methods;
        private List<Attribute> attributes;

        public BaseModel() {

            name = "Generic";
            methods = new List<Method>();
            attributes = new List<Attribute>();
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

        public List<Attribute> GetAttributes() {

            return attributes;
        }

        ~BaseModel() {
            methods.Clear();
            attributes.Clear();
        }
    }
}
