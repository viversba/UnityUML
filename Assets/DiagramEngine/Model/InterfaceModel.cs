using System;
using System.Collections.Generic;

namespace DEngine.Model {

    public class InterfaceModel: BaseModel {

        private List<InterfaceModel> bases;
        private List<string> baseNames;

        public InterfaceModel() {

        }

        public InterfaceModel(string name) {

            this.name = name;
        }
    }
}
