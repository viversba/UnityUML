using System;
using System.Collections.Generic;

namespace DEngine.Model {

    public class StructModel: BaseModel {

        /// <summary>
        /// List of constructors of the current struct
        /// </summary>
        private List<Constructor> constructors;

        public StructModel(): base() {

            constructors = null;
        }

        public StructModel(string name) : base() {
            this.name = name;
            constructors = null;
        }

        public void AddConstructor(Constructor constructor) {
            if (constructors == null)
                constructors = new List<Constructor>();
            constructors.Add(constructor);
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
            return description;
        }
    }
}
