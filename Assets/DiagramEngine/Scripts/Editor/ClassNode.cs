using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ClassNode : BaseNode
{
    private List<Attribute> attributes = new List<Attribute>();
    private List<Method> methods = new List<Method>();
    private List<Constructor> constructors = new List<Constructor>();
    private List<Delegate> delegates = new List<Delegate>();
    private List<Struct> structs = new List<Struct>();
    private List<Class> classes = new List<Class>();

    public int numberOfLines;

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

        AddConstructor("CharacterController");
        AddConstructor("CharacterController", new string[] { "size", "scale" });
        AddConstructor("MapController", new string[] { "name", "texture" });

        AddStruct("ConsoleType", AccessModifier.PUBLIC);
        AddStruct("ConsoleSize", AccessModifier.PRIVATE);
        AddStruct("Types");

        AddDelegate("Hola", "int", AccessModifier.INTERNAL);
        AddDelegate("Nico", "void");
        AddDelegate("Yenny", "float");

    }

    public void AddClass(string name) {
        classes.Add(new Class(name));
        AddLine();
    }

    public void AddMethod(string name, AccessModifier modifier, string returnType) {
        methods.Add(new Method(name, modifier, returnType, MethodType.ABSTRACT));
        AddLine();
    }

    public void AddAttribute(string name, string type, AccessModifier modifier) {
        attributes.Add(new Attribute(name, modifier, type));
        AddLine();
    }

    public void AddConstructor(string name, string[] attribs = null) {
        if (attribs == null){
            constructors.Add(new Constructor(name));
        }
        else {
            List<Attribute> attrbs = new List<Attribute>();
            foreach(string att in attribs) {
                Debug.LogWarning("List of attributes is being declared with the name on both fields");
                #pragma warning List of attributes is being declared with the name on both fields
                attrbs.Add(new Attribute(name, name));
            }
            constructors.Add(new Constructor(name,attrbs));
        }
        AddLine();
    }

    public void AddDelegate(string type, string returnType, AccessModifier modifier) {

        delegates.Add(new Delegate(type, returnType, modifier));
        AddLine();
    }

    public void AddDelegate(string type, string returnType) {

        delegates.Add(new Delegate(type, returnType));
        AddLine();
    }

    public void AddStruct(string name) {

        structs.Add(new Struct(name));
        AddLine();
    }

    public void AddStruct(string name, AccessModifier modifier) {

        structs.Add(new Struct(name, modifier));
        AddLine();
    }

    public void AddLine() {
        numberOfLines++;
    }

    public override void DrawCurves(){

    }

    public override void DrawWindow(){
        base.DrawWindow();

        var header = new GUIStyle();
        var public_ = new GUIStyle();
        var private_ = new GUIStyle();
        var protected_ = new GUIStyle();
        header.normal.textColor = Color.grey;
        public_.normal.textColor = Color.green;
        private_.normal.textColor = Color.red;
        protected_.normal.textColor = Color.magenta;

        GUILayout.Label("Attributes",header);
        foreach (Attribute attribute in attributes) {
            GUILayout.Label(attribute.ToString());
        }
        GUILayout.Label("Methods",header);
        foreach (Method method in methods){
            GUILayout.BeginHorizontal();
            if (method.modifier == AccessModifier.PRIVATE) {
                GUILayout.Label("- ", private_, GUILayout.Width(5));
                GUILayout.Label(method.ToString() + "()");
            }
            else if (method.modifier == AccessModifier.PUBLIC) {
                GUILayout.Label("+ ", public_, GUILayout.Width(5));
                GUILayout.Label(method.ToString() + "()");
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.Label("Constructors",header);
        foreach (Constructor constructor in constructors){
            GUILayout.Label(constructor.ToString());
        }
        GUILayout.Label("Structs",header);
        foreach (Struct struct_ in structs){
            GUILayout.Label(struct_.ToString());
        }
        GUILayout.Label("Delegates",header);
        foreach (Delegate delegate_ in delegates){
            GUILayout.Label(delegate_.ToString());
        }
        GUILayout.Label("Classes",header);
        foreach (Class class_ in classes){
            GUILayout.Label(class_.ToString());
        }
    }

    public override void Tick(float deltaTime)
    {
    }
}
