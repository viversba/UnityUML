using System.Collections;
using System.Collections.Generic;

namespace DEngine.Model {

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

    /// <summary>
    /// Enum providing all constructor types in C#
    /// </summary>
    public enum ConstructorType {
        PUBLIC,
        PRIVATE,
        STATIC
    }

    /// <summary>
    /// Attribute struct used to represent a declared attribute inside a class;
    /// </summary>
    public struct Attribute {

        public string name;
        public string type;
        public AccessModifier modifier;

        public Attribute(string name, string type) {

            modifier = AccessModifier.NONE;
            this.name = name;
            this.type = type;
        }

        public Attribute(string name, AccessModifier modifier, string type) {

            this.name = name;
            this.type = type;
            this.modifier = modifier;
        }

        public override string ToString() {
            return name;
        }
    }

    /// <summary>
    /// Method struct used to represent a declared method iside a class
    /// </summary>
    public struct Method {

        public string name;
        public AccessModifier modifier;
        public string returnType;
        public MethodType type;

        public Method(string name) {
            this.name = name;
            modifier = AccessModifier.PROTECTED;
            returnType = "void";
            type = MethodType.NONE;
        }

        public Method(string name, AccessModifier modifier, string returnType, MethodType type) {
            this.name = name;
            this.modifier = modifier;
            this.returnType = returnType;
            this.type = type;
        }

        public override string ToString() {
            return name;
        }
    }

    /// <summary>
    /// Constructor struct used to represent a declated constructor list inside a class
    /// </summary>
    public struct Constructor {

        public string name;
        public List<Attribute> attributes;
        public ConstructorType type;

        public Constructor(string name) {
            this.name = name;
            attributes = null;
            type = ConstructorType.PRIVATE;
        }

        public Constructor(string name, List<Attribute> attributes, ConstructorType type) {
            this.name = name;
            this.attributes = attributes;
            this.type = type;
        }

        public Constructor(string name, ConstructorType type) {
            this.name = name;
            attributes = null;
            this.type = type;
        }

        public Constructor(string name, List<Attribute> attributes) {
            this.name = name;
            this.attributes = attributes;
            type = ConstructorType.PRIVATE;
        }

        public override string ToString() {
            return name;
        }
    }

    /// <summary>
    /// Constructor struct used to represent a declated delegate list inside a class
    /// </summary>
    public struct Delegate {

        public string type;
        public string returnType;
        public AccessModifier modifier;

        public Delegate(string type, string returnType, AccessModifier modifier) {

            this.type = type;
            this.returnType = returnType;
            this.modifier = modifier;
        }

        public Delegate(string type, string returnType) {

            this.type = type;
            this.returnType = returnType;
            this.modifier = AccessModifier.PROTECTED;
        }

        public override string ToString() {
            return type;
        }
    }

    /// <summary>
    /// Constructor struct used to represent a declated struct list inside a class
    /// </summary>
    public struct Struct {
        public string name;
        public AccessModifier modifier;

        public Struct(string name) {
            this.name = name;
            modifier = AccessModifier.PRIVATE;
        }

        public Struct(string name, AccessModifier modifier) {
            this.name = name;
            this.modifier = modifier;
        }

        public override string ToString() {
            return name;
        }
    }

    /// <summary>
    /// Constructor struct used to represent a declated class list inside a class
    /// </summary>
    public struct Class {

        public string name;
        public ClassType type;
        public AccessModifier modifier;

        public Class(string name) {
            this.name = name;
            type = ClassType.NONE;
            modifier = AccessModifier.INTERNAL;
        }

        public Class(string name, AccessModifier modifier) {
            this.name = name;
            type = ClassType.NONE;
            this.modifier = AccessModifier.INTERNAL;
        }

        public Class(string name, ClassType type) {
            this.name = name;
            this.type = ClassType.NONE;
            modifier = AccessModifier.NONE;
        }

        public Class(string name, ClassType type, AccessModifier modifier) {
            this.name = name;
            this.type = type;
            this.modifier = modifier;
        }

        public override string ToString() {
            return name;
        }

    }
}