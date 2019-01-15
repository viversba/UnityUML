using System.Collections;
using System.Collections.Generic;
using System;

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
    public enum AttributeType { 
        STATIC,
        NONE
    }

    [Serializable]
    /// <summary>
    /// Attribute struct used to represent a declared attribute inside a class;
    /// </summary>
    public struct Attribute {

        public AccessModifier modifier;
        public AttributeType attributeType;
        public string name;
        public string type;


        public Attribute(string name, string type): this(name, AccessModifier.NONE, type, AttributeType.NONE) {}

        public Attribute(string name, AttributeType attributeType, string type): this(name, AccessModifier.NONE, type, attributeType) {}

        public Attribute(string name, AccessModifier modifier, string type, AttributeType attributeType) {

            this.name = name;
            this.type = type;
            this.modifier = modifier;
            this.attributeType = attributeType;
        }

        public override string ToString() {
            string description = modifier != AccessModifier.NONE ? (modifier.ToString() + " ") : "";
            description += attributeType != AttributeType.NONE ? (attributeType.ToString() + " ") : "";
            description += type + " " + name;
            return description;
        }
    }

    [Serializable]
    /// <summary>
    /// Method struct used to represent a declared method iside a class
    /// </summary>
    public struct Method {

        public AccessModifier modifier;
        public MethodType type;
        public string returnType;
        public string name;

        public Method(string name): this(name, AccessModifier.PROTECTED, "void", MethodType.NONE) {}

        public Method(string name, AccessModifier modifier, string returnType, MethodType type) {
            this.name = name;
            this.modifier = modifier;
            this.returnType = returnType;
            this.type = type;
        }

        public override string ToString() {
            string description = modifier != AccessModifier.NONE? (modifier.ToString() + " " ) : "";
            description += type != MethodType.NONE ? (type.ToString() + " " ) : "";
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
            string description = modifier != AccessModifier.NONE ? (modifier.ToString() + " ") : "";
            description += type != MethodType.NONE ? (type.ToString() + " ") : "";
            description += name;
            return description;
        }
    }
}