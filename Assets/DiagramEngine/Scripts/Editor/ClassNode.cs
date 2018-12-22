using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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

/// <summary>
/// Attribute struct used to represent a declared attribute inside a class;
/// </summary>
public struct Attribute {

    string name;
    string type;
    AccessModifier modifier;

    public Attribute(string name, string type) {

        this.modifier = AccessModifier.NONE;
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

    string name;
    AccessModifier modifier;
    string returnType;

    public Method(string name, AccessModifier modifier, string returnType) {
        this.name = name;
        this.modifier = modifier;
        this.returnType = returnType;
    }

    public override string ToString()
    {
        return name;
    }
}

/// <summary>
/// Constructor struct used to represent a declated constructor list inside a class
/// </summary>
public struct Constructor {

    string name;
    List<Attribute> attributes;

    public Constructor(string name) {
        this.name = name;
        this.attributes = null;
    }

    public Constructor(string name, List<Attribute> attributes) {
        this.name = name;
        this.attributes = attributes;
    }
}

/// <summary>
/// Constructor struct used to represent a declated delegate list inside a class
/// </summary>
public struct Delegate {

    string type;
    string returnType;
    AccessModifier modifier;

    public Delegate(string type, string returnType, AccessModifier modifier) {

        this.type = type;
        this.returnType = returnType;
        this.modifier = modifier;
    }

    public override string ToString(){
        return type;
    }
}

/// <summary>
/// Constructor struct used to represent a declated struct list inside a class
/// </summary>
public struct Struct {
    string name;

    public Struct(string name) {
        this.name = name;
    }

    public override string ToString()
    {
        return name;
    }
}

/// <summary>
/// Constructor struct used to represent a declated class list inside a class
/// </summary>
public struct Class {

    string name;

    public Class(string name) {
        this.name = name;
    }

    public override string ToString()
    {
        return name;
    }

}

public class ClassNode : BaseNode
{
    private List<Attribute> attributes = new List<Attribute>();
    private List<Method> methods = new List<Method>();
    private List<Constructor> constructors = new List<Constructor>();
    private List<Delegate> delegates = new List<Delegate>();
    private List<Struct> structs = new List<Struct>();
    private List<Class> classes = new List<Class>();

    public int numberOfLines;

    //List<string> attributesList = new List<string>(),
                //methodsList = new List<string>(),
                //constructorsList = new List<string>(),
                //delegatesList = new List<string>(),
                //structsList = new List<string>(),
                //classesList = new List<string>();

    public ClassNode(string title) {

        windowTitle = title;
        hasInputs = false;
        numberOfLines = 0;
        Initialize();
    }

    public ClassNode() {
        windowTitle = "Generic Class";
        hasInputs = false;
        numberOfLines = 0;
        Initialize();
    }

    private void Initialize() {

        attributes = new List<Attribute>();
        methods = new List<Method>();
        delegates = new List<Delegate>();
        constructors = new List<Constructor>();
        structs = new List<Struct>();
        classes = new List<Class>();

        numberOfLines = 0;

        AddAttribute("nico", "void", AccessModifier.INTERNAL);
        AddAttribute("mona", "int", AccessModifier.PROTECTED);
        AddAttribute("juancho", "float", AccessModifier.PUBLIC);

        AddMethod("GetString", AccessModifier.PRIVATE, "string");
        AddMethod("SetString", AccessModifier.PUBLIC, "void");
        AddMethod("Add", AccessModifier.PROTECTED, "int");
    }

    public void AddClass(string name) {
        classes.Add(new Class(name));
        AddLine();
    }

    public void AddMethod(string name, AccessModifier modifier, string returnType) {
        methods.Add(new Method(name, modifier, returnType));
        AddLine();
    }

    public void AddAttribute(string name, string type, AccessModifier modifier) {
        attributes.Add(new Attribute(name, modifier, type));
        AddLine();
    }

    public void AddConstructor(string name, string[] attribs) {
        if (attribs.Length > 0){
            constructors.Add(new Constructor(name));
        }
        else {
            List<Attribute> attrbs = null;
            foreach(string att in attribs) {
                Debug.LogWarning("List of attributes is being declared with the name on both fields");
                #pragma warning List of attributes is being declared with the name on both fields
                attrbs.Add(new Attribute(name, name));
            }
        }
        AddLine();
    }

    public void AddDelegate(string type, string returnType, AccessModifier modifier) {

        delegates.Add(new Delegate(type, returnType, modifier));
        AddLine();
    }

    public void AddStruct(string name) {

        structs.Add(new Struct(name));
        AddLine();
    }

    public void AddLine() {
        numberOfLines++;
        Debug.Log("Line added!!!!");
    }

    public override void DrawCurves(){
    }

    public override void DrawWindow(){
        base.DrawWindow();

        //Initialize();

        //GUILayout.Label("Ola");
        foreach (Attribute attribute in attributes) {
            GUILayout.Label(attribute.ToString());
        }
        foreach (Method method in methods){
            GUILayout.Label(method.ToString());
        }
        foreach (Constructor constructor in constructors){
            GUILayout.Label(constructor.ToString());
        }
        foreach (Struct struct_ in structs){
            GUILayout.Label(struct_.ToString());
        }
        foreach (Delegate delegate_ in delegates){
            GUILayout.Label(delegate_.ToString());
        }
        foreach (Class class_ in classes){
            GUILayout.Label(class_.ToString());
        }
    }

    public override void Tick(float deltaTime)
    {
    }
}
