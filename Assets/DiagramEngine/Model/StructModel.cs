using System;
using System.Collections.Generic;

namespace DEngine.Model {

    [Serializable]
    public class StructModel: BaseModel {

        /// <summary>
        /// List of constructors of the current struct
        /// </summary>
        private List<Constructor> constructors;
        /// <summary>
        /// List containing all the classes or structs used in this class
        /// </summary>
        [NonSerialized]
        private List<BaseModel> implementations;

        public StructModel(): base() {

            constructors = null;
            Type = EntityTypes.STRUCT;
            implementations = null;
        }

        public StructModel(string name) : base() {
            this.name = name;
            constructors = null;
            Type = EntityTypes.STRUCT;
            implementations = null;
        }

        public void AddConstructor(Constructor constructor) {
            if (constructors == null)
                constructors = new List<Constructor>();
            constructors.Add(constructor);
        }

        public void AddImplementation(BaseModel entity)
        {
            implementations = implementations ?? new List<BaseModel>();

            foreach (var implementation in implementations)
                if (implementation.GetName() == entity.GetName()) return;

            implementations.Add(entity);
        }

        public List<Constructor> GetConstructors() {

            return constructors ?? new List<Constructor>();
        }

        public List<BaseModel> GetImplementations(){
            return implementations ?? new List<BaseModel>();
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
            return description;
        }
    }
}
