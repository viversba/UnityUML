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
        NONE
    }

    [Serializable]
    /// <summary>
    /// Enum providing all method types in C#
    /// </summary>
    public enum MethodType {
        VIRTUAL,
        ABSTRACT,
        STATIC,
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
            description += returnType + " " + name;
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
            this.parameters = parameters;
        }

        public override string ToString() {
            string description = type != MethodType.NONE ? (type.ToString() + " " ) : "";
            description += returnType + " " + name;
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
            this.parameters = parameters;
        }

        public override string ToString() {
            string description = staticType != StaticType.NONE ? (staticType.ToString() + " ") : "";
            description += name;
            return description;
        }
    }

    [Serializable]
    public struct Parameter {

        public bool Params { get; set; }
        public GenericType type;
        public string name;
        public int arraySize;

        public Parameter(GenericType type, string name, bool Params = false, int arraySize = 0){

            this.type = type;
            this.name = name;
            this.Params = Params;
            this.arraySize = arraySize;
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
            this.type = type;
            this.rankSpecifier = rankSpecifier;
        }

        public void AddGenericType(GenericType genericType) {
            type = type ?? new List<GenericType>();
            type.Add(genericType);
        }
    }

    [Serializable]
    public struct ImplementedType{

        public string name;
        public List<string> types;

        public ImplementedType(string name, List<string> types = null) {
            this.name = name;
            this.types = types;
        }

        public void AddType(string type) {
            this.types.Add(type);
        }

        public string GetName() {
            return name;
        }
    }
}