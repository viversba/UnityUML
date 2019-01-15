using System;
using System.Collections.Generic;

namespace DEngine.Model {

    [Serializable]
    public class ClassModel : BaseModel {

        /// <summary>
        /// List of constructors of the current class
        /// </summary>
        private List<Constructor> constructors;
        /// <summary>
        /// List of nested classes of the current class
        /// </summary>
        private List<ClassModel> subClasses;
        /// <summary>
        /// Super Class
        /// </summary>
        private ClassModel superClass;
        /// <summary>
        /// Name of the super class
        /// </summary>
        private string superClassName;

        public ClassModel(string name): base() {

            this.name = name;
            constructors = null;
            superClass = null;
            subClasses = null;
            superClassName = "";
        }

        public ClassModel(): base(){

            constructors = null;
            superClass = null;
            subClasses = null;
            superClassName = "";
        }

        public ClassModel GetSuperClass() {

            return superClass;
        }

        public void SetSuperClass(ClassModel superClass) {
            if (superClass == null)
                superClass = new ClassModel();
            this.superClass = superClass;
        }

        public void AddConstructor(Constructor constructor) {
            if (constructors == null)
                constructors = new List<Constructor>();
            constructors.Add(constructor);
        }

        public void AddClass(ClassModel class_) {

            subClasses.Add(class_);
        }

        public void SetSuperClassName(string superClassName) {

            this.superClassName = superClassName;
        }

        public string GetSuperClassName() {
            return superClassName;
        }

        public void DeleteSuperClass() {
            superClass = null;
            superClassName = "";
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
            if(constructors != null) {
                foreach (Constructor constructor in constructors) {
                    description += constructor.ToString() + "\n";
                }
            }
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
