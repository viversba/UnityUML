using System;
using System.Collections.Generic;

namespace DEngine.Model {

    [Serializable]
    public class InterfaceModel : BaseModel {

        public InterfaceModel(string name) {

            this.name = name;
            Type = EntityTypes.INTERFACE;
        }
    }
}
