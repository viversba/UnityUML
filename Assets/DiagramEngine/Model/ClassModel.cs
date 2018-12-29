using System;
using System.Collections.Generic;

namespace DEngine.Model {

    public class ClassModel : BaseModel {

        private List<Constructor> constructors;
        private List<ClassModel> subClasses;
        private ClassModel superClass;

        public ClassModel(string name) : base(){

            this.name = name;
            constructors = new List<Constructor>();
            superClass = null;
            subClasses = new List<ClassModel>();
        }

        public ClassModel() : base() {

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
