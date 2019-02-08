using System;
using System.Collections.Generic;

namespace DEngine.Model {

    [Serializable]
    public class NamespaceModel {

        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Parent namespace. Will be set to Global namespace by default
        /// </summary>
        [NonSerialized]
        private NamespaceModel parent;
        /// <summary>
        /// List of childs namespaces
        /// </summary>
        [NonSerialized]
        private List<NamespaceModel> childs;
        private List<string> childNames;

        /// <summary>
        /// List containing all entities declared in the current Namespace
        /// </summary>
        private List<BaseModel> entities;
        /// <summary>
        /// Name of the namespace
        /// </summary>
        private string name;

        public NamespaceModel(string name) {

            this.name = name;
            childs = null;
            childNames = null;
            parent = null;
            entities = null;
        }

        public void AddEntity(BaseModel entity) {

            if (entities == null)
                entities = new List<BaseModel>();

            entities.Add(entity);
        }

        public void AddChilds(string[] names) {

            if (name.Length <= 0)
                throw new Exception("empty arrays cannot be passed as arguments");

            int index = -1;

            if (!childNames.Contains(names[0])) {
                childNames.Add(names[0]);
                NamespaceModel newNamespace = new NamespaceModel(names[0]);
                newNamespace.SetParent(this);
                childs.Add(newNamespace);

                index = childs.Count - 1;
            }
            else {
                index = childNames.IndexOf(names[0]);
            }

            if (names.Length > 1) {
                string[] subNames = new string[names.Length - 1];
                Array.Copy(names, 1, subNames, 0, subNames.Length);

                childs[index].AddChilds(subNames);
            }
        }

        public void SetParent(NamespaceModel parent) {
            this.parent = parent;
        }
    }
}
