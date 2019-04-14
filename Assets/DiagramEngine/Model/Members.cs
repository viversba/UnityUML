using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DEngine.Model {


    [Serializable]
    /// <summary>
    /// Enum providing all access modifier types on C#
    /// </summary>
    public enum AccessModifier {
        PRIVATE,
        PUBLIC,
        PROTECTED,
        INTERNAL,
        PROTECTED_INTERNAL,
        PRIVATE_PROTECTED,
        NONE
    }

    [Serializable]
    /// <summary>
    /// Enum providing all method types in C#
    /// </summary>
    public enum MethodType {
        VIRTUAL,
        ABSTRACT,
        NONE
    }

    [Serializable]
    /// <summary>
    /// Enum providing all class types in C#
    /// </summary>
    public enum ClassType {
        ABSTRACT,
        SEALED,
        STATIC,
        NONE
    }

    [Serializable]
    /// <summary>
    /// Enum providing all constructor types in C#
    /// </summary>
    public enum ConstructorType {
        PUBLIC,
        PRIVATE,
        OTHER
    }

    [Serializable]
    public enum StaticType {
        STATIC,
        PARTIAL,
        NONE
    }

    [Serializable]
    public enum EntityTypes {

        CLASS = 0,
        INTERFACE = 1,
        STRUCT = 2,
        ENUM = 3
    }

    [Serializable]
    /// <summary>
    /// Attribute struct used to represent a declared attribute inside a class;
    /// </summary>
    public struct Attribute {

        public AccessModifier modifier;
        public StaticType staticType;
        public GenericType returnType;
        public string name;

        public Attribute(string name, GenericType returnType, AccessModifier modifier = AccessModifier.NONE, StaticType staticType = StaticType.NONE) {

            this.name = name;
            this.returnType = returnType;
            this.modifier = modifier;
            this.staticType = staticType;
        }

        public override string ToString() {
            string description = staticType != StaticType.NONE ? (staticType.ToString() + " ") : "";
            description += modifier != AccessModifier.NONE ? (modifier.ToString() + " ") : "";
            description += returnType.ToString() + " " + name;
            return description;
        }
    }

    [Serializable]
    /// <summary>
    /// Method struct used to represent a declared method iside a class
    /// </summary>
    public struct Method {

        [SerializeField]
        public AccessModifier modifier;
        public MethodType type;
        public StaticType staticType;
        public List<Parameter> parameters;
        public GenericType returnType;
        public string name;

        public Method(string name, GenericType returnType, List<Parameter> parameters = null, AccessModifier modifier = AccessModifier.NONE, MethodType type = MethodType.NONE, StaticType staticType = StaticType.NONE) {
            this.name = name;
            this.modifier = modifier;
            this.returnType = returnType;
            this.type = type;
            this.staticType = staticType;
            if (parameters != null) {
                this.parameters = new List<Parameter>();
                this.parameters.AddRange(parameters);
            }
            else {
                this.parameters = parameters;
            }
        }

        public override string ToString() {
            string description = type != MethodType.NONE ? (type.ToString() + " ") : "";
            description += modifier != AccessModifier.NONE ? (modifier.ToString() + " ") : "";
            description += returnType + " " + name;
            description += "(";
            if (parameters != null && parameters.Count > 0) {
                for (int i = 0; i < parameters.Count - 1; i++) {
                    description += parameters[i].ToString();
                    description += ",";
                }
                description += parameters[parameters.Count - 1].ToString();
            }
            description += ")";
            return description;
        }
    }

    [Serializable]
    /// <summary>
    /// Constructor struct used to represent a declated constructor list inside a class
    /// </summary>
    public struct Constructor {

        public string name;
        public AccessModifier modifier;
        public List<Parameter> parameters;
        public StaticType staticType;

        public Constructor(string name, List<Parameter> parameters = null, AccessModifier modifier = AccessModifier.NONE, StaticType staticType = StaticType.NONE) {
            this.name = name;
            this.staticType = staticType;
            this.modifier = modifier;
            if (parameters != null) {
                this.parameters = new List<Parameter>();
                this.parameters.AddRange(parameters);
            }
            else {
                this.parameters = parameters;
            }

        }

        public override string ToString() {
            string description = modifier != AccessModifier.NONE ? (modifier.ToString() + " ") : "";
            description += staticType != StaticType.NONE ? (staticType.ToString() + " ") : "";
            description += name;
            description += "(";
            if (parameters != null && parameters.Count > 0) {
                for (int i = 0; i < parameters.Count - 1; i++) {
                    description += parameters[i].ToString();
                    description += ",";
                }
                description += parameters[parameters.Count - 1].ToString();
            }
            description += ")";
            return description;
        }

    }

    [Serializable]
    public struct Parameter {

        public bool Params { get; set; }
        public GenericType type;
        public string name;
        public int arraySize;

        public Parameter(GenericType type, string name, bool Params = false, int arraySize = 0) {

            this.type = type;
            this.name = name;
            this.Params = Params;
            this.arraySize = arraySize;
        }

        public override string ToString() {
            string description = "";
            description += type.name;
            if (type.type != null && type.type.Count > 0) {
                description += "<";
                for (int i = 0; i < type.type.Count - 1; i++) {
                    description += type.type[i].ToString();
                    description += ",";
                }
                description += type.type[type.type.Count - 1].ToString();
                description += ">";
            }
            description += " " + name;
            return description;
        }
    }

    [Serializable]
    public class GenericType {

        public string name;
        [NonSerialized]
        public List<GenericType> type;
        public int rankSpecifier;

        public GenericType(string name, List<GenericType> type = null, int rankSpecifier = 0) {
            this.name = name;
            if (type != null) {
                this.type = new List<GenericType>();
                this.type.AddRange(type);
            }
            else {
                this.type = null;
            }
            this.rankSpecifier = rankSpecifier;
        }

        public void AddGenericType(GenericType genericType) {
            type = type ?? new List<GenericType>();
            type.Add(genericType);
        }

        public override string ToString() {
            string description = name;
            if (type != null && type.Count > 0) {
                description += "<";
                for (int i = 0; i < type.Count - 1; i++) {
                    description += type[i].name;
                    description += ",";
                }
                description += type[type.Count - 1].name;
                description += ">";
            }
            return description;
        }
    }

    [Serializable]
    public struct ImplementedType {

        public string Name { get { return this.name; } set { this.name = value; } }

        private string name;
        public List<string> types;

        public ImplementedType(string name = null, List<string> types = null) {
            this.name = name;

            if (types != null) {
                this.types = new List<string>();
                this.types.AddRange(types);
            }
            else {
                this.types = types;
            }
        }

        public void AddType(string type) {
            this.types = this.types ?? new List<string>();
            this.types.Add(type);
        }

        public string GetName() {
            return name;
        }

        public override string ToString() {

            string description = name;
            if (types != null && types.Count > 0) {
                description += "<";
                for (int i = 0; i < types.Count - 1; i++) {
                    description += types[i];
                    description += ",";
                }
                description += types[types.Count - 1];
                description += ">";
            }
            return description;
        }
    }
}