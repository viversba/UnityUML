using System;
using System.Collections.Generic;

namespace DEngine.Model {

    public class ClassModel : BaseModel {

        private List<Constructor> constructors;
        private List<ClassModel> subClasses;
        private ClassModel superClass;

        public ClassModel(string name){

            this.name = name;
            constructors = new List<Constructor>();
            superClass = null;
            subClasses = new List<ClassModel>();
        }

        public ClassModel(){

            constructors = new List<Constructor>();
            superClass = null;
            subClasses = new List<ClassModel>();
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

        public void AddClass(ClassModel class_) {

            subClasses.Add(class_);
        }

        public List<Constructor> GetConstructors() {

            return constructors ?? new List<Constructor>();
        }

        public void ClearConstructors() {

            constructors.Clear();
        }

        public override string ToString() {

            string description = name;
            description += container != null ? "<-" + container.GetName() + "\n" : "\n";
            foreach (Constructor constructor in constructors) {
                description += constructor.ToString() + "\n";
            }
            foreach(Method method in methods) {
                description += method.ToString() + "()\n";
            }
            foreach(Attribute attribute in attributes) {
                description += attribute.ToString() + "\n";
            }
            return description;
        }

        ~ClassModel(){
            constructors.Clear();
        }
    }
}
