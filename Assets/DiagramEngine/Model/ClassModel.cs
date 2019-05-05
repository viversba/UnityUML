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
        private ImplementedType superClassName;
        /// <summary>
        /// List containing all the classes or structs used in this class
        /// </summary>
        [NonSerialized]
        private List<BaseModel> implementations;

        public bool Static { get; set; }
        public bool Sealed { get; set; }
        public bool Abstract { get; set; }

        public ClassModel(string name) {

            this.name = name;
            constructors = null;
            superClass = null;
            subClasses = null;
            superClassName = new ImplementedType();
            Type = EntityTypes.CLASS;
            implementations = null;
        }

        public ClassModel() {

            constructors = null;
            superClass = null;
            subClasses = null;
            superClassName = new ImplementedType();
            Type = EntityTypes.CLASS;
            implementations = null;
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

        public void AddImplementation(BaseModel entity) {
            implementations = implementations ?? new List<BaseModel>();

            foreach (var implementation in implementations)
                if (implementation.GetName() == entity.GetName()) return;

            implementations.Add(entity);
        }

        public void SetSuperClassName(ImplementedType superClassName) {
            this.superClassName = superClassName;
        }

        public ImplementedType GetSuperClassName() {
            return superClassName;
        }

        public void DeleteSuperClass() {
            superClass = null;
            superClassName = new ImplementedType();
        }

        public List<BaseModel> GetImplementations() {
            return implementations ?? new List<BaseModel>();
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
            description += "Constructors: \n";
            if (constructors != null) {
                foreach (Constructor constructor in constructors) {
                    description += constructor.ToString() + "\n";
                }
            }
            description += "Methods: \n";
            if (methods != null) {
                foreach (Method method in methods) {
                    description += method.ToString() + "\n";
                }
            }
            description += "Attributes: \n";
            if (attributes != null) {
                foreach (Attribute attribute in attributes) {
                    description += attribute.ToString() + "\n";
                }
            }
            return description;
        }
    }
}