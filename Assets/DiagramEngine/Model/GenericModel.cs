using System;

namespace DEngine.Model {

    public class GenericModel : BaseModel {
        public GenericModel(string name) {

            this.name = name;
        }

        public InterfaceModel GetAsInterfaceModel() {

            return new InterfaceModel(this.name);
        }

        public ClassModel GetAsClassModel() {

            return new ClassModel(this.name);
        }
    }
}

