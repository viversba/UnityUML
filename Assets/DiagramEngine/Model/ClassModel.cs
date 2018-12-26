using System;
using System.Collections.Generic;

namespace DEngine.Model {

    public class ClassModel : BaseModel {

        private List<InterfaceModel> interfaces;
        private List<Constructor> constructors;
        private ClassModel superClass;

        public ClassModel() : base() {

            constructors = new List<Constructor>();
            superClass = null;
            interfaces = new List<InterfaceModel>();
        }

        public void AddInterface(InterfaceModel interface_) {

            interfaces.Add(interface_);
        }

        public List<InterfaceModel> GetInterfaces() {

            return interfaces;
        }

        public ClassModel GetSuperClass() {

            return superClass;
        }

        public void SetSuperClass(ClassModel superClass) {

            this.superClass = superClass;
        }

        public void AddConstructor(Constructor constructor) {

            constructors.Add(constructor);
        }

        public List<Constructor> GetConstructors() {

            return constructors;
        }

        public void ClearConstructors() {

            constructors.Clear();
        }

        ~ClassModel(){
            constructors.Clear();
        }
    }
}
