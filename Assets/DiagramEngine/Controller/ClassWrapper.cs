using DEngine.Model;
using System.Collections.Generic;
using System;

namespace DEngine.Controller {

    public class ClassWrapper {

        private List<BaseModel> entities;
        private bool isClass;
        public BaseModel currentEntity;

        public ClassWrapper() {

            isClass = false;
            entities = new List<BaseModel>();
            currentEntity = null;
        }

        public void SetContainer() {

            if (entities.Count > 1) {

                currentEntity.SetContainer(entities[entities.Count - 1]);
            }
        }

        public void FinishEntity() {

            entities.RemoveAt(entities.Count - 1);
            if(entities.Count > 0) {
                currentEntity = entities[entities.Count - 1];
            }
            else {
                currentEntity = null;
            }
            isClass = false;
        }

        public void AddClass(string name) {

            entities.Add(new ClassModel(name));
            currentEntity = entities[entities.Count - 1];
            isClass = true;
        }

        public void AddInterface(string name) {

            entities.Add(new InterfaceModel(name));
        }

        //public void SetClass(string name) {

        //    currentEntity.SetName(name);
        //}

        public void AddMethodTo(Method method) {

            currentEntity.AddMethod(method);
        }

        public void AddAttributeTo(Model.Attribute attribute) {

            currentEntity.AddAttribute(attribute);
        }

        public void AddInterface(InterfaceModel interface_) {

            currentEntity.AddInterface(interface_);
        }

        public ClassModel GetClass() {

            return currentEntity as ClassModel;
        }

        public void AddConstructor(Constructor constructor) {

            var currentClass = (ClassModel)currentEntity;
            currentClass.AddConstructor(constructor);
            currentEntity = currentClass;
        }

        public static T ParseEnum<T>(string value) {
            return (T)Enum.Parse(typeof(T),value, true);
        }
    }

}
