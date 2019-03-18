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
        PARTIAL,
        STATIC,
        NONE
    }

    [Serializable]
    /// <summary>
    /// Enum providing all class types in C#
    /// </summary>
    public enum ClassType {
        ABSTRACT,
        PARTIAL,
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
        STATIC,
        OTHER
    }

    [Serializable]
    public enum StaticType { 
        STATIC,
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
        public string name;
        public string returnType;
        public string[] returnSubTypes; 


        public Attribute(string name, string type): this(name, AccessModifier.NONE, type, StaticType.NONE, null) {}

        public Attribute(string name, StaticType staticType, string type): this(name, AccessModifier.NONE, type, staticType, null) {}

        public Attribute(string name, AccessModifier modifier, string type, StaticType staticType) : this(name, modifier, type, staticType, null) {}

        public Attribute(string name, AccessModifier modifier, string returnType, StaticType staticType, string[] returnSubTypes) {

            this.name = name;
            this.returnType = returnType;
            this.modifier = modifier;
            this.staticType = staticType;
            this.returnSubTypes = returnSubTypes ?? null;
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
        public string returnType;
        public string[] returnSubTypes;
        public string name;

        public Method(string name): this(name, AccessModifier.PROTECTED, "void", MethodType.NONE, null) {}

        public Method(string name, AccessModifier modifier, string returnType, MethodType type): this(name, modifier, returnType, type, null) {}

        public Method(string name, AccessModifier modifier, string returnType, MethodType type, string[] returnSubTypes) {
            this.name = name;
            this.modifier = modifier;
            this.returnType = returnType;
            this.type = type;
            this.returnSubTypes = returnSubTypes ?? null;
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
        public MethodType type;

        public Constructor(string name): this(name, AccessModifier.NONE, MethodType.NONE) {}

        public Constructor(string name, AccessModifier modifier): this(name, modifier, MethodType.NONE) {}

        public Constructor(string name, MethodType type): this(name, AccessModifier.NONE, type) {}

        public Constructor(string name, AccessModifier modifier, MethodType type) {

            this.name = name;
            this.type = type;
            this.modifier = modifier;
        }

        public override string ToString() {
            //string description = modifier != AccessModifier.NONE ? (modifier.ToString() + " ") : "";
            string description = type != MethodType.NONE ? (type.ToString() + " ") : "";
            description += name;
            return description;
        }
    }
}