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
        [NonSerialized]
        private List<ClassModel> subClasses;
        /// <summary>
        /// Super Class
        /// </summary>
        [NonSerialized]
        private ClassModel superClass;
        /// <summary>
        /// Name of the super class
        /// </summary>
        private string superClassName;

        public ClassModel(string name) {

            this.name = name;
            constructors = null;
            superClass = null;
            subClasses = null;
            superClassName = "";
            Type = EntityTypes.CLASS;
        }

        public ClassModel() {

            constructors = null;
            superClass = null;
            subClasses = null;
            superClassName = "";
            Type = EntityTypes.CLASS;
        }

        public ClassModel(Dictionary<string,int> k,List<Dictionary<string, int>> ola) {

            constructors = null;
            superClass = null;
            subClasses = null;
            superClassName = "";
            Type = EntityTypes.CLASS;
        }

        public ClassModel GetSuperClass() {

            return superClass;
        }

        public void SetSuperClass(ClassModel superClass) {
            this.superClass = superClass ?? new ClassModel();
        }

        public void AddConstructor(Constructor constructor) {
            constructors = constructors ?? new List<Constructor>();
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
            if (constructors != null) {
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