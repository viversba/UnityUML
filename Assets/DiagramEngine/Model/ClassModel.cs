using System;
using System.Collections.Generic;

namespace DEngine.Model {

    public class ClassModel : BaseModel{

        public List<Constructor> constructors;

        public ClassModel() : base(){

            constructors = new List<Constructor>();
        }

        public void AddConstructo(Constructor constructor) {

            constructors.Add(constructor);
        }

        public List<Constructor> GetConstructors() {

            return constructors;
        }

        public void ClearConstructors() {

            constructors.Clear();
        }
    }
}
