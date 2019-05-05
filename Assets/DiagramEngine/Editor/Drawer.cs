using System;
using System.Linq;
using UnityEngine;
using Model = DEngine.Model;
using System.Collections.Generic;

namespace DEngine.View {

    public static class Drawer {

        public static GUIStyle widthStyle;
        public static GUIStyle header;
        public static GUIStyle public_;
        public static GUIStyle private_;
        public static GUIStyle protected_;
        public static GUIStyle internal_;
        public static GUIStyle primitiveTypeColor;
        public static GUIStyle genericTypeColor;
        public static GUIStyle staticStyle;
        public static GUIStyle classStyle;
        public static GUIStyle interfaceStyle;
        public static GUIStyle structStyle;

        static Drawer() {

            widthStyle = new GUIStyle();
            header = new GUIStyle();
            public_ = new GUIStyle();
            private_ = new GUIStyle();
            protected_ = new GUIStyle();
            internal_ = new GUIStyle();
            primitiveTypeColor = new GUIStyle();
            genericTypeColor = new GUIStyle();
            staticStyle = new GUIStyle();

            classStyle = new GUIStyle();
            interfaceStyle = new GUIStyle();
            structStyle = new GUIStyle();

            widthStyle.stretchWidth = false;
            header.stretchWidth = false;
            public_.stretchWidth = false;
            private_.stretchWidth = false;
            protected_.stretchWidth = false;
            internal_.stretchWidth = false;
            primitiveTypeColor.stretchWidth = false;
            genericTypeColor.stretchWidth = false;
            staticStyle.stretchWidth = false;
            classStyle.stretchWidth = false;
            interfaceStyle.stretchWidth = false;
            structStyle.stretchWidth = false;

            header.normal.textColor = new Color(0.188f, 0.258f, 0.733f, 1f);
            public_.normal.textColor = Color.green;
            private_.normal.textColor = Color.red;
            protected_.normal.textColor = Color.magenta;
            internal_.normal.textColor = Color.cyan;
            primitiveTypeColor.normal.textColor = new Color(0.694f, 0.129f, 0.286f);
            genericTypeColor.normal.textColor = new Color(0.278f, 0.458f, 0.396f);
            staticStyle.normal.textColor = new Color(0.210f, 0.210f, 0.210f);
            classStyle.normal.textColor = new Color(0.168f, 0.552f, 0.003f);
            interfaceStyle.normal.textColor = new Color(0.552f, 0.317f, 0.003f, 1f);
            structStyle.normal.textColor = new Color(0.011f, 0.431f, 0.5883f, 1f);

            staticStyle.fontStyle = FontStyle.Bold;
            interfaceStyle.fontStyle = FontStyle.Italic;
            structStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        public static void DrawMethod(Model::Method method) {

            DrawAccessModifier(method.modifier);
            DrawType(method.returnType);
            DrawSpace();
            if(method.staticType == Model.StaticType.STATIC) {
                GUILayout.Label(method.name, staticStyle);
            }
            else {
                GUILayout.Label(method.name, widthStyle);
            }

            DrawSpace();
            GUILayout.Label("(", widthStyle);
            if (method.parameters != null && method.parameters.Count > 0) {
                DrawSpace();
                for (int i = 0; i < method.parameters.Count - 1; i++) {
                    DrawParameter(method.parameters[i]);
                    GUILayout.Label(",", widthStyle);
                    DrawSpace();
                }
                DrawParameter(method.parameters[method.parameters.Count - 1]);
            }
            DrawSpace();
            GUILayout.Label(")", widthStyle);
        }

        public static void DrawConstructor(Model::Constructor constructor){

            DrawAccessModifier(constructor.modifier);

            if(constructor.staticType == Model.StaticType.STATIC) {
                GUILayout.Label(constructor.name, staticStyle);
            }
            else {
                GUILayout.Label(constructor.name, widthStyle);
            }

            DrawSpace();
            GUILayout.Label("(", widthStyle);
            if (constructor.parameters != null && constructor.parameters.Count > 0){
                DrawSpace();
                for (int i = 0; i < constructor.parameters.Count - 1; i++){
                    DrawParameter(constructor.parameters[i]);
                    GUILayout.Label(",", widthStyle);
                    DrawSpace();
                }
                DrawParameter(constructor.parameters[constructor.parameters.Count - 1]);
            }
            DrawSpace();
            GUILayout.Label(")", widthStyle);
        }

        public static void DrawAttribute(Model::Attribute attribute)
        {

            DrawAccessModifier(attribute.modifier);
            DrawType(attribute.returnType);
            DrawSpace();

            if(attribute.staticType == Model.StaticType.STATIC) {
                GUILayout.Label(attribute.name, staticStyle);
            }
            else {
                GUILayout.Label(attribute.name, widthStyle);
            }
        }

        public static void DrawParameter(Model::Parameter parameter) {

            if (Model::Types.primitiveTypes.Contains(parameter.type.name))
            {
                GUILayout.Label(parameter.type.name, primitiveTypeColor);
            }
            else
            {
                GUILayout.Label(parameter.type.name, genericTypeColor);
            }
            DrawSpace();
            if (parameter.type.type != null && parameter.type.type.Count > 0) {
                DrawSpace();
                GUILayout.Label("<", widthStyle);
                for (int i = 0; i < parameter.type.type.Count - 1; i++) {
                    DrawType(parameter.type.type[i]);
                    GUILayout.Label(",", widthStyle);
                    DrawSpace();
                }
                Drawer.DrawType(parameter.type.type[parameter.type.type.Count - 1]);
                DrawSpace();
                GUILayout.Label(">", widthStyle);
            }
            GUILayout.Label(parameter.name, widthStyle);
        }

        public static void DrawType(Model::GenericType type) {

            if(Model::Types.primitiveTypes.Contains(type.name)) {
                GUILayout.Label(type.name, primitiveTypeColor);
            }
            else {
                GUILayout.Label(type.name, genericTypeColor);
            }
            if (type.type != null && type.type.Count > 0) {
                DrawSpace();
                GUILayout.Label("<", widthStyle);
                for (int i = 0; i < type.type.Count - 1; i++) {
                    DrawType(type.type[i]);
                    GUILayout.Label(",", widthStyle);
                    DrawSpace();
                }
                DrawType(type.type[type.type.Count - 1]);
                GUILayout.Label(">", widthStyle);
            }
        }

        public static void DrawAccessModifier(Model::AccessModifier accessModifier) {

            switch (accessModifier) {
                case Model::AccessModifier.PRIVATE:
                    GUILayout.Label("- ", private_, GUILayout.Width(10));
                    break;
                case Model::AccessModifier.PUBLIC:
                    GUILayout.Label("+ ", public_, GUILayout.Width(10));
                    break;
                case Model::AccessModifier.PROTECTED:
                    GUILayout.Label("# ", protected_, GUILayout.Width(10));
                    break;
                case Model::AccessModifier.INTERNAL:
                case Model::AccessModifier.PROTECTED_INTERNAL:
                    GUILayout.Label("~ ", internal_, GUILayout.Width(10));
                    break;
            }
            DrawSpace();
        }

        public static void DrawSpace(int amount = 1) {

            GUILayout.Label(new String(' ', amount), widthStyle);
        }

        public static void DrawEntitiesList(List<Model::BaseModel> selectedEntities) {
            
            foreach (var entity in selectedEntities)
            {
                GUILayout.BeginHorizontal();
                DrawSpace(3);
                if (entity.Type == Model::EntityTypes.CLASS){
                    GUILayout.Label("(C)  ", classStyle);
                }
                else if (entity.Type == Model::EntityTypes.INTERFACE){
                    GUILayout.Label("(I)  ", interfaceStyle);
                }
                else if (entity.Type == Model::EntityTypes.STRUCT){
                    GUILayout.Label("(S)  ", structStyle);
                }
                GUILayout.Label(entity.GetName(), widthStyle);
                GUILayout.EndHorizontal();
            }
        }
    }
}